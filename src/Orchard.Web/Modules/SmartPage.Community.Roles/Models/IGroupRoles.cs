using System.Collections.Generic;
using Orchard.ContentManagement;

namespace SmartPage.Community.Roles.Models {
    public interface IGroupRoles : IContent {
        IDictionary<string, IEnumerable<string>> Roles { get; }
    }
}