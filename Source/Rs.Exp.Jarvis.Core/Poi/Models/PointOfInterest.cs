using System.Device.Location;

namespace Rs.Exp.Jarvis.Core.Poi.Models
{
    public class PointOfInterest
    {
        public PointOfInterest(string name, GeoCoordinate geoCoordinate)
        {
            Location = geoCoordinate;
            Name = name;
        }

        public GeoCoordinate Location { get; set; }

        public string Name { get; set; }
    }
}