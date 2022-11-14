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
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class PhotosPatientController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IBlobHelper _blobHelper;

        public PhotosPatientController(DataContext context , IBlobHelper blobHelper)
        {
            _context = context;
           _blobHelper = blobHelper;
        }


        [HttpPost]
        public async Task<ActionResult<PatientPhoto>> PostPhotosPatient(PatientImageREquest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

           Patient patient = await _context.patients.FindAsync(request.PatientId);
            if (patient == null)
            {
                return BadRequest("there is no patient exist.");
            }

            Guid imageId = await _blobHelper.UploadBlobAsync(request.Image, "patients");
            PatientPhoto patientPhoto = new()
            {
                ImageId = imageId,
                patient = patient
            };

            _context.patientPhotos.Add(patientPhoto);
            await _context.SaveChangesAsync();
            return Ok(patientPhoto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhotosPatient(int id)
        {
            PatientPhoto patientPhoto = await _context.patientPhotos.FindAsync(id);
            if (patientPhoto == null)
            {
                return NotFound();
            }
            await _blobHelper.DeleteBlobAsync(patientPhoto.ImageId, "patients");
            _context.patientPhotos.Remove(patientPhoto);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
