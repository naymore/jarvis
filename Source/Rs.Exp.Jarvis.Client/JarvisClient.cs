using System;
using System.Device.Location;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Rs.Exp.Jarvis.Core.Movement;
using Rs.Exp.Jarvis.Core.Navigation;
using Rs.Exp.Jarvis.Core.Navigation.Models;
using Rs.Exp.Jarvis.Core.Poi;
using Rs.Exp.Jarvis.Core.Poi.Models;

namespace Rs.Exp.Jarvis.Client
{
    internal class JarvisClient
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IMoveStrategy _moveStrategy;
        private readonly INavigator _navigator;
        private readonly IPoiProvider _poiProvider;

        public JarvisClient(INavigator navigator, IMoveStrategy moveStrategy, IPoiProvider poiProvider)
        {
            _navigator = navigator;
            _moveStrategy = moveStrategy;
            _poiProvider = poiProvider;
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            // TODO: Provide initial location 
            GeoCoordinate currentLocation = new GeoCoordinate(48.642050, 9.458333);

            // Provide "fuzzy" movement observer
            FuzzyLocationFeed fuzzyLocationFeed = new FuzzyLocationFeed(currentLocation, _moveStrategy, 10);
            TestLocationObserver testLocationObserver = new TestLocationObserver("test1");
            IDisposable testLocationObserver1Subscription = fuzzyLocationFeed.Subscribe(testLocationObserver);

            bool running = true;
            while (running)
            {
                // simulate POIModule
                PointOfInterest pointOfInterest = _poiProvider.GetPointOfInterest();
                _logger.Trace("(POI) {0} ({1}, {2})", pointOfInterest.Name, pointOfInterest.Location.Latitude, pointOfInterest.Location.Longitude);

                // get route to POI
                NavigationRoute navigationRoute = await _navigator.GetDirectionsAsync(currentLocation, pointOfInterest.Location);
                _logger.Trace("(NAV) Found a route to {0}. Let's go ({1}m, {2}s)", pointOfInterest.Name, navigationRoute.Distance, navigationRoute.TravelTime);

                // move
                await _moveStrategy.Move(navigationRoute);

                if (cancellationToken.IsCancellationRequested)
                    running = false;
            }

            testLocationObserver1Subscription.Dispose();
        }
    }
}
