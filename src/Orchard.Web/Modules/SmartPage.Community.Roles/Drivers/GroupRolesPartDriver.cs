using System;
using System.Linq;
using JetBrains.Annotations;
using Orchard.ContentManagement.Drivers;
using Orchard.Data;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using SmartPage.Community.Roles.Events;
using SmartPage.Community.Roles.Models;
using SmartPage.Community.Roles.Services;
using SmartPage.Community.Roles.ViewModels;
using Orchard.Security;
using Orchard.UI.Notify;

namespace SmartPage.Community.Roles.Drivers
{
    [UsedImplicitly]
    public class GroupRolesPartDriver : ContentPartDriver<GroupRolesPart>
    {
        private readonly IRepository<GroupRolesPartRecord> _groupRolesRepository;
        private readonly IRoleService _roleService;
        private readonly INotifier _notifier;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IRoleEventHandler _roleEventHandlers;
        private readonly IGroupEventHandler _groupEventHandlers;
        private const string TemplateName = "Parts/Groups.GroupRoles";

        public GroupRolesPartDriver(
            IRepository<GroupRolesPartRecord> groupRolesRepository,
            IRoleService roleService,
            INotifier notifier,
            IAuthenticationService authenticationService,
            IAuthorizationService authorizationService,
            IRoleEventHandler roleEventHandlers, IGroupEventHandler groupEventHandlers)
        {

            _groupRolesRepository = groupRolesRepository;
            _roleService = roleService;
            _notifier = notifier;
            _authenticationService = authenticationService;
            _authorizationService = authorizationService;
            _roleEventHandlers = roleEventHandlers;
            _groupEventHandlers = groupEventHandlers;
            T = NullLocalizer.Instance;
        }

        protected override string Prefix
        {
            get
            {
                return "GroupRoles";
            }
        }

        public Localizer T { get; set; }

        protected override DriverResult Editor(GroupRolesPart groupRoles, dynamic shapeHelper)
        {
            // don't show editor without apply roles permission
            if (!_authorizationService.TryCheckAccess(Permissions.AssignRoles, _authenticationService.GetAuthenticatedUser(), groupRoles))
                return null;

            return ContentShape("Parts_Groups_GroupRoles_Edit",
                    () =>
                    {

                        _groupRolesRepository
                            .Fetch(x => x.Group.Id == context.ContentItem.Id)
                            .Select(x => x).GroupBy(x => x.Group.Name).ToDictionary(user => user.Key, user => user.Select(x => x.Role.Name))
                            ;
                        var roles = _roleService.GetRoles().Select(x => new GroupRoleEntry
                        {
                            RoleId = x.Id,
                            Name = x.Name,
                            Granted = groupRoles.Roles.Contains(x.Name)
                        });
                        var model = new GroupRolesViewModel
                        {
                            Group = groupRoles.As<IGroup>(),
                            GroupRoles = groupRoles,
                            Roles = roles.ToList(),
                        };
                        return shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: model, Prefix: Prefix);
                    });
        }

        protected override DriverResult Editor(GroupRolesPart userRolesPart, IUpdateModel updater, dynamic shapeHelper)
        {
            // don't apply editor without apply roles permission
            if (!_authorizationService.TryCheckAccess(Permissions.AssignRoles, _authenticationService.GetAuthenticatedUser(), userRolesPart))
                return null;

            var model = BuildEditorViewModel(userRolesPart);

            if (!updater.TryUpdateModel(model, Prefix, null, null)) {
                return ContentShape("Parts_Roles_GroupRoles_Edit",
                    () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: model, Prefix: Prefix));
            }

            var currentGroupRoleRecords = _groupRolesRepository.Fetch(x => x.Group.Id == model.Group.Id).ToArray();
            var currentRoleRecords = currentGroupRoleRecords.Select(x => x.Role);
            var targetRoleRecords = model.Roles.Where(x => x.Granted).Select(x => _roleService.GetRole(x.RoleId)).ToArray();

            foreach (var addingRole in targetRoleRecords.Where(x => !currentRoleRecords.Contains(x)))
            {
                _notifier.Warning(T("Adding role {0} to user {1}", addingRole.Name, userRolesPart.As<IGroup>().Name));
                _groupRolesRepository.Create(new GroupRolesPartRecord { Group = model.Group, Role = addingRole, UserId = model.});
                _groupEventHandlers.UserAdded(new UserAddedContext() { User = addingRole, Group = model.Group });
            }
            foreach (var removingRole in currentGroupRoleRecords.Where(x => !targetRoleRecords.Contains(x.Role)))
            {
                _notifier.Warning(T("Removing role {0} from user {1}", removingRole.Role.Name, userRolesPart.As<IGroup>().Name));
                _groupRolesRepository.Delete(removingRole);
                _groupEventHandlers.GroupRemoved(new GroupRemovedContext { Role = removingRole.Role, Group = model.Group });
            }
            return ContentShape("Parts_Roles_GroupRoles_Edit",
                                () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: model, Prefix: Prefix));
        }

        private static GroupRolesViewModel BuildEditorViewModel(GroupRolesPart userRolesPart)
        {
            return new GroupRolesViewModel { Group = userRolesPart.As<IGroup>(), GroupRoles = userRolesPart };
        }

        //TODO: Import Routine
        //protected override void Importing(GroupRolesPart part, Orchard.ContentManagement.Handlers.ImportContentContext context) {
        //    // Don't do anything if the tag is not specified.
        //    if (context.Data.Element(part.PartDefinition.Name) == null) {
        //        return;
        //    }

        //    context.ImportAttribute(part.PartDefinition.Name, "GroupRoles", roles => {

        //        var userRoles = roles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        //        // create new roles
        //        foreach (var role in userRoles) {
        //            var roleRecord = _roleService.GetRoleByName(role);

        //            // create the role if it doesn't already exist
        //            if (roleRecord == null) {
        //                _roleService.CreateRole(role);
        //            }
        //        }

        //        var currentGroupRoleRecords = _groupRolesRepository.Fetch(x => x.UserId == part.ContentItem.Id).ToList();
        //        var currentRoleRecords = currentGroupRoleRecords.Select(x => x.Role).ToList();
        //        var targetRoleRecords = userRoles.Select(x => _roleService.GetRoleByName(x)).ToList();
        //        foreach (var addingRole in targetRoleRecords.Where(x => !currentRoleRecords.Contains(x))) {
        //            _groupRolesRepository.Create(new GroupRolesPartRecord { GroupId = part.ContentItem.Id, Role = addingRole });
        //        }
        //    });
        //}

        //protected override void Exporting(GroupRolesPart part, Orchard.ContentManagement.Handlers.ExportContentContext context) {
        //    context.Element(part.PartDefinition.Name).SetAttributeValue("GroupRoles", string.Join(",", part.Roles));
        //}
    }
}