using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;

namespace SmartPage.Community.Roles.Models {
    public class UserGroupsPart : ContentPart, IUserGroups {

        internal LazyField<IList<string>> _groups = new LazyField<IList<string>>();

        public IList<string> Groups {
            get { return _groups.Value; }
        }
    }
}