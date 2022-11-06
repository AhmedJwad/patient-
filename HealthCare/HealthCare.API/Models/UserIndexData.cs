using HealthCare.API.Data.Entities;

namespace HealthCare.API.Models
{
    public class UserIndexData
    {
        public IEnumerable<User> Users { get; set; }
        public IEnumerable<UserPatient> UserPatients { get; set; }
        public IEnumerable<Patient> patients { get; set; }
        public IEnumerable<History> histories { get; set; }
        public IEnumerable<Detail> details { get; set; }
    }
}
