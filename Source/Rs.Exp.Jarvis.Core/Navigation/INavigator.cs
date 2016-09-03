using System.Collections.Generic;
using System.Device.Location;
using System.Threading.Tasks;
using Rs.Exp.Jarvis.Core.Navigation.Models;

namespace Rs.Exp.Jarvis.Core.Navigation
{
    public interface INavigator
    {
        Task<NavigationRoute> GetDirectionsAsync(GeoCoordinate currentLocation, GeoCoordinate destination, List<GeoCoordinate> optionalWaypoints = null);
    }
}