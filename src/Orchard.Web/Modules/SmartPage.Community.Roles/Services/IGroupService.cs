using System.Collections.Generic;
using SmartPage.Community.Roles.Models;
using Orchard.Security.Permissions;
using Orchard;
using Orchard.Security;

namespace SmartPage.Community.Roles.Services {
    public interface IGroupService : IDependency {
        IEnumerable<GroupRecord> GetGroups();
        GroupRecord GetGroup(int id);
        GroupRecord GetGroupByName(string name);
        void CreateGroup(string groupName, IUser user, string roleName);
        void UpdateGroup(int id, string roleName, IDictionary<int, string> userRoles);
        void DeleteGroup(int id);

        IDictionary<string, int> GetUsersForGroup(int id);
        IDictionary<string, int> GetUsersForGroupByName(string name);

        IDictionary<string, int> GetUsersNotInGroup(int id);
        IDictionary<string, int> GetAllUsers();

        void AddUserToGroup(string userName, string usergroupName, string roleName);
        void RemoveUserFromGroup(string userName, string usergroupName, string roleName);

        /// <summary>
        /// Verify if the role name is unique
        /// </summary>
        /// <param name="name">Group name</param>
        /// <returns>Returns false if a role with the given name already exits</returns>
        bool VerifyGroupUnicity(string name);
    }
}