using Rs.Exp.Jarvis.Core.Navigation.Models;

namespace Rs.Exp.Jarvis.Core.Movement
{
    public abstract class MoveStrategy : IMoveStrategy
    {
        protected double _movementSpeed;

        public abstract void Move(NavigationRoute navigationRoute);

        protected virtual double GetTimeToTarget(double distance)
        {
            double speed = _movementSpeed * 1000 / 60 / 60;

            return distance / speed;
        }
    }
}