using System;
using System.Device.Location;

namespace Rs.Exp.Jarvis.Core.Movement
{
    public class WalkMoveStrategy : MoveStrategy
    {
        protected override double MovementSpeed { get; set; } = 4.87;

        protected override double CalculateBearing(GeoCoordinate startingLocation, GeoCoordinate destinationLocation)
        {
            // for example we could add a little zick-zack from time to time...
            return base.CalculateBearing(startingLocation, destinationLocation);
        }

        public override void SetMovementSpeed(double movementSpeed)
        {
            if (movementSpeed <= 0)
                throw new ArgumentException("Movement speed cannot be 0 or negative.", nameof(movementSpeed));

            if (movementSpeed > 50)
                throw new ArgumentException("Movement speed cannot be higher than 9.", nameof(movementSpeed));
            
            base.SetMovementSpeed(movementSpeed);
        }
    }
}