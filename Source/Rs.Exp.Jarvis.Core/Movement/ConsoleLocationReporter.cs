using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rs.Exp.Jarvis.Core.Movement
{
    public class ConsoleLocationReporter : IObserver<GeoCoordinate>
    {
        private IDisposable unsubscriber;
        private string _instName;

        public ConsoleLocationReporter(string name)
        {
            _instName = name;
        }

        public string Name
        {
            get { return _instName; }
        }

        public virtual void Subscribe(IObservable<GeoCoordinate> provider)
        {
            if (provider != null)
                unsubscriber = provider.Subscribe(this);
        }

        public virtual void OnCompleted()
        {
            Console.WriteLine("The Location Tracker has completed transmitting data to {0}.", this.Name);
            this.Unsubscribe();
        }

        public virtual void OnError(Exception e)
        {
            Console.WriteLine("{0}: The location cannot be determined.", this.Name);
        }

        public virtual void OnNext(GeoCoordinate geoCoordinate)
        {
            Console.WriteLine("{2}: The current location is {0}, {1}", geoCoordinate.Latitude, geoCoordinate.Longitude, this.Name);
        }

        public virtual void Unsubscribe()
        {
            unsubscriber.Dispose();
        }
    }
}
