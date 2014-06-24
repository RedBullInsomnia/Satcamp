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
        bool _bConnected; // true if connected to remote
        System.Timers.Timer _captureTimer; // capture the current image
        System.Timers.Timer _updateTimer; // update the position information in GUI
        ChannelFactory<SatelliteServer.ISatService> _scf; // channel for getting sat service
        SatelliteServer.ISatService _satService; // service got from the channel   

        public Window()
        {
            InitializeComponent();
            _captureTimer = new System.Timers.Timer(250);
            _updateTimer = new System.Timers.Timer(50);
            _updateTimer.Elapsed += _updateTimer_Elapsed;
            _captureTimer.Elapsed += _captureTimer_Elapsed;
            _bConnected = false;

            setButtonsDisconnected();
        }

        void _captureTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //Console.WriteLine("_captureTimer_Elapsed");
            _captureTimer.Enabled = false;
            if (_bConnected)
            {
                this.Invoke(new Action(() =>
                {
                    try
                    {
                        captureButton.Enabled = false;
                        byte[] buffer = _satService.Capture();
                        Console.WriteLine("Received image with " + buffer.Length + " bytes.");
                        MemoryStream stream = new MemoryStream(buffer);
                        pictureBox.Image = new Bitmap(stream);
                        //image = (Bitmap)pictureBox.Image;
                        captureButton.Enabled = true;
                    }
                    catch (System.ServiceModel.CommunicationException excep)
                    {
                        disconnect( excep.Message);
                    }
                }));
            }
            _captureTimer.Enabled = true;
        }

        void _updateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //Console.WriteLine("_updateTimer_Elapsed");
            _updateTimer.Enabled = false;

            if (_bConnected)
            {
                this.Invoke(new Action(() =>
                {
                    try
                    {
                        double[] euler = _satService.GetEulerAngles();
                        tbRoll.Text = euler[0].ToString();
                        tbPitch.Text = euler[1].ToString();
                        tbYaw.Text = euler[2].ToString();

                        pitchTrackBar.Value = _satService.GetServoPos(0);
                        yawTrackBar.Value = _satService.GetServoPos(1);
                    }
                    catch (System.ServiceModel.CommunicationException excep)
                    {
                        disconnect( excep.Message);
                    }

                }));
            }

            _updateTimer.Enabled = true;
        }

        private void captureButton_Click(object sender, EventArgs e)
        {
            Console.WriteLine("captureButton_Click");
            _captureTimer.Enabled = false;
            if (_bConnected)
            {
                this.Invoke(new Action(() =>
                {
                    try
                    {
                        captureButton.Enabled = false;
                        byte[] buffer = _satService.Capture();
                        Console.WriteLine("Received image with " + buffer.Length + " bytes.");
                        using (MemoryStream stream = new MemoryStream(buffer))
                        {
                            pictureBox.Image = Bitmap.FromStream(stream);
                            //image = (Bitmap)pictureBox.Image;
                            //pictureBox.Image.Save("c:\\picture.png", System.Drawing.Imaging.ImageFormat.Png);
                            captureButton.Enabled = true;
                        }
                    }
                    catch (System.ServiceModel.CommunicationException excep)
                    {
                        disconnect( excep.Message);
                    }
                }));
            }
            else
            {
                MessageBox.Show("Please connect to the server first.");
            }
        }

        private void Window_Load(object sender, EventArgs e)
        {
            Console.WriteLine("Window_Load");
            _updateTimer.Start();
            ipTb.Text = Settings.Default["IP"].ToString();
        }

        private void connectBn_Click(object sender, EventArgs e)
        {
            Settings.Default["IP"] = ipTb.Text;
            connect();
        }

        private void Window_FormClosing(object sender, FormClosingEventArgs e)
        {
            Console.WriteLine("Window_FormClosing");
            _updateTimer.Stop();
            Settings.Default.Save();
        }

        private void stabilizeCb_CheckedChanged(object sender, EventArgs e)
        {
            if (_bConnected)
            {
                this.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        _satService.SetStabilization(stabilizeCb.Checked);
                    }
                    catch (System.ServiceModel.CommunicationException excep)
                    {
                        disconnect( excep.Message);
                    }
                }));
            }
        }

        private void pitchTrackBar_Scroll(object sender, EventArgs e)
        {
            if (_bConnected)
            {
                this.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        _satService.SetServoPos(0, pitchTrackBar.Value);
                    }
                    catch (System.ServiceModel.CommunicationException excep)
                    {
                        disconnect( excep.Message);
                    }
                }));
            }
        }

        private void yawTrackBar_Scroll(object sender, EventArgs e)
        {
            if (_bConnected)
            {
                this.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        _satService.SetServoPos(1, yawTrackBar.Value);
                    }
                    catch (System.ServiceModel.CommunicationException excep)
                    {
                        disconnect( excep.Message);
                    }
                }));
            }
        }

        private void videoButton_Click(object sender, EventArgs e)
        {
            Console.WriteLine("videoButton_Click");
            _captureTimer.Enabled = true;
        }

        private void saveImageButton_Click(object sender, EventArgs e)
        {
            Console.WriteLine("saveImageButton_Click");
            if(image != null)
                image.Save("c:\\picture.png", System.Drawing.Imaging.ImageFormat.Png);
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            disconnect();
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
            _updateTimer.Enabled = false;
            _captureTimer.Enabled = false;
            _bConnected = false;
            _scf.Close();
            setButtonsDisconnected();
        }

        /**
         * Activate the connect button and deactivate all the others 
         */
        private void setButtonsDisconnected()
        {
            connectBn.Enabled = true;
            saveImageButton.Enabled = false;
            videoButton.Enabled = false;
            captureButton.Enabled = false;
            disconnectButton.Enabled = false;
        }

        /**
         * Deactivate the connect button and activate all the others 
         */
        private void setButtonsConnected()
        {
            connectBn.Enabled = false;
            saveImageButton.Enabled = true;
            videoButton.Enabled = true;
            captureButton.Enabled = true;
            disconnectButton.Enabled = true;
        }

        private void connect()
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
                
                _scf = new ChannelFactory<SatelliteServer.ISatService>(binding, "net.tcp://" + ipTb.Text + ":8000");
                _satService = _scf.CreateChannel();

                _bConnected = true;

                setButtonsConnected();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to connect to server: " + ex.Message);
                disconnect();
            }
        }
    }
}
