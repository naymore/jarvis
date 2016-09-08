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
            cancellationToken.ThrowIfCancellationRequested();

            // TODO: Provide initial location 
            GeoCoordinate currentLocation = new GeoCoordinate(48.642050, 9.458333);

            // Provide "fuzzy" movement observer
            //FuzzyLocationFeed fuzzyLocationFeed = new FuzzyLocationFeed(currentLocation, _moveStrategy, 10);
            TestLocationObserver testLocationObserver = new TestLocationObserver("test1");
            //IDisposable testLocationObserver1Subscription = fuzzyLocationFeed.Subscribe(testLocationObserver);
            var testLocationObserver1Subscription = _moveStrategy.Subscribe(testLocationObserver);
            
            Task movement = Task.Run(
                async () =>
                    {
                        while (true)
                        {
                            _logger.Trace("walking...");

                            // simulate POIModule
                            PointOfInterest pointOfInterest = _poiProvider.GetPointOfInterest();
                            _logger.Trace(
                                "(POI) {0} ({1}, {2})", pointOfInterest.Name, pointOfInterest.Location.Latitude, pointOfInterest.Location.Longitude);

                            // get route to POI
                            NavigationRoute navigationRoute = await _navigator.GetDirectionsAsync(currentLocation, pointOfInterest.Location);
                            _logger.Trace(
                                "(NAV) Found a route to {0}. Let's go ({1}m, {2}s)", pointOfInterest.Name, navigationRoute.Distance,
                                navigationRoute.TravelTime);

                            // move
                            await _moveStrategy.Move(navigationRoute);
                        }
                    }, cancellationToken);

            Task action = Task.Run(
                async () =>
                    {
                        while (true)
                        {
                            int x = new Random().Next(0, 1000);
                            if (x % 13 == 0)
                            {
                                _logger.Trace("Set movement speed 0!");
                                _moveStrategy.SetMovementSpeed(0);

                                await Task.Delay(10000, cancellationToken);
                            }
                            else if (x % 17 == 0)
                            {
                                _logger.Trace("Set movement speed 20!");
                                _moveStrategy.SetMovementSpeed(20);
                            }

                            await Task.Delay(1000, cancellationToken);
                        }

                    }, cancellationToken);

            await Task.WhenAll(movement, action);

            testLocationObserver1Subscription.Dispose();
        }
    }
}
