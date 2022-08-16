using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HealthCare.API.Data.Entities
{
    public class PatientPhoto
    {
        public int Id { get; set; }

        [JsonIgnore]
        [Required(ErrorMessage = "The field {0} is required.")]
        public Patient patient { get; set; }

        [Display(Name = "Photo")]
        public Guid ImageId { get; set; }

        [Display(Name = "Photo")]
        public string ImageFullPath => ImageId == Guid.Empty
            ? $"https://healthcareapi20220724094946.azurewebsites.net/images/noimage.png"
            : $"https://imagesahmed.blob.core.windows.net/patients/{ImageId}";
    }
}
