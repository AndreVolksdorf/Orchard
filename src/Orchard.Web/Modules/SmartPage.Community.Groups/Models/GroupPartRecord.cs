using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement.Records;
using Orchard.Data.Conventions;
using SmartPage.Community.Roles.Models;

namespace SmartPage.Community.Groups.Models {
    public class GroupPartRecord : ContentPartRecord {
        [StringLength(256)]
        public virtual string Description { get; set; }

        public virtual int DepartmentCount { get; set; }
        public virtual int PostCount { get; set; }

        public virtual GroupRecord GroupRecord { get; set; }

        public virtual bool DepartmentedPosts { get; set; }

        public virtual int Weight { get; set; }
    }
}