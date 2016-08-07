using System;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Core.Common.Models;
using SmartPage.Community.Groups.Models;
using SmartPage.Community.Groups.Services;
using SmartPage.Community.Groups.ViewModels;

namespace SmartPage.Community.Groups.Drivers {
    public class RecentDepartmentsPartDriver : ContentPartDriver<RecentDepartmentsPart> {
        private readonly IGroupService _blogService;
        private readonly IContentManager _contentManager;

        public RecentDepartmentsPartDriver(
            IGroupService blogService, 
            IContentManager contentManager) {
            _blogService = blogService;
            _contentManager = contentManager;
        }

        protected override DriverResult Display(RecentDepartmentsPart part, string displayType, dynamic shapeHelper) {
            return ContentShape("Parts_Departments_RecentDepartments", () => {
                var blog = _contentManager
                    .GetContentTypeDefinitions()
                    .Where(x =>
                        x.Parts.Any(p => p.PartDefinition.Name == part.DepartmentName))
                    .Select(x => x).FirstOrDefault();

                if (blog == null) {
                    return null;
                }

                var blogPosts = _contentManager.Query(VersionOptions.Published, blog.Name)
                    .Join<CommonPartRecord>()
                    .OrderByDescending(cr => cr.CreatedUtc)
                    .Slice(0, part.Count)
                    .Select(ci => ci.As<DepartmentPart>());

                var list = shapeHelper.List();
                list.AddRange(blogPosts.Select(bp => _contentManager.BuildDisplay(bp, "Summary")));

                var blogPostList = shapeHelper.Parts_Groups_GroupPost_List(ContentItems: list);

                return shapeHelper.Parts_Departments_RecentDepartments(ContentItems: blogPostList, Group: blog);
            });
        }

        protected override DriverResult Editor(RecentDepartmentsPart part, dynamic shapeHelper) {
            var viewModel = new RecentDepartmentsViewModel {
                Count = part.Count,
                DepartmentName = part.DepartmentName,
                Departments = _blogService.GetDepartmentTypes().OrderBy(b => b.DisplayName)
            };

            return ContentShape("Parts_Departments_RecentDepartments_Edit",
                                () => shapeHelper.EditorTemplate(TemplateName: "Parts.Departments.RecentDepartments", Model: viewModel, Prefix: Prefix));
        }

        protected override DriverResult Editor(RecentDepartmentsPart part, IUpdateModel updater, dynamic shapeHelper) {
            var viewModel = new RecentDepartmentsViewModel();

            if (updater.TryUpdateModel(viewModel, Prefix, null, null)) {
                part.DepartmentName = viewModel.DepartmentName;
                part.Count = viewModel.Count;
            }

            return Editor(part, shapeHelper);
        }

        protected override void Importing(RecentDepartmentsPart part, ImportContentContext context) {
            // Don't do anything if the tag is not specified.
            if (context.Data.Element(part.PartDefinition.Name) == null) {
                return;
            }

            context.ImportAttribute(part.PartDefinition.Name, "DepartmentName", blog =>
                part.DepartmentName = blog
            );

            context.ImportAttribute(part.PartDefinition.Name, "Count", count =>
               part.Count = Convert.ToInt32(count)
            );
        }

        protected override void Exporting(RecentDepartmentsPart part, ExportContentContext context) {

            context.Element(part.PartDefinition.Name).SetAttributeValue("DepartmentName", part.DepartmentName);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Count", part.Count);
        }
    }
}