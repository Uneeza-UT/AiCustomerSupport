using System.ComponentModel.DataAnnotations;

namespace AiCustomerSupport.Application.DTOs.Identity
{
    public class ResetPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }


        [Required]
        public string Token { get; set; }


        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; }
    }
}
