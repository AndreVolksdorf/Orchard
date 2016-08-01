using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPage.Maps.ViewModels
{
    public class MarkerInfo
    {
        public int Id { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public string Title { get; set; }

        public int ZIndex { get; set; }

        public string ImagePath { get; set; }

        public string InfoWindowContent { get; set; }

        public double Weight { get; set; }

        public string StreetNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Area { get; set; }
    }
}
