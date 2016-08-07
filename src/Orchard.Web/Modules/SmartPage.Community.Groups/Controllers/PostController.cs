using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Themes;
using Orchard.UI.Notify;
using SmartPage.Community.Groups.Extensions;
using SmartPage.Community.Groups.Helpers;
using SmartPage.Community.Groups.Models;
using SmartPage.Community.Groups.Services;
using SmartPage.Community.Groups.ViewModels;

namespace SmartPage.Community.Groups.Controllers {
    [Themed]
    [ValidateInput(false)]
    public class PostController : Controller, IUpdateModel {
        private readonly IOrchardServices _orchardServices;
        private readonly IPostService _postService;

        public PostController(IOrchardServices orchardServices,
            IShapeFactory shapeFactory, IPostService postService) {
            _orchardServices = orchardServices;

            Shape = shapeFactory;
            _postService = postService;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        dynamic Shape { get; set; }
        public ILogger Logger { get; set; }
        public Localizer T { get; set; }

        public ActionResult Create(int contentId, string type) {
            var contentItem = _orchardServices.ContentManager.Get(contentId, VersionOptions.Latest);

            bool isPost = contentItem.Has<GroupPostPart>();
            bool isDepartment = contentItem.Has<DepartmentPart>();

            if (!isPost && !isDepartment)
                return HttpNotFound();
            
            if (string.IsNullOrWhiteSpace(type))
            {
                var postTypes = _postService.GetPostTypes();
                if (postTypes.Count > 1)
                    return Redirect(Url.SelectPostType(contentId));

                if (postTypes.Count == 0)
                {
                    _orchardServices.Notifier.Warning(T("You have no group post types available. Add one to create a group."));
                    return Redirect(Url.DashboardForAdmin());
                }

                type = postTypes.Single().Name;
            }

            var groupPart = HierarchyHelpers.GetGroup(contentItem);
            var part = _orchardServices.ContentManager.New<GroupPostPart>(type);
            part.DepartmentPart = HierarchyHelpers.GetDepartmentPart(contentItem);

            if (!_orchardServices.Authorizer.Authorize(Permissions.EditGroupContent, part, T("Not allowed to create post")))
                return new HttpUnauthorizedResult();

            var model = _orchardServices.ContentManager.BuildEditor(part);

            return View((object)model);
        }

        public ActionResult CreateWithQuote(int contentId) {
            var contentItem = _orchardServices.ContentManager.Get(contentId, VersionOptions.Latest);

            bool isPost = contentItem.Has<GroupPostPart>();
            bool isDepartment = contentItem.Has<DepartmentPart>();

            if (!isPost && !isDepartment)
                return HttpNotFound();

            var groupPart = HierarchyHelpers.GetGroup(contentItem);
            var part = _orchardServices.ContentManager.New<GroupPostPart>(groupPart.PostType);
            part.DepartmentPart = HierarchyHelpers.GetDepartmentPart(contentItem);

            if (!_orchardServices.Authorizer.Authorize(Permissions.EditGroupContent, part, T("Not allowed to create post")))
                return new HttpUnauthorizedResult();

            part.Text = string.Format("<blockquote>{0}</blockquote>{1}", contentItem.As<GroupPostPart>().Text, Environment.NewLine);

            var model = _orchardServices.ContentManager.BuildEditor(part);

            return View("Create", (object)model);
        }

        public ActionResult SelectType(int contentId)
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.EditGroupContent, T("Not allowed to create groups")))
                return new HttpUnauthorizedResult();

