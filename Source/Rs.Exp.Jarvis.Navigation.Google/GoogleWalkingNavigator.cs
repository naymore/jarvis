using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using Rs.Exp.Jarvis.Core.Navigation.Models;
using Rs.Exp.Jarvis.Navigation.Google.Models;

namespace Rs.Exp.Jarvis.Navigation.Google
{
    public class GoogleWalkingNavigator : GoogleNavigator
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public override async Task<NavigationRoute> GetDirectionsAsync(GeoCoordinate currentLocation, GeoCoordinate destination, List<GeoCoordinate> optionalWaypoints = null)
        {
            string url = ConstructUrl(currentLocation, destination, GoogleDirectionsMode.Walking);

            if (_cache.ContainsKey(url))
            {
                _logger.Trace("Retrieving requested route from cache.");
                return _cache[url];
            }

            _logger.Trace("No cached route. Asking Google...");
            using (HttpResponseMessage responseMessage = await _httpClient.GetAsync(url))
            {
                if (!responseMessage.IsSuccessStatusCode)
                {
                    throw new Exception("Error on directions API call");
                }

                string payload = await responseMessage.Content.ReadAsStringAsync();
                GoogleDirectionsResponse directionsResponse = JsonConvert.DeserializeObject<GoogleDirectionsResponse>(payload);

                if (directionsResponse.Status != "OK")
                {
                    throw new Exception("Unexpected response from Google# 4193465");
                }

                if (directionsResponse.Routes.Count != 1 || directionsResponse.Routes[0].Legs.Count != 1)
                {
                    throw new Exception("unexpected response from Google #33593");
                }

                Leg leg = directionsResponse.Routes[0].Legs[0];

                NavigationRoute navigationRoute = new NavigationRoute();
                navigationRoute.Distance = leg.distance.Value;
                navigationRoute.TravelTime = leg.duration.Value;

                List<GeoCoordinate> waypointsList = new List<GeoCoordinate>();
                foreach (Step step in leg.Steps)
                {
                    // get waypoints using Polylines within each step
                    IEnumerable<GeoCoordinate> waypoints = DecodePolyline(step.Polyline.Points);
                    waypointsList.AddRange(waypoints);
                }

                // clean up duplicates
                navigationRoute.Waypoints = waypointsList.Distinct().ToList();

                _cache.Add(url, navigationRoute);

                return navigationRoute;
            }
        }
    }
}