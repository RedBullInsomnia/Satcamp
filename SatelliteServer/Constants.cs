namespace SatelliteServer
{
    public static class Constants
    {
        /** 
        * default camera parameters 
        */
        public const double DEF_FPS = 15.0, DEF_EXP_TIME = 10.0;
        public const double MIN_FPS = 3.0;
        public const double MAX_FPS = 25.0;
        public const double MIN_EXP_TIME = 1.0;
        public const double MAX_EXP_TIME = 200.0;

        /**
        * Information about the serial port for communicating with the sensors
        */
        public const string SERIAL_PORT_NAME = "COM1";
        public const int SERIAL_PORT_BAUD_RATE = 115200;

        /**
         * Servos data
         */
        public const byte PITCH_SERVO_ADDR = 1, YAW_SERVO_ADDR = 0;
        public const double DEF_KI = 0.0, DEF_KP = 0.2;
        public const ushort DEFAULT_SERVO_POS = 6000, MAX_SERVO_POS = 8000, MIN_SERVO_POS = 4000;

        /**
        * default ip adress
        */
        public const string DEFAULT_IP = "192.168.1.96";
    }
}
