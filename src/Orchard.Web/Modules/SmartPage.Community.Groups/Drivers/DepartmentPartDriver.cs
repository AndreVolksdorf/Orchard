using System;
using System.Collections.Generic;
using System.Xml;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Security;
using SmartPage.Community.Groups.Models;
using SmartPage.Community.Groups.Services;
using SmartPage.Community.Groups.ViewModels;

namespace SmartPage.Community.Groups.Drivers
{
    public class DepartmentPartDriver : ContentPartDriver<DepartmentPart>
    {
        private readonly IPostService _postService;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IContentManager _contentManager;
        private readonly IMembershipService _membershipService;

        public DepartmentPartDriver(
            IPostService postService,
            IWorkContextAccessor workContextAccessor,
            IContentManager contentManager,
            IMembershipService membershipService)
        {
            _postService = postService;
            _workContextAccessor = workContextAccessor;
            _contentManager = contentManager;
            _membershipService = membershipService;
        }

        protected override string Prefix
        {
            get { return "DepartmentPart"; }
        }

        protected override DriverResult Display(DepartmentPart part, string displayType, dynamic shapeHelper)
        {
            var results = new List<DriverResult>();

            if (displayType.Equals("SummaryAdmin", StringComparison.OrdinalIgnoreCase))
            {
                results.Add(ContentShape("Parts_Departments_Department_SummaryAdmin",
                    () => shapeHelper.Parts_Departments_Department_SummaryAdmin()));
                results.Add(ContentShape("Parts_Departments_Department_Metadata_SummaryAdmin",
                    () => shapeHelper.Parts_Departments_Department_Metadata_SummaryAdmin()));
            }

            if (part.IsClosed)
            {
                results.Add(ContentShape("Parts_Departments_Department_Closed",
                        () => shapeHelper.Parts_Departments_Department_Closed()));
            }

            results.Add(ContentShape("Parts_Group_Department_Post_Breadcrumb",
                    () => shapeHelper.Parts_Group_Department_Post_Breadcrumb(DepartmentPart: part, GroupPart: part.GroupPart)));

            results.AddRange(new[] {
                ContentShape("Parts_Departments_Department_DepartmentReplyCount",
                    () => shapeHelper.Parts_Departments_Department_DepartmentReplyCount(ReplyCount: part.ReplyCount)),
                ContentShape("Parts_Department_Manage", () => {
                    var newPost = _contentManager.New<GroupPostPart>(part.GroupPart.PostType);
                    newPost.DepartmentPart = part;
                    var postTypes = _postService.GetPostTypes();
                    return shapeHelper.Parts_Department_Manage(NewPost: newPost, PostTypes: postTypes, DepartmentPart: part,ContentId: part.ContentItem.Id);
                }),
                ContentShape("Parts_Department_Description",
                    () => shapeHelper.Parts_Groups_Group_Description(Description: part.Description)),
                ContentShape("Parts_Department_Group", () => {
                    return shapeHelper.Parts_Department_Group(DepartmentPart: part);
                }),
                ContentShape("Group_Metadata_First", () => shapeHelper.Group_Metadata_First(Post: part.FirstPost)),
                ContentShape("Group_Metadata_Latest", () => {
                        var post = part.LatestPost;
                        var pager = new DepartmentPager(_workContextAccessor.GetContext().CurrentSite, part.PostCount);
                        return shapeHelper.Group_Metadata_Latest(Post: post, Pager: pager);
                    }),
                ContentShape("Parts_Department_Posts_Users", () => {
                        var users = _postService.GetUsersPosted(part);
                        return shapeHelper.Parts_Department_Posts_Users(Users: users);
                    })
            });

            return Combined(results.ToArray());
        }

        protected override DriverResult Editor(DepartmentPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_Departments_Department_Fields", () =>
                shapeHelper.EditorTemplate(TemplateName: "Parts.Departments.Department.Fields", Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(DepartmentPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);

            return Editor(part, shapeHelper);
        }

        protected override void Importing(DepartmentPart part, ImportContentContext context)
        {
            var postCount = context.Attribute(part.PartDefinition.Name, "PostCount");
            if (postCount != null)
            {
                part.PostCount = Convert.ToInt32(postCount);
            }

            var isSticky = context.Attribute(part.PartDefinition.Name, "IsSticky");
            if (isSticky != null)
            {
                part.IsSticky = Convert.ToBoolean(isSticky);
            }

            var closedOnUtc = context.Attribute(part.PartDefinition.Name, "ClosedOnUtc");
            if (closedOnUtc != null)
            {
                part.ClosedOnUtc = XmlConvert.ToDateTime(closedOnUtc, XmlDateTimeSerializationMode.Utc);

                var closedBy = context.Attribute(part.PartDefinition.Name, "ClosedBy");
                if (closedBy != null)
                {
                    var contentIdentity = new ContentIdentity(closedBy);
                    part.ClosedBy = _membershipService.GetUser(contentIdentity.Get("User.UserName"));
                }

                var closedDescription = context.Attribute(part.PartDefinition.Name, "ClosedDescription");
                if (closedDescription != null)
                {
                    part.ClosedDescription = closedDescription;
                }
            }
        }

        protected override void Exporting(DepartmentPart part, ExportContentContext context)
        {
            context.Element(part.PartDefinition.Name).SetAttributeValue("PostCount", part.PostCount);
            context.Element(part.PartDefinition.Name).SetAttributeValue("IsSticky", part.IsSticky);

            if (part.ClosedOnUtc != null)
            {
                context.Element(part.PartDefinition.Name)
                    .SetAttributeValue("ClosedOnUtc", XmlConvert.ToString(part.ClosedOnUtc.Value, XmlDateTimeSerializationMode.Utc));

                if (part.ClosedBy != null)
                {
                    var closedByIdentity = _contentManager.GetItemMetadata(part.ClosedBy).Identity;
                    context.Element(part.PartDefinition.Name).SetAttributeValue("ClosedBy", closedByIdentity.ToString());
                }

                context.Element(part.PartDefinition.Name).SetAttributeValue("ClosedDescription", part.ClosedDescription);
            }
        }
    }
}