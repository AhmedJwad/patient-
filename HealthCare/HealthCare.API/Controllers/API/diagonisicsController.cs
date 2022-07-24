using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthCare.API.Data;
using HealthCare.API.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace HealthCare.API.Controllers.API{
   

    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class diagonisicsController : ControllerBase
    {
        private readonly DataContext _context;

        public diagonisicsController(DataContext context)
        {
            _context = context;
        }

        // GET: api/diagonisics
        [HttpGet]
        public async Task<ActionResult<IEnumerable<diagonisic>>> Getdiagonisics()
        {
            return await _context.diagonisics.ToListAsync();
        }

        // GET: api/diagonisics/5
        [HttpGet("{id}")]
        public async Task<ActionResult<diagonisic>> Getdiagonisic(int id)
        {
            var diagonisic = await _context.diagonisics.FindAsync(id);

            if (diagonisic == null)
            {
                return NotFound();
            }

            return diagonisic;
        }

        // PUT: api/diagonisics/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Putdiagonisic(int id, diagonisic diagonisic)
        {
            if (id != diagonisic.Id)
            {
                return BadRequest();
            }

            _context.Entry(diagonisic).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!diagonisicExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/diagonisics
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<diagonisic>> Postdiagonisic(diagonisic diagonisic)
        {
            _context.diagonisics.Add(diagonisic);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Getdiagonisic", new { id = diagonisic.Id }, diagonisic);
        }

        // DELETE: api/diagonisics/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletediagonisic(int id)
        {
            var diagonisic = await _context.diagonisics.FindAsync(id);
            if (diagonisic == null)
            {
                return NotFound();
            }

            _context.diagonisics.Remove(diagonisic);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool diagonisicExists(int id)
        {
            return _context.diagonisics.Any(e => e.Id == id);
        }
    }
}
