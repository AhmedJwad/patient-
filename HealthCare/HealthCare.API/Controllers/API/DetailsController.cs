using HealthCare.API.Data;
using HealthCare.API.Data.Entities;
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
    public class DetailsController : ControllerBase
    {
        private readonly DataContext _context;

        public DetailsController(DataContext context, )
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Detail>> PostDetail(DetailRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            History history = await _context.histories.FindAsync(request.HistoryId);
            if (history == null)
            {
                return BadRequest("there is no history exist");
            }

            diagonisic diagonisic = await _context.diagonisics.FindAsync(request.diagonisicId);
            if (diagonisic == null)
            {
                return BadRequest("there is no details exist");
            }

            Detail detail = new()
            {
                History = history,
                diagonisic = diagonisic,
                Description = request.Description,

            };
            _context.details.Add(detail);
            await _context.SaveChangesAsync();
            return Ok(detail);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDetail(int id, DetailRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            diagonisic diagonisic = await _context.diagonisics.FindAsync(request.diagonisicId);  

            if(diagonisic==null)
            {
                return BadRequest("there is diagnosic exist");
            }
            Detail detail = await _context.details.FindAsync(request.Id);
            if(detail==null)
            {
                return BadRequest("there is no details exist");
            }
            detail.diagonisic = diagonisic;
            detail.Description = request.Description;

            _context.details.Update(detail);
            _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDetail(int id)
        {
            Detail detail = await _context.details.FindAsync(id);
            if (detail == null)
            {
                return NotFound();
            }

            _context.details.Remove(detail);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
