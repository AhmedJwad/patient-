﻿using Aspose.Imaging;
using Braintree;
using Emgu.CV.Structure;
using Emgu.CV;
using Grpc.Core;
using Gst;
using HealthCare.API.Data;
using HealthCare.API.Data.Entities;
using HealthCare.API.Helpers;
using HealthCare.API.Models;
using HealthCare.Common.Enums;
using ImageProcessor.Imaging.Helpers;
using LazZiya.ImageResize;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoyskleyTech.ImageProcessing.Form.Control;
using SixLabors.ImageSharp.Formats.Tiff;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using VisioForge.Libs.AForge.Imaging.Filters;
using VisioForge.Libs.DirectShowLib;
using VisioForge.Libs.MediaFoundation.OPM;
using VisioForge.MediaFramework.GStreamer.Base;
using ColorPalette = System.Drawing.Imaging.ColorPalette;
using DateTime = System.DateTime;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Rectangle = System.Drawing.Rectangle;
using Uri = System.Uri;
using GES;

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

        public async Task<IActionResult> ConverttoRGB(int? Id)
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

            Bitmap rbmp, bmp;

            var httpClient = new HttpClient();
            var stream = await httpClient.GetStreamAsync(model.ImageFullPath);

            //var path0 =Image.FromFile(stream);           



          Bitmap  Almershady = new Bitmap(stream);
            bmp = new Bitmap(Almershady, new System.Drawing.Size(350, 300));

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
            rbmp = new Bitmap(w, h);
            Bitmap gbmp = new Bitmap(w, h);
            Bitmap bbmp = new Bitmap(w, h);
            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < 256; j++)
                {
                    rbmp.SetPixel(i, j, System.Drawing.Color.FromArgb((int)red[i, j], 0, 0));
                    gbmp.SetPixel(i, j, System.Drawing.Color.FromArgb(0, (int)red[i, j], 0));
                    bbmp.SetPixel(i, j, System.Drawing.Color.FromArgb(0, 0, (int)red[i, j]));

                }
            }

            rbmp.Save(Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\red" + ImageFormat.Png + ".jpg"));
            gbmp.Save(Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\green" + ImageFormat.Png + ".jpg"));
            bbmp.Save(Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\blue" + ImageFormat.Png + ".jpg"));
            // bmp.Save(Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\normal" + ImageFormat.Png + ".jpg"));
            string path = ($"images\\red" + ImageFormat.Png + ".jpg");
            string path1 = ($"images\\green" + ImageFormat.Png + ".jpg");
            string path2 = ($"images\\blue" + ImageFormat.Png + ".jpg");
            bmp.Save(Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\normal" + ImageFormat.Png + ".jpg"));
            string path4 = ($"wwwroot\\images\\normal" + ImageFormat.Png + ".jpg");
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
                rasterCachedImage.Save(Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\blackandwhite" + ImageFormat.Png + ".jpg"));
            }
            string path5 = ($"images\\blackandwhite" + ImageFormat.Png + ".jpg");
            model.binaryimage = path5;
            Bitmap temp = bmp;
            Bitmap bmap = (Bitmap)temp.Clone();
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
            bmp = (Bitmap)bmap.Clone();
            Random rnd = new Random();
            int a = rnd.Next();

            bmp.Save(Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\blackandwhite1" + ImageFormat.Png + ".jpg"));
            string path3 = ($"images\\blackandwhite1" + ImageFormat.Png + ".jpg");

            model.imagenormal = path3;

            //convert to binary
            Bitmap Ahmed = _blobHelper.ToGrayscale(Almershady);
            Bitmap bitmap = new Bitmap(Almershady);
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
            bitmap.Save(Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\binaryimage" + ImageFormat.Png + ".jpg"));
            string path6 = ($"images\\binaryimage" + ImageFormat.Png + ".jpg");
            model.binaryorginale = path6;
            //end convert to binary
            //convert to 8 bit         

           
            byte[] imagetobinary = BitmapToByteArray(Almershady);
            string str = PadBold(imagetobinary);
            string try1 = bytes2bin(imagetobinary);         
            string yourByteString = Convert.ToString(try1[20], 2).PadLeft(8, '0');
            char first = yourByteString[0];
            char second = yourByteString[1];
            char third = yourByteString[2];
            char foutth = yourByteString[3];
            char five = yourByteString[4];
            char six = yourByteString[5];
            char seven = yourByteString[6];
            char eight = yourByteString[7];
            if (first == '0')
            {
                five = '1';


            }
            var Ahmedjawad = $"{first}{second}{third}{foutth}{five}{six}{seven}{eight}";

            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    //When we add the value to the string we should invert the  
                    // order because the images are reading from top to bottom
                    //and the textBox is write from left to right.    
                    if (bmp.GetPixel(j, i).A.ToString() == "255" && bmp.GetPixel(j, i).B.ToString() == "255" && bmp.GetPixel(j, i).G.ToString() == "255" && bmp.GetPixel(j, i).R.ToString() == "255")
                    {
                        model.t= model.t + "0";
                    }
                    else
                    {
                        model.t = model.t + "1";
                    }
                }
                model.t = model.t + "\r\n"; // this is to make the enter between lines    
            }
            //model.t = Ahmedjawad;
            var byteArray = Enumerable.Range(0, int.MaxValue / 8)
                          .Select(i => i * 8)    // get the starting index of which char segment
                          .TakeWhile(i => i < try1.Length)
                          .Select(i => try1.Substring(i, 8)) // get the binary string segments
                          .Select(s => Convert.ToByte(s, 2)) // convert to byte
                          .ToArray();
            var bytesAsStrings =
            model.t.Select((c, i) => new { Char = c, Index = i })
           .GroupBy(x => x.Index / 8)
           .Select(g => new string(g.Select(x => x.Char).ToArray()));
            byte[] bytes = bytesAsStrings.Select(s => Convert.ToByte(s, 2)).ToArray();

            int width1 = 350;
            int height1 = 300;
            string bytetobitmap="";
            SaveBitmap(bytetobitmap, width1, height1, bytes);



            Bitmap newbmp = new Bitmap(width1, height1, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            // Create a BitmapData and lock all pixels to be written 
            BitmapData bmpData = newbmp.LockBits(
                                new Rectangle(0, 0, newbmp.Width, newbmp.Height),
                                ImageLockMode.WriteOnly, newbmp.PixelFormat);
            // Copy the data from the byte array into BitmapData.Scan0
            Marshal.Copy(bytes, 0, bmpData.Scan0, bytes.Length);

            // Unlock the pixels
            newbmp.UnlockBits(bmpData);
         
            // Do something with your image, e.g. save it to disc
            newbmp.Save(Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\netbitmap" + ImageFormat.Png + ".jpg"));

            
            Ahmed.Save(Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\bit8" + ImageFormat.Png + ".jpg"));
            //end convert to 8 bit 
            return View(model);
        }
        static string PadBold(byte[] b)
        {
            string bin = Convert.ToString(b[20], 2);
            return new string('0', 8 - bin.Length) + "<b>" + bin + "</b>";
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

                    using (Bitmap image = new Bitmap(width, height, width * 4,
                                PixelFormat.Format24bppRgb, new IntPtr(ptr)))
                    {

                        image.Save(Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\{fileName}" + ImageFormat.Png + ".jpg"));
                    }
                }
            }
        }
        public static byte[] BitmapToByteArray(Bitmap bitmap)
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
            for (int i = 0; i < instance.Length; ++i)
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
            return instance;
        }

    }
}

