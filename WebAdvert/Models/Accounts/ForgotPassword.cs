using System.ComponentModel.DataAnnotations;

namespace WebAdvert.Models.Accounts
{
    public class ForgotPassword
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
