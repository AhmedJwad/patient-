using HealthCare.API.Data;
using HealthCare.API.Data.Entities;
using HealthCare.API.Helpers;
using HealthCare.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace HealthCare.API.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext _context;
        private readonly IuserHelper _userhelper;

        public HomeController(ILogger<HomeController> logger, DataContext context,IuserHelper userhelper)
        {
            _logger = logger;
           _context = context;
           _userhelper = userhelper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("error/404")]
        public IActionResult Error404()
        {
            return View();
        }
        [Authorize(Roles = "patient")]
        public async Task<IActionResult> UserPatients()
        {
            User user = await _userhelper.GetUserAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(DetailsUserPatient), new { id = user.Id });
        }

        [Authorize(Roles = "patient")]
        public async Task<IActionResult> DetailsUserPatient(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }
            User user = await _context.Users.Include(x => x.UserPatients).ThenInclude(x => x.Patients)
                .ThenInclude(p => p.patientPhotos)
                .Include(x => x.UserPatients).ThenInclude(x => x.Patients).ThenInclude(p => p.bloodType)
                .Include(x => x.UserPatients).ThenInclude(x => x.Patients).ThenInclude(p => p.Natianality)
                .Include(x => x.UserPatients).ThenInclude(x => x.Patients).ThenInclude(p => p.gendre)
                .Include(x => x.UserPatients).ThenInclude(x => x.Patients).ThenInclude(p => p.City)
                .Include(x => x.UserPatients).ThenInclude(x => x.Patients).ThenInclude(P => P.histories).ThenInclude(x=>x.Details).ThenInclude(x=>x.diagonisic).FirstOrDefaultAsync(x => x.Id == Id);

            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

    }
}