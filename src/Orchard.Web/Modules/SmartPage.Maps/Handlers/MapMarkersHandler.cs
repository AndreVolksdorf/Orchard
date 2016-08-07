using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using SmartPage.Maps.Models;

namespace SmartPage.Maps.Handlers {
    public class MapMarkersHandler : ContentHandler {
        public MapMarkersHandler(IRepository<MapMarkersPartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}