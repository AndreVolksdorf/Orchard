using System;
using System.Collections.Generic;
using Orchard;
using Orchard.Alias;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using SmartPage.Contact.Models;

namespace SmartPage.Contact.Drivers
{
    public class ContactDriver : ContentPartDriver<ContactPart>
    {
        private readonly IContentManager _contentManager;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IAliasService _aliasService;

        public ContactDriver(IContentManager contentManager,
  IWorkContextAccessor workContextAccessor,
  IAliasService aliasService)
        {
            _contentManager = contentManager;
            _workContextAccessor = workContextAccessor;
            _aliasService = aliasService;
        }

        protected override DriverResult Display(
            ContactPart part, string displayType, dynamic shapeHelper)
        {
            var results = new List<DriverResult>();

            results.Add(ContentShape("Parts_Contact", () =>
            {
                //if (CurrentContent != null)
                //{
                //    var itemTypeName = CurrentContent.ContentItem.TypeDefinition.Name;
                //    if (itemTypeName.Equals("Product",
                //      StringComparison.InvariantCultureIgnoreCase))
                //    {
                //        // final product id check will go here
                //    }
                //}

                return shapeHelper.Parts_Contact(
                    Phone: part.Phone,
                    Website: part.Website,
                    Email: part.Email,
                    Id: part.Id.ToString());
            }));
            return Combined(results.ToArray());
        }

        //GET
        protected override DriverResult Editor(
            ContactPart part, dynamic shapeHelper)
        {

            return ContentShape("Parts_Contact_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/Contact",
                    Model: part,
                    Prefix: Prefix));
        }
        //POST
        protected override DriverResult Editor(
            ContactPart part, IUpdateModel updater, dynamic shapeHelper)
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
