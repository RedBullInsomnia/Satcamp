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
        Bitmap image;
        bool _bConnected, connectedFlag, saveFlag;
        System.Timers.Timer captureTimer;
        System.Timers.Timer updateTimer;
        ChannelFactory<SatelliteServer.ISatService> _scf;
        SatelliteServer.ISatService satService;

        public Window()
        {
            InitializeComponent();
            captureTimer = new System.Timers.Timer(250);
            updateTimer = new System.Timers.Timer(50);
            updateTimer.Elapsed += _updateTimer_Elapsed;
            captureTimer.Elapsed += _captureTimer_Elapsed;
            _bConnected = false;
        }

        void _captureTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            captureTimer.Enabled = false;
            if (_bConnected)
            {
                this.Invoke(new Action(() => acquireFrame()));
                /*{
                    captureBn.Enabled = false;
                    byte[] buffer = satService.Capture();
                    //Console.Write("Received image with " + buffer.Length + " bytes.");
                    MemoryStream stream = new MemoryStream(buffer);
                    pictureBox.Image = new Bitmap(stream);
                    image = (Bitmap)pictureBox.Image;
                    captureBn.Enabled = true;
                }));*/
            }
            captureTimer.Enabled = true;
        }

        void _updateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            updateTimer.Enabled = false;
            this.Invoke(new Action(() =>
            {
                if (_bConnected)
                {
                    double[] euler = satService.GetEulerAngles();
                    tbRoll.Text = euler[0].ToString();
                    tbPitch.Text = euler[1].ToString();
                    tbYaw.Text = euler[2].ToString();

                    pitchTrackBar.Value = satService.GetServoPos(0);
                    yawTrackBar.Value = satService.GetServoPos(1);
                }
            }));
            updateTimer.Enabled = true;
        }

        private void captureBn_Click(object sender, EventArgs e)
        {
            captureTimer.Enabled = false;
            if (_bConnected)
            {
                captureBn.Enabled = false;
                byte[] buffer = satService.Capture();
                // Console.Write("Received image with " + buffer.Length + " bytes.");
                using (MemoryStream stream = new MemoryStream(buffer))
                {
                  pictureBox.Image = Bitmap.FromStream(stream);
                  //image = (Bitmap)pictureBox.Image;
                  captureBn.Enabled = true;
                }
            }
            else
            {
                MessageBox.Show("Please connect to the server first.");
            }
        }

        private void Window_Load(object sender, EventArgs e)
        {
            updateTimer.Start();
            ipTb.Text = Settings.Default["IP"].ToString();
        }

        private void connectBn_Click(object sender, EventArgs e)
        {
            Settings.Default["IP"] = ipTb.Text;
            try
            {
                // initialize the client
                NetTcpBinding binding = new NetTcpBinding();
                binding.MaxReceivedMessageSize = 20000000;
                binding.MaxBufferPoolSize = 20000000;
                binding.MaxBufferSize = 20000000;
                binding.Security.Mode = SecurityMode.None;
                _scf = new ChannelFactory<SatelliteServer.ISatService>(
                            binding,
                            "net.tcp://" + ipTb.Text + ":8000");
                //"net.tcp://192.168.1.137:8000");

                satService = _scf.CreateChannel();
            }
            catch (Exception ex)
            {
                connectBn.Enabled = true;
                MessageBox.Show("Failed to connect to server: " + ex.Message);
                _bConnected = false;
                return;
            }

            _bConnected = true;
            connectBn.Enabled = false;
        }

        private void Window_FormClosing(object sender, FormClosingEventArgs e)
        {
            updateTimer.Stop();
            Settings.Default.Save();
        }

        private void stabilizeCb_CheckedChanged(object sender, EventArgs e)
        {
            if (_bConnected)
            {
                this.BeginInvoke(new Action(() =>
                {
                    satService.SetStabilization(stabilizeCb.Checked);
                }));
            }
        }

        private void pitchTrackBar_Scroll(object sender, EventArgs e)
        {
            if (_bConnected)
            {
                this.BeginInvoke(new Action(() =>
                {
                    satService.SetServoPos(0, pitchTrackBar.Value);
                }));
            }
        }

        private void yawTrackBar_Scroll(object sender, EventArgs e)
        {
            if (_bConnected)
            {
                this.BeginInvoke(new Action(() =>
                {
                    satService.SetServoPos(1, yawTrackBar.Value);
                }));
            }
        }

        private void videoBn_Click(object sender, EventArgs e)
        {
            captureTimer.Enabled = true;
        }

        private void saveImageButton_Click(object sender, EventArgs e)
        {
          if(image != null){
            lock (image)
            {
              String picture_name = "satpic_" + DateTime.Now.ToString("yyyymmdd_HHmmss");
              image.Save("c:\\Users\\Satcamp1\\" + picture_name + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }
          } else
			      MessageBox.Show("You failed");
        }

        /**
           * Acquire a from from the remote server
           */
        private void acquireFrame()
        {
          try
          {
            byte[] buffer = satService.Capture();
            Console.WriteLine("Received image with " + buffer.Length + " bytes.");
            MemoryStream stream = new MemoryStream(buffer);

            lock (image)
            {
              image = (Bitmap)Bitmap.FromStream(stream);

              if (saveFlag)
              {
                saveImage(image);
                saveFlag = false;
              }

              pictureBox.Image = image;
            }
          }
          catch (System.ServiceModel.CommunicationException ex)
          {
            disconnect(ex.Message);
          }
          catch (System.TimeoutException ex)
          {
            disconnect(ex.Message);
          }
          catch (Exception ex)
          {
            disconnect("Ex capture : ### : " + ex.Message);
          }
        }

        /**
           * Save the image currently saved (image class variable) (must be different from null)
           * The accesses to the image must be locked
           */
        private void saveImage(Bitmap img)
        {
          String picture_name = "satpic_" + DateTime.Now.ToString("yyyymmdd_HHmmss");
          image.Save("D:\\Documents\\BitBucket\\Satcamp\\" + picture_name + ".png", System.Drawing.Imaging.ImageFormat.Png);
        }

        /**
         * Disconnect the client (stop timers and close the channel factory) and 
         * display a message in a message box
         */
        private void disconnect(String message)
        {
          MessageBox.Show("Client disconnected : " + message);

          disconnect();
        }

        /**
         * Disconnect the client : stop timers and close the channel factory
         */
        private void disconnect()
        {
          connectedFlag = false;
          saveFlag = false;

         // Console.WriteLine("Mode switched to NONE");
          //mode = Mode.NONE;

          image = null;

          // stop timers
          updateTimer.Enabled = false;
          captureTimer.Enabled = false;

          //if (scf != null)
           // scf.Abort();

          // update button status
          //setButtonsDisconnected();
        }
    }
}