            var contentItem = _orchardServices.ContentManager.Get(contentId, VersionOptions.Latest);
            var postTypes = _postService.GetPostTypes();
            var model = Shape.ViewModel(PostTypes: postTypes, ContentId: contentId, ContentItem: contentItem);
            return View(model);
        }
        
        [HttpPost, ActionName("Create")]
        public ActionResult CreatePOST(int contentId, string type) {
            var contentItem = _orchardServices.ContentManager.Get(contentId, VersionOptions.Latest);

            bool isPost = contentItem.Has<GroupPostPart>();
            bool isDepartment = contentItem.Has<DepartmentPart>();

            if (!isPost && !isDepartment)
                return HttpNotFound();

            var groupPart = HierarchyHelpers.GetGroup(contentItem);
            var post = _orchardServices.ContentManager.New<GroupPostPart>(type);
            var departmentPart = HierarchyHelpers.GetDepartmentPart(contentItem);
            post.DepartmentPart = departmentPart;

            if (isDepartment) {
                // Attach to parent post and NOT to the department
                post.RepliedOn = contentItem.As<DepartmentPart>().Id;
            }
            else {
                post.RepliedOn = contentItem.As<GroupPostPart>().Id;
            }

            if (!_orchardServices.Authorizer.Authorize(Permissions.EditGroupContent, post, T("Not allowed to create post")))
                return new HttpUnauthorizedResult();

            _orchardServices.ContentManager.Create(post.ContentItem);
            var model = _orchardServices.ContentManager.UpdateEditor(post, this);

            if (!ModelState.IsValid) {
                _orchardServices.TransactionManager.Cancel();
                return View((object)model);
            }

            _orchardServices.ContentManager.Publish(post.ContentItem);

            _orchardServices.Notifier.Information(T("Your {0} has been created.", post.TypeDefinition.DisplayName));

            var pager = new DepartmentPager(_orchardServices.WorkContext.CurrentSite, post.DepartmentPart.PostCount);
            return Redirect(Url.PostView(post, pager));
        }

        public ActionResult Edit(int postId)
        {
            var group = _postService.Get(postId, VersionOptions.Latest);

            if (group == null)
                return HttpNotFound();

            if (!_orchardServices.Authorizer.Authorize(Permissions.EditGroupContent, group, T("Not allowed to edit posts")))
                return new HttpUnauthorizedResult();

            dynamic model = _orchardServices.ContentManager.BuildEditor(group);
            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
            return View((object)model);
        }

        [HttpPost, ActionName("Edit")]
        [InternalFormValueRequired("submit.Save")]
        public ActionResult EditPOST(int postId)
        {
            var post = _postService.Get(postId, VersionOptions.DraftRequired);

            if (post == null)
                return HttpNotFound();

            if (!_orchardServices.Authorizer.Authorize(Permissions.ManageGroups, post, T("Not allowed to edit group posts")))
                return new HttpUnauthorizedResult();

            dynamic model = _orchardServices.ContentManager.UpdateEditor(post, this);
            if (!ModelState.IsValid)
            {
                _orchardServices.TransactionManager.Cancel();
                // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
                return View((object)model);
            }

            _orchardServices.ContentManager.Publish(post.ContentItem);
            _orchardServices.Notifier.Information(T("Group post updated"));

            return Redirect(Url.DepartmentView(post.DepartmentPart));
        }

        public ActionResult Delete(int contentId) {
            var contentItem = _orchardServices.ContentManager.Get(contentId);

            var department = contentItem.As<DepartmentPart>();

            if (department != null) {
                if (!_orchardServices.Authorizer.Authorize(Orchard.Core.Contents.Permissions.DeleteContent, contentItem, T("Not allowed to delete department")))
                    return new HttpUnauthorizedResult();

                _orchardServices.ContentManager.Remove(contentItem);
                _orchardServices.Notifier.Information(T("Department has been deleted."));
                return Redirect(Url.GroupView(department.GroupPart));
            }

            var post = contentItem.As<GroupPostPart>();

            if (post != null) {
                if (!_orchardServices.Authorizer.Authorize(Orchard.Core.Contents.Permissions.DeleteContent, contentItem, T("Not allowed to delete post")))
                    return new HttpUnauthorizedResult();

                if (post.IsParentDepartment()) {
                    _orchardServices.ContentManager.Remove(post.DepartmentPart.ContentItem);
                    _orchardServices.Notifier.Information(T("Department has been deleted."));
                    return Redirect(Url.GroupView(post.DepartmentPart.GroupPart));
                }
                else {
                    _orchardServices.ContentManager.Remove(contentItem);
                    _orchardServices.Notifier.Information(T("Post has been deleted."));

                    var pager = new DepartmentPager(_orchardServices.WorkContext.CurrentSite, post.DepartmentPart.PostCount);
                    return Redirect(Url.DepartmentView(post.DepartmentPart, pager));
                }
            }

            return Redirect(Url.Groups());
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }
}