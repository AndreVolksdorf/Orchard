using System.ComponentModel;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace SmartPage.Maps.Models
{
    [OrchardFeature("SmartPage.Maps.GroupMapMarkers")]
    public class GroupMapMarkersPart : ContentPart<GroupMapMarkersPartRecord> {
       
        public int Height
        {
            get { return Retrieve(r => r.Height); }
            set { Store(r => r.Height, value); }
        }
    }
}