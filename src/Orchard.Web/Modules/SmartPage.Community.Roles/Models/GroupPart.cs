using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;

namespace SmartPage.Community.Roles.Models
{
    public class GroupPart : ContentPart<GroupRecord>
    {
        public string Title
        {
            get { return this.As<ITitleAspect>().Title; }
        }

        public IEnumerable<GroupRolesPartRecord> GroupUsers
        {
            get
            {
                return Record.GroupUsers;
            }
        }
    }
}
