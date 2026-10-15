using AiCustomerSupport.Application.Contracts.Email;
using AiCustomerSupport.Application.Contracts.Identity;
using AiCustomerSupport.Application.DTOs.Identity;
using AiCustomerSupport.Application.Exceptions;
using AiCustomerSupport.Application.Models.Email;
using AiCustomerSupport.Application.Models.Identity;
using AiCustomerSupport.Application.Validations.Identity;
using AiCustomerSupport.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AiCustomerSupport.Identity.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly JwtSettings _jwtSettings;
        private readonly FrontendSettings _frontendSettings;

        private readonly ICurrentUserService _currentUserService;
        private readonly IEmailSender _emailSender;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<JwtSettings> jwtSettings,
            IOptions<FrontendSettings> frontendSettings,
            ICurrentUserService currentUserService,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings.Value;
            _frontendSettings = frontendSettings.Value;
            _currentUserService = currentUserService;
            _emailSender = emailSender;
        }


        public async Task<AuthResponse> Login(AuthRequest request)
        {
            // Validate the request.
            var validator = new AuthRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (validationResult.Errors.Any())
            {
                throw new BadRequestException("Invalid credentials. ", validationResult);

            }

            // Find the user by email.
            var user = await _userManager.FindByEmailAsync(request.EmailAddress);

            if (user == null)
            {
                throw new NotFoundException($"User with {request.EmailAddress} not found.", request.EmailAddress);
            }


            // Verify the user's password.
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!result.Succeeded)
            {
                throw new NotFoundException($"Credentials for {request.EmailAddress} aren't valid", request.EmailAddress);
            }


            // Generate a JWT token.
            JwtSecurityToken jwtSecurityToken = await GenerateTokenAsync(user);


            // Create the authentication response.
            var response = new AuthResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                EmailAddress = user.Email,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken)
            };

            return response;
        }



        private async Task<JwtSecurityToken> GenerateTokenAsync(ApplicationUser user)
        {
            // Retrieve the user's claims and roles.
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);


            // Convert the user's roles into role claims.
            var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r)).ToList();


            // Create the claims to include in the JWT token.
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("UserId", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);


            // Create the security key and signing credentials.
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);


            // Create the JWT token with issuer, audience, claims, and expiration.
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials
            );

            return jwtSecurityToken;
        }



        public async Task<RegistrationResponse> Register(RegistrationRequest request)
        {
            // Validate the request.
            var validator = new RegistrationRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (validationResult.Errors.Any())
            {
                throw new BadRequestException("Invalid request. ", validationResult);
            }


            // Create a new user.
            var user = new ApplicationUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.EmailAddress,
                UserName = request.UserName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    Environment.NewLine,
                    result.Errors.Select(e => e.Description));

                throw new BadRequestException(errors);
            }


            // Assign the selecetd role to the new user.
            await _userManager.AddToRoleAsync(user, request.Role);


            // Notify the user about successful registration through email.
            await _emailSender.SendEmailAsync(new EmailMessageData
            {
                To = user.Email,
                Subject = "Welcome to the AI Customer Support System",
                Body = $"Hello {user.FirstName},\n\n" +
                   "Your account has been successfully created.\n\n" +
                   "You can now log in and start using the AI Customer Support System.\n\n" +
                   "Thank you,\n" +
                   "AI Customer Support System"
            });


            return new RegistrationResponse
            {
                UserId = user.Id
            };
        }




        public async Task ChangePassword(ChangePasswordRequest request)
        {
            var user = await _userManager.FindByIdAsync(_currentUserService.UserId);

            if (user == null)
            {
                throw new NotFoundException(nameof(ApplicationUser), _currentUserService.UserId);
            }

            var result = await _userManager.ChangePasswordAsync(
                user,
                request.CurrentPassword,
                request.NewPassword);


            if (!result.Succeeded)
            {
                string errors = string.Join(
                    Environment.NewLine,
                    result.Errors.Select(e => e.Description));

                throw new BadRequestException(errors);
            }

            await _emailSender.SendEmailAsync(new EmailMessageData
            {
                To = user.Email!,
                Subject = "Your Password was Changed",
                Body = $"Hello {user.FirstName},\n\n" +
                        "Your password for the AI Customer Support System has been changed.\n\n" +
                        "If you made this change, no further action is required.\n\n" +
                        "If you did not change your password, please contact our support team immediately.\n\n" +
                        "Thank you,\n" +
                        "AI Customer Support System"
            });
        }



        public async Task ForgotPassword(ForgotPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.EmailAddress);

            if (user == null)
            {
                throw new NotFoundException($"User with {request.EmailAddress} not found.", request.EmailAddress);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetUrl = $"{_frontendSettings.ResetPasswordUrl}" +
                $"?email={Uri.EscapeDataString(request.EmailAddress)}" +
                $"&token={Uri.EscapeDataString(token)}";


            await _emailSender.SendEmailAsync(new EmailMessageData
            {
                To = user.Email!,
                Subject = "Reset Your Password",
                Body = $"Hello {user.FirstName},\n\n" +
                        "We received a request to reset your password.\n\n" +
                        $"Reset your password using the following link:\n{resetUrl}\n\n" +
                        "If you did not request a password reset, you can safely ignore this email.\n\n" +
                        "Thank you,\n" +
                        "AI Customer Support System"
            });
        }




        public async Task ResetPassword(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.EmailAddress);

            if (user == null)
            {
                throw new NotFoundException($"User with {request.EmailAddress} not found.", request.EmailAddress);
            }

            var result = await _userManager.ResetPasswordAsync(
                user,
                request.Token,
                request.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    Environment.NewLine,
                    result.Errors.Select(e => e.Description));

                throw new BadRequestException(errors);
            }


            await _emailSender.SendEmailAsync(new EmailMessageData
            {
                To = user.Email!,
                Subject = "Your Password was Reset",
                Body = $"Hello {user.FirstName},\n\n" +
                        "Your password for the AI Customer Support System has been changed.\n\n" +
                        "If you made this change, no further action is required.\n\n" +
                        "If you did not change your password, please contact our support team immediately.\n\n" +
                        "Thank you,\n" +
                        "AI Customer Support System"
            });
        }
    }
}
