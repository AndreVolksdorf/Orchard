using System.Collections.Generic;
using SmartPage.Community.Roles.Models;
using Orchard.Security;

namespace SmartPage.Community.Roles.ViewModels
{
    public class UserGroupsViewModel
    {
        public UserGroupsViewModel()
        {
            Groups = new List<UserGroupEntry>();
        }

        public IUser User { get; set; }
        public IUserGroups UserGroups { get; set; }
        public IList<UserGroupEntry> Groups { get; set; }
    }

    public class UserGroupEntry
    {
        public int GroupId { get; set; }
        public string Name { get; set; }
        public bool Granted { get; set; }
    }
}
