using System;
using System.Device.Location;
using System.Reactive.Linq;

namespace Rs.Exp.Jarvis.Core.Movement
{
    // see http://stackoverflow.com/questions/39356211/use-reactive-extensions-to-filter-on-significant-changes-in-observable-stream
    public class FuzzyLocationFeed : IObservable<GeoCoordinate>
    {
        private readonly IObservable<GeoCoordinate> _observable;

        public FuzzyLocationFeed(GeoCoordinate currentLocation, IMoveStrategy moveStrategy, int movementThreshold)
        {
            _observable = moveStrategy.Scan(
                Tuple.Create(currentLocation, currentLocation), (acc, cur) =>
                    {
                        double distance = acc.Item1.GetDistanceTo(cur);
                        if (distance >= movementThreshold)
                        {
                            return Tuple.Create(cur, cur);
                        }

                        return Tuple.Create(acc.Item1, cur);
                    }).Where(pair => pair.Item1 == pair.Item2).Select(pair => pair.Item1);
        }

        public IDisposable Subscribe(IObserver<GeoCoordinate> observer)
        {
            return _observable.Subscribe(observer);
        }
    }
}