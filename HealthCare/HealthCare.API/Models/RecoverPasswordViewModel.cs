using System.ComponentModel.DataAnnotations;

namespace HealthCare.API.Models
{
    public class RecoverPasswordViewModel
    {
        [Required(ErrorMessage = "The field {0} is required.")]
        [EmailAddress(ErrorMessage = "You must enter a valid email.")]
        public string Email { get; set; }
    }
}
