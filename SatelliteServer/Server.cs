using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Net;
using System.Net.Sockets;

namespace SatelliteServer
{
    public class Server : BaseThread
    {
        private SatService _service; /** The service object handling the connection */
        private ServiceHost _host; /** Host object representing the host of the service */

        private CameraDriver _cameraDriver; /** The driver of the camera */
        private ServoDriver _servoDriver; /** The driver of the servo */
        private Um6Driver _um6Driver; /** Driver of the sensors */

        /**
         * Information about the serial port for communicating with the sensors
         */
        private const string SERIAL_PORT_NAME = "COM1";
        private const int SERIAL_PORT_BAUD_RATE = 115200;

        /**
         * Servos data
         */
        private const byte PITCH_SERVO_ADDR = 1;
        private const byte YAW_SERVO_ADDR = 0;

        public Server () 
        {
            initCameraDriver();
            initSensors();
            initServos();
            initService();
        }

        /**
         * Initiate the service endpoint for the WCF service contract
         */
        private void initService()
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.MaxReceivedMessageSize = 20000000;
            binding.MaxBufferPoolSize = 20000000;
            binding.MaxBufferSize = 20000000;
            binding.Security.Mode = SecurityMode.None;
            _service = new SatService(_cameraDriver);
            _host = new ServiceHost(_service);
            _host.AddServiceEndpoint(typeof(ISatService),
                                     binding,
                                     "net.tcp://localhost:8000");
            _host.Open();
        }

        /**
         * Initialize the camera driver
         */
        private void initCameraDriver()
        {
            _cameraDriver = new CameraDriver(-1);
            _cameraDriver.Init(-1);
        }

        /**
         * Initializes the sensors drivers
         */
        private void initSensors()
        {
            _um6Driver = new Um6Driver(SERIAL_PORT_NAME, SERIAL_PORT_BAUD_RATE);
            _um6Driver.Init();
        }

        /**
         * Initializes the servo drivers 
         */
        private void initServos()
        {
            _servoDriver = new ServoDriver();
        }

        /**
         * 
         */
        protected override void work()
        {
            while (_go && IsAlive())
            {
                // Transfer servo modification order to the servos 
                // do it with Pitch
                if (_service._servoChanged[0])
                {
                    _service._servoChanged[0] = false;
                    _servoDriver.SetServo(PITCH_SERVO_ADDR, (ushort)_service._servoPos[0]);
                }

                // do it with Yaw
                if(_service._servoChanged[1])
                {
                    _service._servoChanged[1] = false;
                    _servoDriver.SetServo(YAW_SERVO_ADDR, (ushort)_service._servoPos[1]);
                }
            }

            cleanRessources();
        }

        /**
         * Clean the ressources associated with the object
         */
        private void cleanRessources()
        {
            if (_cameraDriver != null)
            {
                _cameraDriver.StopVideo();
                _cameraDriver.ShutDown();
            }

            if(_servoDriver != null)
                _servoDriver.Dispose();
        }
    }
}
