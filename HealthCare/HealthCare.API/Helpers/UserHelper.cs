using HealthCare.API.Data;
using HealthCare.API.Data.Entities;
using HealthCare.API.Models;
using HealthCare.Common.Enums;
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

        public async Task<User> AddUserAsync(AddUserViewModel model, Guid imageId, UserType userType)
        {
            User user = new User
            {
                Address = model.Address,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Username,
                UserName = model.Username,
                PhoneNumber = model.PhoneNumber,
                ImageId = imageId,
                userType = userType,

            };
            IdentityResult result = await _userManager.CreateAsync(user, model.Password);
            if(!result.Succeeded)
            {
                return null;
            }

            User newuser = await GetUserAsync(model.Username);
            await AddUsertoRoleAsync(newuser, user.userType.ToString());
            return newuser;

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

        public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<IdentityResult> DeleteUserAsync(User user)
        {
            return await _userManager.DeleteAsync(user);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<User> GetUserAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User> GetUserAsync(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == id.ToString());
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

        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            User currentuser = await GetUserAsync(user.Email);

            currentuser.FirstName = user.FirstName;
            currentuser.LastName = user.LastName;
            currentuser.Address = user.Address;
            currentuser.PhoneNumber = user.PhoneNumber;
            currentuser.ImageId = user.ImageId;

            return await _userManager.UpdateAsync(currentuser);

        }
    }
}
