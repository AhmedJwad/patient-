using Microsoft.Build.Framework;

namespace HealthCare.API.Models.Request
{
    public class PatientImageREquest
    {
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public byte[] Image { get; set; }
    }
}
