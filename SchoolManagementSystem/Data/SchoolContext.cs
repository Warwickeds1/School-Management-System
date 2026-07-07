using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models;


namespace SchoolManagementSystem.Data
{
    public class SchoolContext : DbContext
    {
        public SchoolContext(DbContextOptions<SchoolContext> options) : base(options) 
        {
        
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Class> Classes { get; set; }

        public DbSet<Attendance> Attendances { get; set; }

        public DbSet<Fee> Fees { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Report> Reports { get; set; }
    }
}
