using SmartPage.Community.Roles.Models;

namespace SmartPage.Community.Roles.Events {
    public class PermissionRoleContext : RoleContext {
        public PermissionRecord Permission { get; set; }
    }
}