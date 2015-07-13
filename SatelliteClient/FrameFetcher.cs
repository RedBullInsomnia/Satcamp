using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;

namespace SatelliteClient
{
    class FrameFetcher : SatelliteServer.BaseThread
    {
        /**
         * The thread object that checks for data
         */
        private ConcurrentQueue<Bitmap> _frameQueue; // store the received frames
        private SatelliteServer.ISatService _satService; /** Service for capturing images */

        public FrameFetcher(SatelliteServer.ISatService service)
        {
            _frameQueue = new ConcurrentQueue<Bitmap>();
            _satService = service;
        }

        /**
         * Return the next (compared to the one returned from the last call) frame fetched from the server
         * Might block if no frame were received
         */
        public Bitmap getNextFrame() 
        {
            Bitmap bitmap;

            if (_frameQueue.TryDequeue(out bitmap))
                throw new Exception();
            else return bitmap;
        }

        /**
         * While enabled request an image from the operation contract interface and store in a concurrent queue
         */
        protected override void work()
        {
            try {
                while (_go && IsAlive())
                {
                    byte[] buffer = _satService.Capture();
                    _frameQueue.Enqueue(new Bitmap(new MemoryStream(buffer)));
                    Console.Write("Received image with " + buffer.Length + " bytes.");
                }
            } catch (Exception e) {
                Console.Error.Write("Exception in Frame Fetcher : {0}\n", e.Message);
            }
        }
    }
}
