using System.Collections.Generic;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Models;
using SmartPage.Community.Groups.Models;

namespace SmartPage.Community.Groups.Services {
    public interface IGroupService : IDependency {
        GroupPart Get(int id, VersionOptions versionOptions);
        IEnumerable<GroupPart> Get();
        IEnumerable<GroupPart> Get(VersionOptions versionOptions);
        void Delete(GroupPart group);
        IList<ContentTypeDefinition> GetGroupTypes();
        IList<ContentTypeDefinition> GetDepartmentTypes();
    }
}