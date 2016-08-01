using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;

namespace SmartPage.Community.Roles.Models {
    public class GroupRolesPart : ContentPart, IGroupRoles {

        internal LazyField<IDictionary<string, IEnumerable<string>>> _roles = new LazyField<IDictionary<string,IEnumerable<string>>>();

        public IDictionary<string, IEnumerable<string>> Roles {
            get { return _roles.Value; }
        }
    }
}