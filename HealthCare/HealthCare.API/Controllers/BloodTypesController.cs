using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthCare.API.Data;
using HealthCare.API.Data.Entities;

namespace HealthCare.API.Controllers
{
    public class BloodTypesController : Controller
    {
        private readonly DataContext _context;

        public BloodTypesController(DataContext context)
        {
            _context = context;
        }

        // GET: BloodTypes
        public async Task<IActionResult> Index()
        {
              return _context.BloodTypes != null ? 
                          View(await _context.BloodTypes.ToListAsync()) :
                          Problem("Entity set 'DataContext.BloodTypes'  is null.");
        }

        // GET: BloodTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.BloodTypes == null)
            {
                return NotFound();
            }

            var bloodType = await _context.BloodTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bloodType == null)
            {
                return NotFound();
            }

            return View(bloodType);
        }

        // GET: BloodTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BloodTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description")] BloodType bloodType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bloodType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bloodType);
        }

        // GET: BloodTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.BloodTypes == null)
            {
                return NotFound();
            }

            var bloodType = await _context.BloodTypes.FindAsync(id);
            if (bloodType == null)
            {
                return NotFound();
            }
            return View(bloodType);
        }

        // POST: BloodTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description")] BloodType bloodType)
        {
            if (id != bloodType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bloodType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BloodTypeExists(bloodType.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(bloodType);
        }

        // GET: BloodTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.BloodTypes == null)
            {
                return NotFound();
            }

            var bloodType = await _context.BloodTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bloodType == null)
            {
                return NotFound();
            }

            return View(bloodType);
        }

        // POST: BloodTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.BloodTypes == null)
            {
                return Problem("Entity set 'DataContext.BloodTypes'  is null.");
            }
            var bloodType = await _context.BloodTypes.FindAsync(id);
            if (bloodType != null)
            {
                _context.BloodTypes.Remove(bloodType);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BloodTypeExists(int id)
        {
          return (_context.BloodTypes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
