using System;
using System.Device.Location;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Rs.Exp.Jarvis.Core.Movement;
using Rs.Exp.Jarvis.Core.Navigation;
using Rs.Exp.Jarvis.Core.Navigation.Models;
using Rs.Exp.Jarvis.Navigation.Google;

namespace Rs.Exp.Jarvis.Client
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
            // problem 0: Get POI coordinates
            // TODO.

            // problem 1: Get a route to a destination
            INavigator navigator = new GoogleWalkingNavigator();
            GeoCoordinate currentLocation = new GeoCoordinate(latitude: 48.642050, longitude: 9.458333);
            GeoCoordinate destination = new GeoCoordinate (latitude: 48.649047, longitude: 9.448158);

            Task<NavigationRoute> t1 = navigator.GetDirectionsAsync(currentLocation, destination);
            NavigationRoute navigationRoute = t1.Result;

            // problem 2: walk along that route to destination
            IMoveStrategy moveStrategy = new WalkMoveStrategy();
            Task walker = Task.Run(() => moveStrategy.Move(navigationRoute));
            walker.Wait();

            ConsoleLocationReporter c1 = new ConsoleLocationReporter("reporter0001");
            ConsoleLocationReporter c2 = new ConsoleLocationReporter("reporter0002");
            LocationProvider locationProvider = new LocationProvider();

            c1.Subscribe(locationProvider);
            c2.Subscribe(locationProvider);

            locationProvider.TrackLocation(destination);
            locationProvider.EndTransmission();

            Console.ReadKey();
            return 0;
        }
    }
}
