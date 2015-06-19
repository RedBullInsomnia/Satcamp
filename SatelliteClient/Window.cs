using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.ServiceModel;
using SatelliteClient.Properties;
using System.IO;

namespace SatelliteClient
{
    public partial class Window : Form
    {
        System.Timers.Timer _updateTimer;
        ChannelFactory<SatelliteServer.ISatService> _scf;
        SatelliteServer.ISatService _satService;
        private OrientationFetcher _orientation_fetcher;
        private FrameFetcher _frame_fetcher;

        /**
         *
         */
        public Window()
        {
            InitializeComponent();
            _updateTimer = new System.Timers.Timer(50);
            _updateTimer.Elapsed += _updateTimer_Elapsed;
            InitFactory("192.168.1.96"); // ip is fixed on the satellite to this one 
            _satService = _scf.CreateChannel();
            _orientation_fetcher = new OrientationFetcher(_satService);
            _frame_fetcher = new FrameFetcher(_satService);
        }

        /**
         * Initialize the channel factory
         */ 
        private void InitFactory(string ip) 
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.MaxReceivedMessageSize = 20000000;
            binding.MaxBufferPoolSize = 20000000;
            binding.MaxBufferSize = 20000000;
            binding.Security.Mode = SecurityMode.None;
            _scf = new ChannelFactory<SatelliteServer.ISatService>(
                        binding,
                        "net.tcp://" + ip + ":8000");
        }

        //void _captureTimer_Elapsed(object sender, ElapsedEventArgs e)
        //{
        //    _captureTimer.Enabled = false;
        //    if (_bConnected)
        //    {
        //        this.Invoke(new Action(() =>
        //        {
        //            captureBn.Enabled = false;
        //            byte[] buffer = _satService.Capture();
        //            Console.Write("Received image with " + buffer.Length + " bytes.");
        //            MemoryStream stream = new MemoryStream(buffer);
        //            pictureBox.Image = new Bitmap(stream);
        //            //image = (Bitmap)pictureBox.Image;
        //            captureBn.Enabled = true;
        //        }));
        //    }
        //    _captureTimer.Enabled = true;
        //}

        void _updateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _updateTimer.Enabled = false;
            Invoke(new Action(() => {
                tbRoll.Text = _orientation_fetcher.GetRoll().ToString();
                tbPitch.Text = _orientation_fetcher.GetPitch().ToString();
                tbYaw.Text = _orientation_fetcher.GetYaw().ToString();

                pitchTrackBar.Value = _orientation_fetcher.GetServoPitch();
                yawTrackBar.Value = _orientation_fetcher.GetServoYaw();
            }));
            _updateTimer.Enabled = true;
        }

        private void captureBn_Click(object sender, EventArgs e)
        {
                captureBn.Enabled = false;
                byte[] buffer = _satService.Capture();
                Console.Write("Received image with " + buffer.Length + " bytes.");
                using (MemoryStream stream = new MemoryStream(buffer))
                {
                  pictureBox.Image = Bitmap.FromStream(stream);
                  //image = (Bitmap)pictureBox.Image;
                  //pictureBox.Image.Save("c:\\picture.png", System.Drawing.Imaging.ImageFormat.Png);
                  captureBn.Enabled = true;
                }
   
        }

        private void connectBn_Click(object sender, EventArgs e)
        {

        }

        private void disconnectBn_Click(object sender, EventArgs e)
        {

        }

        private void Window_Load(object sender, EventArgs e)
        {
            _orientation_fetcher.Start();
            _frame_fetcher.Start();
            _updateTimer.Start();
        }

        private void Window_FormClosing(object sender, FormClosingEventArgs e)
        {
            _updateTimer.Stop();
            _frame_fetcher.Stop();
            _updateTimer.Stop();
        }

        private void stabilizeCb_CheckedChanged(object sender, EventArgs e)
        {
                //this.BeginInvoke(new Action(() =>
                //{
                //    _satService.SetStabilization(stabilizeCb.Checked);
                //}));
        }

        private void pitchTrackBar_Scroll(object sender, EventArgs e)
        {
                /*this.BeginInvoke(new Action(() =>
                {
                    _satService.SetServoPos(0, pitchTrackBar.Value);
                }));*/
            
        }

        private void yawTrackBar_Scroll(object sender, EventArgs e)
        {
                /*this.BeginInvoke(new Action(() =>
                {
                    _satService.SetServoPos(1, yawTrackBar.Value);
                }));*/
            
        }

        private void videoBn_Click(object sender, EventArgs e)
        {
        }

        private void saveImageButton_Click(object sender, EventArgs e)
        {
          //image.Save("c:\\picture.png", System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}
