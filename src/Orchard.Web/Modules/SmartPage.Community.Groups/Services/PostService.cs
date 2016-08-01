using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Core.Common.Models;
using Orchard.Data;
using Orchard.Security;
using SmartPage.Community.Groups.Models;

namespace SmartPage.Community.Groups.Services {
    public class PostService : IPostService {
        private readonly IContentManager _contentManager;
        private readonly IRepository<CommonPartRecord> _commonRepository;

        public PostService(IContentManager contentManager, IRepository<CommonPartRecord> commonRepository) {
            _contentManager = contentManager;
            _commonRepository = commonRepository;
        }

        public IEnumerable<GroupPostPart> Get(DepartmentPart departmentPart) {
            return Get(departmentPart, VersionOptions.Published);
        }

        public IEnumerable<GroupPostPart> Get(DepartmentPart departmentPart, VersionOptions versionOptions) {
            return GetParentQuery(departmentPart, versionOptions)
                .ForPart<GroupPostPart>()
                .List();
        }

        public GroupPostPart Get(int id, VersionOptions versionOptions) {
            return _contentManager.Query<GroupPostPart, GroupPostPartRecord>(versionOptions)
                .WithQueryHints(new QueryHints().ExpandRecords<CommonPartRecord>())
                .Where(x => x.Id == id)
                .List()
                .SingleOrDefault();
        }

        public GroupPostPart GetPositional(DepartmentPart departmentPart, 
            DepartmentPostPositional positional) {
            return GetPositional(departmentPart, VersionOptions.Published, positional);
        }

        public GroupPostPart GetPositional(DepartmentPart departmentPart, VersionOptions versionOptions,
                                      DepartmentPostPositional positional) {
            var query = GetParentQuery(departmentPart, versionOptions);

            if (positional == DepartmentPostPositional.First)
                query = query.OrderBy(o => o.PublishedUtc);

            if (positional == DepartmentPostPositional.Latest)
                query = query.OrderByDescending(o => o.PublishedUtc);

            return query
                .ForPart<GroupPostPart>()
                .Slice(1)
                .SingleOrDefault();
        }

        public IList<ContentTypeDefinition> GetPostTypes()
        {
            var name = typeof(GroupPostPart).Name;

            return _contentManager
                .GetContentTypeDefinitions()
                .Where(x =>
                    x.Parts.Any(p => p.PartDefinition.Name == name))
                .Select(x => x)
                .ToList();
        }

        public IEnumerable<IUser> GetUsersPosted(DepartmentPart part) {
            var users = _commonRepository.Table.Where(o => o.Container.Id == part.Id)
                             .Select(o => o.OwnerId)
                             .Distinct();

            return _contentManager
                .GetMany<IUser>(users, VersionOptions.Published, new QueryHints())
                .ToList();
        }

        public int Count(DepartmentPart departmentPart, VersionOptions versionOptions) {
            return GetParentQuery(departmentPart, versionOptions).Count();
        }

        public void Delete(DepartmentPart departmentPart) {
            Get(departmentPart, VersionOptions.AllVersions)
                .ToList()
                .ForEach(post => _contentManager.Remove(post.ContentItem));
        }

        public IEnumerable<GroupPostPart> Get(DepartmentPart departmentPart, int skip, int count) {
            return Get(departmentPart, skip, count, VersionOptions.Published);
        }

        public IEnumerable<GroupPostPart> Get(DepartmentPart departmentPart, int skip, int count, VersionOptions versionOptions) {
            return GetParentQuery(departmentPart, versionOptions)
                .OrderBy(o => o.CreatedUtc)
                .ForPart<GroupPostPart>()
                .Slice(skip, count)
                .ToList();
        }

        private IContentQuery<CommonPart, CommonPartRecord> GetParentQuery(IContent parentPart, VersionOptions versionOptions) {
            return _contentManager.Query<CommonPart, CommonPartRecord>(versionOptions)
                                  .Where(cpr => cpr.Container == parentPart.ContentItem.Record);
        }
    }
}