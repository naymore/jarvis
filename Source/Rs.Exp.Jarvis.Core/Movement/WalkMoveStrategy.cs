using NLog;

namespace Rs.Exp.Jarvis.Core.Movement
{
    // NOTE: Design... observer pattern? use events on location changed? use consumer/producer for waypoints?
    public class WalkMoveStrategy : MoveStrategy
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public WalkMoveStrategy()
        {
        }
    }
}