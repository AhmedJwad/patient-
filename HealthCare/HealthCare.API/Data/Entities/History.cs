using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HealthCare.API.Data.Entities
{
    public class History
    {
        public int Id { get; set; }

        [Display(Name = "allergies")]
        [MaxLength(100, ErrorMessage = "The field {0} cannot have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string allergies { get; set; }

        [Display(Name = "illnesses")]
        [MaxLength(100, ErrorMessage = "The field {0} cannot have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]

        public string illnesses { get; set; }

        [Display(Name = "surgeries")]
        [DataType(DataType.MultilineText)]
        public string surgeries { get; set; }

        [Display(Name = "immunizations, and results of physical exams and tests")]
        [DataType(DataType.MultilineText)]
        public string Result { get; set; }

        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}")]
        public DateTime Date { get; set; }

        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}")]
        public DateTime DateLocal => Date.ToLocalTime();


        [JsonIgnore]
        public Patient patient { get; set; }

        [JsonIgnore]
        public User user { get; set; }

       
        public ICollection<Detail> Details { get; set; }

        [Display(Name = "# Details")]
        public int DetailsCount => Details == null ? 0 : Details.Count;
    }
}
