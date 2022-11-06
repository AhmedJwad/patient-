using HealthCare.API.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HealthCare.API.Models
{
    public class AgendaViewModel:Agenda
    {
        [Display(Name = "Doctor")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select aDoctor.")]

        public string UserId { get; set; }
        public IEnumerable<SelectListItem> Users { get; set; }

        [Display(Name = "Patient")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Patient.")]

        public int patientId { get; set; }
        public IEnumerable<SelectListItem> patients { get; set; }
        public bool IsMine { get; set; }
        public string Reserved => "Reserved";
    }
}
