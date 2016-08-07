using System.Collections.Generic;
using SmartPage.Community.Roles.Models;

namespace SmartPage.Community.Roles.ViewModels
{
    public class GroupRolesViewModel
    {
        public GroupRolesViewModel()
        {
            GroupRoles = new List<GroupRoleEntry>();
        }

        public GroupRecord Group { get; set; }
        public IList<GroupRoleEntry> GroupRoles { get; set; }
    }
}
