using HealthCare.API.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Text.Json.Serialization;

namespace HealthCare.API.Models
{
    public class patientimageviewmodel
    {
        public int Id { get; set; }

        [JsonIgnore]
        [Required(ErrorMessage = "The field {0} is required.")]
        public Patient patient { get; set; }

        [Display(Name = "Photo")]
        public string ImageId { get; set; }

        public string rbmp { get; set; }

        public string gbmp { get; set; }

        public string bbmp { get; set; }

      

        [Display(Name = "Photo")]
        public string ImageFullPath => ImageId == String.Empty
            ? $"https://healthcareapi20220724094946.azurewebsites.net/images/noimage.png"
            : $"https://imagesahmed.blob.core.windows.net/patients/{ImageId}";

        public string Imagerbmp => rbmp == String.Empty
           ? $"https://healthcareapi20220724094946.azurewebsites.net/images/noimage.png"
           : $"https://localhost:7152/{rbmp}";
        public string Imagegbmp => gbmp == String.Empty
        ? $"https://healthcareapi20220724094946.azurewebsites.net/images/noimage.png"
        : $"https://localhost:7152/{gbmp}";
        public string Imagebbmp => bbmp == String.Empty
       ? $"https://healthcareapi20220724094946.azurewebsites.net/images/noimage.png"
       : $"https://localhost:7152/{bbmp}";
    }
}
