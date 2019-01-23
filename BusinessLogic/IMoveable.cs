using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic
{
    interface IMoveable
    {
        /// <summary>
        /// Defines the method to make a moveable thing move.
        /// Takes TimeInSeconds and Direction as integer values
        /// Returns a Tuple, representing the x and y co-ordinates of the resulting position
        /// </summary>
        /// <param name="TimeInSeconds"></param>
        /// <param name="DirectionInDegrees"></param>
        /// <returns></returns>
        void Move(int TimeInSeconds, int DirectionInDegrees);
    }
}
