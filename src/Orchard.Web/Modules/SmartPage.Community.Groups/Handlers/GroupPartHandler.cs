using System.Linq;
using Orchard.Data;
using Orchard.ContentManagement.Handlers;
using System.Web.Routing;
using Orchard.ContentManagement;
using SmartPage.Community.Groups.Extensions;
using SmartPage.Community.Groups.Models;
using SmartPage.Community.Roles.Models;

namespace SmartPage.Community.Groups.Handlers
{
    public class GroupPartHandler : ContentHandler
    {
        public GroupPartHandler(IRepository<GroupPartRecord> repository)
        {

            Filters.Add(new ActivatingFilter<GroupPart>("Group"));

            Filters.Add(StorageFilter.For(repository));

            OnGetDisplayShape<GroupPart>((context, group) =>
            {
                context.Shape.PostCount = group.PostCount;
            });
        }


        protected override void GetItemMetadata(GetContentItemMetadataContext context)
        {
            var group = context.ContentItem.As<GroupPart>();

            if (group == null)
                return;

            context.Metadata.DisplayRouteValues = new RouteValueDictionary {
                {"Area", Constants.LocalArea},
                {"Controller", "Group"},
                {"Action", "Item"},
                {"groupId", context.ContentItem.Id}
            };
            context.Metadata.CreateRouteValues = new RouteValueDictionary {
                {"Area", Constants.LocalArea},
                {"Controller", "GroupAdmin"},
                {"Action", "Create"}
            };
            context.Metadata.EditorRouteValues = new RouteValueDictionary {
                {"Area", Constants.LocalArea},
                {"Controller", "GroupAdmin"},
                {"Action", "Edit"},
                {"groupId", context.ContentItem.Id}
            };
            context.Metadata.RemoveRouteValues = new RouteValueDictionary {
                {"Area", Constants.LocalArea},
                {"Controller", "GroupAdmin"},
                {"Action", "Remove"},
                {"groupId", context.ContentItem.Id}
            };
            context.Metadata.AdminRouteValues = new RouteValueDictionary {
                {"Area", Constants.LocalArea},
                {"Controller", "GroupAdmin"},
                {"Action", "Item"},
                {"groupId", context.ContentItem.Id}
            };
        }
    }
}