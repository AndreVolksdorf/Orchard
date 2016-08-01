using System;
using System.Linq;
using JetBrains.Annotations;
using Orchard.ContentManagement.Drivers;
using Orchard.Data;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;
using SmartPage.Community.Roles.Events;
using SmartPage.Community.Roles.Services;
using SmartPage.Community.Roles.ViewModels;
using Orchard.Security;
using Orchard.UI.Notify;
using SmartPage.Community.Roles.Models;

namespace SmartPage.Community.Roles.Drivers
{
    [UsedImplicitly]
    public class UserGroupsPartDriver : ContentPartDriver<UserGroupsPart>
    {
        private readonly IRepository<UserGroupsPartRecord> _userGroupsRepository;
        private readonly IGroupService _groupService;
        private readonly INotifier _notifier;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IGroupEventHandler _groupEventHandlers;
        private const string TemplateName = "Parts/Groups.UserGroups";

        public UserGroupsPartDriver(
            IRepository<UserGroupsPartRecord> userGroupsRepository,
            IGroupService groupService,
            INotifier notifier,
            IAuthenticationService authenticationService,
            IAuthorizationService authorizationService,
            IGroupEventHandler groupEventHandlers)
        {

            _userGroupsRepository = userGroupsRepository;
            _groupService = groupService;
            _notifier = notifier;
            _authenticationService = authenticationService;
            _authorizationService = authorizationService;
            _groupEventHandlers = groupEventHandlers;
            T = NullLocalizer.Instance;
        }

        protected override string Prefix
        {
            get
            {
                return "UserGroups";
            }
        }

        public Localizer T { get; set; }

        protected override DriverResult Editor(UserGroupsPart userGroupsPart, dynamic shapeHelper)
        {
            // don't show editor without apply groups permission
            if (!_authorizationService.TryCheckAccess(Permissions.AssignGroups, _authenticationService.GetAuthenticatedUser(), userGroupsPart))
                return null;

            return ContentShape("Parts_Groups_UserGroups_Edit",
                    () =>
                    {
                        var groups = _groupService.GetGroups().Select(x => new UserGroupEntry
                        {
                            GroupId = x.Id,
                            Name = x.Name,
                            Granted = userGroupsPart.Groups.Contains(x.Name)
                        });
                        var model = new UserGroupsViewModel
                        {
                            User = userGroupsPart.As<IUser>(),
                            UserGroups = userGroupsPart,
                            Groups = groups.ToList(),
                        };
                        return shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: model, Prefix: Prefix);
                    });
        }

        protected override DriverResult Editor(UserGroupsPart userGroupsPart, IUpdateModel updater, dynamic shapeHelper)
        {
            // don't apply editor without apply groups permission
            if (!_authorizationService.TryCheckAccess(Permissions.AssignGroups, _authenticationService.GetAuthenticatedUser(), userGroupsPart))
                return null;

            var model = BuildEditorViewModel(userGroupsPart);
            if (updater.TryUpdateModel(model, Prefix, null, null))
            {
                var currentUserGroupRecords = _userGroupsRepository.Fetch(x => x.UserId == model.User.Id).ToArray();
                var currentGroupRecords = currentUserGroupRecords.Select(x => x.Group);
                var targetGroupRecords = model.Groups.Where(x => x.Granted).Select(x => _groupService.GetGroup(x.GroupId)).ToArray();
                foreach (var addingGroup in targetGroupRecords.Where(x => !currentGroupRecords.Contains(x)))
                {
                    _notifier.Warning(T("Adding group {0} to user {1}", addingGroup.Name, userGroupsPart.As<IUser>().UserName));
                    _userGroupsRepository.Create(new UserGroupsPartRecord { UserId = model.User.Id, Group = addingGroup });
                    _groupEventHandlers.UserAdded(new UserAddedContext { Group = addingGroup, User = model.User });
                }
                foreach (var removingGroup in currentUserGroupRecords.Where(x => !targetGroupRecords.Contains(x.Group)))
                {
                    _notifier.Warning(T("Removing group {0} from user {1}", removingGroup.Group.Name, userGroupsPart.As<IUser>().UserName));
                    _userGroupsRepository.Delete(removingGroup);
                    _groupEventHandlers.UserRemoved(new UserRemovedContext { Group = removingGroup.Group, User = model.User });
                }
            }
            return ContentShape("Parts_Groups_UserGroups_Edit",
                                () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: model, Prefix: Prefix));
        }

        private static UserGroupsViewModel BuildEditorViewModel(UserGroupsPart userGroupsPart)
        {
            return new UserGroupsViewModel { User = userGroupsPart.As<IUser>(), UserGroups = userGroupsPart };
        }

        protected override void Importing(UserGroupsPart part, ImportContentContext context)
        {
            // Don't do anything if the tag is not specified.
            if (context.Data.Element(part.PartDefinition.Name) == null)
            {
                return;
            }

            context.ImportAttribute(part.PartDefinition.Name, "Groups", groups =>
            {

                var userGroups = groups.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                // create new groups
                foreach (var group in userGroups)
                {
                    var groupRecord = _groupService.GetGroupByName(group);

                    // create the group if it doesn't already exist
                    if (groupRecord == null)
                    {
                        _groupService.CreateGroup(group);
                    }
                }

                var currentUserGroupRecords = _userGroupsRepository.Fetch(x => x.UserId == part.ContentItem.Id).ToList();
                var currentGroupRecords = currentUserGroupRecords.Select(x => x.Group).ToList();
                var targetGroupRecords = userGroups.Select(x => _groupService.GetGroupByName(x)).ToList();
                foreach (var addingGroup in targetGroupRecords.Where(x => !currentGroupRecords.Contains(x)))
                {
                    _userGroupsRepository.Create(new UserGroupsPartRecord { UserId = part.ContentItem.Id, Group = addingGroup });
                }
            });
        }

        protected override void Exporting(UserGroupsPart part, ExportContentContext context)
        {
            context.Element(part.PartDefinition.Name).SetAttributeValue("Groups", string.Join(",", part.Groups));
        }
    }
}