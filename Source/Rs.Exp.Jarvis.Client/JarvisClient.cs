using System.Device.Location;
using System.Threading.Tasks;
using Rs.Exp.Jarvis.Core.Movement;
using Rs.Exp.Jarvis.Core.Navigation;
using Rs.Exp.Jarvis.Core.Navigation.Models;

namespace Rs.Exp.Jarvis.Client
{
    internal class JarvisClient
    {
        private readonly IMoveStrategy _moveStrategy;
        private readonly INavigator _navigator;

        public JarvisClient(INavigator navigator, IMoveStrategy moveStrategy)
        {
            _navigator = navigator;
            _moveStrategy = moveStrategy;
        }

        public async Task Run()
        {
            GeoCoordinate currentLocation = new GeoCoordinate(latitude: 48.642050, longitude: 9.458333);
            GeoCoordinate destination = new GeoCoordinate(latitude: 48.649047, longitude: 9.448158);

            NavigationRoute navigationRoute = await _navigator.GetDirectionsAsync(currentLocation, destination);

            // what if we had another IObservable<GeoCoordinate> which only fires position changed events (= new coordinates) if the offset is more than x meters?
            // -> part of MoveStrategy? Or composition? Or inheritance? Or cometely decoupled, consumer + producer in one?
            ConsoleLocationReporter c1 = new ConsoleLocationReporter("reporter0001"); 
            c1.Subscribe(_moveStrategy);

            await _moveStrategy.Move(navigationRoute);
        }
    }
}
