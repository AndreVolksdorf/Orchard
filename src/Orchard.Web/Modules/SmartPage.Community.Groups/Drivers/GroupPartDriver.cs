using System;
using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using SmartPage.Community.Groups.Models;
using SmartPage.Community.Groups.Settings;
using SmartPage.Community.Roles.Services;

namespace SmartPage.Community.Groups.Drivers
{
    public class GroupPartDriver : ContentPartDriver<GroupPart>
    {
        private readonly IGroupService _groupRecordService;

        public GroupPartDriver(IGroupService groupRecordService)
        {
            _groupRecordService = groupRecordService;
        }

        protected override string Prefix
        {
            get { return "GroupPart"; }
        }

        protected override DriverResult Display(GroupPart part, string displayType, dynamic shapeHelper)
        {
            List<DriverResult> results = new List<DriverResult>();

            if (displayType.Equals("SummaryAdmin", StringComparison.OrdinalIgnoreCase))
            {
                results.Add(ContentShape("Parts_Groups_Group_SummaryAdmin", () => shapeHelper.Parts_Groups_Group_SummaryAdmin()));
            }

            results.AddRange(new[] {
                ContentShape("Parts_Group_Department_Post_Breadcrumb",
                        () => shapeHelper.Parts_Group_Department_Post_Breadcrumb(GroupPart: part)),
                ContentShape("Parts_Groups_Group_Manage",
                    () => shapeHelper.Parts_Groups_Group_Manage()),
                ContentShape("Parts_Groups_Group_Description",
                    () => shapeHelper.Parts_Groups_Group_Description(Description: part.Description)),
                ContentShape("Parts_Groups_Group_GroupReplyCount",
                    () => shapeHelper.Parts_Groups_Group_GroupReplyCount(ReplyCount: part.ReplyCount)),
                ContentShape("Parts_Groups_Group_GroupDepartmentCount",
                    () => shapeHelper.Parts_Groups_Group_GroupDepartmentCount(DepartmentCount: part.DepartmentCount)),
                ContentShape("Parts_Group_Manage",
                    () => shapeHelper.Parts_Group_Manage())
            });

            return Combined(results.ToArray());
        }

        protected override DriverResult Editor(GroupPart groupPart, dynamic shapeHelper)
        {

            var results = new List<DriverResult> {
                ContentShape("Parts_Groups_Group_Fields", () => {
                    if (!groupPart.ContentItem.HasDraft() && !groupPart.ContentItem.HasPublished()) {
                        var settings = groupPart.TypePartDefinition.Settings.GetModel<GroupPartSettings>();
                        groupPart.DepartmentedPosts = settings.DefaultDepartmentedPosts;
                    }
            groupPart.Groups = _groupRecordService.GetGroups();
                    return shapeHelper.EditorTemplate(TemplateName: "Parts.Groups.Group.Fields", Model: groupPart, Prefix: Prefix);
                })
            };


            if (groupPart.Id > 0)
                results.Add(ContentShape("Group_DeleteButton",
                    deleteButton => deleteButton));

            return Combined(results.ToArray());
        }

        protected override DriverResult Editor(GroupPart groupPart, IUpdateModel updater, dynamic shapeHelper)
        {


            updater.TryUpdateModel(groupPart, Prefix, null, null);

            if (groupPart.GroupId != 0 && groupPart.GroupId.HasValue)
            {
                groupPart.Group = _groupRecordService.GetGroup(groupPart.GroupId.Value);
            }

            return Editor(groupPart, shapeHelper);
        }

        protected override void Importing(GroupPart part, ImportContentContext context)
        {
            var description = context.Attribute(part.PartDefinition.Name, "Description");
            if (description != null)
            {
                part.Description = description;
            }

            var departmentCount = context.Attribute(part.PartDefinition.Name, "DepartmentCount");
            if (departmentCount != null)
            {
                part.DepartmentCount = Convert.ToInt32(departmentCount);
            }

            var postCount = context.Attribute(part.PartDefinition.Name, "PostCount");
            if (postCount != null)
            {
                part.PostCount = Convert.ToInt32(postCount);
            }

            var departmentedPosts = context.Attribute(part.PartDefinition.Name, "DepartmentedPosts");
            if (departmentedPosts != null)
            {
                part.DepartmentedPosts = Convert.ToBoolean(departmentedPosts);
            }

            var weight = context.Attribute(part.PartDefinition.Name, "Weight");
            if (weight != null)
            {
                part.Weight = Convert.ToInt32(weight);
            }
        }

        protected override void Exporting(GroupPart part, ExportContentContext context)
        {
            context.Element(part.PartDefinition.Name).SetAttributeValue("Description", part.Description);
            context.Element(part.PartDefinition.Name).SetAttributeValue("DepartmentCount", part.DepartmentCount);
            context.Element(part.PartDefinition.Name).SetAttributeValue("PostCount", part.PostCount);
            context.Element(part.PartDefinition.Name).SetAttributeValue("DepartmentedPosts", part.DepartmentedPosts);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Weight", part.Weight);
        }
    }
}