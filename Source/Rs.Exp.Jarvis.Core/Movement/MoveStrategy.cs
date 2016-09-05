using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Threading.Tasks;
using NLog;
using Rs.Exp.Jarvis.Core.Navigation.Models;

namespace Rs.Exp.Jarvis.Core.Movement
{
    public abstract class MoveStrategy : IMoveStrategy
    {
        private const double GPS_POLLING_FREQUENCY = 1000; // in milliseconds
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly List<IObserver<GeoCoordinate>> _observers;

        private NavigationRoute _navigationRoute;

        protected MoveStrategy()
        {
            _observers = new List<IObserver<GeoCoordinate>>();
        }

        private double StepLength => MovementSpeed * 1000 / 60 / 60 * GPS_POLLING_FREQUENCY/1000;

        protected virtual double MovementSpeed { get; set; } // in km/h

        public async Task Move(NavigationRoute navigationRoute)
        {
            if (navigationRoute == null || navigationRoute.Waypoints.Count == 0)
                throw new ArgumentException("no route or no waypoints!");

            if (Math.Abs(MovementSpeed) < 0)
                throw new Exception("Moving at the speed of lig... ZERO! 0.0!");

            _navigationRoute = navigationRoute;

            while (_navigationRoute.Waypoints.Count >= 2)
            {
                GeoCoordinate startingLocation = _navigationRoute.Waypoints[0];
                GeoCoordinate destinationLocation = _navigationRoute.Waypoints[1];

                double bearing = CalculateBearing(startingLocation, destinationLocation);
                if (double.IsNaN(bearing))
                {
                    _navigationRoute.Waypoints.RemoveAt(0);
                    continue;
                }

                await DoMinorSteps(startingLocation, destinationLocation, bearing);

                _navigationRoute.Waypoints.RemoveAt(0);
                
                // remove last element (you reached your destination)
                if (_navigationRoute.Waypoints.Count == 1)
                    _navigationRoute.Waypoints.Clear();
            }
        }

        public virtual void SetMovementSpeed(double movementSpeed)
        {
            MovementSpeed = movementSpeed;
        }

        protected virtual double CalculateBearing(GeoCoordinate startingLocation, GeoCoordinate destinationLocation)
        {
            return GeoDesyTools.CalculateBearing(startingLocation, destinationLocation);
        }

        protected virtual async Task DoMinorSteps(GeoCoordinate startingLocation, GeoCoordinate destinationLocation, double bearing)
        {
            GeoCoordinate waypoint = GeoDesyTools.CalculateNextWaypoint(startingLocation, bearing, StepLength);
            TrackLocation(waypoint);
            //_logger.Trace("Calculated intermediary waypoint: {0}", waypoint);

            double distanceToDestination = waypoint.GetDistanceTo(destinationLocation);
            //_logger.Trace("{0:0.0#}m to destination {1}.", distanceToDestination, destinationLocation);

            await Task.Delay(500);

            if (distanceToDestination > StepLength)
            {
                await DoMinorSteps(waypoint, destinationLocation, bearing);
            }
        }

        protected void TrackLocation(GeoCoordinate geoCoordinate)
        {
            foreach (IObserver<GeoCoordinate> observer in _observers)
            {
                if (geoCoordinate.IsUnknown)
                    observer.OnError(new Exception("location unknown"));
                else
                    observer.OnNext(geoCoordinate);
            }
        }

        public void Stop()
        {
            // stop navigation
            _navigationRoute.Waypoints.Clear();

            // stop reporting geo-coordinates
            foreach (IObserver<GeoCoordinate> observer in _observers)
                if (_observers.Contains(observer))
                    observer.OnCompleted();

            _observers.Clear();
        }

        public IDisposable Subscribe(IObserver<GeoCoordinate> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);

            return new Unsubscriber(_observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private readonly List<IObserver<GeoCoordinate>> _observers;
            private readonly IObserver<GeoCoordinate> _observer;

            public Unsubscriber(List<IObserver<GeoCoordinate>> observers, IObserver<GeoCoordinate> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }
    }
}