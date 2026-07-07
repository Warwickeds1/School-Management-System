using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagementSystem.Models
{
    public class Attendance
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Attendance Date")]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Please select a student.")]
        [Display(Name = "Student")]
        public int StudentId { get; set; }

        [ForeignKey("StudentId")]
        public Student? Student { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [StringLength(20)]
        public string Status { get; set; } = "Present"; // Present, Absent, Late, Excused

        [StringLength(150, ErrorMessage = "Remarks cannot exceed 150 characters.")]
        public string? Remarks { get; set; }
    }
}