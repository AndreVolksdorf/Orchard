using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using Orchard;
using Orchard.Alias;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.Drivers;
using Orchard.Core.Common.Models;
using Orchard.Core.Containers.Models;
using Orchard.Environment.Extensions;
using Orchard.Settings;
using Orchard.UI.Navigation;
using SmartPage.Community.Groups.Models;
using SmartPage.Community.Groups.Services;
using SmartPage.Maps.MapControl.Extensions;
using SmartPage.Maps.Models;
using SmartPage.Maps.ViewModels;

namespace SmartPage.Maps.Drivers
{
    public class GroupMapMarkersDriver : ContentPartDriver<GroupMapMarkersPart>
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IContentManager _contentManager;
        private readonly IDepartmentService _departmentService;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IAliasService _aliasService;
        private readonly ISiteService _siteService;

        private IContent _currentContent = null;
        private IContent CurrentContent
        {
            get
            {
                var test = _aliasService.Get(_workContextAccessor.GetContext()
                    .HttpContext.Request.AppRelativeCurrentExecutionFilePath
                    .Substring(1).TrimStart('/'));
                if (_currentContent == null)
                {
                    var itemRoute = _aliasService.Get(_workContextAccessor.GetContext()
                      .HttpContext.Request.AppRelativeCurrentExecutionFilePath
                      .Substring(1).TrimStart('/'));
                    if (itemRoute != null && itemRoute.ContainsKey("groupId")) { 
                        _currentContent = _contentManager.Get(Convert.ToInt32(itemRoute["groupId"]));
                    }
                }
                return _currentContent;
            }
        }

        public GroupMapMarkersDriver(IOrchardServices orchardServices, IContentManager contentManager,
            IDepartmentService departmentService, IWorkContextAccessor workContextAccessor, IAliasService aliasService, ISiteService siteService)
        {
            _orchardServices = orchardServices;
            _contentManager = contentManager;
            _departmentService = departmentService;
            _workContextAccessor = workContextAccessor;
            _aliasService = aliasService;
            _siteService = siteService;
        }

        protected override DriverResult Display(GroupMapMarkersPart part,
          string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_GroupMapMarkers", () =>
            {
                bool isOnMapMarkersPage = false;

                IList<MarkerInfo> markers = new List<MarkerInfo>();

                if (CurrentContent != null)
                {
                    var map = CurrentContent.As<MapPart>();
                    if (map != null)
                    {
                        markers.Add(new MarkerInfo()
                        {
                            Latitude = map.Latitude,
                            Longitude = map.Longitude,
                            Id = CurrentContent.ContentItem.As<ICommonPart>().Id,
                            Title = CurrentContent.ContentItem.As<GroupPart>().Title
                        });
                    }
                    var groupPart = CurrentContent.ContentItem.As<GroupPart>();
                    if (groupPart != null)
                    {

                        var departments = _departmentService.Get(groupPart, VersionOptions.Published);

                        departments.Each(item =>
                            {
                                map = item.As<MapPart>();
                                if (map != null)
                                {
                                    markers.Add(new MarkerInfo()
                                    {
                                        Latitude = map.Latitude,
                                        Longitude = map.Longitude,
                                        Id = item.Id,
                                        Title = item.Title
                                    });
                                }
                            });
                        //.Select(item => _contentManager.BuildDisplay(item, "Summary"));
                    }
                }

                return shapeHelper.Parts_GroupMapMarkers(Markers: markers, Height: part.Height, Link: _aliasService.Get(_workContextAccessor.GetContext()
                      .HttpContext.Request.AppRelativeCurrentExecutionFilePath
                      .Substring(1).TrimStart('/'))
                      );
            });
        }

        protected override DriverResult Editor(GroupMapMarkersPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_GroupMapMarkers_Edit",
              () => shapeHelper.EditorTemplate(
                TemplateName: "Parts/GroupMapMarkers",
                Model: part,
                Prefix: Prefix));
        }

        protected override DriverResult Editor(GroupMapMarkersPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }
}