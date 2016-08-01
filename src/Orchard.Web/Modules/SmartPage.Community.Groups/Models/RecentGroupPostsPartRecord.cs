using Orchard.ContentManagement.Records;

namespace SmartPage.Community.Groups.Models {
    public class RecentDepartmentsPartRecord : ContentPartRecord {
        public RecentDepartmentsPartRecord() {
            Count = 5;
        }

        public virtual string DepartmentName { get; set; }
        public virtual int Count { get; set; }
    }
}
