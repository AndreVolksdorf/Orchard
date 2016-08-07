using System;
using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using SmartPage.Maps.Settings;

namespace SmartPage.Maps.Models
{
    public class MapPart : ContentPart<MapRecord>
    {
        public double Latitude
        {
            get
            {
                var lat = Retrieve(r => r.Latitude);
                return Math.Abs(lat) != 0 ? lat : DefaultLatitude;
            }
            set { Store(r => r.Latitude, value); }
        }

        public double Longitude
        {
            get
            {
                var lon = Retrieve(r => r.Longitude);
                return Math.Abs(lon) != 0 ? lon : DefaultLongitude;
            }
            set { Store(r => r.Longitude, value); }
        }

        public string Name
        {
            get { return Retrieve(r => r.Name); }
            set { Store(r => r.Name, value); }
        }

        [Required]
        public string Street
        {
            get { return Retrieve(r => r.Street); }
            set { Store(r => r.Street, value); }
        }
        public string StreetNumber
        {
            get { return Retrieve(r => r.StreetNumber); }
            set { Store(r => r.StreetNumber, value); }
        }

        [Required]
        public string PostalCode
        {
            get { return Retrieve(r => r.PostalCode); }
            set { Store(r => r.PostalCode, value); }
        }


        [Required]
        public string City
        {
            get { return Retrieve(r => r.City); }
            set { Store(r => r.City, value); }
        }

        public string Area
        {
            get { return Retrieve(r => r.Area); }
            set { Store(r => r.Area, value); }
        }

        public string Country
        {
            get { return Retrieve(r => r.Country); }
            set { Store(r => r.Country, value); }
        }

        public virtual int Height { get { return Settings.GetModel<MapPartSettings>().Height; } }
        public virtual double DefaultLatitude { get { return Settings.GetModel<MapPartSettings>().DefaultLatitude; } }
        public virtual double DefaultLongitude { get { return Settings.GetModel<MapPartSettings>().DefaultLongitude; } }
    }
}