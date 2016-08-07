using System;
using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement.Records;

namespace SmartPage.Community.Groups.Models {
    public class DepartmentPartRecord : ContentPartRecord
    {
        [StringLength(256)]
        public virtual string Description { get; set; }
        public virtual int PostCount { get; set; }
        public virtual bool IsSticky { get; set; }

        public virtual DateTime? ClosedOnUtc { get; set; }
        public virtual int ClosedById { get; set; }
        public virtual string ClosedDescription { get; set; }
    }
}