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
using Intersoft.Crosslight;
using Emgu.CV.OCR;
using System.Diagnostics;
using MessagePack.Formatters;
using VisioForge.MediaFramework.FFMPEGCore.Arguments;
using VisioForge.Libs.ZXing.QrCode.Internal;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using SixLabors.ImageSharp;
using VisioForge.Libs.ZXing.Common;
using HealthCare.Common.Models;
using AForge.Math.Geometry;

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
            //NPCR         
            var httpClient2 = new HttpClient();
            var stream2 = await httpClient.GetStreamAsync(model.Scramble);               
            System.Drawing.Bitmap scramble2 = new System.Drawing.Bitmap(stream2);          
            int Tollerance = 0;
            model.NPCR = CompareImages(scramble2, bmp , Tollerance);
            //End NPCR 
            // Entropy           
            byte[] imagetobyte1 = BitmapToByteArray(bmp);
            byte[] imagetobyte2 = BitmapToByteArray(Almershady);
            double orginal = Entropy(imagetobyte1);
            double scramble = Entropy(imagetobyte2);
           
            model.Entropyorginal = orginal;
            model.Entropyscample = scramble;
            //end  Entropy           
            //chaotic
            chaotic(Almershady);
            //end chaotic
            //generate image k1 k2 k3 
            var k = $"{0}{0}{1}{1}{1}{1}{0}{1}";
            string randomimage= k;
           string newimage = StringToBinary(randomimage);
            var bytesAsStrings12 = newimage.Select((c, i) => new { Char = c, Index = i })
           .GroupBy(x => x.Index / 8)
           .Select(g => new string(g.Select(x => x.Char).ToArray()));
            byte[] bytes12 = bytesAsStrings11.Select(s => Convert.ToByte(s, 2)).ToArray();
            int width = 350;
            int height = 300;
            SaveBitmap2(bytetobitmap, width, height, bytes12);
            string pathgenerateimage = ($"images\\randomimagePng" + ".jpg");
            model.generateimage = pathgenerateimage;
            var httpClient3 = new HttpClient();
            var stream3 = await httpClient.GetStreamAsync(model.Generateimage);
            System.Drawing.Bitmap generateimagebitmap = new System.Drawing.Bitmap(stream3);
            changerrowtocolumn(generateimagebitmap);
            string pathgenerateimagebitmap= ($"images\\changerowtocolumnPng" + ".jpg");
            model.changerowandcolumnimage = pathgenerateimagebitmap;
            var httpClient4 = new HttpClient();
            var stream4 = await httpClient.GetStreamAsync(model.Changerowandcolumnimage);
            System.Drawing.Bitmap changerowandcolumn = new System.Drawing.Bitmap(stream4);           
            System.Drawing.Bitmap Xorimageoperation = BitwiseBlend(Almershady, changerowandcolumn,
                  BitwiseBlendType.Xor, BitwiseBlendType.Xor
                                     , BitwiseBlendType.Xor);
            Xorimageoperation.Save(Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\Xorimageoperation" + System.Drawing.Imaging.ImageFormat.Png + ".jpg"));
            string Xorpath = ($"images\\XorimageoperationPng" + ".jpg");
            model.xorbetweenscrambledimageandkimage = Xorpath;           
            //end generate image 

            //coffecient correlation 
            var httpClient5 = new HttpClient();
            var stream5 = await httpClient.GetStreamAsync(model.Scramble);
            System.Drawing.Bitmap corellationcoffecient1 = new System.Drawing.Bitmap(stream5);
            byte[] imagetobyte3 = BitmapToByteArray(corellationcoffecient1);
            var httpClient6 = new HttpClient();
            var stream6 = await httpClient.GetStreamAsync(model.ImageFullPath);
            System.Drawing.Bitmap corellationcoffecient2 = new System.Drawing.Bitmap(stream6);
            byte[] imagetobyte4 = BitmapToByteArray(corellationcoffecient2);
            int[] bytesAsInts = imagetobyte3.Select(y => (int)y).ToArray();
            int[] bytesAsInts1 = imagetobyte4.Select(y => (int)y).ToArray();
            int n=bytesAsInts.Length;            
            float Ahmed22 = correlationCoefficienthorizontal(bytesAsInts, bytesAsInts1,   n);
            float Ahmed23 = correlationCoefficientVertical(bytesAsInts, bytesAsInts1, n);
            double[] corr = toDoubleArray(imagetobyte3);
            double[] corr1 = toDoubleArray(imagetobyte4);
            double corr2 = Correlation(corr, corr1);
            model.corrhorizontal = Ahmed22;
            model.corrvertical = Ahmed23;
            model.corrdiagnol = corr2;
            //coffecient correlation  for saramble image and xorimage
            var httpClient10 = new HttpClient();
            var stream10 = await httpClient10.GetStreamAsync(model.Xorimage);
            System.Drawing.Bitmap corellationcoffecient10 = new System.Drawing.Bitmap(stream10);
            byte[] imagetobyte10 = BitmapToByteArray(corellationcoffecient10);
            int[] bytesAsIntsxor = imagetobyte10.Select(x => (int)x).ToArray();
            float Ahmed24 = correlationCoefficienthorizontal(bytesAsInts, bytesAsIntsxor, n);
            float Ahmed25 = correlationCoefficientVertical(bytesAsInts, bytesAsIntsxor, n);
            float Ahmed26 = correlationCoefficientxor(bytesAsInts, bytesAsIntsxor, n);
            double[] corr4 = toDoubleArray1(imagetobyte10);
            double corr6 = Correlation1(corr, corr4);
            model.corrhorizontalxorimage = Ahmed24;
            model.corrverticalxorimage = Ahmed25;
            model.corrdiagnolxorimage = corr6;
            //end coffecient correlation  for saramble image and xorimage 
            //end coffecient correlation

            //histgrame between scramble image and xoroperationimageand kimage 
            var httpClient12 = new HttpClient();
                    var stream12 = await httpClient12.GetStreamAsync(model.Xorimage);
                    System.Drawing.Bitmap oxrimage = new System.Drawing.Bitmap(stream12);
                    Histogram3(oxrimage);
                    string histogramXorimagePng = ($"images\\histogramxorimagePng" + ".jpg");
                    model.histogramXorimagePng = histogramXorimagePng;
            //end histgrame between scramble image and xoroperationimageand kimage 
            //entropy between Scramble image and xoroperation image 
            byte[] xorimage = BitmapToByteArray(oxrimage);
            double scramble1 = Entropy(imagetobyte2);
            double oximage = Entropy2(xorimage);
            model.Xorentropy = oximage;
            //end entropy between Scramble image and xoroperation image 

            //NPCR between scramble image and Xorimage 
            int Tollerance1 = 0;
            model.NPCRXorimage= CompareImages(oxrimage, scramble2,  Tollerance1);
            //End NPCR between scramble image and Xorimage 
            //chaotic
            chaotic5D(scramble2);
            string pathchaotic5d = ($"images\\choatic5dPng" + ".jpg");
            model.chaotic5d = pathchaotic5d;
            //end chaotic
            //generate image from scramble image and key image
            System.Drawing.Bitmap generateImage =HealthCare.API.Models.ColorCalculator.ArithmeticBlend(Almershady, changerowandcolumn,
                                    ColorCalculationType.Amplitude);
            generateImage.Save(Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\generateImagefromscambleandkeyimage" + System.Drawing.Imaging.ImageFormat.Png + ".jpg"));
            string generateImagefromscambleandkeyimage = ($"images\\generateImagefromscambleandkeyimagePng" + ".jpg");
            model.generateImagefromscarmableandkeyimage = generateImagefromscambleandkeyimage;
            //end generate image from scramble image and key image
            //SwapColorsRGB
            var httpClient13 = new HttpClient();
            var stream13 = await httpClient13.GetStreamAsync(model.GenerateImagefromscarmableandkeyimage);
            System.Drawing.Bitmap SwapColorsRGB = new System.Drawing.Bitmap(stream13);
            byte fixedValue = 0;
            System.Drawing.Bitmap SwapColorsRGBtoserver = SwapColors(SwapColorsRGB, ColourSwapType.SwapBlueAndGreenFixRed
                , ColourSwapType.SwapBlueAndRedFixGreen, ColourSwapType.SwapBlueAndRed, ColourSwapType.SwapRedAndGreenFixBlue, fixedValue);
            SwapColorsRGBtoserver.Save(Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\SwapColorsRGBtoserver" + System.Drawing.Imaging.ImageFormat.Png + ".jpg"));
            string SwapColorsRGBtoserverstring = ($"images\\SwapColorsRGBtoserverPng" + ".jpg");
            model.swappingARGB = SwapColorsRGBtoserverstring;
            //EndSwapColorsRGB
            return View(model);
        }
     public static System.Drawing.Bitmap SwapColors(System.Drawing.Bitmap sourceImage,ColourSwapType swapType                                   
                  , ColourSwapType swapType2 , ColourSwapType swapType3, ColourSwapType swapType4, byte fixedValue = 0)
        {
            List<ArgbPixel> pixelListSource = GetPixelListFromBitmap(sourceImage);


            List<ArgbPixel> pixelListResult = null;


            switch (swapType)
            {
                case ColourSwapType.ShiftRight:
                    {
                        pixelListResult = (from t in pixelListSource
                                           select new ArgbPixel
                                           {
                                               blue = t.red,
                                               red = t.green,
                                               green = t.blue,
                                               alpha = t.alpha
                                           }).ToList();
                        break;
                    }
                case ColourSwapType.ShiftLeft:
                    {
                        pixelListResult = (from t in pixelListSource
                                           select new ArgbPixel
                                           {
                                               blue = t.green,
                                               red = t.blue,
                                               green = t.red,
                                               alpha = t.alpha
                                           }).ToList();
                        break;
                    }
                case ColourSwapType.SwapBlueAndRed:
                    {
                        pixelListResult = (from t in pixelListSource
                                           select new ArgbPixel
                                           {
                                               blue = t.red,
                                               red = t.blue,
                                               green = t.green,
                                               alpha = t.alpha
                                           }).ToList();
                        break;
                    }

         case ColourSwapType.SwapBlueAndRedFixGreen:
                    {
                        pixelListResult = (from t in pixelListSource
                                           select new ArgbPixel
                                           {
                                               blue = t.red,
                                               red = t.blue,
                                               green = fixedValue,
                                               alpha = t.alpha
                                           }).ToList();
                        break;
                    }
                case ColourSwapType.SwapBlueAndGreen:
                    {
                        pixelListResult = (from t in pixelListSource
                                           select new ArgbPixel
                                           {
                                               blue = t.green,
                                               red = t.red,
                                               green = t.blue,
                                               alpha = t.alpha
                                           }).ToList();
                        break;
                    }
                case ColourSwapType.SwapBlueAndGreenFixRed:
                    {
                        pixelListResult = (from t in pixelListSource
                                           select new ArgbPixel
                                           {
                                               blue = t.green,
                                               red = fixedValue,
                                               green = t.blue,
                                               alpha = t.alpha
                                           }).ToList();
                        break;
                    }
                case ColourSwapType.SwapRedAndGreen:
                    {
                        pixelListResult = (from t in pixelListSource
                                           select new ArgbPixel
                                           {
                                               blue = t.blue,
                                               red = t.green,
                                               green = t.red,
                                               alpha = t.alpha
                                           }).ToList();
                        break;
                    }
                case ColourSwapType.SwapRedAndGreenFixBlue:
                    {
                        pixelListResult = (from t in pixelListSource
                                           select new ArgbPixel
                                           {
                                               blue = fixedValue,
                                               red = t.green,
                                               green = t.red,
                                               alpha = t.alpha
                                           }).ToList();
                        break;
                    }
            }


           System.Drawing.Bitmap resultBitmap = GetBitmapFromPixelList(pixelListResult,
                                    sourceImage.Width, sourceImage.Height);


            return resultBitmap;
        }
        private static List<ArgbPixel> GetPixelListFromBitmap(System.Drawing.Bitmap sourceImage)
        {
            BitmapData sourceData = sourceImage.LockBits(new Rectangle(0, 0,
                        sourceImage.Width, sourceImage.Height),
                        ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);


            byte[] sourceBuffer = new byte[sourceData.Stride * sourceData.Height];
            Marshal.Copy(sourceData.Scan0, sourceBuffer, 0, sourceBuffer.Length);
            sourceImage.UnlockBits(sourceData);


            List<ArgbPixel> pixelList = new List<ArgbPixel>(sourceBuffer.Length / 4);


            using (MemoryStream memoryStream = new MemoryStream(sourceBuffer))
            {
                memoryStream.Position = 0;
                BinaryReader binaryReader = new BinaryReader(memoryStream);


                while (memoryStream.Position + 4 <= memoryStream.Length)
                {
                    ArgbPixel pixel = new ArgbPixel(binaryReader.ReadBytes(4));
                    pixelList.Add(pixel);
                }


                binaryReader.Close();
            }


            return pixelList;
        }
        private static System.Drawing.Bitmap GetBitmapFromPixelList(List<ArgbPixel> pixelList, int width, int height)
        {
            System.Drawing.Bitmap resultBitmap = new System.Drawing.Bitmap(width, height, PixelFormat.Format32bppArgb);


            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
                        resultBitmap.Width, resultBitmap.Height),
                        ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);


            byte[] resultBuffer = new byte[resultData.Stride * resultData.Height];


            using (MemoryStream memoryStream = new MemoryStream(resultBuffer))
            {
                memoryStream.Position = 0;
                BinaryWriter binaryWriter = new BinaryWriter(memoryStream);


                foreach (ArgbPixel pixel in pixelList)
                {
                    binaryWriter.Write(pixel.GetColorBytes());
                }


                binaryWriter.Close();
            }


            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
            resultBitmap.UnlockBits(resultData);


            return resultBitmap;
        }
        static float correlationCoefficientxor(int[] X, int[] Y,
                                                   int n)
        {
            int sum_X = 0, sum_Y = 0, sum_XY = 0;
            int squareSum_X = 0, squareSum_Y = 0;

            for (int i = 0; i < n; i++)
            {
                // sum of elements of array X.
                sum_X = sum_X + X[i];

                // sum of elements of array Y.
                sum_Y = sum_Y + Y[i];

                // sum of X[i] * Y[i].
                sum_XY = sum_XY + X[i] * Y[i];

                // sum of square of array elements.
                squareSum_X = squareSum_X + X[i] * X[i];
                squareSum_Y = squareSum_Y + Y[i] * Y[i];
            }

            // use formula for calculating correlation 
            // coefficient.
            float corr = (float)(n * sum_XY - sum_X * sum_Y) /
                         (float)(Math.Sqrt((n * squareSum_X -
                         sum_X * sum_X) * (n * squareSum_Y -
                         sum_Y * sum_Y)));                        

            return corr;
        }
        public static double[] toDoubleArray1(byte[] byteArr)
        {
            double[ ] arr = new double[byteArr.Length ];
            for (int i = 0; i < 256; i++)
            {
                arr[i ] = byteArr[i];
            }
            return arr;
        }
        public static double[] toDoubleArray(byte[] byteArr)
        {
            double[] arr = new double[byteArr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = byteArr[i];
            }
            return arr;
        }

        public double Correlation1(double[] array1, double[] array2)
        {
            double[] array_xy = new double[array1.Length];
            double[] array_xp2 = new double[array1.Length];
            double[] array_yp2 = new double[array1.Length];
            for (int i = 0; i < array1.Length; i++)
                array_xy[i] = array1[i] * array2[i];
            for (int i = 0; i < array1.Length; i++)
                array_xp2[i] = Math.Pow(array1[i], 2.00);
            for (int i = 0; i < array1.Length; i++)
                array_yp2[i] = Math.Pow(array2[i], 2.00);
            double sum_x = 0.0;
            double sum_y = 0.0;
            foreach (double n in array1)
                sum_x += n;
            foreach (double n in array2)
                sum_y += n;
            double sum_xy = 0;
            foreach (double n in array_xy)
                sum_xy += n;
            double sum_xpow2 = 0;
            foreach (double n in array_xp2)
                sum_xpow2 += n;
            double sum_ypow2 = 0;
            foreach (double n in array_yp2)
                sum_ypow2 += n;
            double Ex2 = Math.Pow(sum_x, 2.00);
            double Ey2 = Math.Pow(sum_y, 2.00);

            return (array1.Length * sum_xy - sum_x * sum_y) /
                   Math.Sqrt((array1.Length * sum_xpow2 - Ex2) * (array1.Length * sum_ypow2 - Ey2))*10;
        }
        public double Correlation(double[] array1, double[] array2)
        {
            double[] array_xy = new double[array1.Length];
            double[] array_xp2 = new double[array1.Length];
            double[] array_yp2 = new double[array1.Length];
            for (int i = 0; i < array1.Length; i++)
                array_xy[i] = array1[i] * array2[i];
            for (int i = 0; i < array1.Length; i++)
                array_xp2[i] = Math.Pow(array1[i], 2.00);
            for (int i = 0; i < array1.Length; i++)
                array_yp2[i] = Math.Pow(array2[i], 2.00);
            double sum_x = 0.0;
            double sum_y = 0.0;
            foreach (double n in array1)
                sum_x += n;
            foreach (double n in array2)
                sum_y += n;
            double sum_xy = 0;
            foreach (double n in array_xy)
                sum_xy += n;
            double sum_xpow2 = 0;
            foreach (double n in array_xp2)
                sum_xpow2 += n;
            double sum_ypow2 = 0;
            foreach (double n in array_yp2)
                sum_ypow2 += n;
            double Ex2 = Math.Pow(sum_x, 2.00);
            double Ey2 = Math.Pow(sum_y, 2.00);

            return (array1.Length * sum_xy - sum_x * sum_y) /
                   Math.Sqrt((array1.Length * sum_xpow2 - Ex2) * (array1.Length * sum_ypow2 - Ey2))*10;
        }
       public static float correlationCoefficienthorizontal(int[] X1, int[] X2 ,
                                                   int n)
        {
            double sum_X = 0.0, sum_Y = 0.0, sum_XY = 0.0;
            double squareSum_X = 0.0, squareSum_Y = 0.0;

            for (int i = 0; i < n; i++)
            {
                // sum of elements of array X.
                sum_X = sum_X + X1[i];

                // sum of elements of array Y.
               // sum_Y = sum_Y + Y[i];

                // sum of X[i] * Y[i].
                sum_XY = sum_XY + X1[i] * X2[i];

                // sum of square of array elements.
                squareSum_X = squareSum_X + X1[i] * X1[i];
               // squareSum_Y = squareSum_Y + Y[i] * Y[i];
            }

            // use formula for calculating correlation 
            // coefficient.
            float corr = (float)(n * sum_XY - sum_X * sum_X) /
                         (float)(Math.Sqrt((n * squareSum_X -
                         sum_X * sum_X) * (n * squareSum_X -
                         sum_X * sum_X)));
                         corr = corr /10;         
            return corr;
        }
       public static float correlationCoefficientVertical(int[] Y1, int[] Y2,
                                                   int n)
        {
            double sum_X = 0.0, sum_Y = 0.0, sum_XY = 0.0;
            double squareSum_X = 0.0, squareSum_Y = 0.0;

            for (int i = 0; i < n; i++)
            {
                // sum of elements of array X.
                //sum_X = sum_X + X[i];

                // sum of elements of array Y.
                sum_Y = Y2[i] + Y1[i];

                // sum of X[i] * Y[i].
                sum_XY = sum_XY + Y2[i] * Y1[i];

                // sum of square of array elements.
                //squareSum_X = squareSum_X + X[i] * X[i];
                squareSum_Y = squareSum_Y + Y1[i] * Y1[i];
            }

            // use formula for calculating correlation 
            // coefficient.
            float corr = (float)(n * sum_XY - sum_Y * sum_Y) /
                        (float)(Math.Sqrt((n * squareSum_Y -
                        sum_Y * sum_Y) * (n * squareSum_Y -
                        sum_Y * sum_Y)));
                        corr = corr / 10;

            return corr;
        }
        
        public static System.Drawing.Bitmap BitwiseBlend(System.Drawing.Bitmap sourceBitmap, System.Drawing.Bitmap blendBitmap,
                                    BitwiseBlendType blendTypeBlue, BitwiseBlendType
                                     blendTypeGreen, BitwiseBlendType blendTypeRed)
        {
            BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0,
                                    sourceBitmap.Width, sourceBitmap.Height),
                                    ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);


            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            sourceBitmap.UnlockBits(sourceData);


            BitmapData blendData = blendBitmap.LockBits(new Rectangle(0, 0,
                                    blendBitmap.Width, blendBitmap.Height),
                                    ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);


            byte[] blendBuffer = new byte[blendData.Stride * blendData.Height];
            Marshal.Copy(blendData.Scan0, blendBuffer, 0, blendBuffer.Length);
            blendBitmap.UnlockBits(blendData);


            int blue = 0, green = 0, red = 0;


            for (int k = 0; (k + 4 < pixelBuffer.Length) &&
                            (k + 4 < blendBuffer.Length); k += 4)
            {
              
                if (blendTypeBlue == BitwiseBlendType.Xor)
                {
                    blue = pixelBuffer[k] ^ blendBuffer[k];
                }


                if (blendTypeGreen == BitwiseBlendType.Xor)
                {
                    green = pixelBuffer[k + 1] ^ blendBuffer[k + 1];
                }
                              
                if (blendTypeRed == BitwiseBlendType.Xor)
                {
                    red = pixelBuffer[k + 2] ^ blendBuffer[k + 2];
                }


                if (blue < 0)
                { blue = 0; }
                else if (blue > 255)
                { blue = 255; }


                if (green < 0)
                { green = 0; }
                else if (green > 255)
                { green = 255; }


                if (red < 0)
                { red = 0; }
                else if (red > 255)
                { red = 255; }


                pixelBuffer[k] = (byte)blue;
                pixelBuffer[k + 1] = (byte)green;
                pixelBuffer[k + 2] = (byte)red;
            }


            System.Drawing.Bitmap resultBitmap = new System.Drawing.Bitmap(sourceBitmap.Width, sourceBitmap.Height);


            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
                                    resultBitmap.Width, resultBitmap.Height),
                                    ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);


            Marshal.Copy(pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length);
            resultBitmap.UnlockBits(resultData);


            return resultBitmap;
        }

        public void changerrowtocolumn(System.Drawing.Bitmap bmp)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = bmpData.Stride * bmp.Height;
            byte[] rgbValues = new byte[bytes];
            byte[] r = new byte[bytes / 3];
            byte[] g = new byte[bytes / 3];
            byte[] b = new byte[bytes / 3];

            // Copy the RGB values into the array.
            Marshal.Copy(ptr, rgbValues, 0, bytes);

            int count = 0;
            int stride = bmpData.Stride;

            for (int column = 0; column < bmpData.Height; column++)
            {
                for (int row = 0; row < bmpData.Width; row++)
                {
                    b[count] = (byte)(rgbValues[(column * stride) + (row * 3)]);
                    g[count] = (byte)(rgbValues[(column * stride) + (row * 3) + 1]);
                    r[count++] = (byte)(rgbValues[(column * stride) + (row * 3) + 2]);
                }
            }
            bmp.Save(Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\changerowtocolumn" + System.Drawing.Imaging.ImageFormat.Png + ".jpg"));
        }
        public void chaotic5D(System.Drawing.Bitmap bmp)
        {
            int a = 6, b = 6, f = 6;
            double d = 0.1,e = 0.1;
            double x = 0.99;
            double y = 0.99;
            double z = 0.99;

            double[] x1 = new double[a];
            double[] x2 = new double[b];
            double[] x3 = new double[f];
            double[] x4 = new double[(int)d];
            double[] x5 = new double[(int)e];
            int ih = bmp.Height;
            int iw = bmp.Width;
            System.Drawing.Color c = new System.Drawing.Color();
            for (int i = 0; i < 100; i++)
            {
                x = 6 - 0.1 * 0.1 - 6 + 6 * 6;
                y = 6 - 0.1 * 6 * 6 + 6 * 6;
                z = 0.1 - 6 * 6 - 6 + 6 * 6;

                //x = 4 * x * (1 - x);
            }
           
            for (int j = 0; j < ih; j++)
            {
                for (int i = 0; i < iw; i++)
                {
                    c = bmp.GetPixel(i, j);
                    int chaos = 0, g;
                    string k ;
                    for (int w = 0; w < 8; w++)
                    {
                        x = 6 - 0.1 * 0.1 - 6 + 6 * 6;
                        y =  6 - 0.1 * 6 * 6 + 6 * 6; ;
                        z =  0.1 - 6 * 6 - 6 + 6 * 6; ;
                       // x = 4 * x * (1 - x);
                        if (x  >= 0.5 || y>=0.5 || z>=0.5) g  = 1;
                        else g = 0;
                        
                        chaos = ((chaos << 1) | g);
                    }
                    //c = 
                    bmp.SetPixel(i, j, System.Drawing.Color.FromArgb(c.R ^ (int)chaos,
                                                     c.G ^ (int)chaos,
                                                     c.B ^ (int)chaos));
                }
            }
            bmp.Save(Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\choatic5d" + System.Drawing.Imaging.ImageFormat.Png + ".jpg"));

        }
        public void chaotic(System.Drawing.Bitmap bmp)
        {
            double x = 0.99;
            int ih = bmp.Height;
            int iw = bmp.Width;
            System.Drawing.Color c = new System.Drawing.Color();
            for (int i = 0; i < 100; i++)
            {
                x = 4 * x * (1 - x);
            }
            for (int j = 0; j < ih; j++)
            {
                for (int i = 0; i < iw; i++)
                {
                    c = bmp.GetPixel(i, j);
                    int chaos = 0, g;
                    string k;
                    for (int w = 0; w < 8; w++)
                    {
                        x = 4 * x * (1 - x);
                        if (x >= 0.5) g = 1;
                        else g = 0;
                        k = Guid.NewGuid().ToString("n");
                        chaos = ((chaos << 1) | g);
                    }
                    //c = 
                    bmp.SetPixel(i, j, System.Drawing.Color.FromArgb(c.R ^ (int)chaos,
                                                     c.G ^ (int)chaos,
                                                     c.B ^ (int)chaos));
                }
            }

        }
        public  double CompareImages(System.Drawing.Bitmap InputImage1, System.Drawing.Bitmap InputImage2, int Tollerance)
        {
          
            System.Drawing.Bitmap Image1 = new System.Drawing.Bitmap(InputImage1, new System.Drawing.Size(350, 300));
            System.Drawing.Bitmap Image2 = new System.Drawing.Bitmap(InputImage2, new System.Drawing.Size(350,300));
            int Image1Size = Image1.Width * Image1.Height;
            int Image2Size = Image2.Width * Image2.Height;
            System.Drawing.Bitmap Image3;
            if (Image1Size > Image2Size)
            {
                Image1 = new System.Drawing.Bitmap(Image1, Image2.Size);
                Image3 = new System.Drawing.Bitmap(Image2.Width, Image2.Height);
            }
            else
            {
                Image1 = new System.Drawing.Bitmap(Image1, Image2.Size);
                Image3 = new System.Drawing.Bitmap(Image2.Width, Image2.Height);
            }
            for (int x = 0; x < Image1.Width; x++)
            {
                for (int y = 0; y < Image1.Height; y++)
                {
                    System.Drawing.Color Color1 = Image1.GetPixel(x, y);
                    System.Drawing.Color Color2 = Image2.GetPixel(x, y);
                    int r = Color1.R > Color2.R ? Color1.R - Color2.R : Color2.R - Color1.R;
                    int g = Color1.G > Color2.G ? Color1.G - Color2.G : Color2.G - Color1.G;
                    int b = Color1.B > Color2.B ? Color1.B - Color2.B : Color2.B - Color1.B;                     
                    Image3.SetPixel(x, y, System.Drawing.Color.FromArgb(r, g, b));
                }
            }
            int Difference = 0;
            for (int x = 0; x < Image1.Width; x++)
            {
                for (int y = 0; y < Image1.Height; y++)
                {
                    System.Drawing.Color Color1 = Image3.GetPixel(x, y);
                    int Media = (Color1.R + Color1.G + Color1.B )/3;
                    if (Media > Tollerance)
                        Difference++;                       
                }
                
            }            
            
            double UsedSize = Image1Size > Image2Size ? Image2Size : Image1Size;
            double result = Difference * 100 / UsedSize;
            return result; 
        }
        
        public static unsafe Double Entropy2(byte[] data)
        {
            int* rgi = stackalloc int[0x100], pi = rgi + 0x100;

            for (int i = data.Length; --i >= 0;)
                rgi[data[i]]++;

            Double H = 0.0, cb = data.Length;
            while (--pi >= rgi)
                if (*pi > 0)                
                        H += *pi * Math.Log(*pi / cb, 1.773);                             
             return -H / cb;
        }
        public static unsafe Double Entropy(byte[] data)
        {
            int* rgi = stackalloc int[0x100], pi = rgi + 0x100;

            for (int i = data.Length; --i >= 0;)
                rgi[data[i]]++;

            Double H = 0.0, cb = data.Length;
            while (--pi >= rgi)
                if (*pi > 0)
                    H += *pi * Math.Log(*pi / cb, 1.963);

            return -H / cb;
        }
        public void Histogram3(System.Drawing.Bitmap bmp)
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
                        new System.Drawing.Point(i, img.Height - 5 - (int)(pct * histHeight))  // Use that percentage of the height
                        );

                }
            }




            img.Save(Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\histogramxorimage" + System.Drawing.Imaging.ImageFormat.Png + ".jpg"));
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

        public void SaveBitmap2(string fileName, int width, int height, byte[] imageData)
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

                    using (System.Drawing.Bitmap image = new System.Drawing.Bitmap(width, height, width * 4,
                                PixelFormat.Format24bppRgb, new IntPtr(ptr)))
                    {

                        image.Save(Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\randomimage" + System.Drawing.Imaging.ImageFormat.Png + ".jpg"));
                    }
                }
            }
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

