using System;
using System.Timers;
using System.Collections.Generic;
using static BusinessLogic.enums;

namespace BusinessLogic
{
    public class Drone : IDrone, IMoveable
    {
        /// <summary>
        /// Set default values in constructor by calling Initialise method
        /// </summary>
        public Drone() => InitialiseDrone();

        #region Properties
        /// <summary>
        /// //Ture if started, false if shut down
        /// </summary>
        public bool Active { get; set; }
        /// <summary>
        /// X-axis starting point of drone at the beginning
        /// </summary>
        public int InitialPositionX{ get; set;}
        /// <summary>
        /// Y-axis starting point of drone at the beginning
        /// </summary>
        public int InitialPositionY { get; set; }
        /// <summary>
        /// Current position of drone on x axis
        /// </summary>
        public int CurrentXpos { get; set; }
        /// <summary>
        /// Current position of drone on y axis
        /// </summary>
        public int CurrentYpos { get; set; }
        /// <summary>
        /// Drone's light is on/off
        /// </summary>
        public bool LightOn { get; set; }
        /// <summary>
        /// Horn sounding on/off
        /// </summary>
        public bool HornOn { get; set; }
        /// <summary>
        /// Flag to denote whether or not drone's boundary co-ordinates have been provided
        /// </summary>
        public bool BoundaryIsSet { get; set; }
        /// <summary>
        /// Max value for the x co-ordinate of the boundary.
        /// Drone must not stray past here on this plane.
        /// </summary>
        public int BoundaryMaxX { get; set; }
        /// <summary>
        /// Max value for the y co-ordinate of the boundary.
        /// Drone must not go past here.
        /// </summary>
        public int BoundaryMaxY { get; set; }
        /// <summary>
        /// Minimum value for the x co-ordinate of the boundary
        /// </summary>
        public int BoundaryMinX { get; set; }
        /// <summary>
        /// Minimum value for the y co-ordinate of the boundary
        /// </summary>
        public int BoundaryMinY { get; set; }
        /// <summary>
        /// property determining whether or not the drone is allowed to move
        /// </summary>
        public bool Go { get; set; }
        #endregion Properties

        #region fields
        private static Timer timer;
        private const string BoundarySetReminder = "Please set boundary before attempting any action commands.";
        private const string InvalidCommandMessage = "Invalid command.  Please refer to the instructions for valid commands.";
        private const int BoundaryAlertAlarmNumberOfRepeats = 3;

        #endregion fields

        #region Methods
        private void InitialiseDrone()
        {
            Active = false;
            InitialPositionX = 0;
            InitialPositionY = 0;
            CurrentXpos = 0;
            CurrentYpos = 0;
            BoundaryIsSet = false;
            HornOn = false;
            LightOn = false;
        }

        #region Drone's Public API
        /// <summary>
        /// The DispatchCommand method, with its two overloads, provides the effective interface between the logic and the UI.
        /// This allows the drone to be called only by the signatures outlined in the spec.
        /// </summary>
        /// <param name="Command"></param>
        public string DispatchCommand(string Command)
        {
            switch (Command)
            {
                case "S":
                    Active = true;
                    return "Drone Started.";
                case "R":
                    RestartDrone();
                    return $"Drone restarted. {BoundarySetReminder}" ;
                case "D":
                    ShutdownDrone();
                    return "Drone has been shut down.";
                case "T":
                    if (BoundaryIsSet)
                    {
                        ToggleFeature(DroneFeatures.Lights);
                        return "Lights toggled.";
                    }
                    else
                    {
                        return BoundarySetReminder;
                    }
                 
                case "F":
                    if (BoundaryIsSet)
                    {
                        FlashLights();
                        return "Lights flashed";
                    }
                    else
                    {
                        return BoundarySetReminder;
                    }

                case "H":
                    if (BoundaryIsSet)
                    {
                        NavigateHome();
                        return "Going home.";
                    }
                    else
                    {
                        return BoundarySetReminder;
                    }
                default: return InvalidCommandMessage;
            }
        }

        /// <summary>
        /// First overload, accepting user comands accompanied by one integer argument.
        /// </summary>
        /// <param name="Command"></param>
        /// <param name="Arg1"></param>
        public string DispatchCommand(string Command, int Arg1)
        {
            switch (Command)
            {
                case "A":
                    if (BoundaryIsSet)
                    {
                        AlertHornForTseconds(Arg1);
                        return $"Horn sounded for {Arg1} secconds.";
                    }
                    else
                    {
                        return BoundarySetReminder;
                    }
                default: return InvalidCommandMessage;
            }
        }

        /// <summary>
        /// Second overload, accepting user commands accompanied by two integer arguments.
        /// </summary>
        /// <param name="Command"></param>
        /// <param name="Arg1"></param>
        /// <param name="Arg2"></param>
        public string DispatchCommand(string Command, int Arg1, int Arg2)
        {
            switch (Command)
            {
                case "B":
                    DefineBoundary(Arg1, Arg2);
                    return $"Max Boundary Co-ords set to: {Arg1}, {Arg2}"; 

                case "P":
                    DefineInitialPosition(Arg1, Arg2);
                    return $"Initial Position Set to: {Arg1}, {Arg2}";

                case "M":
                    if (BoundaryIsSet)
                    {
                        Move(Arg1, Arg2);
                        return "Move completed";    //Expand to perhaps report current co-ords or other more useful stuff
                    }
                    else
                    {
                        return BoundarySetReminder;
                    }
                default:
	                {
                        return "Invalid command.";
                    }
                
            }
        }

        #endregion Drone's Public API

