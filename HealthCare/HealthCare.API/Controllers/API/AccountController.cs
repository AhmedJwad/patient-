using HealthCare.API.Data;
using HealthCare.API.Data.Entities;
using HealthCare.API.Helpers;
using HealthCare.API.Models;
using HealthCare.API.Models.Request;
using HealthCare.Common.Enums;
using HealthCare.Common.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Sentry;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using User = HealthCare.API.Data.Entities.User;

namespace HealthCare.API.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IuserHelper _userhelper;
        private readonly IConfiguration _configuration;
        private readonly DataContext _context;
        private readonly IMailHelper _mailHelper;
        private readonly IBlobHelper _blobHelper;

        public AccountController(IuserHelper userhelper , IConfiguration configuration , DataContext context,
            IMailHelper mailHelper, IBlobHelper blobHelper)
        {
            _userhelper = userhelper;
           _configuration = configuration;
            _context = context;
            _mailHelper = mailHelper;
            _blobHelper = blobHelper;
        }
        [HttpPost]
        public async Task<ActionResult<User>>PostUser(RegisterRequest request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User user = await _userhelper.GetUserAsync(request.Email);
            if(user!=null)
            {
                return BadRequest("There is already a registered user with that email.");
            }
            Guid imageId = Guid.Empty;
            if(request.Image !=null && request.Image.Length > 0)
            {
                imageId = await _blobHelper.UploadBlobAsync(request.Image, "users");
            }

            user = new()
            {
                Address = request.Address,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                UserName = request.Email,
                ImageId = imageId,
               
            };

            if (request.RoleId==1)
            {
                user.userType = UserType.User;
                await _userhelper.AddUserAsync(user, request.Password);
                await _userhelper.AddUsertoRoleAsync(user, user.userType.ToString());
            }
            else if(request.RoleId==2)
            {
                user.userType = UserType.patient;
                await _userhelper.AddUserAsync(user, request.Password);
                await _userhelper.AddUsertoRoleAsync(user, user.userType.ToString());
                var userpatient = new UserPatient
                {
                    User = user,
                    Patients = new List<Patient>(),
                };
                _context.UserPatients.Add(userpatient);
                await _context.SaveChangesAsync();
            }
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

        

        [HttpPost]
        [Route("CreateToken")]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userhelper.GetUserAsync(model.Username);
                if (user != null)
                {
                    Microsoft.AspNetCore.Identity.SignInResult result = await _userhelper.ValidatePasswordAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        return CreateToken(user);
                    }
                }
            }

            return BadRequest();
        }

        private IActionResult CreateToken(User user)
        {
            Claim[] claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken token = new JwtSecurityToken(
                _configuration["Tokens:Issuer"],
                _configuration["Tokens:Audience"],
                claims,
                expires: DateTime.UtcNow.AddDays(99),
                signingCredentials: credentials);
            var results = new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                user
            };

            return Created(string.Empty, results);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if(ModelState.IsValid)
            {
                string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
                User user = await _userhelper.GetUserAsync(email);
                if(user != null)
                {
                    IdentityResult result = await _userhelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if(result.Succeeded)
                    {
                        return NoContent();
                    }
                    else
                    {
                        return BadRequest(result.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    return BadRequest("user no exist");
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPost]
        [Route ("RecoverPassword")]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if(ModelState.IsValid)
            {
                User user = await _userhelper.GetUserAsync(model.Email);
                if(user==null)
                {
                    return BadRequest("The email entered does not correspond to any user.");
                }
                string myToken = await _userhelper.GeneratePasswordResetTokenAsync(user);
                string link = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { token = myToken }, protocol: HttpContext.Request.Scheme);
                _mailHelper.SendMail($"{user.FullName} ",
                    model.Email, "HealthCare - Password Reset", $"<h1>HealthCare - Password Reset</h1>" +
                    $"To set a new password click on the following link:</br></br>" +
                    $"<a href = \"{link}\">Change of password</a>");
                return Ok("Instructions for changing your password have been sent to your email.");
            }
            return BadRequest(model);

        }

        [HttpPost]
        [Route("SocialLogin")]
        public async Task<IActionResult> SocialLogin(SocialLoginRequest model)
        {
            if(ModelState.IsValid)
            {
                User user= await _userhelper.GetUserAsync (model.Email); 
                if(user != null)
                {
                    if (user.loginType != model.LoginType)
                    {
                        return BadRequest("The user has already logged in previously by email or another social network");
                    }
                    Microsoft.AspNetCore.Identity.SignInResult result = await _userhelper.ValidatePasswordAsync(user, model.Id);
                    if (result.Succeeded)
                    {
                        await UpdateUserAsync(user, model);
                        return CreateToken(user);
                    }
                }                
                else
                {
                    await CreateUserAsync( model);
                    user = await _userhelper.GetUserAsync(model.Email);
                    return CreateToken(user);
                }
            }
            return BadRequest();
        }

        private async Task CreateUserAsync(SocialLoginRequest model)
        {
            FirstLastName firstLastName = SeparateFirstandLastName(model.FullName);
            if(string.IsNullOrEmpty(model.FirstName))
            {
                model.FirstName = firstLastName.FirsName;
            }
            if (string.IsNullOrEmpty(model.LastName))
            {
                model.LastName = firstLastName.LastName;
            }
            User user = new()
            {
                Address="Babylon",
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                loginType = model.LoginType,
                PhoneNumber = "022111544",
                SocialImageURL = model.PhotoURL,
                UserName = model.Email,
                userType = UserType.User,
            };
            await _userhelper.AddUserAsync(user, model.Id);
            await _userhelper.AddUsertoRoleAsync(user, user.userType.ToString());
            string token = await _userhelper.GenerateEmailConfirmationTokenAsync(user);
            await _userhelper.ConfirmEmailAsync(user, token);

        }

        private FirstLastName SeparateFirstandLastName(string fullName)
        {
            int pos=fullName.IndexOf(' ');
            FirstLastName firstLastName = new();
            if(pos==-1)
            {
                firstLastName.FirsName = fullName;
                firstLastName.LastName = fullName;
            }
            else
            {
                firstLastName.FirsName = fullName.Substring(0, pos);
                firstLastName.LastName = fullName.Substring(pos + 1, fullName.Length - pos - 1);
            }
            return firstLastName;
        }

        private async Task UpdateUserAsync(User user, SocialLoginRequest model)
        {
            user.SocialImageURL = model.PhotoURL;
            if(!string.IsNullOrEmpty(model.FirstName)) 
            {
                user.FirstName = model.FirstName;
            }
            if (!string.IsNullOrEmpty(model.LastName))
            {
                user.FirstName = model.LastName;
            }
            await _userhelper.UpdateUserAsync(user);
        }
    }
}
