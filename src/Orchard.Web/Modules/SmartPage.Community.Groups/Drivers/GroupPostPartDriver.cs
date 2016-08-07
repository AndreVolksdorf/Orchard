using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Services;
using SmartPage.Community.Groups.Models;
using SmartPage.Community.Groups.Settings;
using SmartPage.Community.Groups.ViewModels;

namespace SmartPage.Community.Groups.Drivers
{
    public class GroupPostPartDriver : ContentPartDriver<GroupPostPart>
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IEnumerable<IHtmlFilter> _htmlFilters;
        private readonly RequestContext _requestContext;
        private readonly IContentManager _contentManager;

        private const string TemplateName = "Parts.Departments.Post.Body";

        public GroupPostPartDriver(IWorkContextAccessor workContextAccessor,
            IEnumerable<IHtmlFilter> htmlFilters,
            RequestContext requestContext,
            IContentManager contentManager)
        {
            _workContextAccessor = workContextAccessor;
            _htmlFilters = htmlFilters;
            _requestContext = requestContext;
            _contentManager = contentManager;
        }

        protected override string Prefix
        {
            get { return "GroupPostPart"; }
        }

        protected override DriverResult Display(GroupPostPart part, string displayType, dynamic shapeHelper)
        {
            return Combined(
            ContentShape("Parts_Group_Department_Post_Breadcrumb",
                    () => shapeHelper.Parts_Group_Department_Post_Breadcrumb(GroupPostPart: part,DepartmentPart: part.DepartmentPart, GroupPart: part.DepartmentPart.GroupPart)),
            ContentShape("Parts_Departments_Post_Body",
                             () =>
                             {
                                 var bodyText = _htmlFilters.Aggregate(part.Text, (text, filter) => filter.ProcessContent(text, GetFlavor(part)));
                                 return shapeHelper.Parts_Departments_Post_Body(Html: new HtmlString(bodyText));
                             }),
                ContentShape("Parts_Departments_Post_Body_Summary",
                             () =>
                             {
                                 var pager = new DepartmentPager(_workContextAccessor.GetContext().CurrentSite, part.DepartmentPart.PostCount);
                                 var bodyText = _htmlFilters.Aggregate(part.Text, (text, filter) => filter.ProcessContent(text, GetFlavor(part)));
                                 return shapeHelper.Parts_Departments_Post_Body_Summary(Html: new HtmlString(bodyText), Pager: pager);
                             }),
                ContentShape("Parts_Department_Post_Manage", () =>
                {
                    var newPost = _contentManager.New<GroupPostPart>(part.ContentItem.ContentType);
                    newPost.DepartmentPart = part.DepartmentPart;
                    return shapeHelper.Parts_Department_Post_Manage(ContentPart: part, NewPost: newPost);
                }),
                ContentShape("Parts_Department_Post_Meta", () =>
                    shapeHelper.Parts_Department_Post_Meta(ContentPart: part)),
                ContentShape("Parts_Department_Post_Metadata_SummaryAdmin", () =>
                    shapeHelper.Parts_Department_Post_Metadata_SummaryAdmin(ContentPart: part))
                );
        }

        protected override DriverResult Editor(GroupPostPart part, dynamic shapeHelper)
        {
            var model = BuildEditorViewModel(part, _requestContext);
            return Combined(ContentShape("Parts_Departments_Post_Body_Edit",
                                () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: model, Prefix: Prefix)));
        }

        protected override DriverResult Editor(GroupPostPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            var model = BuildEditorViewModel(part, _requestContext);
            updater.TryUpdateModel(model, Prefix, null, null);

            return Combined(ContentShape("Parts_Departments_Post_Body_Edit",
                                () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: model, Prefix: Prefix)));
        }

        private static PostBodyEditorViewModel BuildEditorViewModel(GroupPostPart part, RequestContext requestContext)
        {
            return new PostBodyEditorViewModel
            {
                GroupPostPart = part,
                EditorFlavor = GetFlavor(part),
            };
        }

        private static string GetFlavor(GroupPostPart part)
        {
            var typePartSettings = part.Settings.GetModel<GroupPostTypePartSettings>();
            return (typePartSettings != null && !string.IsNullOrWhiteSpace(typePartSettings.Flavor))
                       ? typePartSettings.Flavor
                       : part.PartDefinition.Settings.GetModel<GroupPostPartSettings>().FlavorDefault;
        }

        protected override void Importing(GroupPostPart part, ImportContentContext context)
        {
            var format = context.Attribute(part.PartDefinition.Name, "Format");
            if (format != null)
            {
                part.Format = format;
            }

            var repliedOn = context.Attribute(part.PartDefinition.Name, "RepliedOn");
            if (repliedOn != null)
            {
                part.RepliedOn = context.GetItemFromSession(repliedOn).Id;
            }

            var text = context.Attribute(part.PartDefinition.Name, "Text");
            if (text != null)
            {
                part.Text = text;
            }
        }

        protected override void Exporting(GroupPostPart part, ExportContentContext context)
        {
            context.Element(part.PartDefinition.Name).SetAttributeValue("Format", part.Format);

            if (part.RepliedOn != null)
            {
                var repliedOnIdentity = _contentManager.GetItemMetadata(_contentManager.Get(part.RepliedOn.Value)).Identity;
                context.Element(part.PartDefinition.Name).SetAttributeValue("RepliedOn", repliedOnIdentity.ToString());
            }

            context.Element(part.PartDefinition.Name).SetAttributeValue("Text", part.Text);
        }
    }
}