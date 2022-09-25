using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HealthCare.API.Data.Entities
{
    public class Natianality
    {
        public int Id { get; set; }

        [Display(Name = "Nationality")]
        [MaxLength(50, ErrorMessage = "The field {0} cannot have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string Description { get; set; }

        [JsonIgnore]
        public ICollection<Patient> Patients { get; set; }  
    }
}
