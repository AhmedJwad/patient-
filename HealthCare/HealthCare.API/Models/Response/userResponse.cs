using HealthCare.API.Data.Entities;
using HealthCare.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace HealthCare.API.Models.Response
{
    public class userResponse
    {
        public string id { get; set; }
        public string email { get; set; }
        public string userName { get; set; }
        [Display(Name = "First Name")]
        [MaxLength(50, ErrorMessage = "The field {0} cannot have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string firstName { get; set; }


        [Display(Name = "Last Name")]
        [MaxLength(50, ErrorMessage = "The field {0} cannot have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string lastName { get; set; }

        [Display(Name = "Adress")]
        [MaxLength(100, ErrorMessage = "The field {0} cannot have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string address { get; set; }

        [Display(Name = "Photo")]
        public Guid imageId { get; set; }

        [Display(Name = "Photo")]
        public string imageFullPath => imageId == Guid.Empty
           ? $"https://healthcareapi20220724094946.azurewebsites.net/images/noimage.png"
           : $"https://imagesahmed.blob.core.windows.net/users/{imageId}";


        [Display(Name = "Type of User")]
        public UserType userType { get; set; }

        [Display(Name = "User")]
        public string FullName => $"{firstName} {lastName}";

        public string phoneNumber { get; set; }
        //[JsonIgnore]      
        public ICollection<Patientresponse> patients { get; set; }

        [Display(Name = "# Patients")]
        public int patientsCount => patients == null ? 0 : patients.Count;
        public ICollection<AgendaResponse> Agendas { get; set; }

    

    }
}
