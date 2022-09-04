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

        public string imagenormal { get; set; }
        public string binaryimage { get; set; }

        public string t { get; set; }

        public string scrabmle { get; set; }

        public string binaryorginale { get; set; }

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
        public string Imagenormal => imagenormal == String.Empty
      ? $"https://healthcareapi20220724094946.azurewebsites.net/images/noimage.png"
      : $"https://localhost:7152/{imagenormal}";
        public string Binaryimage => binaryimage == String.Empty
      ? $"https://healthcareapi20220724094946.azurewebsites.net/images/noimage.png"
      : $"https://localhost:7152/{binaryimage}";
        public string BinaryOrginal => binaryorginale == String.Empty
     ? $"https://healthcareapi20220724094946.azurewebsites.net/images/noimage.png"
     : $"https://localhost:7152/{binaryorginale}";
        public string T => t == String.Empty
     ? $"https://healthcareapi20220724094946.azurewebsites.net/images/noimage.png"
     : $"https://localhost:7152/{t}";

        public string Scramble => scrabmle == String.Empty
     ? $"https://healthcareapi20220724094946.azurewebsites.net/images/noimage.png"
     : $"https://localhost:7152/{scrabmle}";
    }
}
