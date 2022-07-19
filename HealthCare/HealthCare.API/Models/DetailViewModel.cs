using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HealthCare.API.Models
{
    public class DetailViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        public int HistoryId { get; set; }

        [Display(Name = "diagonisic")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a diagonisic.")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public int diagonisicId { get; set; }

        public IEnumerable<SelectListItem> diagonisics { get; set; }
    }
}
