using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SmartPage.Community.Roles.Models;

namespace SmartPage.Community.Roles.ViewModels {
    public class GroupEditViewModel  {
        public int Id { get; set; }
        [Required, StringLength(255)]
        public string Name { get; set; }

        public IList<GroupRolesPartRecord> GroupUsers { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public IDictionary<string, int> Users { get; set; }
        public IDictionary<string, int> CurrentUsers { get; set; }
    }
}
