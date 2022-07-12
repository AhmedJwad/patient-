using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Admin")]
    public class BloodTypesController : Controller
    {
        private readonly DataContext _context;

        public BloodTypesController(DataContext context)
        {
            _context = context;
        }

       
        public async Task<IActionResult> Index()
        {
              return _context.BloodTypes != null ? 
                          View(await _context.BloodTypes.ToListAsync()) :
                          Problem("Entity set 'DataContext.BloodTypes'  is null.");
        }

       
       
        
        public IActionResult Create()
        {
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( BloodType bloodType)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(bloodType);
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
            return View(bloodType);
        }

     
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

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,  BloodType bloodType)
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
            
            return View(bloodType);
        }

       
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

            _context.BloodTypes.Remove(bloodType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }    
       

       
    }
}
