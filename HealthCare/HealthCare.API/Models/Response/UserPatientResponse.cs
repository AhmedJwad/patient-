using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace HealthCare.API.Models.Response
{
    public class UserPatientResponse
    {     

        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }      

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

      
    }
}
