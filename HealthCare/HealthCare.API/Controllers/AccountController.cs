using HealthCare.API.Data;
using HealthCare.API.Data.Entities;
using HealthCare.API.Helpers;
using HealthCare.API.Migrations;
using HealthCare.API.Models;
using HealthCare.Common.Enums;
using HealthCare.Common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.API.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext _context;
        private readonly IuserHelper _userhelper;
        private readonly IBlobHelper _blobHelper;
        private readonly IMailHelper _mailHelper;
        private readonly ICombosHelper _combosHelper;

        public AccountController(DataContext context, IuserHelper userhelper , IBlobHelper blobHelper , 
            IMailHelper mailHelper , ICombosHelper combosHelper)
        {
            _context = context;
            _userhelper = userhelper;
            _blobHelper = blobHelper;
            _mailHelper = mailHelper;
           _combosHelper = combosHelper;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(Index), "Home");
            }

            return View(new LoginViewModel());
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _userhelper.LoginSync(model);
                if (result.Succeeded)
                {
                    if (Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(Request.Query["ReturnUrl"].First());
                    }

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Wrong email or password.");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userhelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult NotAuthorized()
        {
            return View();
        }


        public IActionResult Register()
        {
            AddUserViewModel model = new AddUserViewModel
            {
                Roles = _combosHelper.GetComboRoles(),
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AddUserViewModel model)
        {
            if(ModelState.IsValid)
            {
                Guid imageId = Guid.Empty;

                if(model.ImageFile !=null)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
                }
                User user = await _userhelper.GetUserAsync(model.Username);
                if (model.RoleId==1)
                {
                    user = await _userhelper.AddUserAsync(model, imageId, UserType.User);
                }
               else
                {
                    user = await _userhelper.AddUserAsync(model, imageId, UserType.patient);
                    var userpatient = new UserPatient
                    {
                        User = user,
                        Patients = new List<Patient>(),
                    };
                    _context.UserPatients.Add(userpatient);
                    await _context.SaveChangesAsync();
                }

                if(user==null)
                {
                    ModelState.AddModelError(string.Empty, "This email is already being used by another user.");
                    return View(model);
                }



                string myToken = await _userhelper.GenerateEmailConfirmationTokenAsync(user);
                string tokenLink = Url.Action("ConfirmEmail", "Account", new
                {
                    userid = user.Id,
                    token = myToken
                }, protocol: HttpContext.Request.Scheme);

                Response response = _mailHelper.SendMail(
                    $"{model.FirstName} {model.LastName}",
                    model.Username,
                    "HealthCare - Email Confirmation",
                    $"<h1>HealthCare - Email Confirmation</h1>" +
                        $"To enable the user please click on the following link:, " +
                        $"<p><a href = \"{tokenLink}\">Confirm Email</a></p>");
                if (response.IsSuccess)
                {
                    ViewBag.Message = "The instructions to enable the user have been sent to the mail.";
                    return View(model);
                }

                ModelState.AddModelError(string.Empty, response.Message);

            }
            return View(model);
        }

        public async Task<IActionResult> ChangeUser()
        {
            User user = await _userhelper.GetUserAsync(User.Identity.Name);
            if(user ==null)
            {
                return NotFound();
            }

            EditUserViewModel model = new()
            {
                Address=user.Address,
                FirstName=user.FirstName,
                LastName=user.LastName,
                PhoneNumber=user.PhoneNumber,
                CountryCode=user.CountryCode,
                ImageId=user.ImageId,
                Id=user.Id,
               
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUser(EditUserViewModel model)
        {
            if(ModelState.IsValid)
            {
                Guid imageId = model.ImageId;
                if(model.ImageFile !=null)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
                }
                User user = await _userhelper.GetUserAsync(User.Identity.Name);
                if(user==null)
                {
                    return NotFound();
                }
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.PhoneNumber = model.PhoneNumber;
                user.CountryCode = user.CountryCode;
                user.Address = model.Address;
                user.ImageId = imageId;
                await _userhelper.UpdateUserAsync(user);
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if(ModelState.IsValid)
            {
                User user=await _userhelper.GetUserAsync(User.Identity.Name);
                if (user != null)
                {
                    IdentityResult result = await _userhelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(ChangeUser));

                    } else
                    {
                        ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User not found.");
                }
                
            }
            return View(model); ;
        }
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            User user = await _userhelper.GetUserAsync(new Guid(userId));
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = await _userhelper.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return NotFound();
            }

            return View();
        }

        public IActionResult RecoverPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userhelper.GetUserAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "The email entered does not correspond to any user.");
                    return View(model);
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
                ViewBag.Message = "Instructions for changing your password have been sent to your email.";
                return View();

            }

            return View(model);
        }

        public IActionResult ResetPassword(string token)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            User user = await _userhelper.GetUserAsync(model.UserName);
            if (user != null)
            {
                IdentityResult result = await _userhelper.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    ViewBag.Message = "Account changed.";
                    return View();
                }

                ViewBag.Message = "Error changing password.";
                return View(model);
            }

            ViewBag.Message = "User not found.";
            return View(model);
        }
    }
}
