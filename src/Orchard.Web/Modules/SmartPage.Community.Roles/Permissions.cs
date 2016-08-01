using System.Collections.Generic;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace SmartPage.Community.Roles
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission ManageRoles = new Permission { Description = "Managing Roles", Name = "ManageRoles" };
        public static readonly Permission AssignRoles = new Permission { Description = "Assign Roles", Name = "AssignRoles", ImpliedBy = new[] { ManageRoles } };

        public static readonly Permission ManageGroups = new Permission { Description = "Managing Groups", Name = "ManageGroups" };
        public static readonly Permission ManageOwnGroups = new Permission { Description = "Managing own Groups", Name = "ManageOwnGroups", ImpliedBy = new[] { ManageGroups } };
        public static readonly Permission CreateGroups = new Permission { Description = "Create own Groups", Name = "CreateGroups", ImpliedBy = new[] { ManageGroups } };
        public static readonly Permission AssignGroups = new Permission { Description = "Assign Groups", Name = "AssignGroups", ImpliedBy = new[] { ManageRoles } };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[] {
                ManageRoles, AssignRoles,CreateGroups,

                ManageGroups,ManageOwnGroups,AssignGroups
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {ManageRoles, AssignRoles, ManageGroups,AssignGroups}
                },
                new PermissionStereotype {
                    Name = "Authenticated",
                    Permissions = new[] { CreateGroups }
                },
            };
        }

    }
}