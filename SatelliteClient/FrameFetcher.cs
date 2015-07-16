using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SatelliteClient
{
    class FrameFetcher : SatelliteServer.BaseThread
    {
        /**
         * The thread object that checks for data
         */
        private SatelliteServer.ISatService _satService; /** Service for capturing images */
        private PictureBox _pBox;
        private int _frameCnt;
        private MovingAverageDouble _frameRate;
        private System.Timers.Timer _frameRateTimer;

        public FrameFetcher(SatelliteServer.ISatService service, PictureBox pBox)
        {
            _satService = service;
            _pBox = pBox;
            _frameCnt = 0;
            _frameRate = new MovingAverageDouble(5);
            _frameRateTimer = new System.Timers.Timer(1000);
            _frameRateTimer.Elapsed += new System.Timers.ElapsedEventHandler(computeFrameRate);
        }

        private void computeFrameRate(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (this) {
                _frameRate.push((double) _frameCnt); 
                _frameCnt = 0;
            }
        }

        public double getFrameRate()
        {
            lock (this) {
                return _frameRate.get();
            }
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
                    _pBox.Image = new Bitmap(new MemoryStream(buffer));

                    lock(this) { ++_frameCnt; }
                }
                _frameRateTimer.Enabled = false;
            } catch (Exception e) {
                Console.Error.Write("Exception in Frame Fetcher : {0}\n", e.Message);
            }
        }
    }
}
