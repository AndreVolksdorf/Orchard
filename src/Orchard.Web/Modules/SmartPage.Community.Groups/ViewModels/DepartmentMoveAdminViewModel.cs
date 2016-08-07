using System.Collections.Generic;
using SmartPage.Community.Groups.Models;

namespace SmartPage.Community.Groups.ViewModels {
    public class DepartmentMoveAdminViewModel {
        public int DepartmentId { get; set; }
        public int GroupId { get; set; }

        public IEnumerable<GroupPart> AvailableGroups { get; set; }
    }
}