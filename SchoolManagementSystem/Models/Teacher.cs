using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models
{
    public class Teacher
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Gender")]
        public string? Gender { get; set; }

        // 🟢 Changed from 'Subject' to 'SubjectExpertise' to match your View files!
        [Required(ErrorMessage = "Subject is required.")]
        [Display(Name = "Subject")]
        public string Subject { get; set; } = string.Empty;

        // 🟢 Added [EmailAddress] attribute to validate proper email formatting (e.g., name@domain.com)
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Qualification is required.")]
        public string Qualification { get; set; } = string.Empty;
    }
}