using HealthCare.API.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HealthCare.API.Models
{
    public class patientViewmodel
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

        [Display(Name = "Type of blood")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a type of blood.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public int BloodTypeId { get; set; }
        public IEnumerable<SelectListItem> bloodTypes { get; set; }

        [Display(Name = "City")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a City.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public int CityId { get; set; }
        public IEnumerable<SelectListItem> Cities { get; set; }

        [Display(Name = "Nationality")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Nationality.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public int NationalityId { get; set; }
        public IEnumerable<SelectListItem> Nationaliteis { get; set; }

        [Display(Name = "Gendre")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Gendre.")]
       
        public int GendreId { get; set; }
        public IEnumerable<SelectListItem> Gendres { get; set; }

        [Display(Name = "User of patient")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Gendre.")]
        public int UserpatientId { get; set; }
        public IEnumerable<SelectListItem> UserPatients { get; set; }

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

         public string UserId { get; set; }

        [Display(Name = "Photo")]
        public IFormFile? ImageFile { get; set; }

      
        public ICollection<PatientPhoto> patientPhotos { get; set; }

        
    }
}
