using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagementSystem.Models
{
    public class Fee
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a student.")]
        [Display(Name = "Student")]
        public int StudentId { get; set; }

        [ForeignKey("StudentId")]
        public Student? Student { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, 1000000.00, ErrorMessage = "Amount must be greater than zero.")]
        [Column(TypeName = "decimal(18,2)")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Due Date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; } = DateTime.Today;

        [DataType(DataType.Date)]
        [Display(Name = "Payment Date")]
        public DateTime? PaymentDate { get; set; }

        [Required(ErrorMessage = "Payment Status is required.")]
        [StringLength(20)]
        public string Status { get; set; } = "Unpaid"; // Paid, Unpaid, Partially Paid, Overdue

        [Display(Name = "Payment Method")]
        [StringLength(50)]
        public string? PaymentMethod { get; set; } // Cash, Bank Transfer, Card, etc.

        [StringLength(200, ErrorMessage = "Remarks cannot exceed 200 characters.")]
        public string? Remarks { get; set; }
        public bool IsPaid { get; internal set; }
    }
}