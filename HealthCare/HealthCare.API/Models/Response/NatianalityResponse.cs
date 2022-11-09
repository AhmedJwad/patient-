using HealthCare.API.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace HealthCare.API.Models.Response
{
    public class NatianalityResponse
    {
        public int Id { get; set; }

        [Display(Name = "Nationality")]
        [MaxLength(50, ErrorMessage = "The field {0} cannot have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string Description { get; set; }
      
    }
}
