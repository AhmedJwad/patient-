using System.ComponentModel.DataAnnotations;

namespace HealthCare.API.Models.Response
{
    public class HitoryResponse
    {
        public int Id { get; set; }

        public string allergies { get; set; }
     

        public string illnesses { get; set; }

    
        public string surgeries { get; set; }

      
        public string Result { get; set; }

   
        public DateTime Date { get; set; }

      
        public DateTime DateLocal => Date.ToLocalTime();   


        public ICollection<DetailsResponse> Details { get; set; }

        [Display(Name = "# Details")]
        public int DetailsCount => Details == null ? 0 : Details.Count;
    }
}
