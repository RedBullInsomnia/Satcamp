using System;
using System.Windows.Forms;
using System.Timers;
using System.ServiceModel;
using System.IO;

namespace SatelliteClient
{
    public partial class Window : Form
    {
        System.Timers.Timer _updateTimer; // for updating the various text box and labels of the screen

        /** Service objects */
        ChannelFactory<SatelliteServer.ISatService> _scf;
        SatelliteServer.ISatService _satService;
        private const string SERVICE_IP = "192.168.1.96";

        /** Threads running concurrently to the windows */
        private OrientationFetcher _orientation_fetcher; // fetches the angle data + control parameters
        private FrameFetcher _frame_fetcher; // fetched the frames 

        private const double _defKi = 0.0, _defKp = 0.0;
        private const double _defFps = 5.0, _defExpTime = 100.0;

        public Window()
        {
            InitializeComponent();
            kpTextBox.KeyDown += kpTextBox_KeyDown;
            kiTextBox.KeyDown += kiTextBox_KeyDown;
            savePathBox.KeyDown += savePathBox_KeyDown;
            expTimeTextBox.KeyDown += expTimeTextBox_KeyDown;
            fpsTextBox.KeyDown += fpsTextBox_KeyDown;

            _updateTimer = new System.Timers.Timer(50);
            _updateTimer.Elapsed += _updateTimer_Elapsed;
            serviceConnect();
            _orientation_fetcher = new OrientationFetcher(_satService);
            _frame_fetcher = new FrameFetcher(_satService, pictureBox);
        }

        private void serviceConnect()
        {
            if (_scf != null)
            {
                try { _scf.Close(); }
                catch (Exception) { }
            }
                

            NetTcpBinding binding = new NetTcpBinding();
            binding.MaxReceivedMessageSize = 20000000;
            binding.MaxBufferPoolSize = 20000000;
            binding.MaxBufferSize = 20000000;
            binding.Security.Mode = SecurityMode.None;
            Console.WriteLine("Init sat service");
            _scf = new ChannelFactory<SatelliteServer.ISatService>(
                        binding,
                        "net.tcp://" + SERVICE_IP + ":8000");
            _satService = _scf.CreateChannel();
            Console.WriteLine("Sat service ok");
        }

        void _updateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _updateTimer.Enabled = false;
            Invoke(new Action(() =>
            {
                tbRoll.Text = "" + Math.Round(_orientation_fetcher.GetRoll(), 1);
                tbPitch.Text = "" + Math.Round(_orientation_fetcher.GetPitch(), 1);
                tbYaw.Text = "" + Math.Round(_orientation_fetcher.GetYaw(), 1);

                servoYaw.Text = "" + _orientation_fetcher.GetServoYaw();
                servoPitch.Text = "" + _orientation_fetcher.GetServoPitch();

                stabilizeCb.Checked = _orientation_fetcher.GetStabilize();
                frameRateLabel.Text = "Frame rate : " + Math.Round(_frame_fetcher.getFrameRate(), 3) + " fps";
            }));
            _updateTimer.Enabled = true;
        }

        private void connectBn_Click(object sender, EventArgs e)
        {
            Invoke(new Action(() =>
            {
                // try to ping the server
                try
                {
                    _satService.NamedPing("Hello world !");
                }
                catch (Exception)
                {
                    disconnect();
                    MessageBox.Show("Error: impossible to ping the server");
                    Console.Error.WriteLine("Error: impossible to ping the server");
                    return;
                }

                connectBn.Enabled = false;
                disconnectBn.Enabled = true;
                stabilizeCb.Enabled = true; // to enable/disable the controller server side

                _orientation_fetcher.Start();
                _frame_fetcher.Start();

                _updateTimer.Enabled = true;

                // update position of the cursors on the track bar 
                pitchTrackBar.Value = _orientation_fetcher.GetServoPitch();
                yawTrackBar.Value = _orientation_fetcher.GetServoYaw();

                // if the save path is valid 
                checkSavePath(false);

                kiTextBox.Text = "" + Math.Round(_orientation_fetcher.GetKi(), 3);
                kpTextBox.Text = "" + Math.Round(_orientation_fetcher.GetKp(), 3);

                expTimeTextBox.Text = "" + _orientation_fetcher.GetExpTime();
                fpsTextBox.Text = "" + _orientation_fetcher.GetFps();
            }));
        }

        private void disconnectBn_Click(object sender, EventArgs e)
        {
            disconnect();
        }

        /**
         * Operation to execute when the user clicks on the Disconnect button
         * Stops the threads that are interacting with the server
         */
        private void disconnect()
        {
            _updateTimer.Enabled = false;

            if (_orientation_fetcher.IsAlive())
                _orientation_fetcher.Stop();

            if (_frame_fetcher.IsAlive())
                _frame_fetcher.Stop();

            _orientation_fetcher = new OrientationFetcher(_satService);
            _frame_fetcher = new FrameFetcher(_satService, pictureBox);

            if (_scf.State == CommunicationState.Faulted || serviceIsFaulted(_satService))
                serviceConnect();

            clearUI();
        }

        private bool serviceIsFaulted(SatelliteServer.ISatService service)
        {
            try
            {
                service.Ping();
                return false;
            }
            catch (Exception)
            {
                return true;
            }
        }

