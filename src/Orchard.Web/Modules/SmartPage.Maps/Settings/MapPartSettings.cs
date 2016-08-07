using System.ComponentModel.DataAnnotations;

namespace SmartPage.Maps.Settings
{
    public class MapPartSettings
    {
        [Required]
        public int Height { get; set; }
        [Required]
        public double DefaultLatitude { get; set; }
        [Required]
        public double DefaultLongitude { get; set; }
        [Required]
        public string ApiKey { get; set; }
    }
}