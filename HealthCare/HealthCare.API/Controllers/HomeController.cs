using HealthCare.API.Data;
using HealthCare.API.Data.Entities;
using HealthCare.API.Helpers;
using HealthCare.API.Migrations;
using HealthCare.API.Models;
using Intersoft.Crosslight;
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
        public async Task<IActionResult> DetailsUserPatient(string Id  )
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
            int id = 0;

            if (id != null)
            {
                ViewData["DetailsID"] = id;         
            }
            
            return View(user);
        }

        [Authorize(Roles = "patient")]
        public async Task<IActionResult> MyAgenda()
        {

            User user = await _userhelper.GetUserAsync(User.Identity.Name);

            var agenda = await _context.agendas.Include(x => x.user).Include(x => x.pathient)
                .Where(a => a.Date >= DateTime.Today)
                .ToListAsync();

            var list = new List<AgendaViewModel>(agenda.Select(a => new AgendaViewModel
            {
                Date = a.Date,
                Id = a.Id,
                IsAvailable = a.IsAvailable,
                user = a.user,
                pathient = a.pathient,
                Description = a.Description,


            })).ToList();
            list.Where(a => a.user != null && a.user.Email.ToLower().Equals(User.Identity.Name.ToLower())
            ).All(a => { a.IsMine = true; return true; });


            return View(list);
        }
        [Authorize(Roles = "patient")]
        public async Task<IActionResult> Assing(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            Agenda agenda = await _context.agendas.Include(x => x.user)
                .FirstOrDefaultAsync(x => x.Id == Id);
            UserPatient userpationt = await _context.UserPatients.Include(u => u.Patients)
                  .FirstOrDefaultAsync(u => u.User.Email.ToLower().Equals(User.Identity.Name.ToLower()));
            if (agenda == null)
            {
                return NotFound();
            }
            AgendaViewModel model = new AgendaViewModel
            {

                UserId = agenda.user.Id,
                Date = DateTime.Now.Date,
                patientId = userpationt.Patients.FirstOrDefault().Id,
                IsMine = true,

            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assing(AgendaViewModel model)
        {
            UserPatient userpationt = await _context.UserPatients.Include(u => u.Patients)
                .FirstOrDefaultAsync(u => u.User.Email.ToLower().Equals(User.Identity.Name.ToLower()));

            var agenda = await _context.agendas.FindAsync(model.Id);
            if (agenda != null)
            {
                agenda.IsAvailable = false;
                agenda.user = await _context.Users.FindAsync(model.UserId);
                agenda.pathient = await _context.patients.FindAsync(userpationt.Patients.FirstOrDefault().Id);
                agenda.Description = model.Description;
                _context.agendas.Update(agenda);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyAgenda));
            }


            return View(model);
        }


    }
}