namespace SchoolManagementSystem.Models
{
    public class RolePermission
    {
        public int Id { get; set; }
        public string Role { get; set; }
        public string Module { get; set; }
        public bool CanView { get; set; }
        public bool CanAdd { get; set; }
        public bool CanUpdate { get; set; }
    }
}
