using System.ComponentModel.DataAnnotations;

namespace HealthCare.API.Models
{
    public class PatientPhotoViewModel
    {
        public int PatientId { get; set; }

        [Display(Name = "Photo")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public IFormFile ImageFile { get; set; }
    }
}
