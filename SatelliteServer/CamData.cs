using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SatelliteServer
{
    [DataContract]
    public class CamData
    {
        public const int EULER_PITCH = 0,
                         EULER_YAW = 1,
                         EULER_ROLL = 2,
                         SERVO_PITCH = 3, 
                         SERVO_YAW = 4,
                         CAM_FPS = 5,
                         CAM_EXP_TIME = 6,
                         STAB_KI = 7,
                         STAB_KP = 8,
                         STAB_ACTIVE = 9;

        public const int NUM_FIELDS = 10;

        [DataMember]
        public bool[] propChanged { get; set; }

        [DataMember]
        public double eulerPitch { get; set; }

        [DataMember]
        public double eulerYaw { get; set; }

        [DataMember]
        public double eulerRoll { get; set; }

        [DataMember]
        public int servoPitch { get; set; }

        [DataMember]
        public int servoYaw { get; set; }

        [DataMember]
        public double fps { get; set; }

        [DataMember]
        public double expTime { get; set; }

        [DataMember]
        public double ki { get; set; }

        [DataMember]
        public double kp { get; set; }

        [DataMember]
        public bool stabActive { get; set; }

        public CamData()
        {
            propChanged = new bool[NUM_FIELDS];

            for(int i = 0; i < NUM_FIELDS; ++i) {
                propChanged[i] = false;
            }
        }

        public void setBoolProp(bool val, int prop) {
            switch(prop) {
                case STAB_ACTIVE :
                    stabActive = val;
                    break;
                default:
                    throw new ArgumentException("The property is not a boolean");
            }
            modifyProp(prop);
        }

        public void setDoubleProp(double val, int prop) {
            switch(prop) {
                case EULER_PITCH :
                    eulerPitch = val;
                    break;
                case EULER_YAW :
                    eulerYaw = val;
                    break;
                case EULER_ROLL :
                    eulerRoll = val;
                    break;
                case CAM_FPS :
                    fps = val;
                    break;
                case CAM_EXP_TIME :
                    expTime = val;
                    break;
                case STAB_KI :
                    ki = val;
                    break;
                case STAB_KP :
                    kp = val;
                    break;
                default:
                    throw new ArgumentException("The property is not a double");
            }
            modifyProp(prop);
        }

        public void setIntProp(int val, int prop) {
            switch(prop) {
                case SERVO_PITCH :
                    servoPitch = val;
                    break;
                case SERVO_YAW :
                    servoYaw = val;
                    break;
                default:
                    throw new ArgumentException("The property is not an integer");
            }
            modifyProp(prop);
        }

        private void modifyProp(int prop) {
            propChanged[prop] = true;
        }

        /** Return true if any property was changed */
        public bool hasChanged()
        {
            bool changed = false;
            for (int i = 0; i < NUM_FIELDS; ++i)
                changed |= propChanged[i];
            return changed;
        }
    }
}
