using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Security;
using Orchard.Security.Permissions;
using SmartPage.Community.Roles.Models;
using SmartPage.Community.Roles.Services;

namespace SmartPage.Community.Groups.Security
{
    public class GroupsAuthorizationEventHandler : IAuthorizationServiceEventHandler
    {
        private static readonly string[] AnonymousUsergroup = { "Anonymous" };
        private static readonly string[] AuthenticatedUsergroup = { "Authenticated" };
        private readonly IRoleService _roleService;
        private readonly IWorkContextAccessor _workContextAccessor;

        public GroupsAuthorizationEventHandler(IWorkContextAccessor workContextAccessor, IRoleService roleService)
        {
            _workContextAccessor = workContextAccessor;
            _roleService = roleService;
        }

        public void Checking(CheckAccessContext context)
        {
            if (!context.Granted && context.Content != null)
            {
                context.Granted = TryCheckAccess(context);
            }
        }

        public void Complete(CheckAccessContext context) { }

        public void Adjust(CheckAccessContext context)
        {
            if (!context.Granted &&
                context.Content.Is<ICommonPart>())
            {
                if (OwnerVariationExists(context.Permission) &&
                    HasOwnership(context.User, context.Content))
                {
                    context.Adjusted = true;
                    context.Permission = GetOwnerVariation(context.Permission);
                }
            }
        }

        private static bool HasOwnership(IUser user, IContent content)
        {
            if (user == null || content == null)
            {
                return false;
            }

            var common = content.As<ICommonPart>();
            if (common == null || common.Owner == null)
            {
                return false;
            }

            return user.Id == common.Owner.Id;
        }

        private static bool OwnerVariationExists(Permission permission)
        {
            return GetOwnerVariation(permission) != null;
        }

        private static Permission GetOwnerVariation(Permission permission)
        {
            if (permission.Name == Permissions.EditGroupContent.Name)
            {
                return Permissions.EditOwnGroupContent;
            }
            if (permission.Name == Permissions.CloseDepartment.Name)
            {
                return Permissions.CloseOwnDepartment;
            }
            if (permission.Name == Permissions.MoveDepartment.Name)
            {
                return Permissions.MoveOwnDepartment;
            }
            if (permission.Name == Permissions.ManageGroups.Name)
            {
                return Permissions.ManageOwnGroups;
            }
            if (permission.Name == Permissions.StickyDepartment.Name)
            {
                return Permissions.StickyOwnDepartment;
            }
            return null;
        }

        private bool TryCheckAccess(CheckAccessContext context)
        {
            for (var adjustmentLimiter = 0; adjustmentLimiter != 3; ++adjustmentLimiter)
            {
                if (!context.Granted && context.User != null)
                {
                    if (!string.IsNullOrEmpty(_workContextAccessor.GetContext().CurrentSite.SuperUser) &&
                        string.Equals(context.User.UserName, _workContextAccessor.GetContext().CurrentSite.SuperUser, StringComparison.Ordinal))
                    {
                        context.Granted = true;
                    }
                }
                if (context.Content != null)
                {
                    var usergroupPart = context.Content.As<IGroup>();

                    if (usergroupPart != null && !string.IsNullOrWhiteSpace(usergroupPart.GroupName))
                    {
                        if (!context.Granted)
                        {
                            // determine which set of GroupPermissions would satisfy the access check
                            var grantingNames = GroupPermissionNames(context.Permission, Enumerable.Empty<string>()).Distinct().ToArray();

                            // determine what set of usergroups should be examined by the access check
                            string[] usergroupsToExamine;
                            if (context.User == null)
                            {
                                usergroupsToExamine = AnonymousUsergroup;
                            }
                            else if (context.User.Has<IGroupRoles>() && context.User.As<IGroupRoles>().Roles.ContainsKey(usergroupPart.GroupName))
                            {
                                // the current user is not null, so get his usergroups and add "Authenticated" to it
                                usergroupsToExamine = context.User.As<IGroupRoles>().Roles[usergroupPart.GroupName].Select(r => r).ToArray();

                                // when it is a simulated anonymous user in the admin
                                if (!usergroupsToExamine.Contains(AnonymousUsergroup[0]))
                                {
                                    usergroupsToExamine = usergroupsToExamine.Concat(AuthenticatedUsergroup).ToArray();
                                }
                            }
                            else
                            {
                                // the user is not null and has no specific usergroup, then it's just "Authenticated"
                                usergroupsToExamine = AuthenticatedUsergroup;
                            }

                            if (usergroupsToExamine
                                    .Any(usergroup => _roleService.GetPermissionsForRoleByName(usergroup)
                                        .Any(groupPermissionName => grantingNames
                                            .Any(grantingName => string.Equals(groupPermissionName, grantingName, StringComparison.OrdinalIgnoreCase))
                                        )
                                    )
                                )
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return context.Granted;
        }

        private static IEnumerable<string> GroupPermissionNames(Permission groupPermission, IEnumerable<string> stack)
        {
            // the given name is tested
            yield return groupPermission.Name;

            // iterate implied GroupPermissions to grant, it present
            if (groupPermission.ImpliedBy != null && groupPermission.ImpliedBy.Any())
            {
                var stackenum = stack as string[] ?? stack.ToArray();
                foreach (var impliedBy in groupPermission.ImpliedBy)
                {
                    // avoid potential recursion
                    if (stackenum.Contains(impliedBy.Name))
                    {
                        continue;
                    }

                    // otherwise accumulate the implied groupPermission names recursively
                    foreach (var impliedName in GroupPermissionNames(impliedBy, stackenum.Concat(new[] { groupPermission.Name })))
                    {
                        yield return impliedName;
                    }
                }
            }
        }
    }
}