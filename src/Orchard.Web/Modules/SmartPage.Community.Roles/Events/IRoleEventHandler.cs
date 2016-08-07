using Orchard.Events;

namespace SmartPage.Community.Roles.Events {
    public interface IRoleEventHandler : IEventHandler {
        void Created(RoleCreatedContext context);
        void Removed(RoleRemovedContext context);
        void Renamed(RoleRenamedContext context);
        void PermissionAdded(PermissionAddedContext context);
        void PermissionRemoved(PermissionRemovedContext context);
    }
}