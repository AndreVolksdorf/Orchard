using System.Collections.Generic;
using System.Linq;
using Orchard.Autoroute.Models;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Orchard.Security;
using SmartPage.Community.Groups.Models;

namespace SmartPage.Community.Groups.Services {
    public class DepartmentService : IDepartmentService {
        private readonly IContentManager _contentManager;

        public DepartmentService(IContentManager contentManager) {
            _contentManager = contentManager;
        }

        public DepartmentPart Get(int groupId, int departmentId, VersionOptions versionOptions) {
            return _contentManager.Query<CommonPart, CommonPartRecord>(versionOptions)
                      .Where(cpr => cpr.Container.Id == groupId)
                      .Join<DepartmentPartRecord>()
                      .Where(o => o.Id == departmentId)
                      .WithQueryHints(new QueryHints().ExpandRecords<AutoroutePartRecord, TitlePartRecord, CommonPartRecord>())
                      .ForPart<DepartmentPart>()
                      .Slice(1)
                      .SingleOrDefault();
        }

        public DepartmentPart Get(int id, VersionOptions versionOptions) {
            return _contentManager
                .Query<DepartmentPart, DepartmentPartRecord>(versionOptions)
                .WithQueryHints(new QueryHints().ExpandRecords<AutoroutePartRecord, TitlePartRecord, CommonPartRecord>())
                .Where(x => x.Id == id).Slice(1).SingleOrDefault();
        }

        public IEnumerable<DepartmentPart> Get(GroupPart groupPart) {
            return Get(groupPart, VersionOptions.Published);
        }

        public IEnumerable<DepartmentPart> Get(GroupPart groupPart, VersionOptions versionOptions) {
            return Get(groupPart, 0, 0, versionOptions);
        }

        public IEnumerable<DepartmentPart> Get(GroupPart groupPart, int skip, int count) {
            return Get(groupPart, skip, count, VersionOptions.Published);
        }

        public IEnumerable<DepartmentPart> Get(GroupPart groupPart, int skip, int count, VersionOptions versionOptions) {
            return GetParentQuery(groupPart, versionOptions)
                .Join<DepartmentPartRecord>()
                .OrderByDescending(o => o.IsSticky)
                .Join<CommonPartRecord>()
                .OrderByDescending(o => o.ModifiedUtc)
                .ForPart<DepartmentPart>()
                .Slice(skip, count)
                .ToList();
        }

        public IEnumerable<DepartmentPart> Get(GroupPart groupPart, IUser user) {
            return GetParentQuery(groupPart, VersionOptions.Published)
                .Where(o => o.OwnerId == user.Id)
                .Join<DepartmentPartRecord>()
                .OrderByDescending(o => o.IsSticky)
                .Join<CommonPartRecord>()
                .OrderByDescending(o => o.ModifiedUtc)
                .ForPart<DepartmentPart>()
                .List()
                .ToList();
        }

        public int Count(GroupPart groupPart, VersionOptions versionOptions) {
            return GetParentQuery(groupPart, versionOptions).Count();
        }

        public void Delete(GroupPart groupPart) {
            Get(groupPart)
                .ToList()
                .ForEach(department => _contentManager.Remove(department.ContentItem));
        }

        private IContentQuery<CommonPart, CommonPartRecord> GetParentQuery(IContent parentPart, VersionOptions versionOptions) {
            return _contentManager.Query<CommonPart, CommonPartRecord>(versionOptions)
                                  .Where(cpr => cpr.Container == parentPart.ContentItem.Record);
        }
    }
}