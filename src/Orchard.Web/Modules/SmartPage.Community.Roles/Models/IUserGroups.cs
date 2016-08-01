using System.Collections.Generic;
using Orchard.ContentManagement;

namespace SmartPage.Community.Roles.Models {
    public interface IUserGroups : IContent {
        IList<string> Groups { get; }
    }
}