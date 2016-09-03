using System.Collections.Generic;
using System.Device.Location;

namespace Rs.Exp.Jarvis.Core.Navigation.Models
{
    public class NavigationRoute
    {
        private double _distance;

        public List<GeoCoordinate> Waypoints { get; set; }

        /// <summary>
        /// Tracel distance in meters
        /// </summary>
        public double Distance
        {
            get
            {
                if (_distance == 0.0)
                {
                    _distance = CalculateDistance();
                }

                return _distance;
            }
            set
            {
                _distance = value;
            }
        }

        private double CalculateDistance()
        {
            double calculatedDistance = 0.0;

            for (int i = 0; i < Waypoints.Count - 1; i++)
            {
                GeoCoordinate start = Waypoints[i];
                GeoCoordinate end = Waypoints[i + 1];

                calculatedDistance += start.GetDistanceTo(end);
            }

            return calculatedDistance;
        }

        /// <summary>
        /// Travel time in seconds
        /// </summary>
        public int TravelTime { get; set; }
    }
}