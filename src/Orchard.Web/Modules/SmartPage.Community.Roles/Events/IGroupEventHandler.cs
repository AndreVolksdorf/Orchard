using Orchard.Events;

namespace SmartPage.Community.Roles.Events {
    public interface IGroupEventHandler : IEventHandler {
        void Created(GroupCreatedContext context);
        void Removed(GroupRemovedContext context);
        void Renamed(GroupRenamedContext context);
        void UserAdded(UserAddedContext context);
        void UserRemoved(UserRemovedContext context);
    }
}