using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;

namespace SmartPage.Community.Groups.Models {
    public class RecentDepartmentsPart : ContentPart<RecentDepartmentsPartRecord> {

        public string DepartmentName
        {
            get { return Record.DepartmentName; }
            set { Record.DepartmentName = value; }
        }

        [Required]
        public int Count {
            get { return Record.Count; }
            set { Record.Count = value; }
        }
    }
}
