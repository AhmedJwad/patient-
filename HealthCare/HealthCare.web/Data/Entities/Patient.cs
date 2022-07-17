using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HealthCare.web.Data.Entities
{
    public class Patient
    {
        public int Id { get; set; }

        [Display(Name = "First Name")]
        [MaxLength(50, ErrorMessage = "The field {0} cannot have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [MaxLength(50, ErrorMessage = "The field {0} cannot have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string LastName { get; set; }

        [Display(Name = "Adress")]
        [MaxLength(50, ErrorMessage = "The field {0} cannot have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string Address { get; set; }


        [JsonIgnore]
        public BloodType bloodType { get; set; }



        
    }
}
