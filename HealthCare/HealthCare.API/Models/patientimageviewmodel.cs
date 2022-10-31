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

        public string histgrame { get; set; }

        public string histgrameorginal { get; set; }
        public double Entropyorginal { get; set; }
        public double Entropyscample { get; set; }
        public double corrvertical { get; set; }
        public double corrhorizontal { get; set; }
        public double corrdiagnol { get; set; }
        public double corrverticalxorimage { get; set; }
        public double corrhorizontalxorimage { get; set; }
        public double corrdiagnolxorimage { get; set; }
        public double NPCR { get; set; }

        public double NPCRXorimage { get; set; }
        public double Xorentropy { get; set; }

        public string generateimage { get; set; }

        public string changerowandcolumnimage { get; set; }

        public string xorbetweenscrambledimageandkimage { get; set; }

        public string histogramXorimagePng { get; set; }

        public string chaotic5d { get; set; }

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

        public string Histograme => histgrame == String.Empty
     ? $"https://healthcareapi20220724094946.azurewebsites.net/images/noimage.png"
     : $"https://localhost:7152/{histgrame}";

        public string HistogrameOrginae => histgrameorginal == String.Empty
     ? $"https://healthcareapi20220724094946.azurewebsites.net/images/noimage.png"
     : $"https://localhost:7152/{histgrameorginal}";
        public string Generateimage => generateimage == String.Empty
     ? $"https://healthcareapi20220724094946.azurewebsites.net/images/noimage.png"
     : $"https://localhost:7152/{generateimage}";

        public string Changerowandcolumnimage => changerowandcolumnimage == String.Empty
    ? $"https://healthcareapi20220724094946.azurewebsites.net/images/noimage.png"
    : $"https://localhost:7152/{changerowandcolumnimage}";

        public string Xorimage => xorbetweenscrambledimageandkimage == String.Empty
    ? $"https://healthcareapi20220724094946.azurewebsites.net/images/noimage.png"
    : $"https://localhost:7152/{xorbetweenscrambledimageandkimage}";

        public string HistogramXorimagePng => histogramXorimagePng == String.Empty
    ? $"https://healthcareapi20220724094946.azurewebsites.net/images/noimage.png"
    : $"https://localhost:7152/{histogramXorimagePng}";
        public string Chaotic5d => chaotic5d == String.Empty
   ? $"https://healthcareapi20220724094946.azurewebsites.net/images/noimage.png"
   : $"https://localhost:7152/{chaotic5d}";

    }
}
