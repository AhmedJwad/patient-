using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using HealthCare.API.Data;

namespace HealthCare.API.Data.Entities
{
    public class diagonisic
    {
        public int Id { get; set; }

        [Display(Name = "Diagonisic")]
        [MaxLength(50, ErrorMessage = "The field {0} cannot have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string Description { get; set; }

       [JsonIgnore]
        public ICollection<Detail> details { get; set; }
	
    }
}
