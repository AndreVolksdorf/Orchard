using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Orchard.Data;
using SmartPage.Community.Roles.Models;

namespace SmartPage.Community.Roles.ImportExport {
    public class RolesExportEventHandler : IExportEventHandler
    {
        private readonly IRepository<RoleRecord> _roleRecordrepository;
        private readonly IRepository<GroupRecord> _groupRecordRepository;

        public RolesExportEventHandler(IRepository<RoleRecord> roleRecordRepository, IRepository<GroupRecord> groupRecordRepository) {
            _roleRecordrepository = roleRecordRepository;
            _groupRecordRepository = groupRecordRepository;
        }

        public void Exporting(dynamic context) {}

        public void Exported(dynamic context) {

            if (!((IEnumerable<string>) context.ExportOptions.CustomSteps).Contains("Groups")) {
                return;
            }

            var roles = _roleRecordrepository.Table.ToList();
            var groups = _groupRecordRepository.Table.ToList();

            if (!roles.Any() && !groups.Any()) {
                return;
            }

            var root = new XElement("Groups");
            context.Document.Element("Orchard").Add(root);

            //TODO: Exporting routine
            foreach (var groupRecord in groups) {
                root.Add(new XElement("Group"),
                    new XAttribute("Name", groupRecord.Name)
                    );
            }

            foreach (var role in roles) {
                root.Add(new XElement("Role",
                                      new XAttribute("Name", role.Name),
                                      new XAttribute("Permissions", string.Join(",", role.RolesPermissions.Select(rolePermission => rolePermission.Permission.Name)))));
            }
        }
    }
}