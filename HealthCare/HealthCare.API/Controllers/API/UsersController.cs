using HealthCare.API.Data;
using HealthCare.API.Data.Entities;
using HealthCare.API.Helpers;
using HealthCare.API.Models.Request;
using HealthCare.API.Models.Response;
using HealthCare.Common.Enums;
using HealthCare.Common.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sentry;
using System.Xml.Linq;
using User = HealthCare.API.Data.Entities.User;

namespace HealthCare.API.Controllers.API
{
    [ApiController]
   [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IuserHelper _userhelper;
        private readonly IMailHelper _mailHelper;
        private readonly IBlobHelper _blobHelper;

        public UsersController(DataContext context, IuserHelper userhelper, IMailHelper mailHelper, IBlobHelper blobHelper)
        {
            _context = context;
            _userhelper = userhelper;
            _mailHelper = mailHelper;
            _blobHelper = blobHelper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.OrderBy(x => x.FirstName).ThenBy(x => x.LastName).Where
                (x => x.userType == UserType.User).ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            var response = await _context.Users
                .Include(x => x.Patients)
                .ThenInclude(x => x.City)
                .Include(x => x.Patients)
                .ThenInclude(x => x.Natianality)
                .Include(x => x.Patients)
                .ThenInclude(x => x.bloodType)
                .Include(x => x.Patients)
                .ThenInclude(x => x.gendre)
                .Include(x => x.Patients)
                .ThenInclude(x => x.patientPhotos)
                .Include(x => x.Patients)
                .ThenInclude(x => x.histories)
                .ThenInclude(x => x.Details)
                .ThenInclude(x => x.diagonisic)  
                .Include(x=>x.Patients).ThenInclude(x=>x.userPatient).ThenInclude(x=>x.User)
               .FirstOrDefaultAsync(x => x.Id == id);
            var user = new userResponse
            {
                id= response.Id,
                firstName= response.FirstName,
                lastName= response.LastName,
                address= response.Address,
                userType= response.userType,
                imageId= response.ImageId,
                phoneNumber = response.PhoneNumber,    
                email=response.Email,
                userName = response.UserName,
                patients= response.Patients?.Select(p=> new Patientresponse
               {
                   Id=p.Id,
                   FirstName=p.FirstName,
                   LastName=p.LastName,
                   Date=p.Date,
                   Description=p.Description,
                   Address=p.Address,
                   MobilePhone=p.MobilePhone,
                   EPCNNumber=p.EPCNNumber,
                   bloodType=tobloodType(p.bloodType),                   
                   City=tocity(p.City),
                   gendre=togendre(p.gendre),
                   Natianality=tonatinality(p.Natianality),
                   userPatient=touserpatient(p.userPatient),
                   patientPhotos=p.patientPhotos?.Select(pi=> new PatientPhotoResponse
                   {
                       Id=pi.Id,
                       ImageId = pi.ImageId,
                       imageFullPath = pi.ImageFullPath,
                   }).ToList(),
                   histories=p.histories?.Select(h=> new HitoryResponse
                   {
                       Id=p.Id,
                       illnesses=h.illnesses,
                       allergies=h.allergies,
                       Date=h.Date,
                       Result=h.Result,
                       surgeries=h.surgeries,
                       Details=h.Details?.Select(d=> new DetailsResponse
                       {
                          Id=d.Id,
                          Description=d.Description,
                           diagonisic = todiagnoiscresponse(d.diagonisic)
                           
                       }).ToList(),
                   }).ToList(),
               }).ToList(),
            };
            
          
          
            return Ok(user);
        }

        private diagonisicResponse todiagnoiscresponse(diagonisic diagonisic)
        {
            return new diagonisicResponse
            {
                Id = diagonisic.Id,
                Description = diagonisic.Description
            };
        }

        private UserPatientResponse touserpatient(UserPatient UserPatient)
        {
            return new UserPatientResponse
            {
            Id = UserPatient.Id,
            FirstName = UserPatient.User.FirstName,
            LastName = UserPatient.User.LastName,
            Address = UserPatient.User.Address,
            PhoneNumber = UserPatient.User.PhoneNumber,
            Email = UserPatient.User.Email,       
                          
           };
        }

        private NatianalityResponse tonatinality(Natianality natianality)
        {
            return new NatianalityResponse
            {
                Id= natianality.Id,
                Description=natianality.Description,    
            };
        }

        private gendreResponse togendre(gendre gendre)
        {
            return new gendreResponse
            {
                Id = gendre.Id,
                Description = gendre.Description,
            };
        }

        private CityResponce tocity(City city)
        {
            return new CityResponce
            {
                Id=city.Id,
                Description=city.Description,
            };
        }

        private BloodTypeResponse tobloodType(BloodType bloodType)
        {
            return new BloodTypeResponse
            {
                Id = bloodType.Id,
                Description = bloodType.Description,
            };
        }

        [HttpPost]

        public async Task<ActionResult<User>> PostUser(UserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User user = await _userhelper.GetUserAsync(request.Email);
            if (user != null)
            {
                return BadRequest("There is already a registered user with that email.");
            }

            Guid imageId = Guid.Empty;

            if (request.Image != null && request.Image.Length > 0)
            {
                imageId = await _blobHelper.UploadBlobAsync(request.Image, "users");
            }

            user = new User
            {
                Address = request.Address,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                ImageId = imageId,
                PhoneNumber = request.PhoneNumber,
                UserName = request.Email,
                userType = UserType.User,              
            };
            await _userhelper.AddUserAsync(user, "123456");
            await _userhelper.AddUsertoRoleAsync(user, UserType.User.ToString());
            string myToken = await _userhelper.GenerateEmailConfirmationTokenAsync(user);
            string tokenLink = Url.Action("ConfirmEmail", "Account", new
            {
                userid = user.Id,
                token = myToken
            }, protocol: HttpContext.Request.Scheme);

            _mailHelper.SendMail(
                      $"{user.FirstName} {user.LastName}",
                      user.UserName,
                      "HealthCare - Email Confirmation",
                      $"<h1>HealthCare - Email Confirmation</h1>" +
                          $"To enable the user please click on the following link:, " +
                          $"<p><a href = \"{tokenLink}\">Confirm Email</a></p>");
            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<User>> PutUser(string id, UserRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User user = await _userhelper.GetUserAsync(Guid.Parse(request.Id));

            if (user == null)
            {
                return BadRequest("There is no user.");
            }

            Guid imageId = user.ImageId;
            if (request.Image != null && request.Image.Length > 0)
            {
                imageId = await _blobHelper.UploadBlobAsync(request.Image, "users");
            }
            user.Address = request.Address;
            user.FirstName = request.FirstName;
            user.ImageId = imageId;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;          
            await _userhelper.UpdateUserAsync(user);
            return Ok(user);

        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            User user = await _context.Users
                .Include(x=>x.Patients)
                .ThenInclude(p=>p.patientPhotos)
                .Include(x=>x.Patients)
                .ThenInclude(p=>p.histories)
                .ThenInclude(h=>h.Details).FirstOrDefaultAsync(x => x.Id == id);
            if(user.ImageId!= Guid.Empty)
            {
                await _blobHelper.DeleteBlobAsync(user.ImageId, "users");
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    
    }
}
