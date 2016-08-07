using System.Collections.Generic;
using System.Linq;
using Orchard.Commands;
using Orchard.ContentManagement;
using Orchard.Data;
using SmartPage.Community.Roles.Models;
using SmartPage.Community.Roles.Services;
using Orchard.Security;
using Orchard.Users.Models;

namespace SmartPage.Community.Roles.Commands {
    public class UserCommands : DefaultOrchardCommandHandler
    {
        private readonly IGroupService _groupService;
        private readonly IRoleService _roleService;
        private readonly IMembershipService _membershipService;
        private readonly IRepository<GroupRolesPartRecord> _userGroupsRepository;
        private readonly IContentManager _contentManager;

        public UserCommands(
            IRoleService roleService, 
            IMembershipService membershipService, 
            IRepository<GroupRolesPartRecord> userGroupsRepository,
            IContentManager contentManager, IGroupService groupService) {
            _roleService = roleService;
            _membershipService = membershipService;
            _userGroupsRepository = userGroupsRepository;
            _contentManager = contentManager;
            _groupService = groupService;
        }

        [OrchardSwitch]
        public string WithFeature { get; set; }

        [OrchardSwitch]
        public string WithPermission { get; set; }

        [OrchardSwitch]
        public bool IncludeGroups { get; set; }

        [OrchardSwitch]
        public bool IncludePermissions { get; set; }

        [CommandHelp("groups role list [/WithFeature:\"feature\"] [/WithPermission:permission] [/IncludeGroups:true|false] [/IncludePermissions:true|false]\r\n\t" + "Lists all roles by name")]
        [CommandName("groups role list")]
        [OrchardSwitches("WithFeature,WithPermission,IncludeGroups,IncludePermissions")]
        public void RoleList() {
            var roleRecords = _roleService.GetRoles().OrderBy(record => record.Name);

            Context.Output.WriteLine(T("List of Users"));
            Context.Output.WriteLine(T("--------------------------"));

            foreach (var roleRecord in roleRecords) {
                if (WithPermission != null) {
                    if (roleRecord.RolesPermissions.All(record => record.Permission.Name != WithPermission)) {
                        continue;
                    }
                }

                if (WithFeature != null) {
                    if (roleRecord.RolesPermissions.All(record => record.Permission.FeatureName != WithFeature)) {
                        continue;
                    }
                }

                PrintRoleRecord(roleRecord, 2);

                if (IncludePermissions || IncludeGroups)
                    Context.Output.WriteLine();
            }
        }

        [CommandHelp("groups role detail <name> [/WithFeature:\"feature\"] [/WithPermission:permission] [/IncludeGroups:true|false] [/IncludePermissions:true|false]\r\n\t" + "Displays User Details")]
        [CommandName("groups role detail")]
        [OrchardSwitches("WithFeature,WithPermission,IncludeGroups,IncludePermissions")]
        public void RoleDetail(string name) {
            var role = _roleService.GetRoleByName(name);
            PrintRoleRecord(role);
        }

