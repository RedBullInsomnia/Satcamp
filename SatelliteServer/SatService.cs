using System.Linq;
using System.ServiceModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

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
        public bool _bStabilizationChanged; // true when it changes server side
        public bool _bStabilizationActive;
        public double _frameRate;
        public double _expTime;
        public bool _frameRateChanged;
        public bool _expTimeChanged;

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

        public bool GetStablizationActive()
        {

            return _bStabilizationActive;
        }

        public double[] GetEulerAngles()
        {
            return _eulerAngles;
        }

        public double getKi()
        {
            return _ki;
        }

        public double getKp()
        {
            return _kp;
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
            Logger.instance().log("Received a ping with message '" + name + "'");
            return "Hello, " + name;
        }

        public byte[] Capture()
        {
            Bitmap captured = _container.get();

            MemoryStream stream = new MemoryStream();
            stream.Seek(0, SeekOrigin.Begin);
            captured.Save(stream, _jpegEncoder, _jpegEncoderParameters);
            return stream.ToArray();
        }

        public bool Ping() 
        {
            Logger.instance().log("Just received a ping");
            return true;
        }

        public CamData GetCamData()
        {
            CamData data = new CamData();
            data.setIntProp(_servoPos[Server.PITCH_SERVO_ADDR], CamData.SERVO_PITCH);
            data.setIntProp(_servoPos[Server.YAW_SERVO_ADDR], CamData.SERVO_YAW);
            data.setDoubleProp(_eulerAngles[0], CamData.EULER_ROLL);
            data.setDoubleProp(_eulerAngles[1], CamData.EULER_PITCH);
            data.setDoubleProp(_eulerAngles[2], CamData.EULER_YAW);
            if (_bStabilizationChanged)
            {
                data.setBoolProp(_bStabilizationActive, CamData.STAB_ACTIVE);
                _bStabilizationChanged = false;
            }
            return data;
        }

        public void SetCamData(CamData data)
        {
            if (data.propChanged[CamData.SERVO_PITCH])
            {
                _servoPos[Server.PITCH_SERVO_ADDR] = data.servoPitch;
                _servoChanged[Server.PITCH_SERVO_ADDR] = true;
                Logger.instance().log("Servo pitch set to " + data.servoPitch);
            }

            if(data.propChanged[CamData.SERVO_YAW])
            {
                _servoPos[Server.YAW_SERVO_ADDR] = data.servoYaw;
                _servoChanged[Server.YAW_SERVO_ADDR] = true;
                Logger.instance().log("Servo yaw set to " + data.servoYaw);
            }

            if (data.propChanged[CamData.STAB_KI])
            {
                _ki = data.ki;
                _kiChanged = true;
                Logger.instance().log("Ki set to " + data.ki);
            }

            if(data.propChanged[CamData.STAB_KP])
            {
                _kp = data.kp;
                _kpChanged = true;
                Logger.instance().log("Kp set to " + data.kp);
            }

            if(data.propChanged[CamData.STAB_ACTIVE] && data.stabActive != _bStabilizationActive)
            {
                _bStabilizationActive = data.stabActive;
                Logger.instance().log("Stabilization " + (data.stabActive ? "activated" : "deactivated"));
            }

            if (data.propChanged[CamData.CAM_FPS])
            {
                _frameRate = data.fps;
                _frameRateChanged = true;
                Logger.instance().log("Frame rate set to " + data.fps);
            }

            if (data.propChanged[CamData.CAM_EXP_TIME])
            {
                _expTime = data.expTime;
                _expTimeChanged = true;
                Logger.instance().log("Exposure time set to " + data.expTime); 
            }
        }

        public double getExposureTime()
        {
            return _expTime;
        }

        public double getFrameRate()
        {
            return _frameRate;
        }
    }
}
