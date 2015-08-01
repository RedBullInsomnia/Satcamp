using System;
using System.Runtime.InteropServices;
using System.Drawing;

namespace SatelliteServer
{
    public delegate void CameraCaptureHandler(object sender, Bitmap b);
    class CameraDriver
    {
        public event CameraCaptureHandler CameraCapture;
        public CameraDriver(long hWnd)
	    {
            // init our uc480 object
            m_Hwnd = hWnd;
            m_uc480 = new uc480();

            // enable static messages ( no open camera is needed )		
            m_uc480.EnableMessage(uc480.IS_NEW_DEVICE, (int)hWnd);
            m_uc480.EnableMessage(uc480.IS_DEVICE_REMOVAL, (int)hWnd);        

            // init our image struct and alloc marshall pointers for the uc480 memory
            m_Uc480Images = new UC480IMAGE[IMAGE_COUNT];
            int nLoop = 0;
            for (nLoop = 0; nLoop < IMAGE_COUNT; nLoop++)
            {
                m_Uc480Images[nLoop].pMemory = Marshal.AllocCoTaskMem(4);	// create marshal object pointers
                m_Uc480Images[nLoop].MemID = 0;
                m_Uc480Images[nLoop].nSeqNum = 0;
            }

            m_bDrawing = false;
            m_RenderMode = uc480.IS_RENDER_NORMAL;

            // default parameters
            expTime = Constants.DEF_EXP_TIME; // ms
            fps = Constants.DEF_FPS; // frame / s
	    }

