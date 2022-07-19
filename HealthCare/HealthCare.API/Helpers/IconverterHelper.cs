using HealthCare.API.Data.Entities;
using HealthCare.API.Models;

namespace HealthCare.API.Helpers
{
    public interface IconverterHelper
    {
        Task<User> ToUserAsync(UserViewModel model, Guid imageId, bool isNew);

        UserViewModel ToUserViewModel(User user);
        Task<Patient> ToPatientAsync(patientViewmodel model, bool isNew);

        patientViewmodel ToPatientViewModel(Patient patient);

        Task<Detail> ToDetailAsync(DetailViewModel model, bool isNew);

        DetailViewModel ToDetailViewModel(Detail detail);
    }
}
