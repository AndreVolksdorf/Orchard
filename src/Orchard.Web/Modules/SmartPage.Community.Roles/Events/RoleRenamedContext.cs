namespace SmartPage.Community.Roles.Events {
    public class RoleRenamedContext : RoleContext {
        public string PreviousRoleName { get; set; }
        public string NewRoleName { get; set; }
    }
}