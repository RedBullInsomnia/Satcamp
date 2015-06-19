using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SatelliteClient
{
    class OrientationFetcher 
    {
        private Thread _thread; /** Thread that fetches the data */
        private double _roll, _pitch, _yaw; /** Current angles */
        private int _goal_pitch, _goal_yaw; /** Objective servo angles */
        private int _servo_pitch, _servo_yaw; /** Actual servo angles */
        private bool _go; /** True for the thread to go on */
        private bool _stabilize_mode; /** True for the stabilize mode 
                                       *  In this mode, the goal pitch and goal yaw are inactive
                                       */
        private SatelliteServer.ISatService _satService; /** Operation contract service */
        private const double EQUAL_THRESHOLD = 0.001;

        public OrientationFetcher(SatelliteServer.ISatService service)
        {
            _thread = new Thread(new ThreadStart(work));
            _satService = service;
            _goal_pitch = _goal_yaw = 4000;
            _servo_pitch = _servo_yaw = 4000;
            _roll = _pitch = _yaw = 0.0;
            _stabilize_mode = false;
            _go = true;
        }

        public void Stop() { _go = false; }
        public void Start() { _thread.Start(); }
        public void Join() { _thread.Join(); }
        public bool IsAlive() { return _thread.IsAlive; }

        // Angle getters and setters
        public double GetPitch() { lock (this) { return _pitch; } }
        public double GetYaw() { lock (this) { return _yaw; } }
        public double GetRoll() { lock (this) { return _roll; } }

        // Servo angles setters and getters
        public int GetServoPitch() { return (_stabilize_mode ? _servo_pitch : _goal_pitch); }
        public int GetServoYaw() { return (_stabilize_mode ? _servo_yaw : _goal_yaw); }

        public void SetServoPitch(int goal_pitch) { _goal_pitch = goal_pitch; }
        public void SetServoYaw(int goal_yaw) { _goal_yaw = goal_yaw; }

        // Stabilize mode
        public void SetStabilize() { _stabilize_mode = true; }
        public void UnsetStabilize() { _stabilize_mode = false; }

        /**
         * Update the pitch, yaw and roll and send angle modification if necessary
         */
        private void work()
        {
            try {
                while (_go && IsAlive())
                {
                    // update euler angles
                    double[] euler = _satService.GetEulerAngles();

                    lock (this)
                    {
                        _roll = euler[0];
                        _pitch = euler[1];
                        _yaw = euler[2];
                    }

                    // update servo angles         
                    _servo_pitch = _satService.GetServoPos(0);
                    _servo_yaw = _satService.GetServoPos(1);

                    // if servo angles invalid : send request for changing them
                    if (!_stabilize_mode && Math.Abs(_servo_pitch - _goal_pitch) > EQUAL_THRESHOLD)
                        _satService.SetServoPos(0, _goal_pitch);

                    if (!_stabilize_mode && Math.Abs(_servo_yaw - _goal_yaw) > EQUAL_THRESHOLD)
                        _satService.SetServoPos(1, _goal_yaw);
                }
            } catch(Exception e) {
                Console.Error.Write("Error occurred in OrientatinFetcher : {0}", e.Message);
            }
        }
    }
}
