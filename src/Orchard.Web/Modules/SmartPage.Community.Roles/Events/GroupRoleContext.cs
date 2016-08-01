using Orchard.Security;
using SmartPage.Community.Roles.Models;

namespace SmartPage.Community.Roles.Events {
    public class GroupRoleContext : RoleContext {
        public GroupRecord Group { get; set; }
    }
}