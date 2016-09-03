using System.Collections.Generic;
using Newtonsoft.Json;

namespace Rs.Exp.Jarvis.Navigation.Google.Models
{
    //internal class GeocodedWaypoint
    //{
    //    public string geocoder_status { get; set; }
    //    public string place_id { get; set; }
    //    public List<string> types { get; set; }
    //}

    //internal class Bounds
    //{
    //    public GeoLoc northeast { get; set; }
    //    public GeoLoc southwest { get; set; }
    //}

    internal struct ValueObject
    {
        public string Text { get; set; }
        public int Value { get; set; }
    }

    internal struct Polyline
    {
        [JsonProperty("points")]
        public string Points { get; set; }
    }

    internal struct Step
    {
        public ValueObject distance { get; set; }
        public ValueObject duration { get; set; }
        public GeoLoc end_location { get; set; }
        public string html_instructions { get; set; }

        [JsonProperty("polyline")]
        public Polyline Polyline { get; set; }
        public GeoLoc start_location { get; set; }
        public string travel_mode { get; set; }
        public string maneuver { get; set; }
    }

    internal struct Leg
    {
        public ValueObject distance { get; set; }
        public ValueObject duration { get; set; }
        public string end_address { get; set; }
        public GeoLoc end_location { get; set; }
        public string start_address { get; set; }
        public GeoLoc start_location { get; set; }

        [JsonProperty("steps")]
        public List<Step> Steps { get; set; }

        //public List<object> traffic_speed_entry { get; set; }
        //public List<object> via_waypoint { get; set; }
    }

    internal struct OverviewPolyline
    {
        [JsonProperty("points")]
        public string Points { get; set; }
    }

    internal struct GeoLoc
    {
        [JsonProperty("lat")]
        public double Latitude { get; set; }

        [JsonProperty("lng")]
        public double Longitude { get; set; }
    }

    internal struct Route
    {
        //public Bounds bounds { get; set; }
        //public string copyrights { get; set; }

        [JsonProperty("legs")]
        public List<Leg> Legs { get; set; }

        public OverviewPolyline overview_polyline { get; set; }

        //public string summary { get; set; }
        //public List<object> warnings { get; set; }

        public List<int> waypoint_order { get; set; }
    }

    internal class GoogleDirectionsResponse
    {
        //public List<GeocodedWaypoint> geocoded_waypoints { get; set; }

        [JsonProperty("routes")]
        public List<Route> Routes { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}