        /** 
         * Clear text fields and picture box in the ui
         */
        private void clearUI()
        {
            Invoke(new Action(() =>
            {
                yawTrackBar.Value = 6000;
                pitchTrackBar.Value = 6000;
                tbRoll.Clear();
                tbPitch.Clear();
                tbYaw.Clear();
                pictureBox.Image = null;
                frameRateLabel.Text = "";
                imageBn.Enabled = false;
                disconnectBn.Enabled = false;
                connectBn.Enabled = true;
                stabilizeCb.Checked = false;
                stabilizeCb.Enabled = false;
            }));
        }

        private void Window_Load(object sender, EventArgs e)
        {
            clearUI();
        }

        private void Window_FormClosing(object sender, FormClosingEventArgs e)
        {
            disconnect();
        }

        private void stabilizeCb_CheckedChanged(object sender, EventArgs e)
        {
            this.BeginInvoke(new Action(() =>
            {
                if (stabilizeCb.Checked)
                    _orientation_fetcher.SetStabilize();
                else
                    _orientation_fetcher.UnsetStabilize();
            }));
        }

        private void pitchTrackBar_Scroll(object sender, EventArgs e)
        {
            this.BeginInvoke(new Action(() =>
            {
                _orientation_fetcher.SetServoPitch(pitchTrackBar.Value);
            }));
        }

        private void yawTrackBar_Scroll(object sender, EventArgs e)
        {
            this.BeginInvoke(new Action(() =>
            {
                _orientation_fetcher.SetServoYaw(yawTrackBar.Value);
            }));
        }

        private void saveImageButton_Click(object sender, EventArgs e)
        {
            _frame_fetcher.setSaveNext();
        }

        private void savePathBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                checkSavePath(true);
                this.ActiveControl = null;
            }
        }

        /** Check whether the save path is valid. If so, activate the save button and init the frame fetcher object */
        private void checkSavePath(bool showMsgBox)
        {
            if (!folderIsWritable(savePathBox.Text))
            {
                if (showMsgBox)
                    MessageBox.Show("Invalid save path");
                return;
            }

            // replace reverse backslashes by backslashes
            string path = savePathBox.Text.Replace("\\\\", "/");
            path = path.Replace("\\", "/");
            // remove last slash
            if (path.LastIndexOf("/") == (path.Length - 1))
                path = path.Remove(path.Length - 1);

            savePathBox.Text = path;

            _frame_fetcher.setSavePath(savePathBox.Text);
            imageBn.Enabled = isUIConnected();
        }

        /** Check whether the UI is in connected state */
        private bool isUIConnected()
        {
            return !connectBn.Enabled;
        }

        /** Check whether the given folder path is writable */
        private bool folderIsWritable(string folderPath)
        {
            try
            {
                // Attempt to get a list of security permissions from the folder. 
                // This will raise an exception if the path is read only or do not have access to view the permissions. 
                System.Security.AccessControl.DirectorySecurity ds = Directory.GetAccessControl(folderPath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void kpTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Invoke(new Action(() =>
                {
                    commitKParam(kpTextBox, false);
                }));
            }
        }

        private void kiTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Invoke(new Action(() =>
                {
                    commitKParam(kiTextBox, true);
                }));
            }
        }

        private void commitKParam(TextBox paramTextBox, bool ki)
        {
            try
            {
                setKParam(ReadDouble(paramTextBox, 0.0, 1.0), ki);
                this.ActiveControl = null;
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid " + (ki ? "Ki" : "Kp") + " : valid floating point number in [0.0, 1.0] expected");
                paramTextBox.Text = "" + getKParam(ki);
            }
        }

        private double getKParam(bool ki)
        {
            return ki ? _orientation_fetcher.GetKi() : _orientation_fetcher.GetKp();
        }

        private void setKParam(double value, bool ki)
        {
            if (ki)
                _orientation_fetcher.SetKi(value);
            else
                _orientation_fetcher.SetKp(value);
        }

        private void defaultKParamsBn_Click(object sender, EventArgs e)
        {
            Invoke(new Action(() => {
                kiTextBox.Text = "" + _defKi;
                kpTextBox.Text = "" + _defKp;
            }));
        }

        private void fpsTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter) 
            {
                Invoke(new Action(() => {
                    try 
                    {
                        _orientation_fetcher.SetFps(ReadDouble(fpsTextBox, 3.0, 20.0));
                    } 
                    catch (Exception) 
                    {
                        MessageBox.Show("Invalid frame rate : floating point number in [3.0,20.0] expected");
                    }
                }));
            }
        }

        private void expTimeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter) 
            {
                Invoke(new Action(() => {
                    try 
                    {
                        _orientation_fetcher.SetExpTime(ReadDouble(expTimeTextBox, 1.0, 220.0));
                    } 
                    catch (Exception) 
                    {
                        MessageBox.Show("Invalid exposure time : floating point number in [1.0, 220.0] expected");
                    }                 
                }));
            }
        }

        private void defCamParams_Click(object sender, EventArgs e)
        {
            Invoke(new Action(() => {
                fpsTextBox.Text = "" + _defFps;
                expTimeTextBox.Text = "" + _defExpTime;
            }));
        }

        /** Read a double value from a text box and update it with the correct format 
            An excepion is thrown if the double cannot be extracted */
        private static double ReadDouble(TextBox box, double low, double high)
        {
            string strVal = box.Text.Replace(".", ",");
            double val = Double.Parse(strVal);

            if(val < low || val > high)
                throw new Exception();

            box.Text = "" + val;
            return val;
        }
    }
}
