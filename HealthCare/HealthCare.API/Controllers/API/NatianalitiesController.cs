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
    
    public class NatianalitiesController : ControllerBase
    {
        private readonly DataContext _context;

        public NatianalitiesController(DataContext context)
        {
            _context = context;
        }
        // GET: api/Nationaties
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Natianality>>> GetNationaties()
        {
            return await _context.natianalities.OrderBy(x => x.Description).ToListAsync();
        }

        // GET: api/Nationaties/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Natianality>> GetNationaties(int id)
        {
            var natianality = await _context.natianalities.FindAsync(id);

            if (natianality == null)
            {
                return NotFound();
            }

            return natianality;
        }
        // PUT: api/Nationaties/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNationaties(int id, Natianality natianality)
        {
            if (id != natianality.Id)
            {
                return BadRequest();
            }

            _context.Entry(natianality).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                {
                    return BadRequest("This type of natianality exists.");
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
        // POST: api/natianality
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Natianality>> PostNatianality(Natianality natianality)
        {
            _context.natianalities.Add(natianality);
            try
            {
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetNationaties", new { id = natianality.Id }, natianality);

            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                {
                    return BadRequest("This type of Gendre exists.");
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
        // DELETE: api/city/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNationaties(int id)
        {
            var Nationaty = await _context.natianalities.FindAsync(id);
            if (Nationaty == null)
            {
                return NotFound();
            }

            _context.natianalities.Remove(Nationaty);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
