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

namespace SchoolManagementSystem.Controllers
{
    [Authorize]
    public class FeesController : Controller
    {
        
        private readonly SchoolContext _context;

        public FeesController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Fees
        public async Task<IActionResult> Index()
        {
            var schoolContext = _context.Fees.Include(f => f.Student);
            return View(await schoolContext.ToListAsync());
        }

        // GET: Fees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fee = await _context.Fees
                .Include(f => f.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fee == null)
            {
                return NotFound();
            }

            return View(fee);
        }

        // GET: Fees/Create
        public IActionResult Create()
        {
            ViewData["StudentId"] = new SelectList(_context.Students.OrderBy(s => s.Name), "Id", "Name");
            return View(new Fee());
        }

        // POST: Fees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentId,Amount,DueDate,PaymentDate,Status,PaymentMethod,Remarks")] Fee fee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StudentId"] = new SelectList(_context.Students.OrderBy(s => s.Name), "Id", "Name", fee.StudentId);
            return View(fee);
        }

        // GET: Fees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fee = await _context.Fees.FindAsync(id);
            if (fee == null)
            {
                return NotFound();
            }
            ViewData["StudentId"] = new SelectList(_context.Students.OrderBy(s => s.Name), "Id", "Name", fee.StudentId);
            return View(fee);
        }

        // POST: Fees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentId,Amount,DueDate,PaymentDate,Status,PaymentMethod,Remarks")] Fee fee)
        {
            if (id != fee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeeExists(fee.Id))
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
            ViewData["StudentId"] = new SelectList(_context.Students.OrderBy(s => s.Name), "Id", "Name", fee.StudentId);
            return View(fee);
        }

        // GET: Fees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fee = await _context.Fees
                .Include(f => f.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fee == null)
            {
                return NotFound();
            }

            return View(fee);
        }

        // POST: Fees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fee = await _context.Fees.FindAsync(id);
            if (fee != null)
            {
                _context.Fees.Remove(fee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FeeExists(int id)
        {
            return _context.Fees.Any(e => e.Id == id);
        }
    }
}
