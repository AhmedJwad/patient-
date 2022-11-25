using HealthCare.API.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace HealthCare.API.Models.Request
{
    public class DetailRequest
    {
        public int Id { get; set; }

        [Display(Name = "Histories")]
        [Required(ErrorMessage = "The field {0} is required.")]
       
        public int HistoryId { get; set; }

        [Display(Name = "Diagonisic")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public int diagonisicId { get; set; }

        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
    }
}
