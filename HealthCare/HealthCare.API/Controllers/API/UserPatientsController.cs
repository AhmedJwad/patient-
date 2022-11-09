using HealthCare.API.Data;
using HealthCare.API.Data.Entities;
using HealthCare.API.Migrations;
using HealthCare.API.Models;
using HealthCare.API.Models.Response;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.API.Controllers.API

{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    
    public class UserPatientsController : ControllerBase
    {
        private readonly DataContext _context;

        public UserPatientsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetUserpatient()
        {
            List<UserPatient> userpatients = await _context.UserPatients
                 .Include(u => u.User)
                 .Include(u => u.Patients)
                 .ToListAsync();
            List<UserPatientResponse> userpatientsresponse = userpatients.Select(u => new UserPatientResponse
            {
               Id=u.Id,
               FirstName=u.User.FirstName,
               LastName=u.User.LastName,
               Address=u.User.Address,
               Email=u.User.Email,
               PhoneNumber=u.User.PhoneNumber,
            }).ToList();
            return Ok( userpatientsresponse);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserPatient>> GetUserpatient(int id)
        {
            var userpatient = await _context.UserPatients
                .FindAsync(id);

            if (userpatient == null)
            {
                return NotFound();
            }

            return userpatient;
        }

    }
}
