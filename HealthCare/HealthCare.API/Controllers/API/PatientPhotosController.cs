using HealthCare.API.Data;
using HealthCare.API.Data.Entities;
using HealthCare.API.Helpers;
using HealthCare.API.Models.Request;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.API.Controllers.API
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class PatientPhotosController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IBlobHelper _blophelper;

        public PatientPhotosController(DataContext context, IBlobHelper blophelper)
        {
           _context = context;
           _blophelper = blophelper;
        }

        [HttpPost]
        public async Task<ActionResult<PatientPhoto>>PostPatientPhoto(PatientImageREquest request )
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
;            }

            Patient patient = await _context.patients.FindAsync(request.PatientId);
            if(patient == null)
            {
                return BadRequest("there is no patient exist");
            }
            Guid ImageId = await _blophelper.UploadBlobAsync(request.Image, "patients");
            PatientPhoto patientPhoto = new()
            {
                ImageId=ImageId,
                patient=patient,
            };
            _context.patientPhotos.Add(patientPhoto);
            await _context.SaveChangesAsync();
            return Ok(patient);
        }

        [HttpPost]
        public async Task<ActionResult<PatientPhoto>>DeeletePatientPhoto(int id)
        {
            PatientPhoto patientPhoto = await _context.patientPhotos.FindAsync(id); 
            if(patientPhoto == null)
            {
                return NotFound();
            }
            await _blophelper.DeleteBlobAsync(patientPhoto.ImageId, "patients");
            _context.patientPhotos.Remove(patientPhoto);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
