using HealthCare.API.Data;
using HealthCare.API.Data.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.API.Controllers.API
{
    [ApiController]   
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class BloodTypesController : ControllerBase
    {
        private readonly DataContext _context;

        public BloodTypesController(DataContext  context)
        {
           _context = context;
        }

        // GET: api/bloodtypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BloodType>>> Getbloodtypes()
        {
            return await _context.BloodTypes.OrderBy(x => x.Description).ToListAsync();
        }

        // GET: api/bloodtype/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BloodType>> Getbloodtypes(int id)
        {
            var Getbloodtypes = await _context.BloodTypes.FindAsync(id);

            if (Getbloodtypes == null)
            {
                return NotFound();
            }

            return Getbloodtypes;
        }

        // PUT: api/diagonisics/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Putbloodtypes(int id,BloodType bloodType)
        {
            if (id != bloodType.Id)
            {
                return BadRequest();
            }

            _context.Entry(bloodType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                {
                    return BadRequest("This type of blood types exists.");
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
        }
        // POST: api/bloodtypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BloodType>> Postbloodtypes(BloodType bloodType)
        {
            _context.BloodTypes.Add(bloodType);
            try
            {
                await _context.SaveChangesAsync();
                return CreatedAtAction("Getbloodtypes", new { id = bloodType.Id }, bloodType);

            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                {
                    return BadRequest("This type of diagonisic exists.");
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

        }
        // DELETE: api/bloodtypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletebloodtypes(int id)
        {
            var bloodtypes = await _context.BloodTypes.FindAsync(id);
            if (bloodtypes == null)
            {
                return NotFound();
            }

            _context.BloodTypes.Remove(bloodtypes);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
