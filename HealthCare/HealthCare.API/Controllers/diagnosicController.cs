using HealthCare.API.Data;
using HealthCare.API.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.API.Controllers
{
    [Authorize(Roles = "Admin")]
    public class diagnosicController : Controller
    {
       private readonly DataContext _context;
        public diagnosicController(DataContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return _context.diagonisics != null ?
                        View(await _context.diagonisics.ToListAsync()) :
                        Problem("Entity set 'DataContext.BloodTypes'  is null.");
        }




        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(diagonisic diagonisic)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(diagonisic);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(String.Empty, "This type of blood already exists.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }


            }
            return View(diagonisic);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.diagonisics == null)
            {
                return NotFound();
            }

            var diagonisic = await _context.diagonisics.FindAsync(id);
            if (diagonisic == null)
            {
                return NotFound();
            }
            return View(diagonisic);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, diagonisic diagonisic)
        {
            if (id != diagonisic.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(diagonisic);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(String.Empty, "This type of blood already exists.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }


            }

            return View(diagonisic);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.diagonisics == null)
            {
                return NotFound();
            }

            var diagonisic = await _context.diagonisics
                .FirstOrDefaultAsync(m => m.Id == id);
            if (diagonisic == null)
            {
                return NotFound();
            }

            _context.diagonisics.Remove(diagonisic);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



    }
}
