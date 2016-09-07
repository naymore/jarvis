using System.Collections.Generic;
using System.Device.Location;
using Rs.Exp.Jarvis.Core.Poi.Models;

namespace Rs.Exp.Jarvis.Core.Poi
{
    public class FakePoiProvider : IPoiProvider
    {
        private int _counter;

        public PointOfInterest GetPointOfInterest()
        {
            PointOfInterest pointOfInterest;

            switch (_counter)
            {
                case 0:
                    {
                        pointOfInterest = new PointOfInterest("Kino", new GeoCoordinate(48.649047, 9.448158));
                        break;
                    }
                case 1:
                    {
                        pointOfInterest = new PointOfInterest("Bäcker", new GeoCoordinate(48.648298, 9.453697));
                        break;
                    }
                case 2:
                    {
                        pointOfInterest = new PointOfInterest("Cafe", new GeoCoordinate(48.646354, 9.451610));
                        break;
                    }
                default:
                    {
                        _counter = 0;
                        return null;
                    }
            }

            _counter++;

            return pointOfInterest;
        }

        public List<PointOfInterest> GetPointsOfInterest()
        {
            return new List<PointOfInterest>
                       {
                           new PointOfInterest("poi1", new GeoCoordinate(latitude: 48.649047, longitude: 9.448158)),
                           new PointOfInterest("poi2", new GeoCoordinate(latitude: 48.649047, longitude: 9.448158)),
                           new PointOfInterest("poi2", new GeoCoordinate(latitude: 48.649047, longitude: 9.448158))
                       };
        }
    }
}