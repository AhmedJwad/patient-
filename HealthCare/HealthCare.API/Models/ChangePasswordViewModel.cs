using System.ComponentModel.DataAnnotations;

namespace HealthCare.API.Models
{
    public class ChangePasswordViewModel
    {
        [Display(Name = "Current password")]
        [Required(ErrorMessage = "The field {0} is required.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "The field {0} must have a minimum length of {1} characters.")]
        public string OldPassword { get; set; }

        [Display(Name = "New Password")]
        [Required(ErrorMessage = "The field {0} is required.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "The field {0} must have a minimum length of {1} characters.")]
        public string NewPassword { get; set; }

        [Display(Name = "password confirmation")]
        [Required(ErrorMessage = "The field {0} is required.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "The field {0} must have a minimum length of {1} characters.")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation are not the same.")]
        public string Confirm { get; set; }
    }
}
