using Microsoft.AspNetCore.Mvc;
using Mcq.Models;

namespace Mcq.Controllers
{
    public class BaseController : Controller
    {
        protected bool IsUserLoggedIn()
        {
            return HttpContext.Session.GetString("UserId") != null;
        }
    }

    public class PermissionChecker
    {
        private readonly McqsDbContext _context;

        public PermissionChecker(McqsDbContext context)
        {
            _context = context;
        }

        public bool HasPermission(int roleId, int resourceId, int permissionId)
        {
            return _context.RolePermissions.Any(rp =>
                rp.Idrole == roleId &&
                rp.Idresource == resourceId &&
                rp.PermissionLevel == permissionId);
        }
    }
}
