using HealthCare.API.Data.Entities;
using HealthCare.API.Models;
using Microsoft.AspNetCore.Identity;

namespace HealthCare.API.Helpers
{
    public interface IuserHelper
    {
        Task<User> GetUserAsync(string email);
        Task<IdentityResult> AddUserAsync(User user, string password);

        Task CheckRoleAsync(string roleName);

        Task AddUsertoRoleAsync(User user , string roleName);

        Task<bool>IsUserinRoleAsync(User user, string roleName);

        Task<SignInResult> LoginSync(LoginViewModel model);
        Task LogoutAsync();
        
    }
}
