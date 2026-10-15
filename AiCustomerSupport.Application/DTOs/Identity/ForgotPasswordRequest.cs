using System.ComponentModel.DataAnnotations;

namespace AiCustomerSupport.Application.DTOs.Identity
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
    }
}
