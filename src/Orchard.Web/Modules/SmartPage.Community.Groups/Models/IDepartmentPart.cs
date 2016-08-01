using System;
using Orchard.ContentManagement;
using Orchard.Security;

namespace SmartPage.Community.Groups.Models {
    public interface IDepartmentPart : IContent {
        DateTime? ClosedOnUtc { get; set; }
        IUser ClosedBy { get; set; }
        string ClosedDescription { get; set; }
    }
}