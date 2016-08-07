using System.Collections.Generic;
using Orchard;
using Orchard.ContentManagement;
using SmartPage.Community.Groups.Models;

namespace SmartPage.Community.Groups.Services {
    public interface IDepartmentService : IDependency {
        DepartmentPart Get(int groupId, int departmentId, VersionOptions versionOptions);
        DepartmentPart Get(int id, VersionOptions versionOptions);
        IEnumerable<DepartmentPart> Get(GroupPart groupPart);
        IEnumerable<DepartmentPart> Get(GroupPart groupPart, VersionOptions versionOptions);
        IEnumerable<DepartmentPart> Get(GroupPart groupPart, int skip, int count);
        IEnumerable<DepartmentPart> Get(GroupPart groupPart, int skip, int count, VersionOptions versionOptions);
        int Count(GroupPart groupPart, VersionOptions versionOptions);
        void Delete(GroupPart groupPart);
    }
}