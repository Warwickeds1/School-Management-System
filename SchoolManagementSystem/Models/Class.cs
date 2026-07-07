using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagementSystem.Models

{
    public class Class
    {
        [Display(Name = " Class ")]
        public int Id { get; set; }

        [Required]
        [Display(Name= " Grade Level")]
        public string ClassName { get; set; }

        [Required]
        [Display(Name = "Section")]
        public string Section { get; set; }

        [Display(Name = "Room Number")]
        public string RoomNumber { get; set; }


        [Display(Name = "Class Teacher")]
        public int? TeacherId { get; set; }

        [ForeignKey("TeacherId")]
        public virtual Teacher? Teacher { get; set; }

    }
}
