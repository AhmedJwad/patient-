namespace HealthCare.API.Models.Response
{
    public class DetailsResponse
    {
        public int Id { get; set; }             
       public string Description { get; set; }

        public diagonisicResponse diagonisic { get; set; }  
    }
}
