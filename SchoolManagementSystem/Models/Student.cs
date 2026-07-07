using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagementSystem.Models
{
    public class Student
    {
        // 1. Unique Identification Primary Key
        public int Id { get; set; }

        // 2. Student Personal Information
        [Required]
        [Display(Name = "Student Name")]
        public string Name { get; set; }

        // 3. Gender Configuration (Dropdown Bound)
        [Required]
        [Display(Name = "Gender")]
        public string? Gender { get; set; }

        // 4. Age Boundary Check
        [Required]
        [Range(3, 100, ErrorMessage = "Please enter a valid age between 3 and 100.")]
        [Display(Name = "Age")]
        public int Age { get; set; }

        // 5. Profile Picture URL/Path String
        [Display(Name = "Profile Picture")]
        public string? ProfilePicturePath { get; set; } // Marked nullable (?) so it's optional during creation

        // 6. Foreign Key linking to our Class Hub
        [Required]
        [Display(Name = "Assigned Class")]
        public int ClassId { get; set; }

        // Navigation Property for Entity Framework traversal queries
        [ForeignKey("ClassId")]
        public virtual Class? Class { get; set; }
    }
}