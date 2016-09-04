using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rs.Exp.Jarvis.Core.Movement
{
    // see https://msdn.microsoft.com/en-us/library/dd990377(v=vs.110).aspx

    public class LocationProvider : IObservable<GeoCoordinate>
    {
        private readonly List<IObserver<GeoCoordinate>> _observers;

        public LocationProvider()
        {
            _observers = new List<IObserver<GeoCoordinate>>();
        }

        public void TrackLocation(GeoCoordinate geoCoordinate)
        {
            foreach (IObserver<GeoCoordinate> observer in _observers)
            {
                if (geoCoordinate.IsUnknown)
                    observer.OnError(new Exception("location unknown"));
                else
                    observer.OnNext(geoCoordinate);
            }
        }

        public void EndTransmission()
        {
            foreach (IObserver<GeoCoordinate> observer in _observers)
                if (_observers.Contains(observer))
                    observer.OnCompleted();

            _observers.Clear();
        }

        public IDisposable Subscribe(IObserver<GeoCoordinate> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);

            return new Unsubscriber(_observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<GeoCoordinate>> _observers;
            private IObserver<GeoCoordinate> _observer;

            public Unsubscriber(List<IObserver<GeoCoordinate>> observers, IObserver<GeoCoordinate> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }
    }
}
