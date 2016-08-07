using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using SmartPage.Community.Groups.Models;

namespace SmartPage.Community.Groups.Handlers {
    public class RecentDepartmentsPartHandler : ContentHandler {
        public RecentDepartmentsPartHandler(IRepository<RecentDepartmentsPartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}