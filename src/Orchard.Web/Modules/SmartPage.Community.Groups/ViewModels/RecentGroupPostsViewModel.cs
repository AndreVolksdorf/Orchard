using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement.MetaData.Models;
using SmartPage.Community.Groups.Models;

namespace SmartPage.Community.Groups.ViewModels {
    public class RecentDepartmentsViewModel
    {
        [Required]
        public int Count { get; set; }
        
        [Required]
        public string DepartmentName { get; set; }

        public IEnumerable<ContentTypeDefinition> Departments { get; set; }

    }
}