using Orchard.Security;

namespace SmartPage.Community.Roles.ViewModels {
    public class GroupRoleEntry
    {
        public IUser User { get; set; }
        public int RoleId { get; set; }
        public string Name { get; set; }
        public bool Deleted { get; set; }
    }
}