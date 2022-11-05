using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HealthCare.API.Models
{
    public class AddUserViewModel: EditUserViewModel
    {
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "You must enter a valid email.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [MinLength(6, ErrorMessage = "The field {0} must have a minimum length of {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string Password { get; set; }

        [Display(Name = "password confirmation")]
        [Required(ErrorMessage = "The field {0} is required.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "The field {0} must have a minimum length of {1} characters.")]
        [Compare("Password", ErrorMessage = "The password and password confirmation are not the same.")]
        public string PasswordConfirm { get; set; }
      
    }
}
