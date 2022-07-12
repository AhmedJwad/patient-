using HealthCare.API.Data;
using HealthCare.API.Data.Entities;
using HealthCare.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.API.Helpers
{
    public class UserHelper : IuserHelper
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _context;
        private readonly SignInManager<User> _signInManager;

        public UserHelper(UserManager<User> userManager ,RoleManager<IdentityRole> roleManager,
            DataContext context , SignInManager<User> signInManager)
        {
           _userManager = userManager;
           _roleManager = roleManager;
           _context = context;
           _signInManager = signInManager;
        }

       

        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);    
        }

        public async Task AddUsertoRoleAsync(User user, string roleName)
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task CheckRoleAsync(string roleName)
        {
            bool roleExist = await _roleManager.RoleExistsAsync(roleName);
            if(!roleExist)
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = roleName });
            }

        }

        public async Task<User> GetUserAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<bool> IsUserinRoleAsync(User user, string roleName)
        {
           return await _userManager.IsInRoleAsync(user, roleName); 
        }

        public async Task<SignInResult> LoginSync(LoginViewModel model)
        {
            return await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
