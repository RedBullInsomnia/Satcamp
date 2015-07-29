using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private const int SERVO_MAX_POS = 8000, SERVO_MIN_POS = 4000; 
        private double _pitchStabAngle, _yawStabAngle;
        private double _Kp, _Ki;
        private double _perrInt, _yerrInt;
        private double RATIO_ANGLE_SERVO_POS = 11.11;

        public ControlThread(Um6Driver um6Driver, ServoDriver servoDriver, SatService satService, double pitchStabAngle, double yawStabAngle)
        {
            _um6Driver = um6Driver;
            _servoDriver = servoDriver;
            _satService = satService;
            _pitchStabAngle = pitchStabAngle;
            _yawStabAngle = yawStabAngle;
            _perrInt = 0;
            _yerrInt = 0;
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
                    Thread.Sleep(2);
                }
            } catch(Exception e) {
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
            double pitch_error = _um6Driver.Angles[1] - _pitchStabAngle;
            _perrInt += pitch_error;

            int pitchVal = _satService._servoPos[Server.PITCH_SERVO_ADDR] 
                                + (int)(pitch_error * _Kp * RATIO_ANGLE_SERVO_POS) 
                                + (int)(_perrInt * _Ki * RATIO_ANGLE_SERVO_POS);

            Logger.instance().log(" [PITCH] Error         : " + pitch_error);
            //Logger.instance().log(" [PITCH] Integral      : " + _perrInt);
            Logger.instance().log(" [PITCH] Curr pos      : " + _satService._servoPos[Server.PITCH_SERVO_ADDR]);
            Logger.instance().log(" [PITCH] Error contr.  : " + (int)(pitch_error * _Kp * RATIO_ANGLE_SERVO_POS));
            //Logger.instance().log(" [PITCH] Integ. contr. : " + (int)(_perrInt * _Ki * RATIO_ANGLE_SERVO_POS));
            
            // Calculate the error on the yaw axis
            double yaw_error = _um6Driver.Angles[2] - _yawStabAngle;
            _yerrInt += yaw_error;

            int yawVal = _satService._servoPos[Server.YAW_SERVO_ADDR]
                                + (int)(yaw_error * _Kp * RATIO_ANGLE_SERVO_POS)
                                + (int)(_yerrInt * _Ki * RATIO_ANGLE_SERVO_POS);

            Logger.instance().log(" [YAW] Error         : " + yaw_error);
            //Logger.instance().log(" [YAW] Integral      : " + _yerrInt);
            Logger.instance().log(" [YAW] Curr pos      : " + _satService._servoPos[Server.YAW_SERVO_ADDR]);
            Logger.instance().log(" [YAW] Error contr.  : " + (int)(yaw_error * _Kp * RATIO_ANGLE_SERVO_POS));
            //Logger.instance().log(" [YAW] Integ. contr. : " + (int)(_yerrInt * _Ki * RATIO_ANGLE_SERVO_POS));

            // send orders to the servos
            _servoDriver.SetServo(Server.PITCH_SERVO_ADDR, (ushort) clamp(pitchVal, SERVO_MAX_POS, SERVO_MIN_POS));
            Thread.Sleep(1);
            _servoDriver.SetServo(Server.YAW_SERVO_ADDR, (ushort) clamp(yawVal, SERVO_MAX_POS, SERVO_MIN_POS));

            Logger.instance().log("Correction (pitch, yaw) : (" + pitchVal + ", " + yawVal + ")");
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
