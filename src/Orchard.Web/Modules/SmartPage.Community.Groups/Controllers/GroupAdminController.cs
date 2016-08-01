using System.Linq;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using SmartPage.Community.Groups.Extensions;
using SmartPage.Community.Groups.Models;
using SmartPage.Community.Groups.Services;

namespace SmartPage.Community.Groups.Controllers {

    [ValidateInput(false), Admin]
    public class GroupAdminController : Controller, IUpdateModel {
        private readonly IOrchardServices _orchardServices;
        private readonly IGroupService _groupService;
        private readonly SmartPage.Community.Roles.Services.IGroupService _groupRecordService;
        private readonly IDepartmentService _departmentService;
        private readonly ISiteService _siteService;
        private readonly IContentManager _contentManager;

        public GroupAdminController(IOrchardServices orchardServices, 
            IGroupService groupService, 
            IDepartmentService departmentService,
            ISiteService siteService,
            IContentManager contentManager,
            IShapeFactory shapeFactory, Roles.Services.IGroupService groupRecordService) {
            _orchardServices = orchardServices;
            _groupService = groupService;
            _departmentService = departmentService;
            _siteService = siteService;
            _contentManager = contentManager;

            T = NullLocalizer.Instance;
            Shape = shapeFactory;
            _groupRecordService = groupRecordService;
        }

        dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public ActionResult Create(string type) {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ManageGroups, T("Not allowed to create groups")))
                return new HttpUnauthorizedResult();

            if (string.IsNullOrWhiteSpace(type)) {
                var groupTypes = _groupService.GetGroupTypes();
                if (groupTypes.Count > 1)
                    return Redirect(Url.GroupSelectTypeForAdmin());

                if (groupTypes.Count == 0) {
                    _orchardServices.Notifier.Warning(T("You have no group types available. Add one to create a group."));
                    return Redirect(Url.DashboardForAdmin());
                }

                type = groupTypes.Single().Name;
            }

            var group = _orchardServices.ContentManager.New<GroupPart>(type);
            if (group == null)
                return HttpNotFound();

            var model = _orchardServices.ContentManager.BuildEditor(group);
            return View((object)model);
        }

        public ActionResult SelectType() {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ManageGroups, T("Not allowed to create groups")))
                return new HttpUnauthorizedResult();

            var groupTypes = _groupService.GetGroupTypes();
            var model = Shape.ViewModel(GroupTypes: groupTypes);
            return View(model);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult CreatePOST(string type) {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ManageGroups, T("Not allowed to create groups")))
                return new HttpUnauthorizedResult();

            if (string.IsNullOrWhiteSpace(type)) {
                var groupTypes = _groupService.GetGroupTypes();
                if (groupTypes.Count > 1)
                    return Redirect(Url.GroupSelectTypeForAdmin());

                if (groupTypes.Count == 0) {
                    _orchardServices.Notifier.Warning(T("You have no group types available. Add one to create a group."));
                    return Redirect(Url.DashboardForAdmin());
                }

                type = groupTypes.Single().Name;
            }

            var group = _orchardServices.ContentManager.New<GroupPart>(type);

            _orchardServices.ContentManager.Create(group, VersionOptions.Draft);
            var model = _orchardServices.ContentManager.UpdateEditor(group, this);

            if (!ModelState.IsValid) {
                _orchardServices.TransactionManager.Cancel();
                return View((object)model);
            }

            _orchardServices.ContentManager.Publish(group.ContentItem);

            return Redirect(Url.GroupForAdmin(group));
        }

        public ActionResult Edit(int groupId) {
            var group = _groupService.Get(groupId, VersionOptions.Latest);

            if (group == null)
                return HttpNotFound();

            if (!_orchardServices.Authorizer.Authorize(Permissions.ManageGroups, group, T("Not allowed to edit group")))
                return new HttpUnauthorizedResult();

            dynamic model = _orchardServices.ContentManager.BuildEditor(group);
            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
            return View((object)model);
        }

        [HttpPost, ActionName("Edit")]
        [InternalFormValueRequired("submit.Save")]
        public ActionResult EditPOST(int groupId) {
            var group = _groupService.Get(groupId, VersionOptions.DraftRequired);

            if (group == null)
                return HttpNotFound();

            if (!_orchardServices.Authorizer.Authorize(Permissions.ManageGroups, group, T("Not allowed to edit group")))
                return new HttpUnauthorizedResult();

            dynamic model = _orchardServices.ContentManager.UpdateEditor(group, this);
            if (!ModelState.IsValid) {
                _orchardServices.TransactionManager.Cancel();
                // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
                return View((object)model);
            }

            _contentManager.Publish(group.ContentItem);
            _orchardServices.Notifier.Information(T("Group information updated"));

            return Redirect(Url.GroupsForAdmin());
        }

        [HttpPost, ActionName("Edit")]
        [InternalFormValueRequired("submit.Delete")]
        public ActionResult EditDeletePOST(int groupId) {
            return Remove(groupId);
        }

        [HttpPost]
        public ActionResult Remove(int groupId) {
            var group = _groupService.Get(groupId, VersionOptions.Latest);

            if (group == null)
                return HttpNotFound();

            if (!_orchardServices.Authorizer.Authorize(Permissions.ManageGroups, group, T("Not allowed to edit group")))
                return new HttpUnauthorizedResult();

            _groupService.Delete(group);

            _orchardServices.Notifier.Information(T("Group was successfully deleted"));
            return Redirect(Url.GroupsForAdmin());
        }

        public ActionResult List() {
            var list = _orchardServices.New.List();
            list.AddRange(_groupService.Get(VersionOptions.Latest)
                              .Select(b => {
                                  var group = _orchardServices.ContentManager.BuildDisplay(b, "SummaryAdmin");
                                  group.TotalPostCount = _departmentService.Get(b, VersionOptions.Latest).Count();
                                  return group;
                              }));

            dynamic viewModel = _orchardServices.New.ViewModel()
                .ContentItems(list);
            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
            return View((object)viewModel);
        }

        public ActionResult Item(int groupId, PagerParameters pagerParameters) {
            Pager pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);

            GroupPart group = _groupService.Get(groupId, VersionOptions.Latest);

            if (group == null)
                return HttpNotFound();

            if (!_orchardServices.Authorizer.Authorize(Permissions.ManageGroups, group, T("Not allowed to view group")))
                return new HttpUnauthorizedResult();

            var departments = _departmentService.Get(group, pager.GetStartIndex(), pager.PageSize, VersionOptions.Latest).ToArray();
            var departmentsShapes = departments.Select(bp => _contentManager.BuildDisplay(bp, "SummaryAdmin")).ToArray();

            dynamic groupShape = _orchardServices.ContentManager.BuildDisplay(group, "DetailAdmin");

            var list = Shape.List();
            list.AddRange(departmentsShapes);
            groupShape.Content.Add(Shape.Parts_Groups_Department_ListAdmin(ContentItems: list), "5");

            var totalItemCount = _departmentService.Count(group, VersionOptions.Latest);
            groupShape.Content.Add(Shape.Pager(pager).TotalItemCount(totalItemCount), "Content:after");

            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
            return View((object)groupShape);
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }
}