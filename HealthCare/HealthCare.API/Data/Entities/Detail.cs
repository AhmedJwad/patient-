using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HealthCare.API.Data.Entities
{
    public class Detail
    {
        public int Id { get; set; }

        [Display(Name = "Histories")]
        [JsonIgnore]
        [Required(ErrorMessage = "The field {0} is required.")]
        public History History { get; set; }

        [Display(Name = "Diagonisic")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public diagonisic diagonisic { get; set; }

        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }    
    }
}
