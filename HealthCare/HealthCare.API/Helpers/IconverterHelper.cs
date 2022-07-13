using HealthCare.API.Data.Entities;
using HealthCare.API.Models;

namespace HealthCare.API.Helpers
{
    public interface IconverterHelper
    {
        Task<User> ToUserAsync(UserViewModel model, Guid imageId, bool isNew);

        UserViewModel ToUserViewModel(User user);
    }
}
