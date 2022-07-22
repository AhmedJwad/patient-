using HealthCare.API.Data;
using HealthCare.API.Data.Entities;
using HealthCare.API.Helpers;
using HealthCare.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.API.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext _context;
        private readonly IuserHelper _userhelper;
        private readonly IBlobHelper _blobHelper;

        public AccountController(DataContext context, IuserHelper userhelper , IBlobHelper blobHelper)
        {
            _context = context;
            _userhelper = userhelper;
            _blobHelper = blobHelper;
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
            AddUserViewModel modeel = new AddUserViewModel
            {

            };
            return View(modeel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Register(AddUserViewModel model)
        {
            if(ModelState.IsValid)
            {
                Guid imageId = Guid.Empty;

                if(model.ImageFile !=null)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
                }
                User user = await _userhelper.AddUserAsync(model, imageId, model.UserType);

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

                Response response = _mailHelper.SendMail(model.Username, "Vehicles - Confirmación de cuenta", $"<h1>Vehicles - Confirmación de cuenta</h1>" +
                    $"Para habilitar el usuario, " +
                    $"por favor hacer clic en el siguiente enlace: </br></br><a href = \"{tokenLink}\">Confirmar Email</a>");
                if (response.IsSuccess)
                {
                    ViewBag.Message = "Las instrucciones para habilitar su cuenta han sido enviadas al correo.";
                    return View(model);
                }

                ModelState.AddModelError(string.Empty, response.Message);
            }
            return View(model);
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
    }
}
