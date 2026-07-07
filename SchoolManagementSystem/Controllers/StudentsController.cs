using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using SchoolManagementSystem.Services; // Assuming you have a permission service for checking access rights


namespace SchoolManagementSystem.Controllers
{
    
    [Authorize]
    public class StudentsController : Controller
    {
        private readonly SchoolContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IPermissionService _permission;

        public StudentsController(SchoolContext context, IWebHostEnvironment webHostEnvironment, IPermissionService permission)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _permission = permission;
        }

        // Replace the entire Index method with the following:
        [Authorize(Roles = "Admin, Teacher, Student")]
        public async Task<IActionResult> Index(string searchString, int? ClassId)
        {
            // 1. Check permissions first (this is correct)
            if (!_permission.HasAccess(User, "Students", "View"))
            {
                return Forbid();
            }

            // 2. Build the query
            var studentsQuery = _context.Students.Include(s => s.Class).AsQueryable();

            // 3. Apply filters
            if (!string.IsNullOrEmpty(searchString))
            {
                studentsQuery = studentsQuery.Where(s => s.Name.Contains(searchString));
            }

            // 4. Materialize the data into a list (this closes the DataReader immediately)
            var students = await studentsQuery.ToListAsync();

            // 5. Return the list to the View
            return View(students);
        }

        // GET: Students/Details/5
        [Authorize(Roles = "Admin, Teacher, Student")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Class)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        [Authorize(Roles = "Admin, Teacher")]
        public IActionResult Create()
        {
            if (!_permission.HasAccess(User, "Students", "Add")) return Forbid();

            ViewBag.ClassId = new SelectList(_context.Classes.Select(c => new
            {
                Id = c.Id,
                DisplayName = c.ClassName + " (Section " + c.Section + ")"
            }), 
            "Id", "DisplayName");
            
            return View();
        }

        // POST: Students/Create
        [Authorize(Roles = "Admin, Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
    [Bind("Id,Name,Gender,Age,ClassId")] Student student,
    IFormFile profilePicture) // 🟢 Name must match the HTML input name exactly!
        {
            if (ModelState.IsValid)
            {
                if (profilePicture != null && profilePicture.Length > 0)
                {
                    // 1. Generate a unique name for the image file
                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(profilePicture.FileName);

                    // 2. Map the physical path into wwwroot/images/students
                    var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "students");
                    var filePath = Path.Combine(uploadFolder, uniqueFileName);

                    // Ensure the folder exists
                    if (!Directory.Exists(uploadFolder)) Directory.CreateDirectory(uploadFolder);

                    // 3. Save the file to disk
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await profilePicture.CopyToAsync(fileStream);
                    }

                    // 4. Save the file name to your database record string property!
                    student.ProfilePicturePath = uniqueFileName;
                }

                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Reload select lists if validation fails...
            return View(student);
        }


        // GET: Students/Edit/5
        [Authorize(Roles = "Admin, Teacher")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "ClassName", student.ClassId);
            return View(student);
        }

        // POST: Students/Edit/5
        [Authorize(Roles = "Admin, Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Age,Gender,ClassId")] Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
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
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "ClassName", student.ClassId);
            return View(student);
        }

        // GET: Students/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Class)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }
    }
}