        #region Command_Methods
        /// <summary>
        /// For the simplest commands, use a common method to switch features on or off.
        /// Uses polymorphism and an enum as the parameter type for readability
        /// </summary>
        /// <param name="droneFeature"></param>
        protected void ToggleFeature(DroneFeatures droneFeature)
        {
            switch (droneFeature)
            {
                case DroneFeatures.Active:
                    Active = !Active;
                    break;
                case DroneFeatures.Lights:
                    LightOn = !LightOn;
                    break;
                case DroneFeatures.Horn:
                    HornOn = !HornOn;
                    break;
            }
        }

        /// <summary>
        /// Flashes light on and off, once.
        /// Because one flash consists of one 'on' and one 'off', the ToggleFeature method is called twice.
        /// </summary>
        protected void FlashLights()
        {
            for (int i = 0; i < 2; i++)
            {
                ToggleFeature(DroneFeatures.Lights);
            }
        }
        /// <summary>
        /// Overloaded Method that will flash the light on and off, as many times as the number passed in.
        /// Because it calls ToggleFeature, which is completes only half a flash, the loop goes round twice the number that is passed in.
        /// </summary>
        /// <param name="ThisManyTimes"></param>
        protected void FlashLights(int ThisManyTimes)
        {
            for (int i = 0; i < (ThisManyTimes * 2); i++)
            {
                ToggleFeature(DroneFeatures.Lights);
            }
        }

        /// <summary>
        /// Send the drone to the initial starting position on command
        /// </summary>
        private void NavigateHome()
        {
            NavigateToXY(InitialPositionX, InitialPositionY);
        }

        /// <summary>
        /// De-activate the drone on the user's command
        /// </summary>
        private void ShutdownDrone()
        {
            Active = false;
        }

        /// <summary>
        /// Sets drone back to all initial values and un-sets Boundary settings.
        /// </summary>
        private void RestartDrone()
        {
            InitialiseDrone();
            Active = true;
        }


        /// <summary>
        /// sounds horn for t seconds, using a timer to raise an event which then switches it off again.
        /// </summary>
        /// <param name="TimeInSeconds"></param>
        private void AlertHornForTseconds(int TimeInSeconds)
        {
            timer = new Timer(TimeInSeconds * 1000);  
            timer.AutoReset = false;        //only do this once
            timer.Elapsed += OnSecondsUpStopSoundingHorn;
            timer.Enabled = true;
            HornOn = true;
        }
        /// <summary>
        /// Event handler for AlertHornForTseconds
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSecondsUpStopSoundingHorn(object sender, ElapsedEventArgs e)
        {
             HornOn = false;
        }
    
        /// <summary>
        /// Calls Toggle feature for the horn, six times, which will make it play 3 distinct beeps
        /// </summary>
        /// <param name="Ntimes"></param>
        private void SountHornNtimes(int Ntimes)
        {
            for (int i = 0; i < (Ntimes * 2); i++)
            {
                ToggleFeature(DroneFeatures.Horn);
            }
        }

        /// <summary>
        /// Set initial starting position of the drone, according to the co-ordinates entered by the user
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        private void DefineInitialPosition(int arg1, int arg2)
        {
            InitialPositionX = arg1;
            InitialPositionY = arg2;
        }

        /// <summary>
        /// Set the maximum co-ordinates the drone can fly to, based on what the user has input.
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        private void DefineBoundary(int arg1, int arg2)
        {
            BoundaryMaxX = arg1;
            BoundaryMaxY = arg2;
            BoundaryIsSet = true;
        }

        /// <summary>
        /// Propel the drone in the direction given for the time given.
        /// </summary>
        /// <param name="TimeInSeconds"></param>
        /// <param name="DirectionInDegrees"></param>
        /// <returns></returns>
        public void Move(int TimeInSeconds, int DirectionInDegrees)
        {
            DateTime start = DateTime.Now;
            while (DateTime.Now.Subtract(start).Seconds < TimeInSeconds)
            {
                switch (DirectionInDegrees)
                {
                    case 0:
                        // Move North as long as Northerly boundary not reached
                        while (CurrentYpos < BoundaryMaxY)
                        {
                            CurrentYpos++;
                        }
                        if (CurrentYpos == BoundaryMaxY)
                        {
                            Go = false;
                            SountHornNtimes(BoundaryAlertAlarmNumberOfRepeats);
                        }
                        break;
                    case 90:
                        // Move East as long as Eastern boundary not reached
                        while (CurrentXpos < BoundaryMaxX)
                        {
                            CurrentXpos++;
                        }
                        if (CurrentXpos == BoundaryMaxX)
                        {
                            Go = false;
                            SountHornNtimes(BoundaryAlertAlarmNumberOfRepeats);
                        }
                        break;
                    case 180:
                        // Move South as long as Southerly boundary not reached.
                        while (CurrentYpos > BoundaryMinY)
                        {
                            CurrentYpos--;
                        }
                        if (CurrentYpos == BoundaryMinY)
                        {
                            Go = false;
                            SountHornNtimes(BoundaryAlertAlarmNumberOfRepeats);
                        }
                        break;
                    case 270:
                        // Move West as long as the Westerly boundary not reached.
                        while (CurrentXpos > BoundaryMinX)
                        {
                            CurrentXpos--;
                        }
                        if (CurrentXpos == BoundaryMaxX)
                        {
                            Go = false;
                            SountHornNtimes(BoundaryAlertAlarmNumberOfRepeats);
                        }
                        break;
                }
            }

            //return CurrentPosition;
        }
            private void onTimeElapsed(object sender, ElapsedEventArgs e)
            {
                HornOn = false;
            }
        /// <summary>
        /// Navigate to the position provided
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void NavigateToXY(int x, int y)
        {
            //Navigate to position provided
        }

        #endregion Command_Methods

        #endregion Methods
    }
}
