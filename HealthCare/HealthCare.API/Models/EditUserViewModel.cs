using HealthCare.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace HealthCare.API.Models
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
      

        [Display(Name = "First Name")]
        [MaxLength(50, ErrorMessage = "The field {0} cannot have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [MaxLength(50, ErrorMessage = "The field {0} cannot have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string LastName { get; set; }


        [Display(Name = "Address")]
        [MaxLength(100, ErrorMessage = "The field {0} cannot have more than {1} characters.")]
        public string Address { get; set; }


        [Display(Name = "Telephone")]
        [MaxLength(20, ErrorMessage = "The field {0} cannot have more than {1} characters.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Photo")]
        public Guid ImageId { get; set; }

        [Display(Name = "Type of User")]
        public UserType UserType { get; set; }

        [Display(Name = "Photo")]
        public IFormFile? ImageFile { get; set; }


        [Display(Name = "Photo")]
        public string ImageFullPath => ImageId == Guid.Empty
           ? $"https://localhost:7152/images/noimage.png"
           : $"https://imagesahmed.blob.core.windows.net/users/{ImageId}";
    }
}
