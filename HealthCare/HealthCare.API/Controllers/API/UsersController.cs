using HealthCare.API.Data;
using HealthCare.API.Data.Entities;
using HealthCare.Common.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.API.Controllers.API
{
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]   
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
           _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.OrderBy(x => x.FirstName).ThenBy(x=>x.LastName).Where
                (x=>x.userType==UserType.User).ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUsers(string id)
        {
           User user = await _context.Users
                .Include(x=>x.Patients)
                .ThenInclude(x=>x.City)
                .Include(x=>x.Patients)
                .ThenInclude(x=>x.Natianality)
                .Include(x=>x.Patients)
                .ThenInclude(x=>x.bloodType)
                .Include(x=>x.Patients)
                .ThenInclude(x=>x.gendre)
                .Include(x=>x.Patients)
                .ThenInclude(x=>x.patientPhotos)
                .Include(x=>x.Patients)
                .ThenInclude(x=>x.histories)
                .ThenInclude(x=>x.Details)
                .ThenInclude(x=>x.diagonisic)
                .FirstOrDefaultAsync(x=>x.Id==id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }
    }
}
