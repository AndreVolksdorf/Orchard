using System.Collections.Generic;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Security;
using SmartPage.Community.Groups.Models;

namespace SmartPage.Community.Groups.Services {
    public interface IPostService : IDependency {
        GroupPostPart Get(int id, VersionOptions versionOptions);
        IEnumerable<GroupPostPart> Get(DepartmentPart departmentPart);
        IEnumerable<GroupPostPart> Get(DepartmentPart departmentPart, VersionOptions versionOptions);
        IEnumerable<GroupPostPart> Get(DepartmentPart departmentPart, int skip, int count);
        IEnumerable<GroupPostPart> Get(DepartmentPart departmentPart, int skip, int count, VersionOptions versionOptions);
        GroupPostPart GetPositional(DepartmentPart departmentPart,
            DepartmentPostPositional positional);
        GroupPostPart GetPositional(DepartmentPart departmentPart, VersionOptions versionOptions,
            DepartmentPostPositional positional);
        IEnumerable<IUser> GetUsersPosted(DepartmentPart part);
        int Count(DepartmentPart departmentPart, VersionOptions versionOptions);
        void Delete(DepartmentPart departmentPart);
        IList<ContentTypeDefinition> GetPostTypes();
    }
}