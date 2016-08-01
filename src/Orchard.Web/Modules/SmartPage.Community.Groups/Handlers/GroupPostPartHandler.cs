using System.Linq;
using System.Web.Routing;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.Handlers;
using Orchard.Core.Common.Models;
using Orchard.Data;
using Orchard.Services;
using SmartPage.Community.Groups.Extensions;
using SmartPage.Community.Groups.Models;
using SmartPage.Community.Groups.Services;

namespace SmartPage.Community.Groups.Handlers {
    public class GroupPostPartHandler : ContentHandler {
        private readonly IPostService _postService;
        private readonly IDepartmentService _departmentService;
        private readonly IGroupService _groupService;
        private readonly IClock _clock;

        public GroupPostPartHandler(IRepository<GroupPostPartRecord> repository, 
            IPostService postService, 
            IDepartmentService departmentService, 
            IGroupService groupService,
            IClock clock) {
            _postService = postService;
            _departmentService = departmentService;
            _groupService = groupService;
            _clock = clock;

            Filters.Add(StorageFilter.For(repository));

            OnGetDisplayShape<GroupPostPart>(SetModelProperties);
            OnGetEditorShape<GroupPostPart>(SetModelProperties);
            OnUpdateEditorShape<GroupPostPart>(SetModelProperties);

            OnCreated<GroupPostPart>((context, part) => UpdateCounters(part));
            OnPublished<GroupPostPart>((context, part) => { 
                UpdateCounters(part);
                UpdateDepartmentVersioningDates(part);
            });
            OnUnpublished<GroupPostPart>((context, part) => UpdateCounters(part));
            OnVersioned<GroupPostPart>((context, part, newVersionPart) => UpdateCounters(newVersionPart));
            OnRemoved<GroupPostPart>((context, part) => UpdateCounters(part));

            OnRemoved<DepartmentPart>((context, b) =>
                _postService.Delete(context.ContentItem.As<DepartmentPart>()));

            OnIndexing<GroupPostPart>((context, postPart) => context.DocumentIndex
                                                    .Add("body", postPart.Record.Text).RemoveTags().Analyze()
                                                    .Add("format", postPart.Record.Format).Store());
        }

        private void UpdateDepartmentVersioningDates(GroupPostPart postPart) {
            var utcNow = _clock.UtcNow;
            postPart.DepartmentPart.As<ICommonPart>().ModifiedUtc = utcNow;
            postPart.DepartmentPart.As<ICommonPart>().VersionModifiedUtc = utcNow;
        }

        private void SetModelProperties(BuildShapeContext context, GroupPostPart postPart) {
            context.Shape.Department = postPart.DepartmentPart;
        }

        protected override void GetItemMetadata(GetContentItemMetadataContext context)
        {
            var groupPostPart = context.ContentItem.As<GroupPostPart>();

            if (groupPostPart == null)
                return;
            
            context.Metadata.AdminRouteValues = new RouteValueDictionary {
                {"Area", Constants.LocalArea},
                {"Controller", "GroupAdmin"},
                {"Action", "Item"},
                {"groupId", context.ContentItem.Id}
            };
        }

        private void UpdateCounters(GroupPostPart postPart) {
            if (postPart.IsParentDepartment())
                return;

            UpdateDepartmentPartCounters(postPart);
        }

        private void UpdateDepartmentPartCounters(GroupPostPart postPart) {
            var commonPart = postPart.As<CommonPart>();
            if (commonPart != null &&
                commonPart.Record.Container != null) {

                DepartmentPart departmentPart = postPart.DepartmentPart ??
                                        _departmentService.Get(commonPart.Record.Container.Id, VersionOptions.Published);

                departmentPart.PostCount = _postService.Count(departmentPart, VersionOptions.Published);

                UpdateGroupPartCounters(departmentPart);
            }
        }

        private void UpdateGroupPartCounters(DepartmentPart departmentPart) {
            var commonPart = departmentPart.As<CommonPart>();
            if (commonPart != null &&
                commonPart.Record.Container != null) {

                GroupPart groupPart = departmentPart.GroupPart ??
                                      _groupService.Get(commonPart.Record.Container.Id, VersionOptions.Published);

                groupPart.DepartmentCount = _departmentService.Count(groupPart, VersionOptions.Published);
                groupPart.PostCount = _departmentService
                    .Get(groupPart, VersionOptions.Published)
                    .Sum(publishedDepartmentPart => 
                        publishedDepartmentPart.PostCount);
            }
        }
    }
}