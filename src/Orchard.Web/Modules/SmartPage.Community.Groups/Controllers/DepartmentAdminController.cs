using System.Linq;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Mvc.Extensions;
using Orchard.Services;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using SmartPage.Community.Groups.Services;
using SmartPage.Community.Groups.ViewModels;

namespace SmartPage.Community.Groups.Controllers {

    [ValidateInput(false), Admin]
    public class DepartmentAdminController : Controller, IUpdateModel {
        private readonly IOrchardServices _orchardServices;
        private readonly IGroupService _groupService;
        private readonly IDepartmentService _departmentService;
        private readonly ISiteService _siteService;
        private readonly IPostService _postService;
        private readonly IClock _clock;

        public DepartmentAdminController(IOrchardServices orchardServices,
            IGroupService groupService,
            IDepartmentService departmentService,
            ISiteService siteService,
            IShapeFactory shapeFactory,
            IPostService postService,
            IClock clock) {
            _orchardServices = orchardServices;
            _groupService = groupService;
            _departmentService = departmentService;
            _siteService = siteService;
            _postService = postService;
            _clock = clock;

            T = NullLocalizer.Instance;
            Shape = shapeFactory;
        }

        dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public ActionResult List(int groupId) {
            var group = _groupService.Get(groupId, VersionOptions.Latest);

            if (group == null)
                return HttpNotFound();

            if (!_orchardServices.Authorizer.Authorize(Orchard.Core.Contents.Permissions.ViewContent, group, T("Not allowed to view group")))
                return new HttpUnauthorizedResult();

            var list = _orchardServices.New.List();

            list.AddRange(_departmentService.Get(group)
                              .Select(b => _orchardServices.ContentManager.BuildDisplay(b, "SummaryAdmin")));

            dynamic viewModel = _orchardServices.New.ViewModel()
                .ContentItems(list);
            
            return View((object)viewModel);
        }

        public ActionResult Item(int departmentId, PagerParameters pagerParameters) {
            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);
            var departmentPart = _departmentService.Get(departmentId, VersionOptions.Latest);

            if (departmentPart == null)
                return HttpNotFound();

            if (!_orchardServices.Authorizer.Authorize(Orchard.Core.Contents.Permissions.ViewContent, departmentPart, T("Not allowed to view department")))
                return new HttpUnauthorizedResult();

            var posts = _postService.Get(departmentPart, pager.GetStartIndex(), pager.PageSize, VersionOptions.Latest)
                .Select(bp => _orchardServices.ContentManager.BuildDisplay(bp, "SummaryAdmin"));

            dynamic department = _orchardServices.ContentManager.BuildDisplay(departmentPart, "DetailAdmin");

            var list = Shape.List();
            list.AddRange(posts);
            department.Content.Add(Shape.Parts_Departments_Post_ListAdmin(ContentItems: list), "5");

            return View((object)department);
        }

        public ActionResult Move(int departmentId) {
            var departmentPart = _departmentService.Get(departmentId, VersionOptions.Latest);

            if (departmentPart == null)
                return HttpNotFound(T("could not find department").Text);

            if (!_orchardServices.Authorizer.Authorize(Permissions.MoveDepartment, departmentPart, T("Not allowed to move department")))
                return new HttpUnauthorizedResult();

            var groups = _groupService.Get();
            //What if I have 1 group?

            var viewModel = new DepartmentMoveAdminViewModel {
                DepartmentId = departmentPart.Id,
                AvailableGroups = groups
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Move(int departmentId, string returnUrl, DepartmentMoveAdminViewModel viewModel) {
            var departmentPart = _departmentService.Get(departmentId, VersionOptions.Latest);

            if (departmentPart == null)
                return HttpNotFound(T("Could not find department").Text);

            if (!_orchardServices.Authorizer.Authorize(Permissions.MoveDepartment, departmentPart, T("Not allowed to move department")))
                return new HttpUnauthorizedResult();

            var groupPart = _groupService.Get(viewModel.GroupId, VersionOptions.Latest);

            if (groupPart == null)
                return HttpNotFound(T("Could not find group").Text);

            var currentGroupName = departmentPart.GroupPart.As<ITitleAspect>().Title;
            var newGroupName = groupPart.As<ITitleAspect>().Title;

            departmentPart.GroupPart = groupPart;
            
            _orchardServices.ContentManager.Publish(departmentPart.ContentItem);

            _orchardServices.Notifier.Information(T("{0} has been moved from {1} to {2}.", departmentPart.TypeDefinition.DisplayName, currentGroupName, newGroupName));

            return this.RedirectLocal(returnUrl, "~/");
        }

        public ActionResult Close(int departmentId) {
            var departmentPart = _departmentService.Get(departmentId, VersionOptions.Latest);

            if (departmentPart == null)
                return HttpNotFound(T("could not find department").Text);

            if (!_orchardServices.Authorizer.Authorize(Permissions.CloseDepartment, departmentPart, T("Not allowed to close department")))
                return new HttpUnauthorizedResult();

            var viewModel = new DepartmentCloseAdminViewModel {
                DepartmentId = departmentPart.Id,
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Close(int departmentId, string returnUrl, DepartmentCloseAdminViewModel viewModel) {
            var departmentPart = _departmentService.Get(departmentId, VersionOptions.Latest);

            if (departmentPart == null)
                return HttpNotFound(T("Could not find department").Text);

            if (!_orchardServices.Authorizer.Authorize(Permissions.CloseDepartment, departmentPart, T("Not allowed to close department")))
                return new HttpUnauthorizedResult();

            departmentPart.ClosedBy = _orchardServices.WorkContext.CurrentUser;
            departmentPart.ClosedOnUtc = _clock.UtcNow;
            departmentPart.ClosedDescription = viewModel.Description;

            _orchardServices.ContentManager.Publish(departmentPart.ContentItem);

            _orchardServices.Notifier.Information(T("{0} has been closed.", departmentPart.TypeDefinition.DisplayName));

            return this.RedirectLocal(returnUrl, "~/");
        }

        public ActionResult Open(int departmentId, string returnUrl) {
            var departmentPart = _departmentService.Get(departmentId, VersionOptions.Latest);

            if (departmentPart == null)
                return HttpNotFound(T("could not find department").Text);

            if (!_orchardServices.Authorizer.Authorize(Permissions.CloseDepartment, departmentPart, T("Not allowed to open department")))
                return new HttpUnauthorizedResult();

            departmentPart.ClosedBy = null;
            departmentPart.ClosedDescription = null;
            departmentPart.ClosedOnUtc = null;

            _orchardServices.Notifier.Information(T("{0} has been opened.", departmentPart.TypeDefinition.DisplayName));

            return this.RedirectLocal(returnUrl, "~/");
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }
}