        public bool Capture()
        {
            int res = m_uc480.FreezeVideo(uc480.IS_WAIT);
            // capture a single image
            if (res != uc480.IS_SUCCESS)
            {
                Console.WriteLine("Failed to capture image. Error code " + res );
                return false; // throw new Exception("Error freeze image");
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Connects to the camera
        /// </summary>
        public void Init(long hPictureBox)
        {
            m_PictureBoxHwnd = hPictureBox;

            // if opened before, close now
            if (m_uc480.IsOpen())
            {
                m_uc480.ExitCamera();
            }

            // open a camera
            int nRet = m_uc480.InitCamera(0, (int)hPictureBox);
            if (nRet == uc480.IS_STARTER_FW_UPLOAD_NEEDED)
            {
               throw new Exception("The camera requires starter firmware upload");
            }

            if (nRet != uc480.IS_SUCCESS)
            {
                throw new Exception("Camera init failed");
            }

            uc480.SENSORINFO sensorInfo = new uc480.SENSORINFO();
            m_uc480.GetSensorInfo(ref sensorInfo);

            // Set the image size
            int x = 0;
            int y = 0;
            unsafe
            {
                m_uc480.SetImageSize(800, 600);
            }

            // alloc images
            m_uc480.ClearSequence();
            int nLoop = 0;
            for (nLoop = 0; nLoop < IMAGE_COUNT; nLoop++)
            {
                // alloc memory
                m_uc480.AllocImageMem(x, y, 32, ref m_Uc480Images[nLoop].pMemory, ref m_Uc480Images[nLoop].MemID);
                // add our memory to the sequence
                m_uc480.AddToSequence(m_Uc480Images[nLoop].pMemory, m_Uc480Images[nLoop].MemID);
                // set sequence number
                m_Uc480Images[nLoop].nSeqNum = nLoop + 1;
            }

            double enable = 1, zero = 0;
            m_uc480.SetColorMode(uc480.IS_SET_CM_RGB32);
            m_uc480.EnableMessage(uc480.IS_FRAME, (int)m_Hwnd);
            m_uc480.SetBadPixelCorrection(uc480.IS_BPC_ENABLE_SOFTWARE, 1); //trying to set bad pixel correction, needs to be studied more.
            m_uc480.SetPixelClock(30);
            
            m_uc480.SetAutoParameter(uc480.IS_SET_ENABLE_AUTO_WHITEBALANCE,  ref enable, ref zero);
            m_uc480.SetAutoParameter(uc480.IS_SET_ENABLE_AUTO_GAIN, ref enable, ref zero);
            
            m_uc480.FreezeVideo(uc480.IS_WAIT);
            m_bIsStarted = false;
        }

        unsafe public void StartVideo()
        {
            double fps = this.fps, // frame / sec 
                    expTime = this.expTime; // milliseconds
            m_uc480.SetFrameRate(this.fps, ref fps);
            m_uc480.SetExposureTime(this.expTime, ref expTime);

            if (m_uc480.CaptureVideo(uc480.IS_WAIT) == uc480.IS_SUCCESS)
            {
                m_bIsStarted = true;
            }
        }

        public void setAutoGain()
        {
            double autoGainEnable = 1, zero = 0;
            m_uc480.SetAutoParameter(uc480.IS_SET_ENABLE_AUTO_GAIN, ref autoGainEnable, ref zero);
        }

        public bool IsVideoStarted()
        {
            return m_bIsStarted;
        }

        public void StopVideo()
        {
            if (m_uc480.FreezeVideo(uc480.IS_WAIT) == uc480.IS_SUCCESS)
            {
                m_bIsStarted = false;
            }
        }

        /// <summary>
        /// Used at program shutdown to release all ressources allocated to camera
        /// </summary>
        public void ShutDown()
        {
            if (m_uc480.IsOpen())
                m_uc480.ExitCamera();
        }

        public void HandleMessage(int message, long lParam, long wParam)
        {
            switch (wParam)
            {
                case uc480.IS_FRAME:
                    if (!m_bDrawing)
                        DrawImage();
                    break;

                case uc480.IS_DEVICE_REMOVAL:
                case uc480.IS_NEW_DEVICE:
                    //UpdateInfos();
                    break;
            }
        }

        private void DrawImage()
        {
            m_bDrawing = true;
            // draw current memory if a camera is opened
            if (m_uc480.IsOpen())
            {
                int num = 0;
                IntPtr pMem = new IntPtr();
                IntPtr pLast = new IntPtr();
                m_uc480.GetActSeqBuf(ref num, ref pMem, ref pLast);
                if (pLast.ToInt32() == 0)
                {
                    m_bDrawing = false;
                    return;
                }

                int nLastID = GetImageID(pLast);
                int nLastNum = GetImageNum(pLast);
                m_uc480.LockSeqBuf(nLastNum, pLast);

                m_pCurMem = pLast;		// remember current buffer for our tootip ctrl

                int width = 0, height = 0, bitspp = 0, pitch = 0, bytespp = 0;
                m_uc480.InquireImageMem(m_pCurMem, GetImageID(m_pCurMem), ref width, ref height, ref bitspp, ref pitch);
                bytespp = (bitspp + 1) / 8;

                using (Bitmap bmp = new Bitmap(m_uc480.GetDisplayWidth(),
                                        m_uc480.GetDisplayHeight(),
                                        pitch,
                                        System.Drawing.Imaging.PixelFormat.Format32bppRgb,
                                        m_pCurMem))
                {
                    if (_captureBmp == null)
                    {
                        _captureBmp = new Bitmap(m_uc480.GetDisplayWidth(), m_uc480.GetDisplayHeight());
                    }
                    Graphics g = Graphics.FromImage(_captureBmp);
                    g.DrawImage(bmp, new Point(0, 0));
                    g.Dispose();

                    OnCameraCapture(_captureBmp);
                }

                

                //m_uc480.RenderBitmap(nLastID, (int)m_PictureBoxHwnd, m_RenderMode);

                m_uc480.UnlockSeqBuf(nLastNum, pLast);
            }
            m_bDrawing = false;
        }

        int GetImageID( IntPtr pBuffer )
		{
			// get image id for a given memory
			if ( !m_uc480.IsOpen() )
				return 0;

			int i = 0;
			for ( i=0; i<IMAGE_COUNT; i++)
				if ( m_Uc480Images[i].pMemory == pBuffer )
					return m_Uc480Images[i].MemID;
			return 0;
		}
		
		int GetImageNum( IntPtr pBuffer )
		{
			// get number of sequence for a given memory
			if ( !m_uc480.IsOpen() )
				return 0;

			int i = 0;
			for ( i=0; i<IMAGE_COUNT; i++)
				if ( m_Uc480Images[i].pMemory == pBuffer )
					return m_Uc480Images[i].nSeqNum;

			return 0;
		}

        protected virtual void OnCameraCapture(Bitmap b)
        {
            if (CameraCapture != null)
                CameraCapture(this, b);
        }

        public double getExposureTime()
        {
            return expTime;
        }

        public void setExposureTime(double expTime)
        {
            this.expTime = expTime;
        }

        public double getFps()
        {
            return fps;
        }

        public void setFps(double fps)
        {
            this.fps = fps;
        }

        Bitmap _captureBmp;
        private bool m_bIsStarted;
        private int	m_RenderMode;
        private IntPtr m_pCurMem;
        private uc480 m_uc480;
        private static int IMAGE_COUNT = 4;
        private long m_PictureBoxHwnd;
        private long m_Hwnd;
        bool m_bDrawing;
        private struct UC480IMAGE
        {
            public IntPtr pMemory;
            public int MemID;
            public int nSeqNum;
        }
        private UC480IMAGE[] m_Uc480Images;

        private double fps, expTime;
    }
}
