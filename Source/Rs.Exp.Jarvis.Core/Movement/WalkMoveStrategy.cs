using System.Device.Location;
using System.Threading;
using NLog;
using Rs.Exp.Jarvis.Core.Navigation.Models;

namespace Rs.Exp.Jarvis.Core.Movement
{
    // NOTE: Design... observer pattern? use events on location changed? use consumer/producer for waypoints?
    public class WalkMoveStrategy : MoveStrategy
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public WalkMoveStrategy()
        {
            _movementSpeed = 4.3;
        }

        public override void Move(NavigationRoute navigationRoute)
        {
            // NOTE: I expect the first waypoint to be the current location - we need to verify if this holds true for google direction api calls and polylines.
            GeoCoordinate origin = navigationRoute.Waypoints[0];
            navigationRoute.Waypoints.RemoveAt(0);

            while (true)
            {
                if (navigationRoute.Waypoints.Count == 0) break;

                GeoCoordinate destination = navigationRoute.Waypoints[0];

                //double distance = 1; // DEBUG
                double distance = origin.GetDistanceTo(destination);
                double duration = GetTimeToTarget(distance);
                _logger.Trace("Walking to {0} ({1:0.0#}m away), arriving in {2:0.0#} seconds", destination, distance, duration);

                Thread.Sleep((int)duration * 1000);

                origin = navigationRoute.Waypoints[0];
                navigationRoute.Waypoints.RemoveAt(0);
            }
        }
    }
}