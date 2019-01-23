using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace BusinessLogic
{
    public class UtilityMethods
    {
        public class CreateTimer
        {
            private Timer timer;

            public void StartTimer(int TimeInSeconds, bool AutoResetFlag)
            {
                // Create a timer with an n second interval.
                timer = new Timer(TimeInSeconds * 1000);
                //False if just once, otherwise true
                timer.AutoReset = AutoResetFlag;
                // Hook up the Elapsed event for the timer.
                timer.Elapsed += OnTimedEvent;
            }

            private static void OnTimedEvent(object sender, ElapsedEventArgs e)
            {
                //Drone.
            }
        }

        public class MoveDrone
        {

        }
        
    }


}
