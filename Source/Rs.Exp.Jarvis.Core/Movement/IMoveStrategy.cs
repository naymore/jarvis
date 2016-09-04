using System.Threading.Tasks;
using Rs.Exp.Jarvis.Core.Navigation.Models;

namespace Rs.Exp.Jarvis.Core.Movement
{
    public interface IMoveStrategy
    {
        Task Move(NavigationRoute navigationRoute);
    }
}