using AiCustomerSupport.Application.Contracts.Identity;
using AiCustomerSupport.Application.DTOs.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace AiCustomerSupport.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authenticationService;

        public AuthController(IAuthService authenticationService)
        {
            _authenticationService = authenticationService;
        }


        [HttpPost("login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<AuthResponse>> Login(AuthRequest request)
        {
            return Ok(await _authenticationService.Login(request));
        }



        [HttpPost("register")]
        [ProducesResponseType(200)]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<RegistrationResponse>> Register(RegistrationRequest request)
        {
            return Ok(await _authenticationService.Register(request));
        }




        [HttpPost("forgot-password")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            await _authenticationService.ForgotPassword(request);
            return Ok("A password reset link has been sent.");
        }



        [HttpPost("reset-password")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> ResetPassword(ResetPasswordRequest request)
        {
            await _authenticationService.ResetPassword(request);
            return Ok("Password has been reset successfully.");
        }




        [HttpPost("change-password")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> ChangePassword(ChangePasswordRequest request)
        {
            await _authenticationService.ChangePassword(request);
            return Ok("Password has been changed successfully.");
        }
    }
}
