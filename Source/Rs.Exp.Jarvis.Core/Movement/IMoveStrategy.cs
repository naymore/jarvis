using Rs.Exp.Jarvis.Core.Navigation.Models;

namespace Rs.Exp.Jarvis.Core.Movement
{
    public interface IMoveStrategy
    {
        void Move(NavigationRoute navigationRoute);
    }
}