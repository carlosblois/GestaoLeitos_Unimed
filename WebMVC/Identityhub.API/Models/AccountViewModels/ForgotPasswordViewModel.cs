using System.ComponentModel.DataAnnotations;

namespace Identityhub.API.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
