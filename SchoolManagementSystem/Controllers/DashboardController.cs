using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;
using SchoolManagementSystem.Services;

namespace SchoolManagementSystem.Controllers
{
    
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly SchoolContext _context;
        private readonly IPermissionService _permission;
        public DashboardController(SchoolContext context, IPermissionService permission)
        {
            _context = context;
            _permission = permission;
         }

        public IActionResult Index()
        {

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (userRole != "Admin")
            {
                return Forbid();
            }

          


            ViewBag.StudentCount = _context.Students.Count();
            ViewBag.TeacherCount = _context.Teachers.Count();
            ViewBag.ClassCOunt = _context.Classes.Count();
            ViewBag.AttendanceCount = _context.Attendances.Count();
            ViewBag.TotalRevenue = _context.Fees.Sum(f =>(decimal?) f.Amount) ?? 0;
            ViewBag.FeeAverage = _context.Fees.Average(f => (decimal?)f.Amount) ?? 0;
            ViewBag.FeeMax = _context.Fees.Max(f => (decimal?)f.Amount) ?? 0;
            return View();
        }
    }
}
