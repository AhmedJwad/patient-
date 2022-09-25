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
    public class gendresController : ControllerBase
    {
        private readonly DataContext _context;

        public gendresController(DataContext context)
        {
           _context = context;
        }
        // GET: api/gendres
        [HttpGet]
        public async Task<ActionResult<IEnumerable<gendre>>> GetGendres()
        {
            return await _context.gendres.OrderBy(x => x.Description).ToListAsync();
        }

        // GET: api/gendres/5
        [HttpGet("{id}")]
        public async Task<ActionResult<gendre>> GetGendres(int id)
        {
            var gendre = await _context.gendres.FindAsync(id);

            if (gendre == null)
            {
                return NotFound();
            }

            return gendre;
        }
        // PUT: api/gendres/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGendres(int id,gendre gendre)
        {
            if (id != gendre.Id)
            {
                return BadRequest();
            }

            _context.Entry(gendre).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
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
        // POST: api/gendre
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<gendre>> PostGendres(gendre gendre)
        {
            _context.gendres.Add(gendre);
            try
            {
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetGendres", new { id = gendre.Id }, gendre);

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
        public async Task<IActionResult> DeleteGendres(int id)
        {
            var Gendres = await _context.gendres.FindAsync(id);
            if (Gendres == null)
            {
                return NotFound();
            }

            _context.gendres.Remove(Gendres);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
