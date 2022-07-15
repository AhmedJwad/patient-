using HealthCare.API.Data;
using HealthCare.API.Data.Entities;
using HealthCare.API.Models;

namespace HealthCare.API.Helpers
{
    public class ConverterHelper : IconverterHelper
    {
        private readonly DataContext _context;
        private readonly ICombosHelper _combosHelper;

        public ConverterHelper(DataContext context  , ICombosHelper combosHelper)
        {
            _context = context;
           _combosHelper = combosHelper;
        }

      

        public async Task<User> ToUserAsync(UserViewModel model, Guid imageId, bool isNew)
        {
            return new User
            {
                Address=model.Address,  
                PhoneNumber=model.PhoneNumber,  
                Email=model.FirstName,
                LastName=model.LastName,
                ImageId=imageId,
                Id = isNew ? Guid.NewGuid().ToString() : model.Id,
                FirstName =model.FirstName,
                userType=model.UserType,    
                UserName=model.Email,

            };
        }

        public UserViewModel ToUserViewModel(User user)
        {
            return new UserViewModel
            {
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                Email = user.FirstName,
                LastName = user.LastName,
                ImageId = user.ImageId,
                Id = user.Id,
                FirstName = user.FirstName,
                UserType = user.userType,               
            };
        }
    }
}
