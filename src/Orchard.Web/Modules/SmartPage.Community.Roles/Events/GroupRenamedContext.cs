namespace SmartPage.Community.Roles.Events {
    public class GroupRenamedContext : GroupContext {
        public string PreviousGroupName { get; set; }
        public string NewGroupName { get; set; }
    }
}