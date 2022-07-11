using HealthCare.API.Data;
using HealthCare.API.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.API.Controllers
{
    public class nationalitiesController : Controller
    {
        private readonly DataContext _context;
        public nationalitiesController(DataContext context)
        {
            _context = context; 
        }

       public async Task<IActionResult> Index()
        {
            return _context.natianalities != null ?
                View(await _context.natianalities.ToListAsync()):
                Problem("entity set 'Datacontext' is null");
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Natianality natianality)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(natianality);
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
            return View(natianality);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.BloodTypes == null)
            {
                return NotFound();
            }

            var nationality = await _context.natianalities.FindAsync(id);
            if (nationality == null)
            {
                return NotFound();
            }
            return View(nationality);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Natianality natianality)
        {
            if (id != natianality.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(natianality);
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

            return View(natianality);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.natianalities == null)
            {
                return NotFound();
            }

            var natianality = await _context.natianalities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (natianality == null)
            {
                return NotFound();
            }

            _context.natianalities.Remove(natianality);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    }
}
