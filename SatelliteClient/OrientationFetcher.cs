using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SatelliteClient
{
    class OrientationFetcher : SatelliteServer.BaseThread
    {
        private int DEFAULT_SERVO_POS = 4000;
        private int _goal_pitch, _goal_yaw; /** Objective servo angles */
        private bool _stabilize_mode; /** True for the stabilize mode 
                                       *  In this mode, the goal pitch and goal yaw are inactive */
        private MovingAverageDouble _roll, _pitch, _yaw; /** Euler angles as moving averages */
        private MovingAverageInt _servoPitch, _servoYaw; /** Servo angles as moving averages */
        private const int MOVING_AVERAGE_WINDOW = 10; 

        private SatelliteServer.ISatService _satService; /** Operation contract service */
        private const double EQUAL_THRESHOLD = 0.001;

        public OrientationFetcher(SatelliteServer.ISatService service)
        {
            _satService = service;
            _goal_pitch = _goal_yaw = DEFAULT_SERVO_POS;
            _servoPitch = new MovingAverageInt(MOVING_AVERAGE_WINDOW, DEFAULT_SERVO_POS);
            _servoYaw = new MovingAverageInt(MOVING_AVERAGE_WINDOW, DEFAULT_SERVO_POS);
            _roll = new MovingAverageDouble(MOVING_AVERAGE_WINDOW);
            _pitch = new MovingAverageDouble(MOVING_AVERAGE_WINDOW);
            _yaw = new MovingAverageDouble(MOVING_AVERAGE_WINDOW);
            _stabilize_mode = false;
        }

        // Angle getters and setters
        public double GetPitch() { lock (this) { return _pitch.get(); } }
        public double GetYaw() { lock (this) { return _yaw.get(); } }
        public double GetRoll() { lock (this) { return _roll.get(); } }

        // Servo angles setters and getters
        public int GetServoPitch() { return _servoPitch.get(); }
        public int GetServoYaw() { return _servoYaw.get(); }

        public void SetServoPitch(int goal_pitch) { _goal_pitch = goal_pitch; }
        public void SetServoYaw(int goal_yaw) { _goal_yaw = goal_yaw; }

        // Stabilize mode
        public void SetStabilize() { _stabilize_mode = true; }
        public void UnsetStabilize() { _stabilize_mode = false; }

        /**
         * Update the pitch, yaw and roll and send angle modification if necessary
         */
        protected override void work()
        {
            try {
                while (_go && IsAlive())
                {
                    // update euler angles
                    double[] euler = _satService.GetEulerAngles();

                    lock (this)
                    {
                        _roll.push(euler[0]);
                        _pitch.push(euler[1]);
                        _yaw.push(euler[2]);
                    }

                    // update servo angles         
                    _servoPitch.push(_satService.GetServoPos(0));
                    _servoYaw.push(_satService.GetServoPos(1));

                    // if servo angles invalid : send request for changing them
                    if (Math.Abs(_servoPitch.getLast() - _goal_pitch) > EQUAL_THRESHOLD)
                        _satService.SetServoPos(0, _goal_pitch);

                    if (Math.Abs(_servoYaw.getLast() - _goal_yaw) > EQUAL_THRESHOLD)
                        _satService.SetServoPos(1, _goal_yaw);
                }
            } catch(Exception e) {
                Console.Error.Write("Error occurred in OrientatinFetcher : {0}", e.Message);
            }
        }
    }
}
