using System.Linq;
using System.Web.Routing;
using JetBrains.Annotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Core.Common.Models;
using Orchard.Data;
using Orchard.Security;
using SmartPage.Community.Groups.Extensions;
using SmartPage.Community.Groups.Models;
using SmartPage.Community.Groups.Services;

namespace SmartPage.Community.Groups.Handlers
{
    [UsedImplicitly]
    public class DepartmentPartHandler : ContentHandler
    {
        private readonly IPostService _postService;
        private readonly IDepartmentService _departmentService;
        private readonly IGroupService _groupService;
        private readonly IContentManager _contentManager;

        public DepartmentPartHandler(IRepository<DepartmentPartRecord> repository,
            IPostService postService,
            IDepartmentService departmentService,
            IGroupService groupService,
            IContentManager contentManager)
        {
            _postService = postService;
            _departmentService = departmentService;
            _groupService = groupService;
            _contentManager = contentManager;

            Filters.Add(StorageFilter.For(repository));

            OnGetDisplayShape<DepartmentPart>(SetModelProperties);
            OnGetEditorShape<DepartmentPart>(SetModelProperties);
            OnUpdateEditorShape<DepartmentPart>(SetModelProperties);

            OnActivated<DepartmentPart>(PropertyHandlers);
            OnLoading<DepartmentPart>((context, part) => LazyLoadHandlers(part));
            OnCreated<DepartmentPart>((context, part) => UpdateGroupPartCounters(part));
            OnPublished<DepartmentPart>((context, part) => UpdateGroupPartCounters(part));
            OnUnpublished<DepartmentPart>((context, part) => UpdateGroupPartCounters(part));
            OnVersioning<DepartmentPart>((context, part, newVersionPart) => LazyLoadHandlers(newVersionPart));
            OnVersioned<DepartmentPart>((context, part, newVersionPart) => UpdateGroupPartCounters(newVersionPart));
            OnRemoved<DepartmentPart>((context, part) => UpdateGroupPartCounters(part));

            OnRemoved<GroupPart>((context, b) =>
                _departmentService.Delete(context.ContentItem.As<GroupPart>()));
        }

        private void SetModelProperties(BuildShapeContext context, DepartmentPart departmentPart)
        {
            context.Shape.Group = departmentPart.GroupPart;
            context.Shape.StickyClass = departmentPart.IsSticky ? "Sticky" : string.Empty;
        }

        private void UpdateGroupPartCounters(DepartmentPart departmentPart)
        {
            var commonPart = departmentPart.As<CommonPart>();
            if (commonPart != null && commonPart.Record.Container != null)
            {

                var groupPart = departmentPart.GroupPart ?? _groupService.Get(commonPart.Record.Container.Id, VersionOptions.Published);

                // TODO: Refactor this to do the count in the DB and not make 3 DB calls.
                groupPart.DepartmentCount = _departmentService.Count(groupPart, VersionOptions.Published);
                groupPart.PostCount = _departmentService
                    .Get(groupPart, VersionOptions.Published)
                    .Sum(publishedDepartmentPart => _postService
                        .Count(publishedDepartmentPart, VersionOptions.Published));
            }
        }

        protected void LazyLoadHandlers(DepartmentPart part)
        {
            // Add handlers that will load content for id's just-in-time
            part.ClosedByField.Loader(() => _contentManager.Get<IUser>(part.Record.ClosedById));
        }

        protected void PropertyHandlers(ActivatedContentContext context, DepartmentPart part)
        {
            // Add handlers that will update records when part properties are set

            part.ClosedByField.Setter(user =>
            {
                part.Record.ClosedById = user == null
                    ? 0
                    : user.ContentItem.Id;
                return user;
            });

            // Force call to setter if we had already set a value
            if (part.ClosedByField.Value != null)
                part.ClosedByField.Value = part.ClosedByField.Value;

            // Setup FirstPost & LatestPost fields
            part.FirstPostField.Loader(() => _postService.GetPositional(part, DepartmentPostPositional.First));
            part.LatestPostField.Loader(() => _postService.GetPositional(part, DepartmentPostPositional.Latest));
        }

        protected override void GetItemMetadata(GetContentItemMetadataContext context)
        {
            var department = context.ContentItem.As<DepartmentPart>();

            if (department == null)
                return;

            if (department.GroupPart != null)
            {
                context.Metadata.DisplayRouteValues = new RouteValueDictionary
                    {
                        {"Area", Constants.LocalArea},
                        {"Controller", "Department"},
                        {"Action", "Item"},
                        {"groupId", department.GroupPart.ContentItem.Id},
                        {"departmentId", context.ContentItem.Id}
                    };
            }

            context.Metadata.AdminRouteValues = new RouteValueDictionary {
                {"Area", Constants.LocalArea},
                {"Controller", "DepartmentAdmin"},
                {"Action", "Item"},
                {"departmentId", context.ContentItem.Id}
            };
        }
    }
}