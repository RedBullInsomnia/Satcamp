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

        private long _maxFrameInQueue; /** Maximum number of frame in the queue : -1 for no max, > 0 otherwise */
        private BlockingCollection<Bitmap> _captureQueue; /** Queue in which will be stored the consecutive frames received from the camera */
        
        private PictureBox _pBox; /** Picture box on which the image must be displayed */
        private bool _printOnPbox; /** True for sending the picture in the picture box */

        private Logger _logger; /** for outputting message */

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
            this._captureQueue = new BlockingCollection<Bitmap>();
            this._maxFrameInQueue = this._captureQueue.BoundedCapacity;
            this._logger = Logger.instance();

            initCameraDriver((int) hWind, (int) pBox.Handle.ToInt64());
            initSensors();
            initServos();
            initService();
        }

        /**
         * Set the limit number of frames that can be stored internally
         * If the number of frame stored exceeds this value, the oldest frame is discarded
         * Must be greater than 1 for stability reason
         */
        public void setFrameNumberLimit(long newMax)
        {
            if(newMax > 1) 
               _maxFrameInQueue = newMax;
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
            _service = new SatService(_captureQueue);
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
                _pBox.Image = (Bitmap) frame.Clone();
            enqueueFrame(frame);
        }

        /**
         * Enqueue a frame (handle the max number of frame in the queue)
         */
        private void enqueueFrame(Bitmap frame)
        {
            
            /**
             * A little dirty : consider that if the limit is reached, the fact that frames are stored
             * introduce a unacceptable delay so clear the queue
             */
            lock (this) { 
                if (_captureQueue.Count() >= _maxFrameInQueue)
                {
                    while (_captureQueue.Count() > 0)
                    {
                        Bitmap item;
                        _captureQueue.TryTake(out item);
                    }
                }
            }  

            _captureQueue.Add(frame);
        }

        /**
         * Initializes the sensors drivers
         */
        private void initSensors()
        {
            _um6Driver = new Um6Driver(SERIAL_PORT_NAME, SERIAL_PORT_BAUD_RATE);
            _um6Driver.Init();
            _logger.log("Sensors driver correctly initialized");
        }

        /**
         * Initializes the servo drivers 
         */
        private void initServos()
        {
            _servoDriver = new ServoDriver();
            _logger.log("Servos driver correctly initialized");
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
            //ulong logger_freq = 0;
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
                if (_service._servoChanged[1])
                {
                    _service._servoChanged[1] = false;
                    _servoDriver.SetServo(YAW_SERVO_ADDR, (ushort)_service._servoPos[1]);
                }

                //if ((++logger_freq % 100000000) != 0)
                //    _logger.log("Server running (frames stored : " + _captureQueue.Count() + ")");
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
                _logger.log("Closing camera");
                _cameraDriver.StopVideo();
                _cameraDriver.ShutDown();
            }

            if (_servoDriver != null)
            {
                _logger.log("Closing servos drivers");
                _servoDriver.Dispose();
            }
        }
    }
}
