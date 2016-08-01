using System.Linq;
using JetBrains.Annotations;
using Orchard.Data;
using Orchard.ContentManagement.Handlers;
using SmartPage.Community.Roles.Models;

namespace SmartPage.Community.Roles.Handlers {
    [UsedImplicitly]
    public class GroupRolesPartHandler : ContentHandler {
        private readonly IRepository<GroupRolesPartRecord> _groupRolesRepository;

        public GroupRolesPartHandler(IRepository<GroupRolesPartRecord> groupRolesRepository) {
            _groupRolesRepository = groupRolesRepository;
            
            Filters.Add(new ActivatingFilter<GroupRolesPart>("User"));

            OnInitialized<GroupRolesPart>((context, usergroupUsers) => {
                usergroupUsers._roles.Loader(value =>
                    _groupRolesRepository
                        .Fetch(x => x.UserId == context.ContentItem.Id)
                        .Select(x => x).GroupBy(x => x.GroupRecord.Name).ToDictionary(user => user.Key, user => user.Select(x => x.RoleRecord.Name)));
            });
            
        }
    }
}