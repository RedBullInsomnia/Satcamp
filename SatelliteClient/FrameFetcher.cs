using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SatelliteClient
{
    /** Thread in charge of fetching the frames from the server */
    class FrameFetcher : SatelliteServer.BaseThread
    {
        private SatelliteServer.ISatService _satService; /** Service for capturing images */
        private PictureBox _pBox; /** The picture box in which must be displayed the image */

        /** Data and objects to compute the frame rate */
        private int _frameCnt;
        private MovingAverageDouble _frameRate;
        private System.Timers.Timer _frameRateTimer;

        /** Data and objects for saving frames on disk */
        private bool _saveNext;
        private string _savePath;

        public FrameFetcher(SatelliteServer.ISatService service, PictureBox pBox)
        {
            _satService = service;
            _saveNext = false;
            _savePath = "";
            _pBox = pBox;
            _frameCnt = 0;
            _frameRate = new MovingAverageDouble(5);
            _frameRateTimer = new System.Timers.Timer(1000);
            _frameRateTimer.Elapsed += new System.Timers.ElapsedEventHandler(computeFrameRate);
        }

        private void computeFrameRate(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (this)
            {
                _frameRate.push((double)_frameCnt);
                _frameCnt = 0;
            }
        }

        public double getFrameRate()
        {
            lock (this)
            {
                return _frameRate.get();
            }
        }

        public void setSaveNext()
        {
            _saveNext = true;
        }

        public void setSavePath(string path)
        {
            _savePath = path;
        }

        /**
         * While enabled request an image from the operation contract interface and store in a concurrent queue
         */
        protected override void work()
        {
            try
            {
                Console.WriteLine("Frame fetcher started");
                _frameRateTimer.Enabled = true;
                while (_go && IsAlive())
                {
                    byte[] buffer = _satService.Capture();
                    Bitmap image = new Bitmap(new MemoryStream(buffer));

                    /**
                     * The image saving was placed here mainly to keep code simple
                     */
                    if (_saveNext)
                    {
                        image.Save(_savePath + "/" + getFileName(), System.Drawing.Imaging.ImageFormat.Png);
                        _saveNext = false;
                    }

                    _pBox.Image = image;

                    lock (this) { ++_frameCnt; }
                }
                _frameRateTimer.Enabled = false;
            }
            catch (Exception e)
            {
                Console.Error.Write("Exception in Frame Fetcher : {0}\n", e.Message);
            }
        }

        private string getFileName()
        {
            return "capture_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff") + ".png";
        }
    }
}
