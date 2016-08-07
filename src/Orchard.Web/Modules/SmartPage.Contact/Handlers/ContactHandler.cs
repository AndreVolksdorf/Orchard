using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using SmartPage.Contact.Models;

namespace SmartPage.Contact.Handlers
{
    public class ContactHandler : ContentHandler
    {
        public ContactHandler(IRepository<ContactRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));

            OnInitializing<ContactPart>((context, part) =>
            {
                //part.Height = part.Settings.GetModel<MapPartSettings>().Height;
            });
        }
        
    }
}
