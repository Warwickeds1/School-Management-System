using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data; // Ensure this matches your DbContext namespace
using SchoolManagementSystem.Models.ViewModels;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Controllers
{
    public class ReportsController : Controller
    {
        private readonly SchoolContext _context;

        public ReportsController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Reports/StudentDashboard

        // GET: Reports/StudentDashboard
        public async Task<IActionResult> StudentDashboard([Bind(Prefix = "Filters")] ReportFilterInput inputs)
        {
            var viewModel = new StudentReportViewModel
            {
                Filters = inputs ?? new ReportFilterInput()
            };

            // 1. Populate standard dropdowns
            viewModel.StudentDropdown = await _context.Students.AsNoTracking()
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name }).ToListAsync();

            viewModel.ClassDropdown = await _context.Classes.AsNoTracking()
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = $"{c.ClassName} - {c.Section}" }).ToListAsync();

            // 2. Populate Month & Year Dropdowns
            viewModel.YearDropdown = Enumerable.Range(2024, 4) // Generates 2024, 2025, 2026, 2027
                .Select(y => new SelectListItem { Value = y.ToString(), Text = y.ToString() }).ToList();

            viewModel.MonthDropdown = Enumerable.Range(1, 12)
                .Select(m => new SelectListItem
                {
                    Value = m.ToString(),
                    Text = System.Globalization.DateTimeFormatInfo.CurrentInfo.GetMonthName(m)
                }).ToList();

            // 3. Deferred LINQ Pipeline Setup
            var query = _context.Students.Include(s => s.Class).AsNoTracking();

            if (inputs.SelectedStudentId.HasValue)
            {
                query = query.Where(s => s.Id == inputs.SelectedStudentId.Value);
            }
            if (inputs.SelectedClassId.HasValue)
            {
                query = query.Where(s => s.ClassId == inputs.SelectedClassId.Value);
            }

            var baseStudents = await query.ToListAsync();

            // 4. PASTE START: This handles the new target evaluations and filters the rows
            int targetMonth = inputs.SelectedMonth ?? DateTime.Now.Month;
            int targetYear = inputs.SelectedYear ?? DateTime.Now.Year;

            foreach (var student in baseStudents)
            {
                // Mocking an enrollment time slot for each student to test filter limits
                int mockEnrollmentMonth = (student.Id % 3) + 1; // Generates months 1, 2, or 3 (Jan, Feb, Mar)
                int mockEnrollmentYear = 2026;

                // If the user selected a filter, check if student data matches it
                if (inputs.SelectedMonth.HasValue && mockEnrollmentMonth != targetMonth)
                {
                    continue; // Exits loop iteration early; drops this student row
                }
                if (inputs.SelectedYear.HasValue && mockEnrollmentYear != targetYear)
                {
                    continue; // Exits loop iteration early; drops this student row
                }

                var row = new StudentReportRow
                {
                    StudentId = student.Id,
                    StudentName = student.Name,
                    Age = student.Age,
                    Gender = student.Gender,
                    ClassName = student.Class?.ClassName ?? "N/A",
                    Section = student.Class?.Section ?? "N/A",
                    ReportingMonthDisplay = $"{System.Globalization.DateTimeFormatInfo.CurrentInfo.GetMonthName(mockEnrollmentMonth)} {mockEnrollmentYear}",

                    FeeStatus = (student.Id % 2 == 0) ? "Paid" : "Pending",
                    AttendancePercentage = (student.Id * 7) % 25 + 75
                };

                // Standard dropdown constraint handling
                if (!string.IsNullOrEmpty(inputs.SelectedFeeStatus) && inputs.SelectedFeeStatus != "All" && row.FeeStatus != inputs.SelectedFeeStatus)
                    continue;

                if (inputs.AttendanceThreshold == "Low" && row.AttendancePercentage >= 85)
                    continue;
                if (inputs.AttendanceThreshold == "Normal" && row.AttendancePercentage < 85)
                    continue;

                viewModel.ReportResults.Add(row);
            }
            // PASTE END

            return View(viewModel);
        }
    }
}