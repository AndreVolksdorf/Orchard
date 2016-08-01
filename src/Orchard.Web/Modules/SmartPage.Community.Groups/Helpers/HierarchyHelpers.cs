using Orchard.ContentManagement;
using SmartPage.Community.Groups.Models;

namespace SmartPage.Community.Groups.Helpers {
    public static class HierarchyHelpers {
        public static GroupPart GetGroup(IContent content) {
            var postPart = content.As<GroupPostPart>();
            var departmentPart = content.As<DepartmentPart>();

            if (postPart == null) {
                return departmentPart == null ? null : departmentPart.GroupPart;
            }
            return postPart.DepartmentPart.GroupPart;
        }

        public static DepartmentPart GetDepartmentPart(IContent content) {
            return content.Has<DepartmentPart>() ? content.As<DepartmentPart>() : content.As<GroupPostPart>().DepartmentPart;
        }
    }
}