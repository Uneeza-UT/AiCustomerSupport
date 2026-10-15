using AiCustomerSupport.Application.DTOs.Identity;

namespace AiCustomerSupport.Application.Contracts.Identity
{
    public interface IAuthService
    {
        Task<AuthResponse> Login(AuthRequest request);
        Task<RegistrationResponse> Register(RegistrationRequest request);
        Task ChangePassword(ChangePasswordRequest request);
        Task ForgotPassword(ForgotPasswordRequest request);
        Task ResetPassword(ResetPasswordRequest request);
    }
}
