using HealthCare.API.Data;
using HealthCare.API.Data.Entities;
using HealthCare.API.Helpers;
using HealthCare.API.Models.Request;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.API.Controllers.API
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IuserHelper _userhelper;
        private readonly IBlobHelper _blobHelper;

        public PatientsController(DataContext context, IuserHelper userhelper, IBlobHelper blobHelper)
        {
           _context = context;
           _userhelper = userhelper;
           _blobHelper = blobHelper;
        }

        public async Task<ActionResult<Patient>>postPatient(Patientrequest request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            City city = await _context.Cities.FindAsync(request.CityId);
            gendre gendre = await _context.gendres.FindAsync(request.gendreId);
            Natianality natianality = await _context.natianalities.FindAsync(request.NatianalityId);
            BloodType bloodType = await _context.BloodTypes.FindAsync(request.bloodTypeId);
            UserPatient userPatient = await _context.UserPatients.FindAsync(request.userPatientId);
            if(city==null)
            {
                return BadRequest("there is no city exist");
            }
            if (gendre == null)
            {
                return BadRequest("there is nogendre exist");
            }
            if (natianality == null)
            {
                return BadRequest("there is no (natianality exist");
            }
            if (bloodType == null)
            {
                return BadRequest("there is no blood type exist");
            }
            if (userPatient == null)
            {
                return BadRequest("there is nouserPatient exist");
            }
            User user = await _userhelper.GetUserAsync(Guid.Parse(request.UserId));
            if (user == null)
            {
                return BadRequest("there is no user exist.");
            }

            Patient patient = await _context.patients.FirstOrDefaultAsync(x => x.FirstName.ToUpper() == request.FirstName.ToUpper());
            if(patient==null)
            {
                return BadRequest("there is no patient exist.");
            }
            Guid ImagId = Guid.Empty;
            List<PatientPhoto> patientPhoto = new();
            if (request.Image !=null && request.Image.Length >0)
            {
                ImagId = await _blobHelper.UploadBlobAsync(request.Image, "patients");
                patientPhoto.Add(new PatientPhoto
                {
                    ImageId = ImagId
                });
            }
            patient = new Patient
            {
                City = city,
                bloodType = bloodType,
                gendre = gendre,
                Natianality = natianality,
                userPatient = userPatient,
                User = user,
                patientPhotos = patientPhoto,
                Date = request.Date,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Description = request.Description,
                Address = request.Address,
                EPCNNumber = request.EPCNNumber,
                MobilePhone = request.MobilePhone,
                histories = new List<History>(),
                Agendas = new List<Agenda>(),               

            };
            _context.patients.Add(patient);
            await _context.SaveChangesAsync();
            return Ok(patient);
        }
    }
}
