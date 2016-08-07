using Orchard.Security;
using SmartPage.Community.Roles.Models;

namespace SmartPage.Community.Roles.Events {
    public class UserGroupContext : GroupContext {
        public IUser User { get; set; }
    }
}