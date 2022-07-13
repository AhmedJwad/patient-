using HealthCare.API.Data;
using HealthCare.API.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.API.Controllers
{
    [Authorize(Roles = "Admin")]
    public class GendresController : Controller
    {
        private readonly DataContext _context;

        public GendresController(DataContext context )
        {
           _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return _context.gendres != null ?
                        View(await _context.gendres.ToListAsync()) :
                        Problem("Entity set 'DataContext.gendres'  is null.");
        }
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(gendre gendre)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(gendre);
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
            return View(gendre);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.gendres == null)
            {
                return NotFound();
            }

            var gendre = await _context.gendres.FindAsync(id);
            if (gendre == null)
            {
                return NotFound();
            }
            return View(gendre);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, gendre gendre)
        {
            if (id != gendre.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gendre);
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

            return View(gendre);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.gendres == null)
            {
                return NotFound();
            }

            var gendre = await _context.gendres
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gendre == null)
            {
                return NotFound();
            }

            _context.gendres.Remove(gendre);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
