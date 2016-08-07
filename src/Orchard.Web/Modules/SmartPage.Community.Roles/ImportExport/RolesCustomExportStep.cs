using System.Collections.Generic;

namespace SmartPage.Community.Roles.ImportExport {
    public class RolesCustomExportStep : ICustomExportStep {
        public void Register(IList<string> steps) {
            steps.Add("Roles");
        }
    }
}