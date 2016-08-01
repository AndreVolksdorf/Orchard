using Orchard.ContentManagement.Records;

namespace SmartPage.Maps.Models {
    public class MapMarkersPartRecord : ContentPartRecord
    {
        public virtual int Height { get; set; }
        public virtual bool ScanContainedItems { get; set; }
    }
}