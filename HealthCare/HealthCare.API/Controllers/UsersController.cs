using HealthCare.API.Data;
using HealthCare.API.Data.Entities;
using HealthCare.API.Helpers;
using HealthCare.API.Models;
using HealthCare.Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.API.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly DataContext _context;
        private readonly IuserHelper _userhelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IconverterHelper _converterhleper;
        private readonly IBlobHelper _blobHelper;

        public UsersController(DataContext context , IuserHelper  userhelper, ICombosHelper combosHelper,
            IconverterHelper converterhleper , IBlobHelper blobHelper)
        {
            _context = context;
           _userhelper = userhelper;
            _combosHelper = combosHelper;
           _converterhleper = converterhleper;
            _blobHelper = blobHelper;
        }

        

        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.Include(x=>x.Patients)
                .Where(x => x.userType == UserType.User).ToListAsync());

        }

        public IActionResult Create( )
        {
           UserViewModel model = new()
            {
                Id = Guid.NewGuid().ToString(),
               
            };

            return View(model);
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( UserViewModel model)
        {
            
           if(ModelState.IsValid)
            {
                Guid imageId = Guid.Empty;
                if (model.ImageFile != null)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");

                }
                User user = await _converterhleper.ToUserAsync(model, imageId, true);             
                user.userType = UserType.User;
                await _userhelper.AddUserAsync(user, "123456");
                await _userhelper.AddUsertoRoleAsync(user, UserType.User.ToString());
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

       public async Task<IActionResult>Edit(string Id)
        {
            if(string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }
            User user = await _userhelper.GetUserAsync(Guid.Parse(Id));
            if (user== null)
            {
                return NotFound();
            }

            UserViewModel model = _converterhleper.ToUserViewModel(user);          
                
            
            return View(model); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> Edit(UserViewModel model)
        {
            
            if (ModelState.IsValid)
            {
                Guid imageId = model.ImageId;
                if (model.ImageFile != null)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
                }
                

                User user = await _converterhleper.ToUserAsync(model, imageId, false);
                await _userhelper.UpdateUserAsync(user);
                return RedirectToAction(nameof(Index));
            }

            
            return View(model);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            User user = await _userhelper.GetUserAsync(Guid.Parse(id));
            if (user == null)
            {
                return NotFound();
            }

            if (user.ImageId != Guid.Empty)
            {
                await _blobHelper.DeleteBlobAsync(user.ImageId, "users");
            }

            await _userhelper.DeleteUserAsync(user);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(string Id)
        {
            if(string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }
            User user = await _context.Users.Include(x => x.Patients)
                .ThenInclude(p => p.patientPhotos)
                .Include(x => x.Patients).ThenInclude(p => p.bloodType)
                .Include(x => x.Patients).ThenInclude(p => p.Natianality)
                .Include(x => x.Patients).ThenInclude(p => p.gendre)
                .Include(x => x.Patients).ThenInclude(p => p.City)
                .Include(x => x.Patients).ThenInclude(P => P.histories).FirstOrDefaultAsync(x => x.Id == Id);

            if(user== null)
            {
                return NotFound();
            }
            return View(user);
        }


        public async Task<IActionResult>AddPatient(string Id)
        {
            if(string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }
            User user = await _context.Users.Include(x => x.Patients)
                .FirstOrDefaultAsync(x => x.Id == Id);
            if(user == null)
            {
                return NotFound();
            }
            patientViewmodel model = new patientViewmodel
            {
                UserId = user.Id,
                Date = DateTime.Now.Date,
                bloodTypes= _combosHelper.GetComboBloodtypes(),
                Cities=_combosHelper.GetCities(),
                Gendres=_combosHelper.Getgendres(),
                Nationaliteis= _combosHelper.GetNationalities(),
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPatient(patientViewmodel patientViewmodel)
        {
           
                User user = await _context.Users
                .Include(x => x.Patients)
                .FirstOrDefaultAsync(x => x.Id == patientViewmodel.UserId);
                if (user == null)
                {
                    return NotFound();
                }

                Guid imageId = Guid.Empty;
                if (patientViewmodel.ImageFile != null)
                {
                    imageId = await _blobHelper.UploadBlobAsync(patientViewmodel.ImageFile, "patients");
                }

                Patient patient = await _converterhleper.ToPatientAsync(patientViewmodel, true);
                if (patient.patientPhotos == null)
                {
                    patient.patientPhotos = new List<PatientPhoto>();
                }

                patient.patientPhotos.Add(new PatientPhoto
                {
                    ImageId = imageId
                });

                try
                {
                    user.Patients.Add(patient);
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new { id = user.Id });
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un vehículo con esa placa.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }

           
            

            patientViewmodel.bloodTypes = _combosHelper.GetComboBloodtypes();
            patientViewmodel.Nationaliteis = _combosHelper.GetNationalities();
            patientViewmodel.Cities = _combosHelper.GetCities();
            patientViewmodel.Gendres = _combosHelper.Getgendres();
            return View(patientViewmodel);
        }

        public async Task<IActionResult> EditPatient(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            Patient patient = await _context.patients.Include(x => x.User)
                .Include(x => x.patientPhotos).Include(x => x.City).Include(x => x.bloodType)
                .Include(x => x.Natianality).Include(x => x.gendre).FirstOrDefaultAsync(x => x.Id == Id);
            if (patient == null)
            {
                return NotFound();
            }

            patientViewmodel model = _converterhleper.ToPatientViewModel(patient);
           
            return View(model);
              
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPatient(int Id , patientViewmodel model)
        {
            if(Id != model.Id)
            {
                return NotFound();
            }
            if(ModelState.IsValid)
            {
                try
                {                   

                  Patient patient = await _converterhleper.ToPatientAsync(model, false);                  
                    _context.patients.Update(patient);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details) , new {Id = model.UserId});
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un vehículo con esta placa.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }
            model.bloodTypes = _combosHelper.GetComboBloodtypes();
            model.Nationaliteis = _combosHelper.GetNationalities();
            model.Cities = _combosHelper.GetCities();
            model.Gendres = _combosHelper.Getgendres();
            return View(model);

        }
        public async Task<IActionResult> DeletePatient(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

           Patient patient = await _context.patients
                .Include(x => x.User)
                .Include(x => x.patientPhotos)
                .Include(x => x.histories)
                .ThenInclude(x => x.Details)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (patient == null)
            {
                return NotFound();
            }

            _context.patients.Remove(patient);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = patient.User.Id });
        }

    }
}
