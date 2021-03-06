using HealthCare.Common.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HealthCare.API.Data.Entities
{
    public class User:IdentityUser
    {
        [Display(Name = "First Name")]
        [MaxLength(50, ErrorMessage = "The field {0} cannot have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string FirstName { get; set; }


        [Display(Name = "Last Name")]
        [MaxLength(50, ErrorMessage = "The field {0} cannot have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string LastName { get; set; }

        [Display(Name = "Adress")]
        [MaxLength(100, ErrorMessage = "The field {0} cannot have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string Address { get; set; }

        [Display(Name ="Photo")]
        public Guid ImageId { get; set; }

        [Display(Name = "Photo")]
        public string ImageFullPath => ImageId == Guid.Empty
           ? $"https://localhost:7152/images/noimage.png"
           : $"https://imagesahmed.blob.core.windows.net/users/{ImageId}";


        [Display(Name = "Type of User")]
        public UserType userType { get; set; }

        [Display(Name = "User")]
        public string FullName => $"{FirstName} {LastName}";

        [Display(Name = "# Patients")]
        public int PatientsCount => Patients == null ? 0 : Patients.Count;

        public ICollection<Patient> Patients { get; set; }
        public ICollection<History> histories { get; set; }
    }
}
