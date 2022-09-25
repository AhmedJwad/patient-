using AForge.Imaging.Filters;
using Aspose.Imaging;
using dotnetCHARTING;
using Emgu.CV.Structure;
using Emgu.CV;
using HealthCare.API.Data;
using HealthCare.API.Data.Entities;
using HealthCare.API.Helpers;
using HealthCare.API.Models;
using HealthCare.Common.Enums;
using Intersoft.Crosslight.UI.DataVisualization;
using LazZiya.ImageResize;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoyskleyTech.ImageProcessing.Image;
using Pango;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using System.Web.Helpers;
using VisioForge.Libs.MediaFoundation.OPM;
using Xamarin.Forms;
using DateTime = System.DateTime;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Rectangle = System.Drawing.Rectangle;
using System;
using Chart = System.Web.Helpers.Chart;
using VisioForge.Libs.DirectShowLib;

namespace HealthCare.API.Controllers
{
    [Authorize(Roles = "Admin , User")]
    public class UsersController : Controller
    {
        private readonly DataContext _context;
        private readonly IuserHelper _userhelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IconverterHelper _converterhleper;
        private readonly IBlobHelper _blobHelper;

        public UsersController(DataContext context, IuserHelper userhelper, ICombosHelper combosHelper,
            IconverterHelper converterhleper, IBlobHelper blobHelper)
        {
            _context = context;
            _userhelper = userhelper;
            _combosHelper = combosHelper;
            _converterhleper = converterhleper;
            _blobHelper = blobHelper;
        }


