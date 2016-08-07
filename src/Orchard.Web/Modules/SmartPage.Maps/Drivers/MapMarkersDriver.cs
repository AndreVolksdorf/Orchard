using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using System.Web.Routing;
using Orchard;
using Orchard.Alias;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.Drivers;
using Orchard.Core.Common.Models;
using Orchard.Core.Containers.Models;
using Orchard.Settings;
using Orchard.UI.Navigation;
using SmartPage.Maps.Models;
using SmartPage.Maps.ViewModels;

namespace SmartPage.Maps.Drivers
{
    public class MapMarkersDriver : ContentPartDriver<MapMarkersPart>
    {
        private readonly IContentManager _contentManager;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IAliasService _aliasService;
        private readonly ISiteService _siteService;

        private IContent _currentContent = null;
        private IContent CurrentContent
        {
            get
            {
                if (_currentContent == null)
                {
                    var itemRoute = _aliasService.Get(_workContextAccessor.GetContext()
                      .HttpContext.Request.AppRelativeCurrentExecutionFilePath
                      .Substring(1).TrimStart('/'));
                    if (itemRoute != null && itemRoute.ContainsKey("Id"))
                        _currentContent = _contentManager.Get(Convert.ToInt32(itemRoute["Id"]));
                }
                return _currentContent;
            }
        }

        public MapMarkersDriver(IContentManager contentManager, IWorkContextAccessor workContextAccessor, IAliasService aliasService, ISiteService siteService)
        {
            _contentManager = contentManager;
            _workContextAccessor = workContextAccessor;
            _aliasService = aliasService;
            _siteService = siteService;
        }

        protected override DriverResult Display(MapMarkersPart part,
          string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_MapMarkers", () =>
            {
                bool isOnMapMarkersPage = false;

                IList<MarkerInfo> markers = new List<MarkerInfo>();

                if (CurrentContent != null)
                {
                    var map = CurrentContent.ContentItem.As<MapPart>();
                    if (map != null)
                    {
                        markers.Add(new MarkerInfo()
                        {
                            StreetNumber = map.StreetNumber,
                            Area = map.Area,
                            Country = map.Country,
                            Street = map.Street,
                            PostalCode = map.PostalCode,
                            City = map.City,
                            Latitude = map.Latitude,
                            Longitude = map.Longitude,
                            Id = CurrentContent.ContentItem.As<ICommonPart>().Id
                        });
                    }
                    if (part.ScanContainedItems)
                    {
                        var containerPart = CurrentContent.ContentItem.As<ContainerPart>();
                        if (containerPart != null)
                        {
                            var container = CurrentContent.ContentItem;
                            var query = _contentManager
                            .Query(VersionOptions.Published)
                            .Join<CommonPartRecord>().Where(x => x.Container.Id == container.Id)
                            .Join<ContainablePartRecord>().OrderByDescending(x => x.Position).
                            Join<MapRecord>();

                            var pager = new Pager(_siteService.GetSiteSettings(), containerPart.PagerParameters);
                            pager.PageSize = containerPart.PagerParameters.PageSize != null && containerPart.Paginated
                                            ? pager.PageSize
                                            : containerPart.PageSize;

                            var startIndex = containerPart.Paginated ? pager.GetStartIndex() : 0;
                            var pageOfItems = query.Slice(startIndex, pager.PageSize).ToList();

                            pageOfItems.ForEach(item =>
                            {
                                map = item.As<MapPart>();
                                if (map != null)
                                {
                                    markers.Add(new MarkerInfo()
                                    {
                                        StreetNumber = map.StreetNumber,
                                        Area = map.Area,
                                        Country = map.Country,
                                        Street = map.Street,
                                        PostalCode = map.PostalCode,
                                        City = map.City,
                                        Latitude = map.Latitude,
                                        Longitude = map.Longitude,
                                        Id = CurrentContent.ContentItem.As<ICommonPart>().Id
                                    });
                                }
                            });
                            //.Select(item => _contentManager.BuildDisplay(item, "Summary"));
                        }
                    }
                }
                return shapeHelper.Parts_MapMarkers(Markers: markers, Height: part.Height, Link: _aliasService.Get(_workContextAccessor.GetContext()
                      .HttpContext.Request.AppRelativeCurrentExecutionFilePath
                      .Substring(1).TrimStart('/'))
                      );
            });
        }

        protected override DriverResult Editor(MapMarkersPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_MapMarkers_Edit",
              () => shapeHelper.EditorTemplate(
                TemplateName: "Parts/MapMarkers",
                Model: part,
                Prefix: Prefix));
        }

        protected override DriverResult Editor(MapMarkersPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }
}