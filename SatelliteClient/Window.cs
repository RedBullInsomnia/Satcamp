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
    enum Mode // Client program mode
    {
        MODE_VIDEO, MODE_CAPTURE, NONE
    }

    public partial class Window : Form
    {
        bool connectedFlag, // true if connected to remote
             saveFlag; // true if the user has requested to save the image

        Mode mode; // current mode of the client

        Bitmap image; // save the current capture

        System.Timers.Timer captureTimer; // acquisition frequency for video -> 250ms
        System.Timers.Timer updateTimer; // update the position information in GUI -> 50 ms

        ChannelFactory<SatelliteServer.ISatService> scf; // channel for getting sat service
        SatelliteServer.ISatService satService; // service got from the channel

        public Window()
        {
            InitializeComponent();

            // init timers
            captureTimer = new System.Timers.Timer(250); 
            updateTimer = new System.Timers.Timer(50); 
            updateTimer.Elapsed += updateTimer_Elapsed;
            captureTimer.Elapsed += captureTimer_Elapsed;

            disconnect(); // to reset all class variables
        }

        void captureTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            captureTimer.Enabled = false;
            if (connectedFlag)
            {
                if(mode != Mode.MODE_VIDEO)
                    setModeVideo();

                this.Invoke(new Action(() => acquireFrame()));
            }
            captureTimer.Enabled = true;
        }

        void updateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (connectedFlag)
            {
                updateTimer.Enabled = false;
                this.Invoke(new Action(() => updateOrientation()));
                updateTimer.Enabled = true;
            }  
        }

        private void captureButton_Click(object sender, EventArgs e)
        {
            if (connectedFlag)
            {
                if(mode != Mode.MODE_CAPTURE)
                    setModeCapture();

                this.Invoke(new Action(() => acquireFrame()));
            }
            else
            {
                MessageBox.Show("Please connect to the server first.");
            }
        }

        private void Window_Load(object sender, EventArgs e)
        {
            Console.WriteLine("Window_Load");
            updateTimer.Start();
            ipTb.Text = Settings.Default["IP"].ToString();
        }

        private void connectBn_Click(object sender, EventArgs e)
        {
            connect();
        }

        private void Window_FormClosing(object sender, FormClosingEventArgs e)
        {
            Console.WriteLine("Window_FormClosing");
            updateTimer.Stop();
            Settings.Default.Save();
        }

        private void stabilizeCb_CheckedChanged(object sender, EventArgs e)
        {
            if (connectedFlag)
            {
                this.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        satService.SetStabilization(stabilizeCb.Checked);
                    }
                    catch (System.ServiceModel.CommunicationException ex)
                    {
                        disconnect(ex.Message);
                    }
                    catch (System.TimeoutException ex)
                    {
                        disconnect(ex.Message);
                    }
                }));
            }
            else
            {
                MessageBox.Show("Please connect to the server first.");
            }
        }

        private void pitchTrackBar_Scroll(object sender, EventArgs e)
        {
            if (connectedFlag)
            {
                this.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        satService.SetServoPos(0, pitchTrackBar.Value);
                    }
                    catch (System.ServiceModel.CommunicationException ex)
                    {
                        disconnect(ex.Message);
                    }
                    catch (System.TimeoutException ex)
                    {
                        disconnect(ex.Message);
                    }
                }));
            }
            else
            {
                MessageBox.Show("Please connect to the server first.");
            }
        }

        private void yawTrackBar_Scroll(object sender, EventArgs e)
        {
            if (connectedFlag)
            {
                this.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        satService.SetServoPos(1, yawTrackBar.Value);
                    }
                    catch (System.ServiceModel.CommunicationException ex)
                    {
                        disconnect(ex.Message);
                    }
                    catch (System.TimeoutException ex)
                    {
                        disconnect(ex.Message);
                    }
                }));
            }
            else
                MessageBox.Show("Please connect to the server first.");
        }
        

        private void videoButton_Click(object sender, EventArgs e)
        {
            captureTimer.Enabled = true;
        }

        private void saveImageButton_Click(object sender, EventArgs e)
        {
            if(mode == Mode.MODE_VIDEO)
                saveFlag = true;
            else if(image != null)
                saveImage(image);
            else
                MessageBox.Show("You must have acquired some frames before trying to save them.");
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
            connectedFlag = false;
            saveFlag = false;

            Console.WriteLine("Mode switched to NONE");
            mode = Mode.NONE;

            image = null;

            // stop timers
            updateTimer.Enabled = false;
            captureTimer.Enabled = false;

            if(scf != null) 
                scf.Close();

            // update button status
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


        /**
         * Connect the client to the remote server (using the ip in the text field)
         */
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
                
                scf = new ChannelFactory<SatelliteServer.ISatService>(binding, "net.tcp://" + ipTb.Text + ":8000");
                satService = scf.CreateChannel();

                connectedFlag = true;

                setButtonsConnected();                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to connect to server: " + ex.Message);
                disconnect();
            }
        }

        /**
         * Update orientation information from remote server
         */
        private void updateOrientation()
        {
            try
            {
                double[] euler = satService.GetEulerAngles();

                tbRoll.Text = euler[0].ToString();
                tbPitch.Text = euler[1].ToString();
                tbYaw.Text = euler[2].ToString();

                pitchTrackBar.Value = satService.GetServoPos(0);
                yawTrackBar.Value = satService.GetServoPos(1);
            }
            catch (System.ServiceModel.CommunicationException ex)
            {
                disconnect(ex.Message);
            }
            catch (System.TimeoutException ex)
            {
                disconnect(ex.Message);
            }
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

                image = (Bitmap) Bitmap.FromStream(stream);

                if(saveFlag)
                    saveImage(image);

                pictureBox.Image = image;
            }
            catch (System.ServiceModel.CommunicationException ex)
            {
                disconnect(ex.Message);
            }
            catch (System.TimeoutException ex)
            {
                disconnect(ex.Message);
            }
        }

        private void saveImage(Bitmap img)
        {
            String picture_name = "satpic_" + DateTime.Now.ToString("yyyymmdd_HHmmss");
            image.Save("D:\\Documents\\BitBucket\\Satcamp\\" + picture_name + ".png", System.Drawing.Imaging.ImageFormat.Png);
        }

        /**
         * Switch to capture mode
         */
        private void setModeCapture()
        {
            Console.WriteLine("Mode switched to MODE_CAPTURE");
            captureTimer.Enabled = false; // stop synchronous image acquisition
            mode = Mode.MODE_CAPTURE;
            saveFlag = false;
        }

        /**
         * Switch to video mode
         */
        private void setModeVideo()
        {
            Console.WriteLine("Mode switched to MODE_VIDEO");
            captureTimer.Enabled = true; // start synchronous image acquisition
            mode = Mode.MODE_VIDEO;
        }
    }
}
