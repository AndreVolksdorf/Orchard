using System;
using Orchard.Settings;
using Orchard.UI.Navigation;

namespace SmartPage.Community.Groups.ViewModels {
    public class DepartmentPager : Pager {
        public DepartmentPager(ISite site, int itemCount) :
            this(site, (int) Math.Ceiling((decimal) itemCount/(decimal) site.PageSize), site.PageSize) {
        }

        public DepartmentPager(ISite site, PagerParameters pagerParameters)
            : base(site, pagerParameters) {
        }

        public DepartmentPager(ISite site, int? page, int? pageSize)
            : base(site, page, pageSize) {
        }
    }
}