        public async Task<IActionResult> Patients()
        {
            User user = await _userhelper.GetUserAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Details), new { id = user.Id });
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.Include(x => x.Patients)
                .Where(x => x.userType == UserType.User).ToListAsync());

        }

        public IActionResult Create()
        {
            UserViewModel model = new()
            {
                Id = Guid.NewGuid().ToString(),

            };

            return View(model);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel model)
        {

            if (ModelState.IsValid)
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

        public async Task<IActionResult> Edit(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }
            User user = await _userhelper.GetUserAsync(Guid.Parse(Id));
            if (user == null)
            {
                return NotFound();
            }

            UserViewModel model = _converterhleper.ToUserViewModel(user);


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserViewModel model)
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
            if (string.IsNullOrEmpty(Id))
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

            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }


        public async Task<IActionResult> AddPatient(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }
            User user = await _context.Users.Include(x => x.Patients)
                .FirstOrDefaultAsync(x => x.Id == Id);
            if (user == null)
            {
                return NotFound();
            }
            patientViewmodel model = new patientViewmodel
            {
                UserId = user.Id,
                Date = DateTime.Now.Date,
                bloodTypes = _combosHelper.GetComboBloodtypes(),
                Cities = _combosHelper.GetCities(),
                Gendres = _combosHelper.Getgendres(),
                Nationaliteis = _combosHelper.GetNationalities(),
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
        public async Task<IActionResult> EditPatient(int Id, patientViewmodel model)
        {
            if (Id != model.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    Patient patient = await _converterhleper.ToPatientAsync(model, false);
                    _context.patients.Update(patient);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new { Id = model.UserId });
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
            if (Id == null)
            {
                return NotFound();
            }

            PatientPhoto patientPhoto = await _context.patientPhotos.Include(x => x.patient)
                .FirstOrDefaultAsync(x => x.Id == Id);
            if (patientPhoto == null)
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
            if (Id == null)
            {
                return NotFound();
            }

            Patient patient = await _context.patients.FirstOrDefaultAsync(x => x.Id == Id);
            if (patient == null)
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
            if (ModelState.IsValid)
            {
                Guid ImageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "patients");
                Patient patient = await _context.patients.Include(x => x.patientPhotos)
                    .FirstOrDefaultAsync(x => x.Id == model.PatientId);

                if (patient.patientPhotos == null)
                {
                    patient.patientPhotos = new List<PatientPhoto>();
                }
                patient.patientPhotos.Add(new PatientPhoto
                {
                    ImageId = ImageId,
                });

                _context.patients.Update(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(EditPatient), new { Id = patient.Id });

            }
            return View(model);
        }


        public async Task<IActionResult> DetailsPatient(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            Patient patient = await _context.patients.Include(x => x.User)
                .Include(x => x.bloodType).Include(x => x.Natianality)
                .Include(x => x.City).Include(x => x.gendre)
                .Include(x => x.patientPhotos).Include(x => x.histories).ThenInclude(h => h.Details)
                .ThenInclude(h => h.diagonisic)
                .Include(x => x.histories).ThenInclude(h => h.user).FirstOrDefaultAsync(x => x.Id == Id);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }

        public async Task<IActionResult> AddHistory(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            Patient patient = await _context.patients.FindAsync(Id);

            if (patient == null)
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
            if (ModelState.IsValid)
            {
                Patient patient = await _context.patients.Include(x => x.histories)
                    .FirstOrDefaultAsync(x => x.Id == model.PatientId);
                if (patient == null)
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
                if (patient.histories == null)
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

        public async Task<IActionResult> EditHistory(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            History history = await _context.histories.Include(x => x.patient)
                .FirstOrDefaultAsync(x => x.Id == Id);
            if (history == null)
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
        public async Task<IActionResult> EditHistory(int Id, HistoryViewmodel model)
        {
            if (ModelState.IsValid)
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
            if (Id == null)
            {
                return NotFound();
            }
            History history = await _context.histories.Include(x => x.patient)
                .ThenInclude(p => p.patientPhotos)
                .Include(x => x.Details).ThenInclude(p => p.diagonisic)
                .FirstOrDefaultAsync(x => x.Id == Id);
            if (history == null)
            {
                return NotFound();
            }
            return View(history);
        }

        public async Task<IActionResult> AddDetails(int? Id)
        {
            if (Id == null)
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

        public async Task<IActionResult> ConverttoRGB(int? Id )
        {
            double[,] red, green, blue;
            if (Id == null)
            {  
                return NotFound();
            }

            PatientPhoto patientPhoto = await _context.patientPhotos.Include(x => x.patient)
                .FirstOrDefaultAsync(x => x.Id == Id);
            if (patientPhoto == null)
            {
                return NotFound();
            }
            patientimageviewmodel model = new patientimageviewmodel
            {
                Id = patientPhoto.Id,
                ImageId = patientPhoto.ImageId.ToString(),
                patient = patientPhoto.patient,
            };

            System.Drawing.Bitmap rbmp, bmp;

            var httpClient = new HttpClient();
            var stream = await httpClient.GetStreamAsync(model.ImageFullPath);

            //var path0 =Image.FromFile(stream);           



            System.Drawing.Bitmap Almershady = new System.Drawing.Bitmap(stream);           
            bmp = new System.Drawing.Bitmap(Almershady, new System.Drawing.Size(350, 300));

            int w;
            int h;
            w = bmp.Width;
            h = bmp.Height;
            red = new double[w, h];
            green = new double[w, h];
            blue = new double[w, h];
            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < 256; j++)
                {
                    red[i, j] = bmp.GetPixel(i, j).R;
                    green[i, j] = bmp.GetPixel(i, j).G;
                    blue[i, j] = bmp.GetPixel(i, j).B;

                }
            }
            rbmp = new System.Drawing.Bitmap(w, h);
            System.Drawing.Bitmap gbmp = new System.Drawing.Bitmap(w, h);
            System.Drawing.Bitmap bbmp = new System.Drawing.Bitmap(w, h);
            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < 256; j++)
                {
                    rbmp.SetPixel(i, j, System.Drawing.Color.FromArgb((int)red[i, j], 0, 0));
                    gbmp.SetPixel(i, j, System.Drawing.Color.FromArgb(0, (int)red[i, j], 0));
                    bbmp.SetPixel(i, j, System.Drawing.Color.FromArgb(0, 0, (int)red[i, j]));

                }
            }

            rbmp.Save(Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\red" + System.Drawing.Imaging.ImageFormat.Png + ".jpg"));
            gbmp.Save(Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\green" + System.Drawing.Imaging.ImageFormat.Png + ".jpg"));
            bbmp.Save(Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\blue" + System.Drawing.Imaging.ImageFormat.Png + ".jpg"));
            // bmp.Save(Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\normal" + ImageFormat.Png + ".jpg"));
            string path = ($"images\\red" + System.Drawing.Imaging.ImageFormat.Png + ".jpg");
            string path1 = ($"images\\green" + System.Drawing.Imaging.ImageFormat.Png + ".jpg");
            string path2 = ($"images\\blue" + System.Drawing.Imaging.ImageFormat.Png + ".jpg");
            bmp.Save(Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\normal" + System.Drawing.Imaging.ImageFormat.Png + ".jpg"));
            string path4 = ($"wwwroot\\images\\normal" + System.Drawing.Imaging.ImageFormat.Png + ".jpg");
            model.rbmp = path;
            model.gbmp = path1;
            model.bbmp = path2;
            using (Aspose.Imaging.Image image = Aspose.Imaging.Image.Load(path4))
            {
                // Cast the image to RasterCachedImage and check if image is cached
                RasterCachedImage rasterCachedImage = (RasterCachedImage)image;
                if (!rasterCachedImage.IsCached)
                {
                    // Cache image if not already cached
                    rasterCachedImage.CacheData();
                }

                // Transform image to its binary representation
                rasterCachedImage.BinarizeFixed(100);

                // Save the image
                rasterCachedImage.Save(Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\blackandwhite" +System.Drawing.Imaging.ImageFormat.Png + ".jpg"));
            }
            string path5 = ($"images\\blackandwhite" + System.Drawing.Imaging.ImageFormat.Png + ".jpg");
            model.binaryimage = path5;
            System.Drawing.Bitmap temp = bmp;
            System.Drawing.Bitmap bmap = (System.Drawing.Bitmap)temp.Clone();
            System.Drawing.Color col;
            for (int i = 0; i < bmap.Width; i++)
            {
                for (int j = 0; j < bmap.Height; j++)
                {
                    col = bmap.GetPixel(i, j);
                    byte gray = (byte)(.299 * col.R + .587 * col.G + .114 * col.B);
                    bmap.SetPixel(i, j, System.Drawing.Color.FromArgb(gray, gray, gray));
                }
            }
            bmp = (System.Drawing.Bitmap)bmap.Clone();
            Random rnd = new Random();
            int a = rnd.Next();

            bmp.Save(Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\blackandwhite1" + System.Drawing.Imaging.ImageFormat.Png + ".jpg"));
            string path3 = ($"images\\blackandwhite1" + System.Drawing.Imaging.ImageFormat.Png + ".jpg");

            model.imagenormal = path3;

            //convert to binary
            System.Drawing.Bitmap Ahmed = _blobHelper.ToGrayscale(Almershady);
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(Almershady);
            BitmapData ImageData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            byte[] buffer = new byte[3 * bitmap.Width * bitmap.Height];
            IntPtr pointer = ImageData.Scan0;
            Marshal.Copy(pointer, buffer, 0, buffer.Length);
            for (int i = 0; i < bitmap.Height * 3 * bitmap.Width; i += 3)
            {
                byte b = buffer[i];
                byte g = buffer[i + 1];
                byte r = buffer[i + 2];
                byte grayscale = (byte)((r + g + b) / 3);
                if (grayscale < 128)
                {
                    buffer[i] = 0;
                    buffer[i + 1] = 0;
                    buffer[i + 2] = 0;
                }
                else
                {
                    buffer[i] = 255;
                    buffer[i + 1] = 255;
                    buffer[i + 2] = 255;
                }
            }

            string ByteString = Convert.ToString(buffer[20], 2).PadLeft(8, '0');
            Marshal.Copy(buffer, 0, pointer, buffer.Length);
            bitmap.UnlockBits(ImageData);
            bitmap.Save(Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\binaryimage" + System.Drawing.Imaging.ImageFormat.Png + ".jpg"));
            string path6 = ($"images\\binaryimage" + System.Drawing.Imaging.ImageFormat.Png + ".jpg");
            model.binaryorginale = path6;
            //end convert to binary
            //convert to 8 bit         

            // Almershady = new Bitmap(Almershady, new System.Drawing.Size(350, 300));

            byte[] imagetobinary = BitmapToByteArray(Almershady);
            string try1 = bytes2bin(imagetobinary);
            char [] try6 = try1.ToCharArray();
            string str = Convert.ToString(try1[20], 2).PadLeft(8, '0');

            char example = try1[0];
            char first = str[0];
            char second = str[1];
            char third = str[2];
            char foutth = str[3];
            char five = str[4];
            char six = str[5];
            char seven = str[6];
            char eight = str[7];

            //example change 
            for (int i = 0; i < try6.Length; i++)
            {
                try6[i] = (try6[i] == '0') ? '1' : '0';
            }
           
          

            string try7 = new string(try6);
            // Returns '0' for '1' and '1' for '0'
            first = (first == '0') ? '1' : '0';
            eight = (eight == '0') ? '1' : '0';
            //if (first == '0')
            //{
            //    first = '1';
            //}
            //else if(first == '1')
            //{
            //    first = '0';
            //}

            if (second == '0')
            {
                seven = str[6];

            }
            else if (second == '1')
            {
                seven = (seven == '0') ? '1' : '0';
            }
            if (third == '0')
            {
                six = str[5];

            }
            else if (second == '1')
            {
                six = (six == '0') ? '1' : '0';
            }
            if (six == '0')
            {
                third = str[2];
            }
            else if (six == '1')
            {
                third = (third == '0') ? '1' : '0';
            }
            if (seven == '0')
            {
                second = str[1];
            }
            else if (seven == '1')
            {
                second = (second == '0') ? '1' : '0';
            }
            if (first == '0')
            {
                foutth = str[3];
            }
            else if (first == '1')
            {
                foutth = (foutth == '0') ? '1' : '0';
            }
            if (eight == '0')
            {
                five = str[4];
            }
            else if (eight == '1')
            {
                five = (five == '0') ? '1' : '0';
            }
            var Ahmedjawad = $"{first}{second}{third}{foutth}{five}{six}{seven}{eight}";
            str = Ahmedjawad;
           
            // str = AhmedjawyourByteStringad ;           
            first = '0';
            string result = StringToBinary(str);
            var bytesAsStrings11 = result.Select((c, i) => new { Char = c, Index = i })
           .GroupBy(x => x.Index / 8)
           .Select(g => new string(g.Select(x => x.Char).ToArray()));
            byte[] bytes11 = bytesAsStrings11.Select(s => Convert.ToByte(s, 2)).ToArray();
            result = StringtoBinary(try1);
            var bytesAsStrings = result.Select((c, i) => new { Char = c, Index = i })
           .GroupBy(x => x.Index / 8)
           .Select(g => new string(g.Select(x => x.Char).ToArray()));
            byte[] bytes = bytesAsStrings.Select(s => Convert.ToByte(s, 2)).ToArray();

         
            int width1 = 800;
            int height1 = 600;            
            string bytetobitmap = "";


            SaveBitmap(bytetobitmap, Almershady.Width, Almershady.Height, bytes11);          
            System.Drawing.Bitmap newbmp = new System.Drawing.Bitmap(Almershady.Width, Almershady.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            // Create a BitmapData and lock all pixels to be written 
            BitmapData bmpData = newbmp.LockBits(
                                new Rectangle(0, 0, newbmp.Width, newbmp.Height),
                                ImageLockMode.WriteOnly, newbmp.PixelFormat);
            // Copy the data from the byte array into BitmapData.Scan0
            Marshal.Copy(bytes, 0, bmpData.Scan0, bytes.Length);

            // Unlock the pixels
            newbmp.UnlockBits(bmpData);

            // Do something with your image, e.g. save it to disc
            newbmp.Save(Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\orginalimage" + System.Drawing.Imaging.ImageFormat.Png + ".jpg"));
            string path8 = ($"images\\orginalimage" + System.Drawing.Imaging.ImageFormat.Png + ".jpg");
            string path9 = ($"images\\Png" + ".jpg");
            model.t = path8;
            model.scrabmle = path9;
            Ahmed.Save(Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\bit8" + System.Drawing.Imaging.ImageFormat.Png + ".jpg"));

            //end convert to 8 bit 
            //histogram
           
            var httpClient1 = new HttpClient();
            var stream1 = await httpClient.GetStreamAsync(model.Scramble);
            System.Drawing.Bitmap almershady2 = new System.Drawing.Bitmap(stream1);          
            Histogram(bmp);           
            Histogram2(Almershady);          
            HistogramEqualization equalization = new HistogramEqualization();
            equalization.ApplyInPlace(almershady2);
            almershady2.Save(Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\histogram" + System.Drawing.Imaging.ImageFormat.Png + ".jpg"));
            string path11 = ($"images\\histogram3" + System.Drawing.Imaging.ImageFormat.Png + ".jpg");
            string path12 = ($"images\\histogram2" + System.Drawing.Imaging.ImageFormat.Png + ".jpg");
            model.histgrame = path11;
            model.histgrameorginal=path12;
        
          

            //end histogram
            return View(model);
        }

       

        public void Histogram2(System.Drawing.Bitmap bmp)
        {
            
            int[] histogram_r = new int[256];
            float max = 0;

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    int redValue = bmp.GetPixel(i, j).R;
                    histogram_r[redValue]++;
                    if (max < histogram_r[redValue])
                        max = histogram_r[redValue];
                }
            }

            int histHeight = 128;
            System.Drawing.SolidBrush brush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            System.Drawing.Font font = new System.Drawing.Font("Lucida Console", 14, System.Drawing.FontStyle.Bold);

            System.Drawing.Bitmap img = new System.Drawing.Bitmap(256, histHeight + 10);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(img))
            {
                for (int i = 0; i < histogram_r.Length; i++)
                {
                    float pct = histogram_r[i] / max;   // What percentage of the max is this value?
                    g.DrawLine(Pens.Black,
                        new System.Drawing.Point(i, img.Height - 5),
                        new System.Drawing.Point(i, img.Height - 5 - (int)(pct * histHeight  ))  // Use that percentage of the height
                        );                 

                }
            }

           
           
          
            img.Save(Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\histogram3" + System.Drawing.Imaging.ImageFormat.Png + ".jpg"));
        }       


      
        public void Histogram(System.Drawing.Bitmap bmp)
        {         
          
            int[] histogram_r = new int[256];
            float max = 0;

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    int redValue = bmp.GetPixel(i, j).R;
                    histogram_r[redValue]++;
                    if (max < histogram_r[redValue])
                        max = histogram_r[redValue];
                }
            }

            int histHeight = 128;
            System.Drawing.Bitmap img = new System.Drawing.Bitmap(256, histHeight + 10);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(img))
            {
                for (int i = 0; i < histogram_r.Length; i++)
                {
                    float pct = histogram_r[i] / max;   // What percentage of the max is this value?
                    g.DrawLine(Pens.Black,
                        new System.Drawing.Point(i, img.Height - 5),
                        new System.Drawing.Point(i, img.Height - 5 - (int)(pct * histHeight ))  // Use that percentage of the height
                        );
                }
            }
            img.Save(Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\histogram2" + System.Drawing.Imaging.ImageFormat.Png + ".jpg"));
        }
   

       public static string StringtoBinary(string data)
        {
            string sb;

            sb = data;
            return sb.ToString();
        }
        public  string StringToBinary(string data)
        {
           
            StringBuilder sb = new StringBuilder();            
            foreach (char c in data.ToCharArray())
            {
                //26299
                for (int i = 0; i <=256 ; i++)
                {                   
                    for (int j = 0; j <=255;  j++)
                    {
                        sb.Append(Convert.ToString((c * j * 8), 2));
                    }            
                }
               
                                  

            }

            return sb.ToString();
        }
        static string PadBold(byte[] b)
        {
            string bin = Convert.ToString(b[20], 2);
            return new string('0', 8 - bin.Length) +  bin  ;
        }


        public void SaveBitmap(string fileName, int width, int height, byte[] imageData)
        {

            byte[] data = new byte[width * height * 4];

            int o = 0;

            for (int i = 0; i < width * height; i++)
            {
                byte value = imageData[i];


                data[o++] = value;
                data[o++] = value;
                data[o++] = value;
                data[o++] = 0;
            }

            unsafe
            {
                fixed (byte* ptr = data)
                {

                    using (System.Drawing. Bitmap image = new System.Drawing.Bitmap(width, height, width * 4,
                                PixelFormat.Format24bppRgb, new IntPtr(ptr)))
                    {

                        image.Save(Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\{fileName}" +System.Drawing.Imaging.ImageFormat.Png + ".jpg"));
                    }
                }
            }
        }
        public static byte[] BitmapToByteArray(System.Drawing.Bitmap bitmap)
        {

            BitmapData bmpdata = null;

            try
            {
                bmpdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                int numbytes = bmpdata.Stride * bitmap.Height;
                byte[] bytedata = new byte[numbytes];
                IntPtr ptr = bmpdata.Scan0;

                Marshal.Copy(ptr, bytedata, 0, numbytes);

                return bytedata;
            }
            finally
            {
                if (bmpdata != null)
                    bitmap.UnlockBits(bmpdata);
            }

        }
        public  string bytes2bin(byte[] bytes)
        {
            StringBuilder buffer = new StringBuilder(bytes.Length * 8);
            foreach (byte b in bytes)
            {
                buffer.Append(lookup[b]);
            }
            string binary = buffer.ToString();
            return binary;
        }

        static readonly string[] lookup = InitLookup();
        private static string[] InitLookup()
        {
            string[] instance = new string[1 + byte.MaxValue];
            StringBuilder buffer = new StringBuilder("00000000");
            for (int i = 0; i < instance.Length ; ++i)
            {

                buffer[0] = (char)('0' + ((i >> 7) & 1));
                buffer[1] = (char)('0' + ((i >> 6) & 1));
                buffer[2] = (char)('0' + ((i >> 5) & 1));
                buffer[3] = (char)('0' + ((i >> 4) & 1));
                buffer[4] = (char)('0' + ((i >> 3) & 1));
                buffer[5] = (char)('0' + ((i >> 2) & 1));
                buffer[6] = (char)('0' + ((i >> 1) & 1));
                buffer[7] = (char)('0' + ((i >> 0) & 1));

                instance[i] = buffer.ToString();
            }
            return instance ;
        }     
      
    }
}

