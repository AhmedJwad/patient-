using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace HealthCare.API.Data.Entities
{
    public class UserPatient
    {
        public int Id { get; set; }

   
        public User User { get; set; }
         [JsonIgnore]      
        public ICollection<Patient> Patients { get; set; }

        [Display(Name = "# Patients")]
        public int PatientsCount => Patients == null ? 0 : Patients.Count;
    }
}
