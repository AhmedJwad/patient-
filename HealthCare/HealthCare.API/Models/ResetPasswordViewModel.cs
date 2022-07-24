using System.ComponentModel.DataAnnotations;

namespace HealthCare.API.Models
{
    public class ResetPasswordViewModel
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "The field {0} is required.")]
        [EmailAddress(ErrorMessage = "You must enter a valid email.")]
        public string UserName { get; set; }

        [Display(Name = "New Password")]
        [Required(ErrorMessage = "The field {0} is required.")]
        [MinLength(6, ErrorMessage = "The field {0} must have a minimum length of {1} characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "password confirmation")]
        [Required(ErrorMessage = "The field {0} is required.")]
        [MinLength(6, ErrorMessage = "The field {0} must have a minimum length of {1} characters.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The new password and confirmation are not the same.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Token { get; set; }
    }
}
