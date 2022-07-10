using System.ComponentModel.DataAnnotations;

namespace HealthCare.API.Data.Entities
{
    public class diagonisic
    {
        public int Id { get; set; }

        [Display(Name = "Diagonisic")]
        [MaxLength(50, ErrorMessage = "The field {0} cannot have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string Description { get; set; }
    }
}
