using System.ComponentModel.DataAnnotations;

namespace HealthCare.API.Models
{
    public class LoginViewModel
    {
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "You must enter a valid email.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string  Username { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [MinLength(6, ErrorMessage = "The field {0} must have a minimum length of {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string Password { get; set; }

        [Display(Name = "Remember Me?")]
        public bool RememberMe { get; set; }
    }
}
