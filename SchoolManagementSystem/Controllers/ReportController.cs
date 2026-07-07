using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Controllers
{
    public class ReportController : Controller
    {
        private readonly Data.SchoolContext _context;

        public ReportController(Data.SchoolContext context)
        {
            _context = context;
        }

        // GET: Reports
        public async Task<IActionResult> Index()
        {
            return View(await _context.Reports.OrderByDescending(r => r.GeneratedDate).ToListAsync());
        }

        // GET: Reports/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Reports/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Category,Remarks")] Report report, DateTime? startDate, DateTime? endDate)
        {
            if (ModelState.IsValid)
            {
                // 1. Build Query Parameters JSON
                var parameters = new
                {
                    StartDate = startDate?.ToString("yyyy-MM-dd") ?? "All Time",
                    EndDate = endDate?.ToString("yyyy-MM-dd") ?? "All Time"
                };
                report.QueryParametersJson = JsonSerializer.Serialize(parameters);
                report.GeneratedDate = DateTime.Now;
                report.GeneratedBy = User.Identity?.Name ?? "Administrator";

                // Change the type of snapshotData from object to object? to allow null assignment
                object? snapshotData = null;

                if (report.Category == "Financial")
                {
                    var feesQuery = _context.Fees.Include(f => f.Student).AsQueryable();
                    if (startDate.HasValue) feesQuery = feesQuery.Where(f => f.DueDate >= startDate.Value);
                    if (endDate.HasValue) feesQuery = feesQuery.Where(f => f.DueDate <= endDate.Value);

                    var feesList = await feesQuery.ToListAsync();
                    snapshotData = new
                    {
                        TotalInvoiced = feesList.Sum(f => f.Amount),
                        TotalCollected = feesList.Where(f => f.IsPaid).Sum(f => f.Amount),
                        TotalOutstanding = feesList.Where(f => !f.IsPaid).Sum(f => f.Amount),
                        TotalInvoicesCount = feesList.Count,
                        PaidInvoicesCount = feesList.Count(f => f.IsPaid),
                        OutstandingInvoicesCount = feesList.Count(f => !f.IsPaid),
                        LineItems = feesList.Select(f => new
                        {
                            StudentName = f.Student?.Name ?? "Unknown Student",
                            Amount = f.Amount,
                            DueDate = f.DueDate.ToString("yyyy-MM-dd"),
                            IsPaid = f.IsPaid
                        }).ToList()
                    };
                }
                else if (report.Category == "Attendance")
                {
                    var attendanceQuery = _context.Attendances.Include(a => a.Student).AsQueryable();
                    if (startDate.HasValue) attendanceQuery = attendanceQuery.Where(a => a.Date >= startDate.Value);
                    if (endDate.HasValue) attendanceQuery = attendanceQuery.Where(a => a.Date <= endDate.Value);

                    var attendanceList = await attendanceQuery.ToListAsync();
                    int totalCount = attendanceList.Count;
                    int presentCount = attendanceList.Count(a => a.Status == "Present");
                    double rate = totalCount > 0 ? (double)presentCount / totalCount * 100 : 100.0;

                    snapshotData = new
                    {
                        TotalLogs = totalCount,
                        PresentCount = presentCount,
                        AbsentCount = attendanceList.Count(a => a.Status == "Absent"),
                        LeaveCount = attendanceList.Count(a => a.Status == "Leave"),
                        AttendanceRate = Math.Round(rate, 1),
                        LineItems = attendanceList.Select(a => new
                        {
                            StudentName = a.Student?.Name ?? "Unknown Student",
                            Date = a.Date.ToString("yyyy-MM-dd"),
                            Status = a.Status
                        }).ToList()
                    };
                }
                else // Enrollment Category default
                {
                    var totalStudents = await _context.Students.CountAsync();
                    var totalClasses = await _context.Classes.CountAsync();
                    var totalTeachers = await _context.Teachers.CountAsync();

                    snapshotData = new
                    {
                        TotalStudents = totalStudents,
                        TotalClasses = totalClasses,
                        TotalTeachers = totalTeachers,
                        AverageClassSize = totalClasses > 0 ? (double)totalStudents / totalClasses : 0
                    };
                }

                report.DataSnapshotJson = JsonSerializer.Serialize(snapshotData);

                _context.Add(report);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(report);
        }

        // GET: Reports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var report = await _context.Reports.FirstOrDefaultAsync(m => m.Id == id);
            if (report == null) return NotFound();

            return View(report);
        }

        // GET: Reports/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var report = await _context.Reports.FirstOrDefaultAsync(m => m.Id == id);
            if (report == null) return NotFound();

            return View(report);
        }

        // POST: Reports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report != null)
            {
                _context.Reports.Remove(report);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}