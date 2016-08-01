using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Data.Conventions;

namespace SmartPage.Community.Roles.Models
{
    public class GroupRecord
    {
        public GroupRecord()
        {
            GroupUsers = new List<GroupRolesPartRecord>();
        }

        public virtual int Id { get; set; }
        public virtual string Name { get; set; }

        [CascadeAllDeleteOrphan]
        public virtual IList<GroupRolesPartRecord> GroupUsers { get; set; }
    }
}