        private void PrintRoleRecord(RoleRecord roleRecord, int initialIndent = 0) {
            var secondIndent = initialIndent + 2;

            Context.Output.Write(new string(' ', initialIndent));
            Context.Output.WriteLine(T("{0}", roleRecord.Name));

            if (IncludePermissions) {
                Context.Output.Write(new string(' ', secondIndent));
                Context.Output.WriteLine(T("List of Permissions"));

                Context.Output.Write(new string(' ', secondIndent));
                Context.Output.WriteLine(T("--------------------------"));

                var permissionsEnumerable =
                    roleRecord.RolesPermissions
                        .Where(record => WithFeature == null || record.Permission.FeatureName == WithFeature)
                        .Where(record => WithPermission == null || record.Permission.Name == WithPermission);

                var orderedPermissionsEnumerable =
                    permissionsEnumerable
                        .OrderBy(record => record.Permission.FeatureName)
                        .ThenBy(record => record.Permission.Name);

                foreach (var rolesPermissionsRecord in orderedPermissionsEnumerable) {
                    Context.Output.Write(new string(' ', secondIndent));
                    Context.Output.Write("Feature Name:".PadRight(15));
                    Context.Output.WriteLine(rolesPermissionsRecord.Permission.FeatureName);

                    Context.Output.Write(new string(' ', secondIndent));
                    Context.Output.Write("Permission:".PadRight(15));
                    Context.Output.WriteLine(rolesPermissionsRecord.Permission.Name);

                    Context.Output.Write(new string(' ', secondIndent));
                    Context.Output.Write("Description:".PadRight(15));
                    Context.Output.WriteLine(rolesPermissionsRecord.Permission.Description);
                    Context.Output.WriteLine();
                }
            }

            if (IncludeGroups) {
                var userUsersPartRecords = _userGroupsRepository.Fetch(record => record.GroupRecord.Name == roleRecord.Name);

                Context.Output.Write(new string(' ', secondIndent));
                Context.Output.WriteLine(T("List of Groups"));

                Context.Output.Write(new string(' ', secondIndent));
                Context.Output.WriteLine(T("--------------------------"));

                foreach (var userUsersPartRecord in userUsersPartRecords) {
                    var userUsersPart = _contentManager.Get<UserPart>(userUsersPartRecord.UserId);
                    var user = userUsersPart.As<IUser>();
                    
                    Context.Output.Write(new string(' ', secondIndent));
                    Context.Output.Write("GroupName:".PadRight(15));
                    Context.Output.WriteLine(user.UserName);
                    
                    Context.Output.WriteLine();
                }
            }
        }

        [CommandHelp("groups permission list [/WithFeature:\"feature\"]\r\n\t" + "Lists Permissions")]
        [CommandName("groups permission list")]
        [OrchardSwitches("WithFeature")]
        public void PermissionList() {
            var installedPermissions = _roleService.GetInstalledPermissions();

            IEnumerable<string> featureNames;
            if (WithFeature == null) {
                featureNames = installedPermissions.Keys.OrderBy(s => s);
            }
            else {
                var matchedFeature = installedPermissions.Keys.FirstOrDefault(s => s == WithFeature || s == string.Format("{0} Feature", WithFeature));
                if (matchedFeature == null) {
                    Context.Output.WriteLine("Feature '{0}' is not found", WithFeature);
                    return;
                }

                featureNames = new[] {matchedFeature};
            }

            Context.Output.WriteLine(T("List of Permissions"));
            Context.Output.WriteLine(T("--------------------------"));

            const int firstIndent = 2;
            const int secondIndent = 4;

            foreach (var featureName in featureNames) {
                Context.Output.Write(new string(' ', firstIndent));
                Context.Output.Write("Feature:".PadRight(8));
                Context.Output.WriteLine(featureName);

                foreach (var permission in installedPermissions[featureName].OrderBy(permission => permission.Name)) {
                    if (permission.Category != null) {
                        Context.Output.Write(new string(' ', secondIndent));
                        Context.Output.Write("Category:".PadRight(15));
                        Context.Output.WriteLine(permission.Category);
                    }

                    Context.Output.Write(new string(' ', secondIndent));
                    Context.Output.Write("Permission:".PadRight(15));
                    Context.Output.WriteLine(permission.Name);

                    Context.Output.Write(new string(' ', secondIndent));
                    Context.Output.Write("Description:".PadRight(15));
                    Context.Output.WriteLine(permission.Description);

                    Context.Output.WriteLine();
                }

                Context.Output.WriteLine();
            }
        }

        [CommandHelp("user groups <username>\r\n\t" + "Lists a Group's Users")]
        [CommandName("user groups")]
        public void GetGroupUsers(string username) {
            var user = _membershipService.GetUser(username);

            if (user == null) {
                Context.Output.WriteLine("Username not found");
                return;
            }

            Context.Output.WriteLine(T("List of Groups"));
            Context.Output.WriteLine(T("--------------------------"));

            foreach (var role in user.As<GroupRolesPart>().Roles) {
                Context.Output.Write(new string(' ', 2));
                Context.Output.WriteLine(role.Key);
            }
        }

