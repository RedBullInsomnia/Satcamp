using System;
using System.Threading;

namespace SatelliteServer
{
    /**
     * @class ControlThread
     * @brief A class implementing a controller for stabilizing the camera
     */
    class ControlThread : BaseThread
    {
        // Drivers
        private Um6Driver _um6Driver;
        private ServoDriver _servoDriver;

        // Service
        private SatService _satService;

        // Control parameters

        private double _pitchStabAngle, _yawStabAngle;
        private double _Kp, _Ki;
        //private double _perrInt, _yerrInt;
        private double RATIO_ANGLE_SERVO_POS = 11.11;

        public ControlThread(Um6Driver um6Driver, ServoDriver servoDriver, double pitchStabAngle, double yawStabAngle)
        {
            _um6Driver = um6Driver;
            _servoDriver = servoDriver;
            _pitchStabAngle = pitchStabAngle;
            _yawStabAngle = yawStabAngle;
        }

        protected override void work()
        {
            try
            {
                Logger.instance().log("Controller tries to stabilize to (pitch, yaw) : (" + _pitchStabAngle + ", " + _yawStabAngle + ")");
                Logger.instance().log("Start controller thread");
                while (_go && IsAlive() && _satService._bStabilizationActive)
                {
                    // update data
                    _Kp = _satService._kp;
                    _Ki = _satService._ki;

                    pid();
                    Thread.Sleep(1);
                }
            }
            catch (Exception e)
            {
                Logger.instance().log("Error occurred during the execution of the control thread : " + e.Message);
                _satService._bStabilizationActive = false;
                _satService._bStabilizationChanged = true;
            }

            _satService._bStabilizationActive = false;
            Logger.instance().log("Leave thread controller");
        }

        void pid()
        {
            //Calculate the error on the pitch axis
            double pitch_error = _um6Driver.Pitch - _pitchStabAngle;

            int _pitchVal = _satService._servoPos[Constants.PITCH_SERVO_ADDR]
                                + (int)(pitch_error * _Kp * RATIO_ANGLE_SERVO_POS);

            /* Debug
            Logger.instance().log(" [PITCH] Error         : " + pitch_error);
            Logger.instance().log(" [PITCH] Integral      : " + _perrInt);
            Logger.instance().log(" [PITCH] Curr pos      : " + _satService._servoPos[Constants.PITCH_SERVO_ADDR]);
            Logger.instance().log(" [PITCH] Error contr.  : " + (int)(pitch_error * _Kp * RATIO_ANGLE_SERVO_POS));
            Logger.instance().log(" [PITCH] Integ. contr. : " + (int)(_perrInt * _Ki * RATIO_ANGLE_SERVO_POS));
            */

            // Calculate the error on the yaw axis
            double yaw_error = _um6Driver.Yaw - _yawStabAngle;

            ushort _yawVal = (ushort)(_satService._servoPos[Constants.YAW_SERVO_ADDR]
                                + (yaw_error * _Kp * RATIO_ANGLE_SERVO_POS));

            /* Debug
            Logger.instance().log(" [YAW] Error         : " + yaw_error);
            Logger.instance().log(" [YAW] Integral      : " + _yerrInt);
            Logger.instance().log(" [YAW] Curr pos      : " + _satService._servoPos[Constants.YAW_SERVO_ADDR]);
            Logger.instance().log(" [YAW] Error contr.  : " + (int)(yaw_error * _Kp * RATIO_ANGLE_SERVO_POS));
            Logger.instance().log(" [YAW] Integ. contr. : " + (int)(_yerrInt * _Ki * RATIO_ANGLE_SERVO_POS));
            */

            // send orders to the servos
            if (_pitchVal != _satService._servoPos[Constants.PITCH_SERVO_ADDR])
            {
                ushort pitchVal = (ushort)clamp(_pitchVal, Constants.MAX_SERVO_POS, Constants.MIN_SERVO_POS);
                _servoDriver.SetServo(Constants.PITCH_SERVO_ADDR, pitchVal);
                //_satService.SetServoPos(Constants.PITCH_SERVO_ADDR, pitchVal);
            }

            Thread.Sleep(1);
            if (_yawVal != _satService._servoPos[Constants.YAW_SERVO_ADDR])
            {
                ushort yawVal = (ushort)clamp(_yawVal, Constants.MAX_SERVO_POS, Constants.MIN_SERVO_POS);
                _servoDriver.SetServo(Constants.YAW_SERVO_ADDR, yawVal);
                //_satService.SetServoPos(Constants.YAW_SERVO_ADDR, yawVal);
            }

            // Debug
            // Logger.instance().log("Correction (pitch, yaw) : (" + pitchVal + ", " + yawVal + ")");
        }

        private int clamp(int a, int max, int min)
        {
            if (a < min)
                a = min;
            else if (a > max)
                a = max;

            return a;
        }
    }
}
