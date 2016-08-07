using Orchard.Events;

namespace SmartPage.Community.Roles.ImportExport {
    public interface IExportEventHandler : IEventHandler {
        void Exporting(dynamic context);
        void Exported(dynamic context);
    }
}