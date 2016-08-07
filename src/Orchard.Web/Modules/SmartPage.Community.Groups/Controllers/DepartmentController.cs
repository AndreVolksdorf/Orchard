using System.Linq;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Mvc;
using Orchard.Security;
using Orchard.Settings;
using Orchard.Themes;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using SmartPage.Community.Groups.Extensions;
using SmartPage.Community.Groups.Models;
using SmartPage.Community.Groups.Services;

namespace SmartPage.Community.Groups.Controllers
{
    [Themed]
    [ValidateInput(false)]
    public class DepartmentController : Controller, IUpdateModel
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IGroupService _groupService;
        private readonly IDepartmentService _departmentService;
        private readonly IPostService _postService;
        private readonly ISiteService _siteService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IAuthenticationService _authenticationService;

        public DepartmentController(IOrchardServices orchardServices,
            IGroupService groupService,
            IDepartmentService departmentService,
            IPostService postService,
            ISiteService siteService,
            IShapeFactory shapeFactory,
            IAuthorizationService authorizationService,
            IAuthenticationService authenticationService)
        {
            _orchardServices = orchardServices;
            _groupService = groupService;
            _departmentService = departmentService;
            _postService = postService;
            _siteService = siteService;
            _authorizationService = authorizationService;
            _authenticationService = authenticationService;

            T = NullLocalizer.Instance;
            Shape = shapeFactory;
        }

        dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public ActionResult Create(int groupId)
        {
            var groupPart = _groupService.Get(groupId, VersionOptions.Latest);
            if (groupPart == null)
                return HttpNotFound();

            var department = _orchardServices.ContentManager.New<DepartmentPart>(groupPart.DepartmentType);
            department.GroupPart = groupPart;

            if (!_orchardServices.Authorizer.Authorize(Permissions.EditGroupContent, department, T("Not allowed to create department")))
                return new HttpUnauthorizedResult();

            //var post = _orchardServices.ContentManager.New<GroupPostPart>(groupPart.PostType);
            //post.DepartmentPart = department;

            var departmentModel = _orchardServices.ContentManager.BuildEditor(department);
            //var postModel = _orchardServices.ContentManager.BuildEditor(post);

            //DynamicZoneExtensions.RemoveItemFrom(departmentModel.Sidebar, "Content_SaveButton");

            //var viewModel = Shape.ViewModel()
            //    .Department(departmentModel)
            //    .Post(postModel);
            //return View((object)viewModel);

            return View((object)departmentModel);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult CreatePOST(int groupId)
        {
            var groupPart = _groupService.Get(groupId, VersionOptions.Latest);
            if (groupPart == null)
                return HttpNotFound();

            var department = _orchardServices.ContentManager.Create<DepartmentPart>(groupPart.DepartmentType, VersionOptions.Draft, o => { o.GroupPart = groupPart; });

            if (!_orchardServices.Authorizer.Authorize(Permissions.EditGroupContent, department, T("Not allowed to create department")))
                return new HttpUnauthorizedResult();

            var departmentModel = _orchardServices.ContentManager.UpdateEditor(department, this);


            //var post = _orchardServices.ContentManager.Create<GroupPostPart>(groupPart.PostType, VersionOptions.Draft, o => { o.DepartmentPart = department; });

            //if (!_orchardServices.Authorizer.Authorize(Permissions.EditGroupContent, post, T("Not allowed to create post")))
            //    return new HttpUnauthorizedResult();

            //var postModel = _orchardServices.ContentManager.UpdateEditor(post, this);
            //post.DepartmentPart = department;

            if (!ModelState.IsValid)
            {
                _orchardServices.TransactionManager.Cancel();
                departmentModel.GroupPart = groupPart;
                
                //var viewModel = Shape.ViewModel()
                //.Department(departmentModel)
                //.Post(postModel)
                ;

                return View((object)departmentModel);
            }

            _orchardServices.ContentManager.Publish(department.ContentItem);
            //_orchardServices.ContentManager.Publish(post.ContentItem);

            _orchardServices.Notifier.Information(T("Your {0} has been created.", department.TypeDefinition.DisplayName));
            return Redirect(Url.DepartmentView(department));
        }


        public ActionResult Edit(int departmentId)
        {
            var group = _departmentService.Get(departmentId, VersionOptions.Latest);

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
        public ActionResult EditPOST(int departmentId)
        {
            var department = _departmentService.Get(departmentId, VersionOptions.DraftRequired);

            if (department == null)
                return HttpNotFound();

            if (!_orchardServices.Authorizer.Authorize(Permissions.ManageGroups, department, T("Not allowed to edit group")))
                return new HttpUnauthorizedResult();

            dynamic model = _orchardServices.ContentManager.UpdateEditor(department, this);
            if (!ModelState.IsValid)
            {
                _orchardServices.TransactionManager.Cancel();
                // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
                return View((object)model);
            }

            _orchardServices.ContentManager.Publish(department.ContentItem);
            _orchardServices.Notifier.Information(T("Department updated"));

            return Redirect(Url.DepartmentView(department));
        }


        public ActionResult Item(int groupId, int departmentId, PagerParameters pagerParameters)
        {
            var departmentPart = _departmentService.Get(groupId, departmentId, VersionOptions.Published);
            if (departmentPart == null)
                return HttpNotFound();

            if (!_orchardServices.Authorizer.Authorize(Orchard.Core.Contents.Permissions.ViewContent, departmentPart, T("Not allowed to view department")))
                return new HttpUnauthorizedResult();

            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);
            var posts = _postService.Get(departmentPart, pager.GetStartIndex(), pager.PageSize, VersionOptions.Published)
                .Select(b => _orchardServices.ContentManager.BuildDisplay(b, "Summary"));

            dynamic department = _orchardServices.ContentManager.BuildDisplay(departmentPart);

            var pagerObject = Shape.Pager(pager).TotalItemCount(departmentPart.PostCount);

            var list = Shape.List();
            list.AddRange(posts);
            department.Content.Add(Shape.Parts_Departments_Post_List(ContentPart: departmentPart, ContentItems: list, Pager: pagerObject), "5");

            var part = _orchardServices.ContentManager.New<GroupPostPart>(departmentPart.GroupPart.PostType);

            /* Get Quick Reply Edit Post*/
            /*if (!departmentPart.IsClosed && IsAllowedToCreatePost(part))
            {
                dynamic model = _orchardServices.ContentManager.BuildEditor(part);

                var firstPostId = _postService.GetPositional(departmentPart, DepartmentPostPositional.First).Id;

                department.Content.Add(Shape.Parts_Department_Post_Create(ContentEditor: model, ContentId: firstPostId), "10");
            }*/

            return new ShapeResult(this, department);
        }

        private bool IsAllowedToCreatePost(GroupPostPart postPart)
        {
            return _authorizationService.TryCheckAccess(Orchard.Core.Contents.Permissions.PublishContent, _authenticationService.GetAuthenticatedUser(), postPart);
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