using System.ComponentModel.DataAnnotations;

namespace HealthCare.API.Models.Request
{
    public class RegisterRequest
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "The field {0} is required.")]
        public string Email { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} cannot have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string FirstName { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} cannot have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string LastName { get; set; }

        [MaxLength(100, ErrorMessage = "The field {0} cannot have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string Address { get; set; }


        [MaxLength(20, ErrorMessage = "The field {0} cannot have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "The field {0} is required.")]
        [MinLength(6, ErrorMessage = "The field {0} cannot have more than {1} characters.")]
        public string Password { get; set; }
        public int RoleId { get; set; }
        public byte[] Image { get; set; }

    }
}
