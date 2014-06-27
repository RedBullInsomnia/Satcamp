using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Drawing;
using System.Threading;
using System.IO;
using System.Drawing.Imaging;

namespace SatelliteServer
{

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    class SatService : ISatService
    {
        private double[] _eulerAngles;
        private int[] _servoPos;
        private bool[] _servoChanged;
        private volatile bool _bStabilizationActive;

        private static const int MAX_CHANNEL = 10;

        private Object capture_lock = new Object(),
                       servo_lock = new Object(),
                       stab_lock = new Object();

        private MemoryStream _captureStream;
        private Bitmap _camImage;
        private AutoResetEvent _camEvent;
        private CameraDriver _camDriver;
        

        public SatService(CameraDriver camDriver)
        {
            _camEvent = new AutoResetEvent(false);
            _camDriver = camDriver;
            _camDriver.CameraCapture += _camDriver_CameraCapture;

            _bStabilizationChanged = false;

            _servoPos = new int[MAX_CHANNEL];
            _servoChanged = new bool[MAX_CHANNEL];
            for (int ii = 0; ii < MAX_CHANNEL; ii++)
            {
                _servoPos[ii] = 6000;
                _servoChanged[ii] = false;
            }
            _eulerAngles = new double[3];
        }

        public void SetStabilization(bool active)
        {
            lock (stab_lock)
            {
                _bStabilizationActive = active;
            }
        }

        public bool GetStablizationActive()
        {
            lock (stab_lock)
            {
                return _bStabilizationActive;
            }
        }

        public double[] GetEulerAngles()
        {
            lock (_eulerAngles)
            {
                return _eulerAngles;
            }
        }

        /** 
         * Set the "angle_id" value with "angle_value"
         */
        public void SetEulerAngle(int angle_id, double angle_value)
        {
            if(angle_id < 0 || angle_id >= 3)
                return;

            lock (_eulerAngles)
            {
                _eulerAngles[angle_id] = angle_value;
            }
        }

        public bool HasServoPosChanged(int channel)
        {
            if(channel >= MAX_CHANNEL || channel < 0)
                return false;

            lock (servo_lock)
            {
                return _servoChanged[channel];
            }
        }

        public void SetServoPos(int channel, int val)
        {
            if(channel >= MAX_CHANNEL || channel < 0)
                return;

            lock (servo_lock)
            {
                _servoPos[channel] = val;
                _servoChanged[channel] = true;
            }
        }

        public int GetServoPos(int channel)
        {
            if(channel >= MAX_CHANNEL || channel < 0)
                return -2;

            lock (servo_lock)
            {
                return _servoPos[channel];
            }
        }

        public int GetServoPosIfChanged(int channel)
        {
            if(channel >= MAX_CHANNEL || channel < 0)
                return -2;

            lock (servo_lock)
            {
                if(!_servoChanged[channel])
                    return -1;

                _servoChanged[channel] = false;
                return _servoPos[channel];
            }
        }

        void _camDriver_CameraCapture(object sender, Bitmap b)
        {
            lock (capture_lock)
            {
                if (_camImage == null)
                {
                    _camImage = new Bitmap(b.Width,b.Height);
                }
                Graphics g = Graphics.FromImage(_camImage);
                g.DrawImage(b, new Point(0, 0));
                g.Dispose();
                _camEvent.Set();
            }
        }

        public string Ping(string name)
        {
            Console.WriteLine("SERVER - Processing Ping('{0}')", name);
            return "Hello, " + name;
        }

        public byte[] Capture()
        {
            lock(capture_lock)
            {
                if (_camDriver.IsVideoStarted() == false)
                _camDriver.StartVideo();  

                _camEvent.WaitOne();

                if (_captureStream == null)
                {
                    _captureStream = new MemoryStream();
                }

                ImageCodecInfo jpgEncoder = ImageCodecInfo.GetImageEncoders().Single(x => x.FormatDescription == "JPEG");
                System.Drawing.Imaging.Encoder encoder2 = System.Drawing.Imaging.Encoder.Quality;
                EncoderParameters parameters = new System.Drawing.Imaging.EncoderParameters(1);
                EncoderParameter parameter = new EncoderParameter(encoder2, 50L);
                parameters.Param[0] = parameter;

                
                _captureStream.Seek(0, SeekOrigin.Begin);
                _camImage.Save(_captureStream, jpgEncoder, parameters);
                //_camImage.Save(_captureStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] buffer = new byte[_captureStream.Length];
                Console.WriteLine("Sending image with size " + buffer.Length);
                //_captureStream.Read(buffer,0,(int)_captureStream.Length);
                buffer = _captureStream.ToArray();
            }

            return buffer;
        }    
    }
}
