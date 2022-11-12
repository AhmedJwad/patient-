using HealthCare.API.Data;
using HealthCare.API.Data.Entities;
using HealthCare.API.Helpers;
using HealthCare.API.Migrations;
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
        [HttpPost]
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
            if(patient!=null)
            {
                return BadRequest("there is a patient exist.");
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
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return Ok(patient);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> PutPatient(int id, Patientrequest request)
        {
            if(id != request.Id)
            {
                return BadRequest();
            }

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);  
            }
            City city = await _context.Cities.FindAsync(request.CityId);
            gendre gendre = await _context.gendres.FindAsync(request.gendreId);
            Natianality natianality = await _context.natianalities.FindAsync(request.NatianalityId);
            BloodType bloodType = await _context.BloodTypes.FindAsync(request.bloodTypeId);
            UserPatient userPatient = await _context.UserPatients.FindAsync(request.userPatientId);
            if (city == null)
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
            Patient patient = await _context.patients.FindAsync(request.Id);
            if (patient == null)
            {
                return BadRequest("there is no patient exist.");
            }
            patient.City = city;
            patient.bloodType = bloodType;
            patient.gendre = gendre;
            patient.Natianality = natianality;
            patient.userPatient = userPatient;
            patient.User = user;          
            patient.Date = request.Date;
            patient.FirstName = request.FirstName;
            patient.LastName = request.LastName;
            patient.Description = request.Description;
            patient.Address = request.Address;
            patient.EPCNNumber = request.EPCNNumber;
            patient.MobilePhone = request.MobilePhone;
            try
            {
                _context.patients.Update(patient);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                {
                    return BadRequest("Ya existe esta marca.");
                }
                else
                {
                    return BadRequest(dbUpdateException.InnerException.Message);
                }
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }


            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
           Patient patient = await _context.patients
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
            return NoContent();
        }
    }
}
