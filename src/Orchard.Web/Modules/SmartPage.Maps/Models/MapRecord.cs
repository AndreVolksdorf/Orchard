using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.ContentManagement.Records;

namespace SmartPage.Maps.Models
{
    public class MapRecord : ContentPartRecord
    {
        public virtual double Latitude { get; set; }
        public virtual double Longitude { get; set; }
        public virtual string Name { get; set; }
        public virtual string StreetNumber { get; set; }
        public virtual string Street { get; set; }
        public virtual string City { get; set; }
        public virtual string PostalCode { get; set; }
        public virtual string Country { get; set; }
        public virtual string Area { get; set; }
    }
}
