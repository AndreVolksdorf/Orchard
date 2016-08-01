using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.UI.Navigation;
using Orchard.Utility.Extensions;
using SmartPage.Community.Groups.Models;

namespace SmartPage.Community.Groups.Extensions
{
    public static class UrlHelperExtensions
    {

        // Default Route
        public static string Groups(this UrlHelper urlHelper)
        {
            return urlHelper.Action("List", "Group", new { area = Constants.LocalArea });
        }

        /* Group */

        public static string GroupForAdmin(this UrlHelper urlHelper, GroupPart groupPart)
        {
            return urlHelper.Action("Item", "GroupAdmin", new { groupId = groupPart.Id, area = Constants.LocalArea });
        }

        public static string GroupsForAdmin(this UrlHelper urlHelper)
        {
            return urlHelper.Action("List", "GroupAdmin", new { area = Constants.LocalArea });
        }

        public static string GroupCreateForAdmin(this UrlHelper urlHelper)
        {
            return urlHelper.Action("Create", "GroupAdmin", new { area = Constants.LocalArea });
        }

        public static string GroupSelectTypeForAdmin(this UrlHelper urlHelper)
        {
            return urlHelper.Action("SelectType", "GroupAdmin", new { area = Constants.LocalArea });
        }

        public static string GroupEdit(this UrlHelper urlHelper, GroupPart groupPart)
        {
            return urlHelper.Action("Edit", "Group", new { groupId = groupPart.Id, area = Constants.LocalArea, returnUrl = urlHelper.RequestContext.HttpContext.Request.ToUrlString() });
        }

        public static string GroupEditForAdmin(this UrlHelper urlHelper, GroupPart groupPart)
        {
            return urlHelper.Action("Edit", "GroupAdmin", new { groupId = groupPart.Id, area = Constants.LocalArea, returnUrl = urlHelper.RequestContext.HttpContext.Request.ToUrlString() });
        }

        public static string GroupView(this UrlHelper urlHelper, GroupPart groupPart)
        {
            return urlHelper.Action("Item", "Group", new { groupId = groupPart.Id, area = Constants.LocalArea });
        }

        /* Department */

        public static string DepartmentForAdmin(this UrlHelper urlHelper, DepartmentPart departmentPart)
        {
            return urlHelper.Action("Item", "DepartmentAdmin", new { departmentId = departmentPart.Id, area = Constants.LocalArea });
        }

        public static string DepartmentForEditor(this UrlHelper urlHelper, DepartmentPart departmentPart)
        {
            return urlHelper.Action("Edit", "Department", new { departmentId = departmentPart.Id, area = Constants.LocalArea, returnUrl = urlHelper.RequestContext.HttpContext.Request.ToUrlString() });
        }

        public static string DepartmentMoveForAdmin(this UrlHelper urlHelper, DepartmentPart departmentPart)
        {
            return urlHelper.Action("Move", "DepartmentAdmin", new { departmentId = departmentPart.Id, area = Constants.LocalArea, returnUrl = urlHelper.RequestContext.HttpContext.Request.ToUrlString() });
        }

        public static string DepartmentCloseForAdmin(this UrlHelper urlHelper, DepartmentPart departmentPart)
        {
            return urlHelper.Action("Close", "DepartmentAdmin", new { departmentId = departmentPart.Id, area = Constants.LocalArea, returnUrl = urlHelper.RequestContext.HttpContext.Request.ToUrlString() });
        }

        public static string DepartmentOpenForAdmin(this UrlHelper urlHelper, DepartmentPart departmentPart)
        {
            return urlHelper.Action("Open", "DepartmentAdmin", new { departmentId = departmentPart.Id, area = Constants.LocalArea, returnUrl = urlHelper.RequestContext.HttpContext.Request.ToUrlString() });
        }

        public static string DepartmentCreate(this UrlHelper urlHelper, int groupId, string type)
        {
            return urlHelper.Action("Create", "Department", new { groupId = groupId, type = type, area = Constants.LocalArea, returnUrl = urlHelper.RequestContext.HttpContext.Request.ToUrlString() });
        }

