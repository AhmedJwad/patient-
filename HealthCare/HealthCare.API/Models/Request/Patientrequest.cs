using System.ComponentModel.DataAnnotations;

namespace HealthCare.API.Models.Request
{
    public class Patientrequest
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
        public string UserId { get; set; }
        public int Age => DateLocal == null ? 0 : DateTime.Today.Year - DateLocal.Year;     

        [Display(Name = "Patient")]
        public string FullName => $"{FirstName} {LastName}";
        public int CityId { get; set; }
        public int NatianalityId { get; set; }
        public int gendreId { get; set; }
        public int bloodTypeId { get; set; }
        public int userPatientId { get; set; }

        public byte[] Image { get; set; }
      
    }
}
