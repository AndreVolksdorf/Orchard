using Orchard.Localization;
using Orchard.UI.Navigation;
using Orchard.Security;

namespace SmartPage.Community.Roles
{
    public class AdminMenu : INavigationProvider
    {
        public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.AddImageSet("groups")
                .Add(T("Groups"), "1.5", BuildMenu);
        }

        private void BuildMenu(NavigationItemBuilder menu)
        {
            menu.Add(T("Manage Usergroups"), "3.0",
                item => item.Action("Index", "GroupAdmin", new { area = "SmartPage.Community.Roles" }).Permission(Permissions.ManageGroups));

            menu.Add(T("New Usergroup"), "3.1",
                item =>
                    item.Action("Create", "GroupAdmin", new { area = "SmartPage.Community.Roles" }).Permission(Permissions.ManageGroups));

            menu.Add(T("Manage Roles"), "4",
                item =>
                    item.Action("Index", "Admin", new { area = "SmartPage.Community.Roles" }).Permission(Permissions.ManageRoles));

        }
        
    }
}
