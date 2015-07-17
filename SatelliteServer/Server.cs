using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms; // Message
using System.Threading.Tasks;
using System.ServiceModel;
using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;

namespace SatelliteServer
{
    public class Server : BaseThread
    {
        private SatService _service; /** The service object handling the connection */
        private ServiceHost _host; /** Host object representing the host of the service */

        private CameraDriver _cameraDriver; /** The driver of the camera */
        private ServoDriver _servoDriver; /** The driver of the servo */
        private Um6Driver _um6Driver; /** Driver of the sensors */

        private ThreadSafeContainer<Bitmap> _container; /** Queue in which will be stored the consecutive frames received from the camera */
        
        private PictureBox _pBox; /** Picture box on which the image must be displayed */
        private bool _printOnPbox; /** True for sending the picture in the picture box */

        private Logger _logger; /** for outputting message */

        private ControlThread _controller; /** Thread for stabilizing the servo */

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

        /**
         * Construct the server object :
         * - hWind : window handle
         * - pBox : the picture box on which the image must be sent
         * - printOnBox : true if the server thread must set the picture in the picture box
         */
        public Server(long hWind, PictureBox pBox, bool printOnPbox)
        {
            this._pBox = pBox;
            this._printOnPbox = printOnPbox;
            this._container = new ThreadSafeContainer<Bitmap>();
            this._logger = Logger.instance();
            this._controller = null;

            initCameraDriver((int) hWind, (int) pBox.Handle.ToInt64());
            initSensors();
            initServos();
            initService();
            initController();
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
            _service = new SatService(_container);
            _host = new ServiceHost(_service);
            _host.AddServiceEndpoint(typeof(ISatService),
                                     binding,
                                     "net.tcp://localhost:8000");
            _host.Open();
            _logger.log("Service launched on 'localhost:8000'");
        }

        /**
         * Initialize the camera driver
         */
        private void initCameraDriver(int hWind, int hPictBox)
        {
            _cameraDriver = new CameraDriver(hWind);
            _cameraDriver.Init(hPictBox);
            _cameraDriver.CameraCapture += cameraCaptureHandler;
            _logger.log("Camera driver successfully initialized");
        }

        /**
         * Manage the capture of a frame
         */
        private void cameraCaptureHandler(object sender, Bitmap frame)
        {
            if (_printOnPbox)
                _pBox.Image = bitmapDeepCopy(frame);
            enqueueFrame(bitmapDeepCopy(frame));
        }

        private Bitmap bitmapDeepCopy(Bitmap input)
        {
            return input.Clone(new Rectangle(0, 0, input.Width, input.Height), input.PixelFormat);
        }

        /**
         * Enqueue a frame (handle the max number of frame in the queue)
         */
        private void enqueueFrame(Bitmap frame)
        {
            _container.set(frame);
        }

        /**
         * Initializes the sensors drivers
         */
        private void initSensors()
        {
            _um6Driver = new Um6Driver(SERIAL_PORT_NAME, SERIAL_PORT_BAUD_RATE);
            _um6Driver.Init();
            _logger.log("Sensors driver successfully initialized");
        }

        /**
         * Initializes the servo drivers 
         */
        private void initServos()
        {
            _servoDriver = new ServoDriver();
            _logger.log("Servos driver successfully initialized");
        }

        /**
         * Start the controller thread  
         */
        private void startController() 
        {
            if (_controller != null)
                return;

            _controller = new ControlThread(_um6Driver, _service);
            _controller.Start();
        }

        /**
         * Stop the controller thread and set _controller to null
         */
        private void stopController()
        {
            if (_controller == null)
                return;

            _controller.Stop();
            _controller = null;
        }

        public void passMessage(ref Message m)
        {
            _cameraDriver.HandleMessage(m.Msg, m.LParam.ToInt64(), m.WParam.ToInt32());
        }

        /**
         * 
         */
        protected override void work()
        {
            _logger.log("Server main loop has started");
            _cameraDriver.StartVideo();

            while (_go && IsAlive())
            {
                // fetch euler angles
                _service._eulerAngles = new double[3] { _um6Driver.Angles[0], _um6Driver.Angles[1], _um6Driver.Angles[2] };

                if(_service._bStabilizationActive)
                {
                    if (_controller == null)
                        startController();
                } 
                else 
                {
                    // stop controller thread if it was launched
                    if (_controller != null && _controller.IsAlive())
                        stopController();

                    // Transfer servo modification order to the servos 
                    // do it with Pitch
                    if (_service._servoChanged[PITCH_SERVO_ADDR])
                    {
                        _service._servoChanged[PITCH_SERVO_ADDR] = false;
                        _servoDriver.SetServo(PITCH_SERVO_ADDR, (ushort)_service._servoPos[PITCH_SERVO_ADDR]);
                    }

                    // do it with Yaw
                    if (_service._servoChanged[YAW_SERVO_ADDR])
                    {
                        _service._servoChanged[YAW_SERVO_ADDR] = false;
                        _servoDriver.SetServo(YAW_SERVO_ADDR, (ushort)_service._servoPos[YAW_SERVO_ADDR]);
                    }
                }   
            }

            _logger.log("Server main loop has ended");
            cleanRessources();
        }

        /**
         * Clean the ressources associated with the object
         */
        private void cleanRessources()
        {
            if (_cameraDriver != null)
            {
                _logger.log("Shutting down camera");
                _cameraDriver.StopVideo();
                _cameraDriver.ShutDown();
            }

            if (_servoDriver != null)
            {
                _logger.log("Shutting down servos");
                _servoDriver.Dispose();
            }

            if (_host != null)
            {
                _logger.log("Closing service endpoint");
                _host.Close();
            }

            if (_controller != null)
            {
                _logger.log("Stop controller thread");
                _controller.Stop();
            }
        }
    }
}