using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;
using SmartPage.Maps.Models;

namespace SmartPage.Maps.Handlers {
    
    public class GroupMapMarkersHandler : ContentHandler {
        public GroupMapMarkersHandler(IRepository<GroupMapMarkersPartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}