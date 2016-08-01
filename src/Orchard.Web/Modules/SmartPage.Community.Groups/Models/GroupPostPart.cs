using System.Web.Management;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using SmartPage.Community.Roles.Models;

namespace SmartPage.Community.Groups.Models {
    public class GroupPostPart : ContentPart<GroupPostPartRecord>, IGroup {
        public int? RepliedOn {
            get { return Record.RepliedOn; }
            set { Record.RepliedOn = value; }
        }

        public string Text {
            get { return Record.Text; }
            set { Record.Text = value; }
        }

        public string Format {
            get { return Record.Format; }
            set { Record.Format = value; }
        }

        public DepartmentPart DepartmentPart {
            get { return this.As<ICommonPart>().Container.As<DepartmentPart>(); }
            set { this.As<ICommonPart>().Container = value; }
        }

        public bool IsParentDepartment() {
            return RepliedOn == null;
        }

        public string GroupName
        {
            get { return this.DepartmentPart != null ? DepartmentPart.GroupName : string.Empty; }
        }
    }
}