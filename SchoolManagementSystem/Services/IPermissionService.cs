using System.Security.Claims;

namespace SchoolManagementSystem.Services
{
    public interface IPermissionService
    {
        bool HasAccess(ClaimsPrincipal user, string module, string permissionType);
    }
}
