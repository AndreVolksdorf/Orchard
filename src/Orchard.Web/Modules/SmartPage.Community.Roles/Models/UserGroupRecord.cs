using System.Collections.Generic;
using Orchard.Data.Conventions;

namespace SmartPage.Community.Roles.Models {
    public class UserGroupRecord {
        public UserGroupRecord() {
            UsersGroups = new List<UsersGroupsRecord>();
        }

        public virtual int Id { get; set; }
        public virtual string Name { get; set; }

        [CascadeAllDeleteOrphan]
        public virtual IList<UsersGroupsRecord> UsersGroups { get; set; }
    }
}