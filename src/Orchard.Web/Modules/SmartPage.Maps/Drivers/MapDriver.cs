using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Orchard;
using Orchard.Alias;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using SmartPage.Maps.Models;

namespace SmartPage.Maps.Drivers
{
    public class MapDriver : ContentPartDriver<MapPart>
    {
        private readonly IContentManager _contentManager;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IAliasService _aliasService;

        public MapDriver(IContentManager contentManager,
  IWorkContextAccessor workContextAccessor,
  IAliasService aliasService)
        {
            _contentManager = contentManager;
            _workContextAccessor = workContextAccessor;
            _aliasService = aliasService;
        }

        protected override DriverResult Display(
            MapPart part, string displayType, dynamic shapeHelper)
        {
            var results = new List<DriverResult>();
            
            results.Add(ContentShape("Parts_Map", () =>
            {
                if (CurrentContent != null)
                {
                    var itemTypeName = CurrentContent.ContentItem.TypeDefinition.Name;
                    if (itemTypeName.Equals("Product",
                      StringComparison.InvariantCultureIgnoreCase))
                    {
                        // final product id check will go here
                    }
                }

                return shapeHelper.Parts_Map(
                    Longitude: part.Longitude,
                    Latitude: part.Latitude,
                    Name: part.Name,
                    Street: part.Street,
                    StreetNumber: part.StreetNumber,
                    City: part.City,
                    PostalCode: part.PostalCode,
                    Area: part.Area,
                    Country: part.Country,
                    Id: part.Id.ToString());
            }));
            results.Add(ContentShape("Parts_MapCard", () =>
            {
                if (CurrentContent != null)
                {
                    var itemTypeName = CurrentContent.ContentItem.TypeDefinition.Name;
                    if (itemTypeName.Equals("Product",
                      StringComparison.InvariantCultureIgnoreCase))
                    {
                        // final product id check will go here
                    }
                }

                return shapeHelper.Parts_MapCard(
                    Name: part.Name,
                    Street: part.Street,
                    StreetNumber: part.StreetNumber,
                    City: part.City,
                    PostalCode: part.PostalCode,
                    Area: part.Area,
                    Country: part.Country,
                    Id: part.Id.ToString());
            }));
            return Combined(results.ToArray());
        }

        //GET
        protected override DriverResult Editor(
            MapPart part, dynamic shapeHelper)
        {

            return ContentShape("Parts_Map_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/Map",
                    Model: part,
                    Prefix: Prefix));
        }
        //POST
        protected override DriverResult Editor(
            MapPart part, IUpdateModel updater, dynamic shapeHelper)
        {

            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }

        private IContent _currentContent = null;
        private IContent CurrentContent
        {
            get
            {
                if (_currentContent != null)
                {
                    return _currentContent;
                }
                var itemRoute = _aliasService.Get(_workContextAccessor.GetContext()
                    .HttpContext.Request.AppRelativeCurrentExecutionFilePath
                    .Substring(1).Trim('/'));
                if (itemRoute.ContainsKey("Id"))
                    _currentContent = _contentManager.Get(Convert.ToInt32(itemRoute["Id"]));

                return _currentContent;
            }
        }
    }
}
