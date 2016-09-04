using System.Device.Location;
using System.Threading.Tasks;
using NLog;
using Rs.Exp.Jarvis.Core.Navigation.Models;

namespace Rs.Exp.Jarvis.Core.Movement
{
    public abstract class MoveStrategy : IMoveStrategy
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private double _movementSpeed = 4.3; // km/h
        private double _gpsPollingFrequency = 1; // seconds

        protected double StepLength => _movementSpeed * 1000 / 60 / 60 * _gpsPollingFrequency;

        public virtual async Task Move(NavigationRoute navigationRoute)
        {
            while (navigationRoute.Waypoints.Count >= 2)
            {
                GeoCoordinate startingLocation = navigationRoute.Waypoints[0];
                GeoCoordinate destinationLocation = navigationRoute.Waypoints[1];

                double bearing = GeoDesyTools.CalculateBearing(startingLocation, destinationLocation);
                if (double.IsNaN(bearing))
                {
                    navigationRoute.Waypoints.RemoveAt(0);
                    continue;
                }

                await DoMinorSteps(startingLocation, destinationLocation, bearing, StepLength);

                navigationRoute.Waypoints.RemoveAt(0);
            }
        }

        protected virtual async Task DoMinorSteps(GeoCoordinate startingLocation, GeoCoordinate destinationLocation, double bearing, double distance)
        {
            GeoCoordinate waypoint = GeoDesyTools.CalculateNextWaypoint(startingLocation, bearing, distance);
            //_logger.Trace("Calculated intermediary waypoint: {0}", waypoint);

            double distanceToDestination = waypoint.GetDistanceTo(destinationLocation);
            _logger.Trace("{0:0.0#}m to destination {1}.", distanceToDestination, destinationLocation);

            await Task.Delay(500);

            if (distanceToDestination > StepLength)
            {
                await DoMinorSteps(waypoint, destinationLocation, bearing, distance);
            }
        }
    }
}