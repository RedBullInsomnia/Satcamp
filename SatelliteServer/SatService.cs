using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Drawing;
using System.Threading;
using System.IO;
using System.Drawing.Imaging;
using System.Collections.Concurrent;
using System.Drawing.Imaging;

namespace SatelliteServer
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, 
                     ConcurrencyMode = ConcurrencyMode.Multiple, 
                     UseSynchronizationContext = false)]
    class SatService : ISatService
    {
        private ThreadSafeContainer<Bitmap> _container; /** queue containing captures frame */
        
        /** 
         * Image compression components
         */
        private ImageCodecInfo _jpegEncoder;
        private EncoderParameters _jpegEncoderParameters;

        public double[] _eulerAngles;
        public int[] _servoPos;
        public double _ki, _kp;
        public bool _kiChanged, _kpChanged;
        public bool[] _servoChanged;
        public bool _bStabilizationChanged;
        public bool _bStabilizationActive;

        public SatService(ThreadSafeContainer<Bitmap> container)
        {
            initJpegEncoder();

            _container = container;

            _bStabilizationChanged = false;

            _servoPos = new int[10];
            _servoChanged = new bool[10];
            for (int ii = 0; ii < 10; ii++)
            {
                _servoPos[ii] = 6000;
                _servoChanged[ii] = false;
            }
            _eulerAngles = new double[3];
        }

        private void initJpegEncoder()
        {
            _jpegEncoder = ImageCodecInfo.GetImageEncoders().Single(x => x.FormatDescription == "JPEG");
            _jpegEncoderParameters = new EncoderParameters(1);
            _jpegEncoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 50L);
        }

        public void SetStabilization(bool active)
        {
            Logger.instance().log("Stabilization set");
            _bStabilizationActive = active;
            _bStabilizationChanged = true;
        }

        public bool GetStablizationActive()
        {
            return _bStabilizationActive;
        }

        public double[] GetEulerAngles()
        {
            return _eulerAngles;
        }

        double getKi()
        {
            return _ki;
        }

        double getKp()
        {
            return _kp;
        }

        void setKi(double ki)
        {
            _ki = ki;
            _kiChanged = true;
        }

        void setKp(double kp)
        {
            _kp = kp;
            _kpChanged = true;
        }

        public void SetServoPos(int channel, int val)
        {
            Logger.instance().log("Servo on channel '" + channel + "' set to position " + val);
            _servoPos[channel] = val;
            _servoChanged[channel] = true;
        }

        public int GetServoPos(int channel)
        {
            return _servoPos[channel];
        }

        public string NamedPing(string name)
        {
            Logger.instance().log("Processing named ping from '" + name + "')");
            return "Hello, " + name;
        }

        public byte[] Capture()
        {
            Bitmap captured = _container.get();
            Logger.instance().log("Process image");
            
            MemoryStream stream = new MemoryStream();
            stream.Seek(0, SeekOrigin.Begin);
            captured.Save(stream, _jpegEncoder, _jpegEncoderParameters);
            return stream.ToArray();
        }

        public bool Ping() 
        {
            Logger.instance().log("Processing unamed ping");
            return true;
        }
    }
}
