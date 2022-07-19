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
                        ModelState.AddModelError(string.Empty, "patient already exist in database");
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

        public async Task<IActionResult> DeleteImagePatient(int? Id)
        {
            if(Id == null)
            {
                return NotFound();
            }

            PatientPhoto patientPhoto = await _context.patientPhotos.Include(x => x.patient)
                .FirstOrDefaultAsync(x => x.Id == Id);
            if(patientPhoto == null)
            {
                return NotFound();
            }
            try
            {
                await _blobHelper.DeleteBlobAsync(patientPhoto.ImageId, "patients");
            }
            catch (Exception)
            { }
            _context.patientPhotos.Remove(patientPhoto);    
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(EditPatient), new { id = patientPhoto.patient.Id });           

        }

        public async Task<IActionResult> AddPatientImage(int? Id)
        {
            if(Id == null)
            {
                return NotFound();
            }

            Patient patient = await _context.patients.FirstOrDefaultAsync(x => x.Id == Id);
            if(patient == null)
            {
                return NotFound();
            }
            PatientPhotoViewModel model = new PatientPhotoViewModel
            {
                PatientId = patient.Id,

            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPatientImage(PatientPhotoViewModel model)
        {
          if(ModelState.IsValid)
            {
                Guid ImageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "patients");
                Patient patient = await _context.patients.Include(x => x.patientPhotos)
                    .FirstOrDefaultAsync(x => x.Id == model.PatientId);

                if(patient.patientPhotos==null)
                {
                    patient.patientPhotos =new List<PatientPhoto>();
                }
                patient.patientPhotos.Add(new PatientPhoto
                {
                    ImageId=ImageId,
                });

                _context.patients.Update(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(EditPatient), new { Id = patient.Id });

            }
          return View(model);   
        }


        public async Task<IActionResult> DetailsPatient(int? Id)
        {
            if(Id==null)
            {
                return NotFound();
            }

            Patient patient = await _context.patients.Include(x => x.User)
                .Include(x => x.bloodType).Include(x => x.Natianality)
                .Include(x => x.City).Include(x => x.gendre)
                .Include(x=>x.patientPhotos).Include(x=>x.histories).ThenInclude(h=>h.Details)
                .ThenInclude(h=>h.diagonisic)
                .Include(x=>x.histories).ThenInclude(h=>h.user).FirstOrDefaultAsync(x => x.Id == Id);
            if(patient==null)
            {
                return NotFound();
            }
            return View(patient);
        }

        public async Task<IActionResult> AddHistory(int? Id)
        {
            if(Id==null)
            {
                return NotFound();
            }

            Patient patient = await _context.patients.FindAsync(Id);

            if(patient == null)
            {
                return NotFound();
            }

            HistoryViewmodel model = new HistoryViewmodel
            {
                PatientId = patient.Id,
                Date = DateTime.Now,
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddHistory(HistoryViewmodel model)
        {
            if(ModelState.IsValid)
            {
                Patient patient = await _context.patients.Include(x => x.histories)
                    .FirstOrDefaultAsync(x => x.Id == model.PatientId);
                if(patient==null)
                {

                    return NotFound();
                }

                User user = await _userhelper.GetUserAsync(User.Identity.Name);

                History history = new History
                {
                    illnesses = model.illnesses,
                    allergies = model.allergies,
                    surgeries = model.surgeries,
                    Result = model.Result,
                    Date = model.Date,
                    user = user,
                };
                if(patient.histories== null)
                {
                    patient.histories = new List<History>();
                }

                patient.histories.Add(history);
                _context.patients.Update(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(DetailsPatient), new { id = model.PatientId });
            }
            return View(model);
        }

        public async Task<IActionResult>EditHistory(int? Id)
        {
            if(Id==null)
            {
                return NotFound();
            }

            History history = await _context.histories.Include(x => x.patient)
                .FirstOrDefaultAsync(x => x.Id == Id);
            if(history == null)
            {
                return NotFound();
            }

            HistoryViewmodel model = new HistoryViewmodel
            {
                PatientId = history.patient.Id,
                illnesses = history.illnesses,
                surgeries = history.surgeries,
                allergies = history.allergies,
                Result = history.Result,
                Date = history.Date,
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditHistory(int Id , HistoryViewmodel model)
        {
            if(ModelState.IsValid)
            {
                History history = await _context.histories.FindAsync(Id);
                history.illnesses = model.illnesses;
                history.allergies = model.allergies;
                history.surgeries = model.surgeries;
                history.Result = model.Result;
                history.Date = model.Date;
                _context.histories.Update(history);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(DetailsPatient), new { id = model.PatientId });
            };
            return View(model);
        }
        public async Task<IActionResult> DeleteHistory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            History history = await _context.histories
                .Include(x => x.Details)
                .Include(x => x.patient)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (history == null)
            {
                return NotFound();
            }

            _context.histories.Remove(history);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DetailsPatient), new { id = history.patient.Id });
        }

        public async Task<IActionResult> DetailsHistory(int? Id)
        {
            if(Id==null)
            {
                return NotFound();
            }
            History history = await _context.histories.Include(x => x.patient)
                .ThenInclude(p => p.patientPhotos)
                .Include(x => x.Details).ThenInclude(p => p.diagonisic)
                .FirstOrDefaultAsync(x => x.Id == Id);
            if(history == null)
            {
                return NotFound();
            }
            return View(history);
        }

        public async Task<IActionResult>AddDetails(int? Id)
        {
            if(Id==null)
            {
                return NotFound();
            }
            History history = await _context.histories.FindAsync(Id);
            DetailViewModel model = new DetailViewModel
            {
                HistoryId = history.Id,
                diagonisics = _combosHelper.GetCombodiagnosic(),
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDetails(DetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                History history = await _context.histories
                    .Include(x => x.Details)
                    .FirstOrDefaultAsync(x => x.Id == model.HistoryId);
                if (history == null)
                {
                    return NotFound();
                }

                if (history.Details == null)
                {
                    history.Details = new List<Detail>();
                }

                Detail detail = await _converterhleper.ToDetailAsync(model, true);
                history.Details.Add(detail);
                _context.histories.Update(history);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(DetailsHistory), new { id = model.HistoryId });
            }

            model.diagonisics = _combosHelper.GetCombodiagnosic();
            return View(model);
        }

        public async Task<IActionResult> EditDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Detail detail = await _context.details
                .Include(x => x.History)
                .Include(x => x.diagonisic)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (detail == null)
            {
                return NotFound();
            }

            DetailViewModel model = _converterhleper.ToDetailViewModel(detail);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDetails(int id, DetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                Detail detail = await _converterhleper.ToDetailAsync(model, false);
                _context.details.Update(detail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(DetailsHistory), new { id = model.HistoryId });
            }

            model.diagonisics = _combosHelper.GetCombodiagnosic();
            return View(model);
        }

        public async Task<IActionResult> DeleteDetail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Detail detail = await _context.details
                .Include(x => x.History)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (detail == null)
            {
                return NotFound();
            }

            _context.details.Remove(detail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DetailsHistory), new { id = detail.History.Id });
        }
    }
}