        public static string DepartmentCreate(this UrlHelper urlHelper, GroupPart groupPart)
        {
            return urlHelper.Action("Create", "Department", new { groupId = groupPart.Id, area = Constants.LocalArea, returnUrl = urlHelper.RequestContext.HttpContext.Request.ToUrlString() });
        }

        public static string DepartmentView(this UrlHelper urlHelper, DepartmentPart departmentPart)
        {
            return urlHelper.Action("Item", "Department", new { groupId = departmentPart.GroupPart.Id, departmentId = departmentPart.Id, area = Constants.LocalArea });
        }

        public static string DepartmentView(this UrlHelper urlHelper, DepartmentPart departmentPart, Pager pager)
        {
            return urlHelper.Action("Item", "Department", new { groupId = departmentPart.GroupPart.Id,  departmentId = departmentPart.Id, page = pager.Page, area = Constants.LocalArea });
        }

        public static string DepartmentDelete(this UrlHelper urlHelper, DepartmentPart departmentPart)
        {
            return urlHelper.Action("Delete", "Post", new { contentId = departmentPart.Id, area = Constants.LocalArea, returnUrl = urlHelper.RequestContext.HttpContext.Request.ToUrlString() });
        }

        /* Post */

        public static string PostReply(this UrlHelper urlHelper, GroupPostPart postPart)
        {
            return PostCreateByContent(urlHelper, postPart);
        }

        public static string SelectPostType(this UrlHelper urlHelper, int contentId)
        {
            return urlHelper.Action("SelectType", "Post", new { area = Constants.LocalArea, contentId = contentId });
        }

        public static string PostReplyWithQuote(this UrlHelper urlHelper, GroupPostPart postPart)
        {
            return urlHelper.Action("CreateWithQuote", "Post", new { contentId = postPart.Id, area = Constants.LocalArea, returnUrl = urlHelper.RequestContext.HttpContext.Request.ToUrlString() });
        }

        public static string PostView(this UrlHelper urlHelper, GroupPostPart postPart)
        {
            return string.Format("{0}#{1}", DepartmentView(urlHelper, postPart.DepartmentPart), postPart.Id);
        }

        public static string PostView(this UrlHelper urlHelper, GroupPostPart postPart, Pager pager)
        {
            if (pager.Page >= 2)
                return string.Format("{0}#{1}", DepartmentView(urlHelper, postPart.DepartmentPart, pager), postPart.Id);
            else
                return PostView(urlHelper, postPart);
        }

        public static string PostDelete(this UrlHelper urlHelper, GroupPostPart postPart)
        {
            return urlHelper.Action("Delete", "Post", new { contentId = postPart.Id, area = Constants.LocalArea, returnUrl = urlHelper.RequestContext.HttpContext.Request.ToUrlString() });
        }
        public static string PostEdit(this UrlHelper urlHelper, GroupPostPart postPart)
        {
            return urlHelper.Action("Edit", "Post", new { postId = postPart.Id, area = Constants.LocalArea, returnUrl = urlHelper.RequestContext.HttpContext.Request.ToUrlString() });
        }

        private static string PostCreateByContent(this UrlHelper urlHelper, IContent content)
        {
            return urlHelper.Action("Create", "Post", new { contentId = content.Id, area = Constants.LocalArea, returnUrl = urlHelper.RequestContext.HttpContext.Request.ToUrlString() });
        }

        public static string PostCreate(this UrlHelper urlHelper, DepartmentPart departmentPart)
        {
            return PostCreateByContent(urlHelper, departmentPart);
        }

        public static string PostCreate(this UrlHelper urlHelper, DepartmentPart departmentPart, string postType)
        {
            return urlHelper.Action("Create", "Post", new { contentId = departmentPart.Id, type = postType, area = Constants.LocalArea, returnUrl = urlHelper.RequestContext.HttpContext.Request.ToUrlString() });
        }

        /* External */
        public static string DashboardForAdmin(this UrlHelper urlHelper)
        {
            return urlHelper.Action("index", "admin", new { area = "Dashboard" });
        }
    }
}