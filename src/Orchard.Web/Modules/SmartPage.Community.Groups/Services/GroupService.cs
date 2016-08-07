using System.Collections.Generic;
using System.Linq;
using Orchard.Autoroute.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using SmartPage.Community.Groups.Models;

namespace SmartPage.Community.Groups.Services {
    public class GroupService : IGroupService {
        private readonly IContentManager _contentManager;

        public GroupService(IContentManager contentManager) {
            _contentManager = contentManager;
        }

        public IEnumerable<GroupPart> Get() {
            return Get(VersionOptions.Published);
        }

        public IEnumerable<GroupPart> Get(VersionOptions versionOptions) {
            return _contentManager.Query<GroupPart, GroupPartRecord>(versionOptions)
                .WithQueryHints(new QueryHints().ExpandRecords<AutoroutePartRecord, TitlePartRecord, CommonPartRecord>())
                .OrderBy(o => o.Weight)
                .List()
                .ToList();
        }

        public GroupPart Get(int id, VersionOptions versionOptions) {
            return _contentManager.Query<GroupPart, GroupPartRecord>(versionOptions)
                .WithQueryHints(new QueryHints().ExpandRecords<AutoroutePartRecord, TitlePartRecord, CommonPartRecord>())
                .Where(x => x.Id == id)
                .List()
                .SingleOrDefault();
        }

        public void Delete(GroupPart group) {
            _contentManager.Remove(group.ContentItem);
        }

        public IList<ContentTypeDefinition> GetGroupTypes()
        {
            var name = typeof(GroupPart).Name;

            return _contentManager
                .GetContentTypeDefinitions()
                .Where(x =>
                    x.Parts.Any(p => p.PartDefinition.Name == name))
                .Select(x => x)
                .ToList();
        }

        public IList<ContentTypeDefinition> GetDepartmentTypes()
        {
            var name = typeof(DepartmentPart).Name;

            return _contentManager
                .GetContentTypeDefinitions()
                .Where(x =>
                    x.Parts.Any(p => p.PartDefinition.Name == name))
                .Select(x => x)
                .ToList();
        }
    }
}