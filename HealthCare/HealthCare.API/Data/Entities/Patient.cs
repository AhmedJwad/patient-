
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HealthCare.API.Data.Entities
{
    public class Patient
    {
        public int Id { get; set; }

        [Display(Name = "First Name")]
        [MaxLength(50, ErrorMessage = "The field {0} cannot have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [MaxLength(50, ErrorMessage = "The field {0} cannot have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string LastName { get; set; }

        [Display(Name = "Adress")]
        [MaxLength(50, ErrorMessage = "The field {0} cannot have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string Address { get; set; }              
       

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}")]
        [Display(Name = "Birth of Date")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public DateTime Date { get; set; }
        
        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}")]
        public DateTime DateLocal => Date.ToLocalTime();

        [Display(Name = "EPCN")]        
        [Required(ErrorMessage = "The field {0} is required.")]
        public int EPCNNumber { get; set; }

        [Display(Name = "Mobile Phone")]        
        [Required(ErrorMessage = "The field {0} is required.")]
        [DataType(DataType.PhoneNumber)]
        public string MobilePhone { get; set; }

        [Display(Name = "Description")]        
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [JsonIgnore]
        public User User { get; set; }
        public int Age => DateLocal == null ? 0 : DateTime.Today.Year - DateLocal.Year;

        [Display(Name = "# Photos")]
        public int PatientPhotosCount => patientPhotos == null ? 0 : patientPhotos.Count;

        [Display(Name = "Patient")]
        public string FullName => $"{FirstName} {LastName}";

        [Display(Name = "Photo")]
        public string ImageFullPath => patientPhotos == null || patientPhotos.Count == 0
           ? $"https://healthcareapi20220724094946.azurewebsites.net/images/noimage.png"
           : patientPhotos.FirstOrDefault().ImageFullPath;

        [Display(Name = "# histories")]
        public int HistoriesCount => histories == null ? 0 : histories.Count;
  
        public ICollection<PatientPhoto>patientPhotos { get; set; }
       
        public ICollection<History> histories { get; set; }
        public City City { get; set; }
        public Natianality Natianality { get; set; }
        public gendre gendre { get; set; }
        public BloodType bloodType { get; set; }
        public UserPatient userPatient { get; set; }
        public ICollection<Agenda> Agendas { get; set; }

    }
}
