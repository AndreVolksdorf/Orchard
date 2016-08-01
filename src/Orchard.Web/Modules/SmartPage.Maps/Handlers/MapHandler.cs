using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using SmartPage.Maps.Models;
using SmartPage.Maps.Settings;

namespace SmartPage.Maps.Handlers
{
    public class MapHandler : ContentHandler
    {
        public MapHandler(IRepository<MapRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));

            OnInitializing<MapPart>((context, part) =>
            {
                //part.Height = part.Settings.GetModel<MapPartSettings>().Height;
            });
        }
        
    }
}
