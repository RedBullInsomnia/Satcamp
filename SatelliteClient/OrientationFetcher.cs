using System;
using System.Threading;
using SatelliteServer;

namespace SatelliteClient
{
    class OrientationFetcher : BaseThread
    {
        private ushort _goal_pitch, _goal_yaw; /** Objective servo angles */
        private bool _stabilizeMode, _stabilizeModeGoal; /** True for the stabilize mode */
        private ExponentialAverage _roll, _pitch, _yaw; /** Euler angles as moving averages */
        private int _servoPitch, _servoYaw;
        private const int MOVING_AVERAGE_WINDOW = 10;
        private const double ALPHA = 0.75;

        /** Ki and Kp parameters */
        private double _Ki, _Kp, _KiGoal, _KpGoal;
        private double EQUAL_THRESHOLD = 0.0000001;

        /** Camera parameters */
        private double _fps, _expTime;
        private double _goalFps, _goalExpTime;

        private ISatService _satService; /** Operation contract service */
        private const double SERVO_EQUAL_THRESHOLD = 0.001;

        public OrientationFetcher(ISatService service)
        {
            _satService = service;
            _goal_pitch = _goal_yaw = Constants.DEFAULT_SERVO_POS;
            _fps = _goalFps = Constants.DEF_FPS;
            _Ki = _KiGoal = 0;
            _Kp = _KpGoal = 0.2;
            _expTime = _goalExpTime = Constants.DEF_EXP_TIME;
            _servoPitch = _servoYaw = Constants.DEFAULT_SERVO_POS;
            _roll = new ExponentialAverage(ALPHA);
            _pitch = new ExponentialAverage(ALPHA);
            _yaw = new ExponentialAverage(ALPHA);
            _stabilizeMode = false;
        }

        // Angle getters and setters
        public double GetPitch() { lock (this) { return _pitch.get(); } }
        public double GetYaw() { lock (this) { return _yaw.get(); } }
        public double GetRoll() { lock (this) { return _roll.get(); } }

        // Servo angles setters and getters
        public int GetServoPitch() { return _servoPitch; }
        public int GetServoYaw() { return _servoYaw; }

        public double GetKi() { return _Ki; }
        public double GetKp() { return _Kp; }

        public void SetServoPitch(ushort goal_pitch) { _goal_pitch = goal_pitch; }
        public void SetServoYaw(ushort goal_yaw) { _goal_yaw = goal_yaw; }

        public void SetKi(double Ki) { _KiGoal = Ki; }
        public void SetKp(double Kp) { _KpGoal = Kp; }

        public void SetExpTime(double expTime) { _goalExpTime = expTime;  }
        public void SetFps(double fps) { _goalFps = fps; }

        public double GetExpTime() { return _goalExpTime; }
        public double GetFps() { return _goalFps; }

        // Stabilize mode
        public void SetStabilize() { _stabilizeModeGoal = true; }
        public void UnsetStabilize() { _stabilizeModeGoal = false; }
        public bool GetStabilize() { return _stabilizeModeGoal;  }

        /**
         * Update the pitch, yaw and roll and send angle modification if necessary
         */
        protected override void work()
        {
            try 
            {
                // init Ki and Kp with values from server
                _Ki = _satService.getKi();
                _Kp = _satService.getKp();

                _fps = _goalFps = _satService.getFrameRate();
                _expTime = _goalExpTime = _satService.getExposureTime();

                // init stabilization mode
                _stabilizeMode = _satService.GetStablizationActive();

                Console.WriteLine("Write console orientation fetcher");

                while (_go && IsAlive())
                {
                    CamData data = _satService.GetCamData();

                    // update euler angles
                    double[] euler = new double[] { data.eulerPitch, data.eulerYaw, data.eulerRoll };

                    lock (this)
                    {
                        _pitch.push(euler[0]);
                        _yaw.push(euler[1]);
                        _roll.push(euler[2]);
                    }

                    // update servo angles         
                    _servoPitch = data.servoPitch;
                    _servoYaw = data.servoYaw;

                    bool goal = _stabilizeModeGoal, curr = _stabilizeMode, server = data.stabActive;
                    updateStabMode(goal, curr, server);
                    // prepare CamData object to return to the servo with new orders

                    CamData orders = new CamData();

                    // set stabilize mode 
                    if (sendStabOrder(goal, curr, server))
                        orders.setBoolProp(goal, CamData.STAB_ACTIVE);

                    // if servo angles invalid : send request for changing them
                    if (Math.Abs(_servoPitch - _goal_pitch) > SERVO_EQUAL_THRESHOLD && !_stabilizeMode)
                        orders.setIntProp(_goal_pitch, CamData.SERVO_PITCH);

                    if (Math.Abs(_servoYaw - _goal_yaw) > SERVO_EQUAL_THRESHOLD && !_stabilizeMode)
                        orders.setIntProp(_goal_yaw, CamData.SERVO_YAW);

                    if (Math.Abs(_KiGoal - _Ki) > EQUAL_THRESHOLD)
                    {
                        _Ki = _KiGoal;
                        orders.setDoubleProp(_Ki, CamData.STAB_KI);
                    }

                    if (Math.Abs(_KpGoal - _Kp) > EQUAL_THRESHOLD)
                    {
                        _Kp = _KpGoal;
                        orders.setDoubleProp(_Kp, CamData.STAB_KP);
                    }

                    if (Math.Abs(_goalFps - _fps) > EQUAL_THRESHOLD)
                    {
                        _fps = _goalFps;
                        orders.setDoubleProp(_fps, CamData.CAM_FPS);
                    }

                    if (Math.Abs(_goalExpTime - _expTime) > EQUAL_THRESHOLD)
                    {
                        _expTime = _goalExpTime;
                        orders.setDoubleProp(_expTime, CamData.CAM_EXP_TIME);
                    }

                    if(orders.hasChanged()) // send only if there are changes
                        _satService.SetCamData(orders);

                    Thread.Sleep(200);
                }
            } catch(Exception e) {
                Console.Error.Write("Error occurred in OrientationFetcher : {0}", e.Message);
            }

            Console.WriteLine("Leave orientation fetcher");
        }

        // if it changes server side (due to an error) reset the parameter and a request is not sent
        private void updateStabMode(bool goal, bool curr, bool server)
        {
            _stabilizeModeGoal = _stabilizeMode = goal; //goal && (!curr || (goal & server));
        }

        private bool sendStabOrder(bool goal, bool curr, bool server)
        {
            return goal != curr;//(!goal && server) || (goal && !server && !curr);
        }
    }
}
