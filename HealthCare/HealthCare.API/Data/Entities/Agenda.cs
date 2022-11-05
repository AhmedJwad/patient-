using System.ComponentModel.DataAnnotations;

namespace HealthCare.API.Data.Entities
{
    public class Agenda
    {
        public int Id { get; set; }
        [Display(Name = "Date")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Is Available?")]
        public bool IsAvailable { get; set; }
        public bool IsMine { get; set; }
        [Display(Name = "Date*")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime DateLocal => Date.ToLocalTime();

        public User user { get; set; }
        public Patient pathient { get; set; }
    }
}
