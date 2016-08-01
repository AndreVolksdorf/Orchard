using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Orchard.Localization;
using Orchard.UI.Admin;
using Orchard.Logging;
using Orchard.Mvc;
using Orchard;
using Orchard.Mvc.Extensions;
using SmartPage.Community.Roles.Models;
using SmartPage.Community.Roles.Services;
using SmartPage.Community.Roles.ViewModels;
using Orchard.Security;
using Orchard.UI.Notify;

namespace SmartPage.Community.Roles.Controllers
{
    [ValidateInput(false), Admin]
    public class GroupAdminController : Controller
    {
        private readonly IGroupService _groupService;
        private readonly IRoleService _roleService;
        private readonly IAuthorizationService _authorizationService;

        public GroupAdminController(
            IOrchardServices services,
            IGroupService groupService,
            INotifier notifier,
            IAuthorizationService authorizationService, IRoleService roleService)
        {
            Services = services;
            _groupService = groupService;
            _authorizationService = authorizationService;
            _roleService = roleService;

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ActionResult Index()
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageGroups, T("Not authorized to manage groups")))
                return new HttpUnauthorizedResult();

            var model = new GroupsIndexViewModel { Rows = _groupService.GetGroups().OrderBy(r => r.Name).ToList() };

            return View(model);
        }

        [HttpPost, ActionName("Index")]
        public ActionResult IndexPOST()
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageGroups, T("Not authorized to manage groups")))
                return new HttpUnauthorizedResult();

            foreach (string key in Request.Form.Keys)
            {
                if (key.StartsWith("Checkbox.") && Request.Form[key] == "true")
                {
                    int groupId = Convert.ToInt32(key.Substring("Checkbox.".Length));
                    _groupService.DeleteGroup(groupId);
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageGroups, T("Not authorized to manage groups")))
                return new HttpUnauthorizedResult();

            var model = new GroupCreateViewModel { };
            return View(model);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult CreatePOST()
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageGroups, T("Not authorized to manage groups")))
                return new HttpUnauthorizedResult();

            var viewModel = new GroupCreateViewModel();
            TryUpdateModel(viewModel);

            if (string.IsNullOrEmpty(viewModel.Name))
            {
                ModelState.AddModelError("Name", T("Group name can't be empty"));
            }

            var group = _groupService.GetGroupByName(viewModel.Name);
            if (group != null)
            {
                ModelState.AddModelError("Name", T("Group with same name already exists"));
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            _groupService.CreateGroup(viewModel.Name, Services.WorkContext.CurrentUser, "Administrator");
            foreach (string key in Request.Form.Keys)
            {
                if (key.StartsWith("Checkbox.") && Request.Form[key] == "true")
                {
                    _groupService.AddUserToGroup(Services.WorkContext.CurrentUser.UserName, viewModel.Name, "Administrator");
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var group = _groupService.GetGroup(id);
            if (group == null)
            {
                return HttpNotFound();
            }

            if (!Services.Authorizer.Authorize(Permissions.ManageGroups, T("Not authorized to manage groups")))
                return new HttpUnauthorizedResult();


            var model = new GroupEditViewModel
            {
                Name = group.Name,
                Id = group.Id,
                GroupUsers = group.GroupUsers,
                CurrentUsers = _groupService.GetUsersForGroup(id),
                Users = _groupService.GetUsersNotInGroup(id),
                Roles = _roleService.GetRoles().Select(r => r.Name)
            };

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Save")]
        public ActionResult EditSavePOST(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageGroups, T("Not authorized to manage groups")))
                return new HttpUnauthorizedResult();

            var viewModel = new GroupEditViewModel();
            TryUpdateModel(viewModel);

            if (string.IsNullOrEmpty(viewModel.Name))
            {
                ModelState.AddModelError("Name", T("Group name can't be empty"));
            }

            var group = _groupService.GetGroupByName(viewModel.Name);
            if (group != null && group.Id != id)
            {
                ModelState.AddModelError("Name", T("Group with same name already exists"));
            }

            if (!ModelState.IsValid)
            {
                return Edit(id);
            }

            // Save
            IDictionary<int, string> userRoles = new Dictionary<int, string>();
            foreach (string key in Request.Form.Keys)
            {
                if (key.StartsWith("UserId."))
                {

                    var role = Request.Form[key];
                    var userId = int.Parse(key.Substring("UserId.".Length));
                    userRoles.Add(userId, role);
                }
            }
            _groupService.UpdateGroup(viewModel.Id, viewModel.Name, userRoles);
            
            Services.Notifier.Information(T("Your Group has been saved."));
            return RedirectToAction("Edit", new { id });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Delete")]
        public ActionResult EditDeletePOST(int id)
        {
            return Delete(id, null);
        }

        [HttpPost]
        public ActionResult Delete(int id, string returnUrl)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageGroups, T("Not authorized to manage groups")))
                return new HttpUnauthorizedResult();

            _groupService.DeleteGroup(id);
            Services.Notifier.Information(T("Group was successfully deleted."));

            return this.RedirectLocal(returnUrl, () => RedirectToAction("Index"));
        }
    }
}
