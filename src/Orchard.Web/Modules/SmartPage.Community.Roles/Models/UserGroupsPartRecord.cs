using System.Collections.Generic;

namespace SmartPage.Community.Roles.Models
{
    public class UserGroupsPartRecord
    {
        public virtual int Id { get; set; }
        public virtual int UserId { get; set; }
        public virtual GroupRecord Group { get; set; }
    }
}