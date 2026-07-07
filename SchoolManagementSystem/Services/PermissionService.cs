using System.Security.Claims;
using System.Linq;
using SchoolManagementSystem.Data; // Update with your actual data namespace

namespace SchoolManagementSystem.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly SchoolContext _context;

        public PermissionService(SchoolContext context)
        {
            _context = context;
        }

        public bool HasAccess(ClaimsPrincipal user, string module, string permissionType)
        {
            if (user?.Identity?.IsAuthenticated != true) return false;

            // Extract the user role claim
            var role = user.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(role)) return false;

            // Global Bypass: Admins have default full permission control unless explicitly restricted
            if (role == "Admin" && module != "Users") return true;

            // Find matching permissions row from database
            var permission = _context.RolePermissions
                .FirstOrDefault(p => p.Role == role && p.Module == module);

            if (permission == null) return false;

            // Evaluate flag matching requested action
            return permissionType.ToLower() switch
            {
                "view" => permission.CanView,
                "add" => permission.CanAdd,
                "update" => permission.CanUpdate,
                _ => false
            };
        }
    }
}