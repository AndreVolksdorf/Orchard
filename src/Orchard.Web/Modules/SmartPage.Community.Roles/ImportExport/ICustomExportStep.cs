using System.Collections.Generic;
using Orchard.Events;

namespace SmartPage.Community.Roles.ImportExport {
    public interface ICustomExportStep : IEventHandler {
        void Register(IList<string> steps);
    }
}