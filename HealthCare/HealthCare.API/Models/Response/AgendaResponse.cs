namespace HealthCare.API.Models.Response
{
    public class AgendaResponse
    {
        public int Id { get; set; }     
        public DateTime Date { get; set; }

  
        public string Description { get; set; }

     
        public bool IsAvailable { get; set; }
        public bool IsMine { get; set; }
    
        public DateTime DateLocal => Date.ToLocalTime();

       
    }
}
