using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatelliteServer
{
    class ControlThread : BaseThread
    {
        private Um6Driver _um6Driver;
        private ISatService _satService;
        private const int SERVO_MAX_POS = 8000, SERVO_MIN_POS = 4000; 
        
        private int pitchStabAngle, yawStabAngle;
        private double Kp, Ki;
        
        public ControlThread(Um6Driver driver, SatService satService)
        {
            _um6Driver = driver;
            _satService = satService;
        }

        protected override void work()
        {
            while (_go && IsAlive())
            {
                // update data
                Kp = _satService.getKp();
                Ki = _satService.getKi();

                pitchStabAngle = 0; // TODO ????
                yawStabAngle = 0; // TODO ????

                customPid();
            }
        }

        void customPid()
        {
            //Calculate the error on the pitch axis
            double pitch_error = _um6Driver.Angles[1] - pitchStabilizationAngle;
            int tmp_pitchtrackbarvalue = _stabPitchServo + (int)(pitch_error * Kp)
                                                             + (int)(perr_int * Ki);
            
            // Calculate the error on the yaw axis
            double yaw_error = _um6Driver.Angles[2] - yawStabilizationAngle;
            int tmp_yawtrackbarvalue = _stabYawServo + (int)(yaw_error * Kp)
                                                         + (int)(yerr_int * Ki);
            // Clamp pitch servo
            clamp(tmp_pitchtrackbarvalue, SERVO_MAX_POS, SERVO_MIN_POS);
            // Clamp yaw servo
            clamp(tmp_yawtrackbarvalue, SERVO_MAX_POS, SERVO_MIN_POS);
        }

        private int clamp(int a, int max, int min)
        {
            if (a < min)
                a = min;

            if (a > max)
                a = max;

            return a;
        }
    }
}
