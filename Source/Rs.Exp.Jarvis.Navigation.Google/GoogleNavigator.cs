using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Rs.Exp.Jarvis.Core.Navigation;
using Rs.Exp.Jarvis.Core.Navigation.Models;
using Rs.Exp.Jarvis.Navigation.Google.Models;

namespace Rs.Exp.Jarvis.Navigation.Google
{
    public abstract class GoogleNavigator : INavigator //, IDisposable // TODO
    {
        private const string GOOGLE_API_KEY = "AIzaSyCcK2ShPN5vR0RkZAmsKNyqVlWLfaXXqb0";
        protected readonly HttpClient _httpClient;
        protected readonly Dictionary<string, NavigationRoute> _cache = new Dictionary<string, NavigationRoute>();

        protected GoogleNavigator()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public abstract Task<NavigationRoute> GetDirectionsAsync(GeoCoordinate currentLocation, GeoCoordinate destination, List<GeoCoordinate> optionalWaypoints = null);

        protected virtual string ConstructUrl(GeoCoordinate currentLocation, GeoCoordinate destination, GoogleDirectionsMode directionsMode, List<GeoCoordinate> optionalWaypoints = null)
        {
            // see https://developers.google.com/maps/documentation/directions/

            if (currentLocation.IsUnknown)
                throw new ArgumentNullException(nameof(currentLocation));

            if (destination.IsUnknown)
                throw new ArgumentNullException(nameof(destination));

            StringBuilder sb = new StringBuilder("https://maps.googleapis.com/maps/api/directions/json");
            sb.Append("?key=").Append(GOOGLE_API_KEY);
            sb.Append("&origin=").Append(currentLocation);
            sb.Append("&destination=").Append(destination);
            sb.Append("&units=").Append("metric");

            if (optionalWaypoints != null && optionalWaypoints.Count != 0)
            {
                sb.Append("&waypoints=");
                for (int i = 0; i < optionalWaypoints.Count; i++)
                {
                    sb.Append(optionalWaypoints[i]);

                    if (i == optionalWaypoints.Count - 1)
                    {
                        sb.Append("|");
                    }
                }
            }

            sb.Append("&mode=").Append(directionsMode.ToString().ToLowerInvariant());
            sb = sb.Replace(' ', '+');

            return sb.ToString();
        }

        protected virtual IEnumerable<GeoCoordinate> DecodePolyline(string polyline)
        {
            // see https://developers.google.com/maps/documentation/utilities/polylinealgorithm?hl=de
            // copy from https://gist.github.com/shinyzhu/4617989

            if (string.IsNullOrEmpty(polyline))
                throw new ArgumentNullException(nameof(polyline));

            char[] polylineChars = polyline.ToCharArray();
            int index = 0;
            int currentLat = 0;
            int currentLng = 0;

            while (index < polylineChars.Length)
            {
                // calculate next latitude
                int sum = 0;
                int shifter = 0;
                int next5Bits;

                do
                {
                    next5Bits = (int)polylineChars[index++] - 63;
                    sum |= (next5Bits & 31) << shifter;
                    shifter += 5;
                }
                while (next5Bits >= 32 && index < polylineChars.Length);

                if (index >= polylineChars.Length)
                    break;

                currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                //calculate next longitude
                sum = 0;
                shifter = 0;
                do
                {
                    next5Bits = (int)polylineChars[index++] - 63;
                    sum |= (next5Bits & 31) << shifter;
                    shifter += 5;
                }
                while (next5Bits >= 32 && index < polylineChars.Length);

                if (index >= polylineChars.Length && next5Bits >= 32)
                    break;

                currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                yield return new GeoCoordinate(Convert.ToDouble(currentLat) / 1E5, Convert.ToDouble(currentLng) / 1E5);
            }
        }
    }
}