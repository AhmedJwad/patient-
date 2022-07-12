using HealthCare.API.Data;
using HealthCare.API.Helpers;
using HealthCare.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.API.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext _context;
        private readonly IuserHelper _userhelper;

        public AccountController(DataContext context, IuserHelper userhelper)
        {
            _context = context;
            _userhelper = userhelper;
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


    }
}
