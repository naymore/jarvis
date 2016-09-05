using System;
using System.Device.Location;
using System.Threading.Tasks;
using Rs.Exp.Jarvis.Core.Navigation.Models;

namespace Rs.Exp.Jarvis.Core.Movement
{
    public interface IMoveStrategy : IObservable<GeoCoordinate>
    {
        Task Move(NavigationRoute navigationRoute);

        void SetMovementSpeed(double newValue);

        void Stop();
    }
}