using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Orchard.Security.Permissions;

namespace SmartPage.Community.Roles.ViewModels {
    public class GroupCreateViewModel  {
        [Required, StringLength(255)]
        public string Name { get; set; }
    }
}
