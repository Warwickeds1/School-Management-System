using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using SchoolManagementSystem.Services; 
namespace SchoolManagementSystem.Controllers
{
    [Authorize]
    public class ClassesController : Controller
    {
        private readonly SchoolContext _context;
        
        private readonly IPermissionService _permission;

        public ClassesController(SchoolContext context,  IPermissionService permission)
        {
            _context = context;
            _permission = permission;
        }

        // GET: Classes
        public async Task<IActionResult> Index()
        {
            {
                if (!_permission.HasAccess(User, "Classes", "View")) return Forbid();

                // .Include pulls the linked teacher row automatically
                var classes = await _context.Classes.Include(c => c.Teacher).ToListAsync();
                return View(classes);


            }
            
        }

        // GET: Classes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@class == null)
            {
                return NotFound();
            }

            return View(@class);
        }

        // GET: Classes/Create
        public IActionResult Create()
        {
            {
                if (!_permission.HasAccess(User, "Classes", "Add")) return Forbid();

                // Loads Teachers into a key-value selection list (Id as value, Name as text)
                ViewBag.TeacherId = new SelectList(_context.Teachers, "Id", "Name");
                return View();
            }
            
        }

        // POST: Classes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClassName,Section,RoomNumber,TeacherId")] Class classModel)
        {
            if (!_permission.HasAccess(User, "Classes", "Add")) return Forbid();

            if (ModelState.IsValid)
            {
                _context.Add(classModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // IMPORTANT: If we reach here, validation failed. 
            // We MUST re-populate the ViewBag so the dropdown works when the page reloads!
            ViewBag.TeacherId = new SelectList(_context.Teachers, "Id", "Name", classModel.TeacherId);

            return View(classModel);
        }

        // GET: Classes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            {
                if (!_permission.HasAccess(User, "Classes", "Update")) return Forbid();
                if (id == null) return NotFound();

                var classModel = await _context.Classes.FindAsync(id);
                if (classModel == null) return NotFound();

                // Selects the current assigned teacher by default
                ViewBag.TeacherId = new SelectList(_context.Teachers, "Id", "Name", classModel.TeacherId);
                return View(classModel);
            }
          
        }

        // POST: Classes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClassName,Section,RoomNumber,TeacherId")] Class @class)
        {
            if (id != @class.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@class);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassExists(@class.Id))
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
            return View(@class);
        }

        // GET: Classes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@class == null)
            {
                return NotFound();
            }

            return View(@class);
        }

        // POST: Classes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @class = await _context.Classes.FindAsync(id);
            if (@class != null)
            {
                _context.Classes.Remove(@class);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClassExists(int id)
        {
            return _context.Classes.Any(e => e.Id == id);
        }
    }
}