        [CommandHelp("user add group <username> <group> <role>\r\n\t" + "Adds a Group to a User")]
        [CommandName("user add group")]
        public void GroupAddUser(string username, string group, string role) {
            var user = _membershipService.GetUser(username);

            if (user == null) {
                Context.Output.WriteLine("User not found");
                return;
            }

            var roleRecord = _roleService.GetRoleByName(group);
            if (roleRecord == null)
            {
                Context.Output.WriteLine("Role not found");
                return;
            }

            var groupRecord = _groupService.GetGroupByName(group);
            if (groupRecord == null)
            {
                Context.Output.WriteLine("Group not found");
                return;
            }

            var existingAssociation = _userGroupsRepository.Get(record => record.UserId == user.Id && record.GroupRecord.Id == groupRecord.Id && record.RoleRecord.Id == roleRecord.Id);
            if (existingAssociation != null)
                return;

            Context.Output.WriteLine(T("Adding role {0} to user {1}", roleRecord.Name, user.UserName));
            _userGroupsRepository.Create(new GroupRolesPartRecord() { GroupRecord = groupRecord, UserId = user.Id, RoleRecord = roleRecord});
        }

        [CommandHelp("user remove group <username> <role>\r\n\t" + "Removes a User from a Group")]
        [CommandName("user remove group")]
        public void GroupRemoveUser(string username, string role) {
            var user = _membershipService.GetUser(username);

            if (user == null) {
                Context.Output.WriteLine("Group not found");
                return;
            }

            var roleRecord = _groupService.GetGroupByName(role);
            if (roleRecord == null) {
                Context.Output.WriteLine("User not found");
                return;
            }

            var existingAssociation = _userGroupsRepository.Get(record => record.UserId == user.Id && record.GroupRecord.Id == roleRecord.Id);
            if (existingAssociation == null)
                return;

            Context.Output.WriteLine(T("Removing group {0} from user {1}", roleRecord.Name, user.UserName));
            _userGroupsRepository.Delete(existingAssociation);
        }

        [CommandHelp("groups role add permission <role> <permission>\r\n\t" + "Adds a Permission to a User")]
        [CommandName("groups role add permission")]
        [OrchardSwitches("Force")]
        public void RoleAddPermission(string role, string addPermission) {
            var roleRecord = _roleService.GetRoleByName(role);
            if (roleRecord == null) {
                Context.Output.WriteLine("Role not found");
                return;
            }

            var currentPermissions = _roleService.GetPermissionsForRole(roleRecord.Id).ToList();
            if (currentPermissions.Contains(addPermission))
                return;

            Context.Output.WriteLine(T("Adding permission {0} to role {1}", addPermission, role));

            currentPermissions.Add(addPermission);
            _roleService.UpdateRole(roleRecord.Id, roleRecord.Name, currentPermissions);
        }

        [CommandHelp("groups role remove permission <role> <permission>\r\n\t" + "Removes a Permission from a User")]
        [CommandName("groups role remove permission")]
        public void UserRemovePermission(string role, string removePermission) {
            var roleRecord = _roleService.GetRoleByName(role);
            if (roleRecord == null) {
                Context.Output.WriteLine("Role not found");
                return;
            }

            var currentPermissions = _roleService.GetPermissionsForRole(roleRecord.Id).ToList();
            if (!currentPermissions.Contains(removePermission))
                return;

            Context.Output.WriteLine(T("Removing permission {0} from role {1}", removePermission, role));

            currentPermissions.Remove(removePermission);
            _roleService.UpdateRole(roleRecord.Id, roleRecord.Name, currentPermissions);
        }

        [CommandHelp("groups role create <role>\r\n\t" + "Creates a Role")]
        [CommandName("groups role create")]
        public void RoleCreate(string role) {
            var existingUser = _roleService.GetRoleByName(role);
            if (existingUser != null) {
                Context.Output.WriteLine(T("Role {0} already exists", role));
                return;
            }

            Context.Output.WriteLine(T("Creating role {0}", role));
            _roleService.CreateRole(role);
        }

        [CommandHelp("groups role delete <role>\r\n\t" + "Deletes a Role")]
        [CommandName("groups role delete")]
        public void RoleDelete(string role) {
            var existingUser = _roleService.GetRoleByName(role);
            if (existingUser == null) {
                Context.Output.WriteLine(T("Role {0} doesn't exist", role));
                return;
            }

            Context.Output.WriteLine(T("Deleting role {0}", role));
            _roleService.DeleteRole(existingUser.Id);
        }

    }
}