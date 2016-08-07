using System.Linq;
using System.Web.Mvc;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Mvc;
using Orchard;
using Orchard.Mvc.Extensions;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Orchard.Settings;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Themes;
using SmartPage.Community.Groups.Services;
using SmartPage.Community.Groups.Models;
using SmartPage.Community.Groups.Extensions;

namespace SmartPage.Community.Groups.Controllers
{
    [Themed]
    public class GroupController : Controller, IUpdateModel
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IGroupService _groupService;
        private readonly SmartPage.Community.Roles.Services.IGroupService _baseGroupService;
        private readonly IDepartmentService _departmentService;
        private readonly ISiteService _siteService;
        public IOrchardServices Services { get; set; }

        public GroupController(IOrchardServices orchardServices,
            IGroupService groupService,
            IDepartmentService departmentService,
            SmartPage.Community.Roles.Services.IGroupService baseGroupService,
            ISiteService siteService,
            IShapeFactory shapeFactory,
            IOrchardServices services)
        {
            _orchardServices = orchardServices;
            _groupService = groupService;
            _departmentService = departmentService;
            _siteService = siteService;
            _baseGroupService = baseGroupService;
            Services = services;

            Shape = shapeFactory;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        dynamic Shape { get; set; }
        protected ILogger Logger { get; set; }
        public Localizer T { get; set; }

        public ActionResult List()
        {
            var groups = _groupService.Get().Select(fbc => _orchardServices.ContentManager.BuildDisplay(fbc, "Summary"));

            var list = Shape.List();
            list.AddRange(groups);

            dynamic viewModel = Shape.ViewModel()
                .ContentItems(list);

            return View((object)viewModel);
        }

        public ActionResult Item(int groupId, PagerParameters pagerParameters)
        {
            var groupPart = _groupService.Get(groupId, VersionOptions.Published);
            if (groupPart == null)
                return HttpNotFound();

            if (!_orchardServices.Authorizer.Authorize(Orchard.Core.Contents.Permissions.ViewContent, groupPart, T("Not allowed to view group")))
                return new HttpUnauthorizedResult();

            Pager pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);

            var departments = _departmentService
                .Get(groupPart, pager.GetStartIndex(), pager.PageSize, VersionOptions.Published)
                .Select(b => _orchardServices.ContentManager.BuildDisplay(b, "Summary"));

            dynamic group = _orchardServices.ContentManager.BuildDisplay(groupPart);

            var list = Shape.List();
            list.AddRange(departments);
            group.Content.Add(Shape.Parts_Groups_Department_List(ContentPart: groupPart, ContentItems: list), "5");

            var totalItemCount = groupPart.DepartmentCount;
            group.Content.Add(Shape.Pager(pager).TotalItemCount(totalItemCount), "Content:after");

            return new ShapeResult(this, group);
        }


        public ActionResult Create(string type)
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.CreateGroups, T("Not allowed to create groups")))
                return new HttpUnauthorizedResult();

            if (string.IsNullOrWhiteSpace(type))
            {
                var groupTypes = _groupService.GetGroupTypes();
                if (groupTypes.Count > 1)
                    return Redirect(Url.GroupSelectTypeForAdmin());

                if (groupTypes.Count == 0)
                {
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

        public ActionResult SelectType()
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.CreateGroups, T("Not allowed to create groups")))
                return new HttpUnauthorizedResult();

            var groupTypes = _groupService.GetGroupTypes();
            var model = Shape.ViewModel(GroupTypes: groupTypes);
            return View(model);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult CreatePOST(string type)
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.CreateGroups, T("Not allowed to create groups")))
                return new HttpUnauthorizedResult();

            if (string.IsNullOrWhiteSpace(type))
            {
                var groupTypes = _groupService.GetGroupTypes();
                if (groupTypes.Count > 1)
                    return Redirect(Url.GroupSelectTypeForAdmin());

                if (groupTypes.Count == 0)
                {
                    _orchardServices.Notifier.Warning(T("You have no group types available. Add one to create a group."));
                    return Redirect(Url.DashboardForAdmin());
                }

                type = groupTypes.Single().Name;
            }

            var group = _orchardServices.ContentManager.New<GroupPart>(type);

            _orchardServices.ContentManager.Create(group, VersionOptions.Draft);
            var model = _orchardServices.ContentManager.UpdateEditor(group, this);

            if (group.Group == null)
            {
                var baseGroup = _baseGroupService.GetGroupByName(group.Title);
                if (baseGroup != null)
                {
                    ModelState.AddModelError("Name", T("Group with same name already exists"));
                }
                else
                {
                    _baseGroupService.CreateGroup(group.Title, Services.WorkContext.CurrentUser, "Administrator");
                    group.Group = _baseGroupService.GetGroupByName(group.Title);
                }
            }

            if (!ModelState.IsValid)
            {
                _orchardServices.TransactionManager.Cancel();
                return View((object)model);
            }

            _orchardServices.ContentManager.Publish(group.ContentItem);

            return Redirect(Url.GroupView(group));
        }

        public ActionResult Edit(int groupId)
        {
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
        public ActionResult EditPOST(int groupId)
        {
            var group = _groupService.Get(groupId, VersionOptions.DraftRequired);

            if (group == null)
                return HttpNotFound();

            if (!_orchardServices.Authorizer.Authorize(Permissions.ManageGroups, group, T("Not allowed to edit group")))
                return new HttpUnauthorizedResult();

            dynamic model = _orchardServices.ContentManager.UpdateEditor(group, this);
            if (!ModelState.IsValid)
            {
                _orchardServices.TransactionManager.Cancel();
                // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
                return View((object)model);
            }

            _orchardServices.ContentManager.Publish(group.ContentItem);
            _orchardServices.Notifier.Information(T("Group information updated"));

            return Redirect(Url.Groups());
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties)
        {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage)
        {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }
}