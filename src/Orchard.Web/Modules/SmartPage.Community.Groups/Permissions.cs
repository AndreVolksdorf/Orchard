using System.Collections.Generic;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace SmartPage.Community.Groups
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission ManageGroups = new Permission { Description = "Manage groups for others", Name = "ManageGroups" };
        public static readonly Permission ManageOwnGroups = new Permission { Description = "Manage own groups", Name = "ManageOwnGroups", ImpliedBy = new[] { ManageGroups } };
        public static readonly Permission CreateGroups = new Permission { Description = "Create own group", Name = "CreateGroups" };

        public static readonly Permission EditGroupContent = new Permission { Description = "Create Content for groups", Name = "EditGroupContent", ImpliedBy = new[] { ManageGroups } };
        public static readonly Permission EditOwnGroupContent = new Permission { Description = "Create Content for own groups", Name = "EditOwnGroupContent", ImpliedBy = new[] { ManageOwnGroups, EditGroupContent } };

        public static readonly Permission MoveDepartment = new Permission { Description = "Move any department to another group", Name = "MoveDepartment" };
        public static readonly Permission MoveOwnDepartment = new Permission { Description = "Move your own department to another group", Name = "MoveOwnDepartment", ImpliedBy = new[] { MoveDepartment } };
        public static readonly Permission StickyDepartment = new Permission { Description = "Allows you to mark any department as Sticky", Name = "StickyDepartment" };
        public static readonly Permission StickyOwnDepartment = new Permission { Description = "Allows you to mark your own department as Sticky", Name = "StickyOwnDepartment", ImpliedBy = new[] { StickyDepartment } };

        public static readonly Permission CloseDepartment = new Permission { Description = "Allows you to close any department", Name = "CloseDepartment" };
        public static readonly Permission CloseOwnDepartment = new Permission { Description = "Allows you to close your own department", Name = "CloseOwnDepartment", ImpliedBy = new[] { CloseDepartment } };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[] {
                ManageGroups,
                ManageOwnGroups,
                CreateGroups,
                EditGroupContent,
                EditOwnGroupContent,

                MoveOwnDepartment,
                MoveDepartment,
                StickyOwnDepartment,
                StickyDepartment,

                CloseOwnDepartment,
                CloseDepartment,
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {

            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {ManageGroups, CreateGroups}
                },
                new PermissionStereotype {
                    Name = "Editor",
                    Permissions = new[] {ManageGroups, CreateGroups }
                },
                new PermissionStereotype {
                    Name = "Moderator",
                    Permissions = new[] {ManageGroups, CreateGroups }
                },
                new PermissionStereotype {
                    Name = "Author",
                    Permissions = new[] {ManageOwnGroups, CreateGroups }
                },
                new PermissionStereotype {
                    Name = "Contributor",
                },

                /*Need to handle*/
                new PermissionStereotype {
                    Name = "Anonymous",
                },
                new PermissionStereotype {
                    Name = "Authenticated",
                    Permissions = new[] { CreateGroups }
                },
            };
        }
    }
}