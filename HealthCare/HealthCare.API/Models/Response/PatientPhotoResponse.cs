using HealthCare.API.Data.Entities;
using Newtonsoft.Json;

namespace HealthCare.API.Models.Response
{
    public class PatientPhotoResponse
    {
        public int Id { get; set; }
        public Guid ImageId { get; set; }
        public string imageFullPath { get; set; }
    }
}
