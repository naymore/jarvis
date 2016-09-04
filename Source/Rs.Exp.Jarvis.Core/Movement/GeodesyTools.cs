using System.Device.Location;
using Gavaghan.Geodesy;

namespace Rs.Exp.Jarvis.Core.Movement
{
    /// <summary>
    /// This class merely acts as a proxy for the Gavaghan.Geodesy library.
    /// </summary>
    public static class GeoDesyTools
    {
        private static readonly GeodeticCalculator _geodeticCalculator = new GeodeticCalculator();

        public static GeoCoordinate CalculateNextWaypoint(GeoCoordinate startingLocation, double bearing, double distance)
        {
            GlobalCoordinates startingLocation2 = startingLocation.ToGlobalCoordinates();

            GlobalCoordinates targetLocation = _geodeticCalculator.CalculateEndingGlobalCoordinates(Ellipsoid.WGS84, startingLocation2, bearing, distance);

            return targetLocation.ToGeoCoordinate();
        }

        public static double CalculateBearing(GeoCoordinate startingLocation, GeoCoordinate destinationLocation)
        {
            GeodeticCurve result = _geodeticCalculator.CalculateGeodeticCurve(Ellipsoid.WGS84, startingLocation.ToGlobalCoordinates(), destinationLocation.ToGlobalCoordinates());
            double bearing = result.Azimuth.Degrees;

            return bearing;
        }

        private static GlobalCoordinates ToGlobalCoordinates(this GeoCoordinate geoCoordinate)
        {
            return new GlobalCoordinates(new Angle(geoCoordinate.Latitude), new Angle(geoCoordinate.Longitude));
        }

        private static GeoCoordinate ToGeoCoordinate(this GlobalCoordinates globalCoordinates)
        {
            return new GeoCoordinate(globalCoordinates.Latitude.Degrees, globalCoordinates.Longitude.Degrees);
        }
    }
}