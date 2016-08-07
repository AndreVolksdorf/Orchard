using System.Linq;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.UI.Navigation;
using SmartPage.Community.Groups.Extensions;
using SmartPage.Community.Groups.Services;

namespace SmartPage.Community.Groups {
    public class AdminMenu : INavigationProvider {
        private readonly IGroupService _groupService;

        public AdminMenu(IGroupService groupService) {
            _groupService = groupService;
        }

        public Localizer T { get; set; }

        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder) {
            builder.AddImageSet("groups")
                .Add(T("Groups"), "1.5", BuildMenu);
        }

        private void BuildMenu(NavigationItemBuilder menu) {
            var groups = _groupService.Get(VersionOptions.AllVersions);
            var groupCount = groups.Count();
            var singleGroup = groupCount == 1 ? groups.ElementAt(0) : null;

            if (groupCount > 0 && singleGroup == null) {
                menu.Add(T("Manage Groups"), "1",
                         item => item.Action("List", "GroupAdmin", new { area = Constants.LocalArea }).Permission(Permissions.ManageOwnGroups));
            }
            else if (singleGroup != null)
                menu.Add(T("Manage Group"), "1.0",
                        item => item.Action("Item", "GroupAdmin", new { area = Constants.LocalArea, groupId = singleGroup.Id }).Permission(Permissions.ManageOwnGroups));
            
            menu.Add(T("New Group"), "1.1",
                    item =>
                    item.Action("Create", "GroupAdmin", new { area = Constants.LocalArea }).Permission(Permissions.CreateGroups));
        }
    }
}