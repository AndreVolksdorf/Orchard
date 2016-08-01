using Orchard.Security;

namespace SmartPage.Community.Roles.Models {
    public class UsersGroupsRecord {
        public virtual int Id { get; set; }
        public virtual GroupRecord Group { get; set; }
        public virtual IUser User { get; set; }
    }
}