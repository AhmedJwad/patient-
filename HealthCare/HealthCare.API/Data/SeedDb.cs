using HealthCare.API.Data.Entities;
using HealthCare.API.Helpers;
using HealthCare.Common.Enums;

namespace HealthCare.API.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IuserHelper _userHelper;

        public SeedDb( DataContext context , IuserHelper userHelper )
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task SeedAsync( )
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckBloodTypeAsync();
            await CheckNationalityypeAsync();
            await CheckCitiesypeAsync();
            await CheckGendreAsync();
            await CheckRoleAsync();
            await CheckUserAsync("Ahmed", "Almershady", "Ahmednet380@gmail.com", "350 634 2747", "Babylon", UserType.Admin);
            await CheckUserAsync("Ahmed", "Almershady", "Ahmednet751@gmail.com", "350 634 2747", "Hilla", UserType.Admin);
            await CheckUserAsync("Ahmed", "Jwad", "Amm380@yahoo.com", "350 634 2747", "Baghdad", UserType.User);
            await CheckUserAsync("Ahmed", "Kadhum", "Ahmed@yopmail.com", "350 634 2747", "Babil", UserType.User);
            await CheckUserAsync("saad", "Ali", "Saad@yopmail.com", "350 634 2747", "Basrah", UserType.User);          


        }

        

        private async Task CheckUserAsync( string firstname ,string lastname , string email , string phonenumber, string address, UserType userType )
        {
            User user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {
                user = new User
                {
                    FirstName = firstname,
                    LastName = lastname,
                    Email = email,
                    PhoneNumber = phonenumber,
                    Address = address,                   
                    userType = userType,
                    UserName = email,                 
                   
                };
               await _userHelper.AddUserAsync(user, "123456");
               await _userHelper.AddUsertoRoleAsync(user, userType.ToString());
                string token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);
            }
            
        }

        private async Task CheckRoleAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
            await _userHelper.CheckRoleAsync(UserType.patient.ToString());
        }

        private async Task CheckCitiesypeAsync()
        {
            if (!_context.Cities.Any())
            {
                _context.Cities.Add(new City { Description = "Baghdad" });
                _context.Cities.Add(new City { Description = "Mosul" });
                _context.Cities.Add(new City { Description = "Basra" });
                _context.Cities.Add(new City { Description = "Nasiriyah" });
                _context.Cities.Add(new City { Description = "Hillah" });
                _context.Cities.Add(new City { Description = "Najaf" });
                _context.Cities.Add(new City { Description = "Kut" });
                _context.Cities.Add(new City { Description = "Diwaniyah	" });
                _context.Cities.Add(new City { Description = "Karbala" });
                _context.Cities.Add(new City { Description = "Amarah" });
                _context.Cities.Add(new City { Description = "Samawah" });
                await _context.SaveChangesAsync();
            }
        }

        private async Task CheckNationalityypeAsync()
        {
            if (!_context.natianalities.Any())
            {
                _context.natianalities.Add(new Natianality { Description = "Arabs" });
                _context.natianalities.Add(new Natianality { Description = "Kurds" });
                _context.natianalities.Add(new Natianality { Description = "Turkmens" });
                await _context.SaveChangesAsync();
            }
        }

        private async Task CheckBloodTypeAsync()
        {
            if (!_context.BloodTypes.Any())
            {
                _context.BloodTypes.Add(new BloodType { Description = "A+" });
                _context.BloodTypes.Add(new BloodType { Description = "A-" });
                _context.BloodTypes.Add(new BloodType { Description = "B+" });
                _context.BloodTypes.Add(new BloodType { Description = "B-" });
                _context.BloodTypes.Add(new BloodType { Description = "O+" });
                _context.BloodTypes.Add(new BloodType { Description = "O-" });
                _context.BloodTypes.Add(new BloodType { Description = "AB+" });
                _context.BloodTypes.Add(new BloodType { Description = "AB-" });
                await _context.SaveChangesAsync();
            }
        }
        private async Task CheckGendreAsync()
        {
            if (!_context.gendres.Any())
            {
                _context.gendres.Add(new gendre { Description = "Male" });
                _context.gendres.Add(new gendre { Description = "Female" });

                await _context.SaveChangesAsync();
            }
        }
    }
}
