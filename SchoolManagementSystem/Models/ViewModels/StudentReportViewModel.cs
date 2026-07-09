using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SchoolManagementSystem.Models.ViewModels
{
    // 1. Captures the inputs selected by the user in the UI form
    public class ReportFilterInput
    {
        public int? SelectedStudentId { get; set; }
        public int? SelectedClassId { get; set; }
        public string? SelectedFeeStatus { get; set; } // "Paid", "Pending", or "All"

        
        // NEW: Monthly Filter Parameters
        public int? SelectedYear { get; set; }
        public int? SelectedMonth { get; set; }
        public string? AttendanceThreshold { get; set; } // "Low", "Normal", "All"
    }

    // 2. Holds the flattened history data we display in the report grid rows
    public class StudentReportRow
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
       
        // Context shifts to monthly metrics
        public string FeeStatus { get; set; } = "Pending";
        public int AttendancePercentage { get; set; }
        public string ReportingMonthDisplay { get; set; } = string.Empty; // e.g., "July 2026"
    }

    // 3. The Master Master-Object passed to the HTML view page
    public class StudentReportViewModel
    {
        // For the filter criteria state preservation
        public ReportFilterInput Filters { get; set; } = new ReportFilterInput();

        // Lists to populate our UI dropdown menus
        public List<SelectListItem> StudentDropdown { get; set; } = new();
        public List<SelectListItem> ClassDropdown { get; set; } = new();

        // NEW: Time Dropdowns
        public List<SelectListItem> MonthDropdown { get; set; } = new();
        public List<SelectListItem> YearDropdown { get; set; } = new();

        public List<StudentReportRow> ReportResults { get; set; } = new();
    }
}