using System;
using System.Collections.Generic;
using System.Linq;
using Orchard;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Security;
using SmartPage.Community.Roles.Events;
using SmartPage.Community.Roles.Models;
using Orchard.Security.Permissions;
using Orchard.Users.Models;
using SmartPage.Community.Roles.Drivers;

namespace SmartPage.Community.Roles.Services
{
    public class GroupService : IGroupService
    {
        private const string SignalName = "SmartPage.Community.Groups.Services.GroupService";

        private readonly IRepository<GroupRecord> _groupRepository;
        private readonly IRepository<GroupRolesPartRecord> _userGroupsRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;
        private readonly IGroupEventHandler _groupEventHandler;
        private readonly IRepository<RoleRecord> _roleRepository;
        private readonly IRoleService _roleService;
        private readonly IOrchardServices _orchardServices;

        public GroupService(
            IRepository<GroupRecord> groupRepository,
            IRepository<GroupRolesPartRecord> userGroupsRepository,
            ICacheManager cacheManager,
            ISignals signals,
            IGroupEventHandler groupEventHandler,
            IRepository<RoleRecord> roleRepository,
            IRoleService roleService, 
            IOrchardServices orchardServices)
        {

            _groupRepository = groupRepository;
            _userGroupsRepository = userGroupsRepository;
            _cacheManager = cacheManager;
            _signals = signals;
            _groupEventHandler = groupEventHandler;
            _roleRepository = roleRepository;
            _roleService = roleService;
            _orchardServices = orchardServices;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public IEnumerable<GroupRecord> GetGroups()
        {
            var roles = from role in _groupRepository.Table select role;
            return roles.ToList();
        }

        public GroupRecord GetGroup(int id)
        {
            return _groupRepository.Get(id);
        }

        public GroupRecord GetGroupByName(string name)
        {
            return _groupRepository.Get(x => x.Name == name);
        }

        public void CreateGroup(string groupName, IUser user, string roleName)
        {
            if (GetGroupByName(groupName) != null)
                return;

            var groupRecord = new GroupRecord { Name = groupName };
            _groupRepository.Create(groupRecord);
            var roleRecord = _roleRepository.Get(x => x.Name == roleName);
            groupRecord.GroupUsers.Add(new GroupRolesPartRecord { UserId = user.Id, GroupRecord = groupRecord, RoleRecord = roleRecord });

            _groupEventHandler.Created(new GroupCreatedContext { Group = groupRecord });
            TriggerSignal();
        }

        public void UpdateGroup(int id, string groupName, IDictionary<int, string> userRoles)
        {
            var groupRecord = GetGroup(id);
            var currentGroupName = groupRecord.Name;
            var currentUsers = groupRecord.GroupUsers.ToDictionary(x => x.UserId);
            groupRecord.Name = groupName;
            groupRecord.GroupUsers.Clear();

            if (!string.Equals(currentGroupName, groupName))
            {
                _groupEventHandler.Renamed(new GroupRenamedContext { Group = groupRecord, NewGroupName = groupName, PreviousGroupName = currentGroupName });
            }

            foreach (var groupUser in userRoles)
            {
                var userRecord = _orchardServices.ContentManager.Query<UserPart, UserPartRecord>().Where(u => u.Id == groupUser.Key).List().FirstOrDefault();
                var roleRecord = _roleRepository.Get(x => x.Name == groupUser.Value);
                groupRecord.GroupUsers.Add(new GroupRolesPartRecord { UserId = userRecord.Id, GroupRecord = groupRecord, RoleRecord = roleRecord});

                if (!currentUsers.ContainsKey(groupUser.Key))
                _groupEventHandler.UserAdded(new UserAddedContext() { Group = groupRecord, User = userRecord });
                else
                {
                    currentUsers.Remove(groupUser.Key);
                    _groupEventHandler.UserRemoved(new UserRemovedContext() { Group = groupRecord, User = userRecord });
                }
            }
            
            TriggerSignal();
        }

        public void AddUserToGroup(string userName, string usergroupName, string roleName)
        {
            var groupRecord = GetGroupByName(usergroupName);
            var roleRecord = _roleService.GetRoleByName(roleName);
            var userRecord = _orchardServices.ContentManager.Query<UserPart, UserPartRecord>().Where(u => u.UserName == userName).List().FirstOrDefault();
            

            groupRecord.GroupUsers.Add(new GroupRolesPartRecord() { UserId = userRecord.Id, RoleRecord = roleRecord, GroupRecord = groupRecord });

            _groupEventHandler.UserAdded(new UserAddedContext { Group = groupRecord, User = userRecord });
            TriggerSignal();
        }

        public void RemoveUserFromGroup(string userName, string usergroupName, string roleName)
        {
            var groupRecord = GetGroupByName(usergroupName);
            var roleRecord = _roleService.GetRoleByName(roleName);
            var userRecord = _orchardServices.ContentManager.Query<UserPart, UserPartRecord>().Where(u => u.UserName == userName).List().FirstOrDefault();
            var groupRole = _userGroupsRepository.Fetch(gr => gr.RoleRecord.Id == roleRecord.Id && gr.GroupRecord.Id == groupRecord.Id && gr.UserId == userRecord.Id).First();

            _userGroupsRepository.Delete(groupRole);
            _groupEventHandler.UserRemoved(new UserRemovedContext { User = userRecord, Group = groupRecord });
            TriggerSignal();
        }

        public void DeleteGroup(int id)
        {

            var currentGroupGroupRecords = _userGroupsRepository.Fetch(x => x.GroupRecord.Id == id);
            foreach (var userGroupRecord in currentGroupGroupRecords)
            {
                _userGroupsRepository.Delete(userGroupRecord);
            }

            var roleRecord = GetGroup(id);
            _groupRepository.Delete(roleRecord);
            _groupEventHandler.Removed(new GroupRemovedContext { Group = roleRecord });
            TriggerSignal();
        }

        public IDictionary<string, int> GetUsersForGroup(int id)
        {
            var roleRecord = GetGroup(id).GroupUsers.Select(ug => ug.UserId).ToArray();
            return _orchardServices.ContentManager.Query<UserPart, UserPartRecord>().List().ToArray().Where(u => roleRecord.Contains(u.Id)).ToDictionary(x => x.UserName, x => x.Id);
        }

        public IDictionary<string, int> GetUsersForGroupByName(string name)
        {
            return _cacheManager.Get(name, ctx =>
            {
                MonitorSignal(ctx);
                return GetUsersForGroupByNameInner(name);
            });
        }

        public IDictionary<string, int> GetUsersNotInGroup(int id)
        {
            var roleRecord = GetGroup(id).GroupUsers.Select(ug => ug.UserId).ToArray();
            return _orchardServices.ContentManager.Query<UserPart, UserPartRecord>().List().ToArray().Where(u => !roleRecord.Contains(u.Id)).ToDictionary(x => x.UserName, x => x.Id);
        }

        public IDictionary<string, int> GetAllUsers()
        {
            return _orchardServices.ContentManager.Query<UserPart, UserPartRecord>().List().ToDictionary(x => x.UserName, x => x.Id);
        }

        /// <summary>
        /// Verify if the role name is unique
        /// </summary>
        /// <param name="name">Group name</param>
        /// <returns>Returns false if a role with the given name already exits</returns>
        public bool VerifyGroupUnicity(string name)
        {
            return (_groupRepository.Get(x => x.Name == name) == null);
        }


        IDictionary<string, int> GetUsersForGroupByNameInner(string name)
        {
            var roleRecord = GetGroupByName(name);
            return roleRecord == null ? new Dictionary<string, int>() : GetUsersForGroup(roleRecord.Id);
        }


        public void AddUserToUsergroup(int userId, string usergroupName, string roleName)
        {
            var usergroupRecord = GetGroupByName(usergroupName);
            var usergroupRoleRecord = _roleService.GetRoleByName(roleName);
            usergroupRecord.GroupUsers.Add(new GroupRolesPartRecord() { UserId = userId, RoleRecord = usergroupRoleRecord, GroupRecord = usergroupRecord });


            //TODO: Add EventHandler
            //_groupEventHandler.UserAdded(new UserAddedContext { rol = usergroupRoleRecord, Usergroup = usergroupRecord, User = userPart });
            TriggerSignal();
        }

        private void MonitorSignal(AcquireContext<string> ctx)
        {
            ctx.Monitor(_signals.When(SignalName));
        }

        private void TriggerSignal()
        {
            _signals.Trigger(SignalName);
        }
    }
}