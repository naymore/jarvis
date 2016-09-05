using System;
using System.Device.Location;
using NLog;

namespace Rs.Exp.Jarvis.Core.Movement
{
    public class ConsoleLocationReporter : IObserver<GeoCoordinate>
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private IDisposable _unsubscriber;

        public ConsoleLocationReporter(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public virtual void Subscribe(IObservable<GeoCoordinate> provider)
        {
            if (provider != null)
                _unsubscriber = provider.Subscribe(this);
        }

        public virtual void OnCompleted()
        {
            _logger.Trace("The Location Tracker has completed transmitting data to {0}.", Name);
            Unsubscribe();
        }

        public virtual void OnError(Exception e)
        {
            _logger.Error("{0}: The location cannot be determined.", Name);
        }

        public virtual void OnNext(GeoCoordinate geoCoordinate)
        {
            _logger.Trace("{2}: The current location is {0}, {1}", geoCoordinate.Latitude, geoCoordinate.Longitude, Name);
        }

        public virtual void Unsubscribe()
        {
            _unsubscriber.Dispose();
        }
    }
}