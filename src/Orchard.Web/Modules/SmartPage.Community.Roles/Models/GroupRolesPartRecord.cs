namespace SmartPage.Community.Roles.Models {
    public class GroupRolesPartRecord
    {
        public virtual int Id { get; set; }
        public virtual int UserId { get; set; }
        public virtual RoleRecord RoleRecord { get; set; }
        public virtual GroupRecord GroupRecord { get; set; }
    }
}