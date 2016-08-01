using System.ComponentModel;
using Orchard.ContentManagement;

namespace SmartPage.Maps.Models {
    public class MapMarkersPart : ContentPart<MapMarkersPartRecord> {
        [DisplayName("Scan contained items for map markers")]
        public bool ScanContainedItems
        {
            get { return Retrieve(r => r.ScanContainedItems); }
            set { Store(r => r.ScanContainedItems, value); }
        }

        public int Height
        {
            get { return Retrieve(r => r.Height); }
            set { Store(r => r.Height, value); }
        }
    }
}