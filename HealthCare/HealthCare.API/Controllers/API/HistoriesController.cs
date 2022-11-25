using HealthCare.API.Data;
using HealthCare.API.Data.Entities;
using HealthCare.API.Helpers;
using HealthCare.API.Models.Request;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HealthCare.API.Controllers.API
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class HistoriesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IuserHelper _userhelper;

        public HistoriesController(DataContext context , IuserHelper userhelper)
        {
           _context = context;
           _userhelper = userhelper;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<History>> Gethistories(int id)
        {
            var history = await _context.histories.Include(x=>x.Details)
               .ThenInclude(x=>x.diagonisic).FirstOrDefaultAsync(x=>x.Id==id);

            if (history == null)
            {
                return NotFound();
            }

            return history;
        }

        [HttpPost]
        public async Task<ActionResult<History>>Posthistory(HistoryRequest historyRequest)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Patient patient = await _context.patients.FindAsync(historyRequest.patientId);
            if(patient == null)
            {
                return BadRequest("there is no patient exist");
            }
            string email = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            User user=await _userhelper.GetUserAsync(email);
            if(user==null)
            {
                return BadRequest("there is no user exist");
            }
            History history = new()
            {
                Id=historyRequest.Id,
                illnesses=historyRequest.illnesses,
                Date=DateTime.UtcNow,
                allergies=historyRequest.allergies,
                Result=historyRequest.Result,
                Details=new List<Detail>(),
                user=user,
                patient=patient,
                surgeries=historyRequest.surgeries,
            };
            _context.histories.Add(history);
            await _context.SaveChangesAsync();
            return Ok(history);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutHistory(int id, HistoryRequest request)
        {
           
           if (id!=request.Id)
            {
                return BadRequest();
            }
           if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            History history = await _context.histories.FindAsync(request.Id);
            if(history==null)
            {
                return BadRequest("there is no history exist");
            }
            history.surgeries = request.surgeries;
            history.illnesses = request.illnesses;
            history.Result = request.Result;
            history.allergies = request.allergies;           
            _context.histories.Update(history);
            await _context.SaveChangesAsync();
            return NoContent();

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult>Deletehistory(int id)
        {
            History history = await _context.histories.Include(x => x.Details)
                .FirstOrDefaultAsync(x => x.Id == id);
            if(history==null)
            {
                return NotFound();
            }

            _context.histories.Remove(history);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
