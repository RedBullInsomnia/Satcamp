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
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class SatService : ISatService
    {
        private BlockingCollection<Bitmap> _captureQueue; /** queue containing captures frame */
        
        /** 
         * Image compression components
         */
        private ImageCodecInfo _jpegEncoder;
        private EncoderParameters _jpegEncoderParameters;

        public double[] _eulerAngles;
        public int[] _servoPos;
        public bool[] _servoChanged;
        public bool _bStabilizationChanged;
        public bool _bStabilizationActive;

        public SatService(BlockingCollection<Bitmap> captureQueue)
        {
            initJpegEncoder();

            _captureQueue = captureQueue;

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

        public string Ping(string name)
        {
            Logger.instance().log("Processing Ping(" + name + ")");
            return "Hello, " + name;
        }

        public byte[] Capture()
        {
            Bitmap captured = _captureQueue.Take(); // blocking

            MemoryStream stream = new MemoryStream();
            stream.Seek(0, SeekOrigin.Begin);
            captured.Save(stream, _jpegEncoder, _jpegEncoderParameters);
            return stream.ToArray();
        }
    }
}
