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
using System.Net;
using System.Net.Sockets;
using MathNet.Numerics.LinearAlgebra.Double;

namespace SatelliteServer
{
  public partial class Window : Form
  {
    // For stabilization
    Double[] Stab_angles = new double[3];
    double Kp = 5;
    double Ki = 0;
    double perr_int = 0; //integral of pitch error
    double yerr_int = 0; //integral of yaw error
    int _stabPitchServo, _stabYawServo;
    bool useCustomPid;

    // Everything else
    Um6Driver _um6Driver;
    CameraDriver _cameraDriver;
    ServoDriver _servoDriver;
    SatService _service;
    ServiceHost _host;
    ushort _lastPitchVal;
    ushort _lastYawVal;
    System.Timers.Timer _updateTimer;

    // A delta of 11 on the trackbar means a delta of 1° for the camera
    const double PitchAngleCoefficient = 11.11;

    public Window()
    {
      InitializeComponent();
      _um6Driver = new Um6Driver("COM1", 115200);
      _um6Driver.Init();
      _servoDriver = new ServoDriver();
      _updateTimer = new System.Timers.Timer(20); // The system will update every 20 ms
      _updateTimer.Elapsed += _updateTimer_Elapsed;
      _cameraDriver = new CameraDriver(this.Handle.ToInt64());
      _cameraDriver.Init(pictureBox.Handle.ToInt64());
      _cameraDriver.CameraCapture += _cameraDriver_CameraCapture;

      // initialize the service
      //_host = new ServiceHost(typeof(SatService));
      NetTcpBinding binding = new NetTcpBinding();
      binding.MaxReceivedMessageSize = 20000000;
      binding.MaxBufferPoolSize = 20000000;
      binding.MaxBufferSize = 20000000;
      binding.Security.Mode = SecurityMode.None;
      _service = new SatService(_cameraDriver);
      _host = new ServiceHost(_service);
      _host.AddServiceEndpoint(typeof(ISatService),
                             binding,
                             "net.tcp://localhost:8000");
      _host.Open();

      // Initial positions
      _stabPitchServo = 4000;
      _stabYawServo = 6000;

      // Initial pid parameters
      kiText.Text = Ki.ToString();
      kpText.Text = Kp.ToString();
    }

    void _updateTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      _updateTimer.Enabled = false;
      this.Invoke(new Action(() =>
      {
        if (_um6Driver != null)
        {
          // Update Roll, pitch and yaw values in interface
          tbRoll.Text = _um6Driver.Angles[0].ToString("F1");
          tbPitch.Text = _um6Driver.Angles[1].ToString("F1"); // F1 parameter specifies precision of 1 decimal
          tbYaw.Text = _um6Driver.Angles[2].ToString("F1"); 
          _service._eulerAngles = new double[3] { _um6Driver.Angles[0], _um6Driver.Angles[1], _um6Driver.Angles[2] };
        }

        // Update only if needed and then reset
        if (_service._servoChanged[0] == true)
        {
          pitchTrackBar.Value = _service._servoPos[0];
          _service._servoChanged[0] = false;
        }

        // Same here
        if (_service._servoChanged[1] == true)
        {
          yawTrackBar.Value = _service._servoPos[1];
          _service._servoChanged[1] = false;
        }

        // Check is stabilisation is wanted
        if (_service._bStabilizationChanged)
        {
          stabilizeCb.Checked = _service._bStabilizationActive;
          _service._bStabilizationChanged = false;
        }
        _service._bStabilizationActive = stabilizeCb.Checked;

        if (_servoDriver != null)
        {
          if (pitchTrackBar.Value != _lastPitchVal)
          {
            // if the trackbar value changed, we update the servo position
            _servoDriver.SetServo((Byte)1, (ushort)pitchTrackBar.Value);
            _lastPitchVal = (ushort)pitchTrackBar.Value;
            _service._servoPos[0] = pitchTrackBar.Value;
          }

          if (yawTrackBar.Value != _lastYawVal)
          {
            // same as above
            _servoDriver.SetServo((Byte)0, (ushort)yawTrackBar.Value);
            _lastYawVal = (ushort)yawTrackBar.Value;
            _service._servoPos[1] = yawTrackBar.Value;
          }
        }

        // do stabilization if necessary
        if (stabilizeCb.Checked)
        {
          if (true == useCustomPid)
          {
            customPid();
          }
          else
          {
            //Calculate the error on the pitch axis
            double pitch_error = _um6Driver.Angles[1] - Stab_angles[1]; //- _stabPitchServo;
            perr_int += pitch_error;
            perrText.Text = perr_int.ToString("F2"); // Print integral to interface
            int tmp_pitchtrackbarvalue = pitchTrackBar.Value + (int)(pitch_error * Kp * PitchAngleCoefficient)
                                                             + (int)(perr_int * Ki * PitchAngleCoefficient);
            // Clamp pitch servo
            pitchTrackBar.Value = clamp(tmp_pitchtrackbarvalue, pitchTrackBar.Maximum, pitchTrackBar.Minimum);

            // Calculate the error on the yaw axis
            double yaw_error = _um6Driver.Angles[2] - Stab_angles[2]; // -_stabYawServo;
            yerr_int += yaw_error;
            yerrorText.Text = yerr_int.ToString("F2"); // Print integral to interface
            int tmp_yawtrackbarvalue = yawTrackBar.Value + (int)(yaw_error * Kp * PitchAngleCoefficient)
                                                         + (int)(yerr_int * Ki * PitchAngleCoefficient);

            // Clamp yaw servo
            yawTrackBar.Value = clamp(tmp_yawtrackbarvalue, yawTrackBar.Maximum, yawTrackBar.Minimum);
          }
        }
      }));
      _updateTimer.Enabled = true;
    }

    void customPid()
    {
      //Calculate the error on the pitch axis
      double pitch_error = _um6Driver.Angles[1] - Stab_angles[1];
      int tmp_pitchtrackbarvalue = _stabPitchServo + (int)(pitch_error * Kp * PitchAngleCoefficient)
                                                       + (int)(perr_int * Ki * PitchAngleCoefficient);
      // Clamp pitch servo
      pitchTrackBar.Value = clamp(tmp_pitchtrackbarvalue, pitchTrackBar.Maximum, pitchTrackBar.Minimum);

      // Calculate the error on the yaw axis
      double yaw_error = _um6Driver.Angles[2] - Stab_angles[2]; 
      int tmp_yawtrackbarvalue = _stabYawServo + (int)(yaw_error * Kp * PitchAngleCoefficient)
                                                   + (int)(yerr_int * Ki * PitchAngleCoefficient);
      // Clamp yaw servo
      yawTrackBar.Value = clamp(tmp_yawtrackbarvalue, yawTrackBar.Maximum, yawTrackBar.Minimum);
    }

    private int clamp(int a, int max, int min)
    {
      if (a < min)
        a = min;
      
      if (a > max)
        a = max;

      return a;
    }

    private void captureBn_Click(object sender, EventArgs e)
    {
      _cameraDriver.StartVideo();
    }

    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    protected override void WndProc(ref Message m)
    {
      // Listen for operating system messages
      if (_cameraDriver != null)
      {
        switch (m.Msg)
        {
          case uc480.IS_UC480_MESSAGE:
            _cameraDriver.HandleMessage(m.Msg, m.LParam.ToInt64(), m.WParam.ToInt32());
            break;
        }

      }
      base.WndProc(ref m);
    }

    void _cameraDriver_CameraCapture(object sender, Bitmap b)
    {
      pictureBox.Image = b;
    }

    private void Window_Load(object sender, EventArgs e)
    {
      if (_updateTimer != null)
      {
        _updateTimer.Start();
      }
      GetIpAddress();
    }

    private void Window_FormClosing(object sender, FormClosingEventArgs e)
    {
      _updateTimer.Stop();
      _cameraDriver.StopVideo();
      //TO DO : find a way to relax servos when program ends
    }

    public void GetIpAddress()
    {
      IPHostEntry host;
      string localIP = "?";
      host = Dns.GetHostEntry(Dns.GetHostName());
      foreach (IPAddress ip in host.AddressList)
      {
        if (ip.AddressFamily == AddressFamily.InterNetwork)
        {
          localIP = ip.ToString();
        }
      }
      ipLabel.Text = "IP Address: " + localIP;
    }

    private void stabilizeCb_CheckedChanged(object sender, EventArgs e)
    {
      if (stabilizeCb.Checked == true)
      {
        // Parse pid parameters Ki and Kp
        Ki = float.Parse(kiText.Text);
        Kp = float.Parse(kpText.Text);

        // reset error integrals
        perr_int = 0;
        yerr_int = 0;

        // save desired angles
        Stab_angles[0] = _um6Driver.Angles[0]; //Roll
        Stab_angles[1] = _um6Driver.Angles[1]; //Pitch
        Stab_angles[2] = _um6Driver.Angles[2]; //Yaw

        // Print saved angles
        stabPitch.Text = Stab_angles[1].ToString("F1"); //Precision of 1 decimal
        stabYaw.Text = Stab_angles[2].ToString("F1");
        stabRoll.Text = Stab_angles[0].ToString("F1");

        //Save values of trackbars
        _stabPitchServo = pitchTrackBar.Value;
        _stabYawServo = yawTrackBar.Value;
      }
    }

    private void kpText_TextChanged(object sender, EventArgs e)
    {
      Kp = float.Parse(kpText.Text);
    }

    private void kiText_TextChanged(object sender, EventArgs e)
    {
      Ki = float.Parse(kiText.Text);
    }

    private void pidCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      useCustomPid = customPidCheckBox.Checked;
      kiText.Enabled = !customPidCheckBox.Checked;
      kpText.Enabled = !customPidCheckBox.Checked;
    }

    private void pitchTrackBar_Scroll(object sender, EventArgs e)
    {
      numericPitch.Value = pitchTrackBar.Value;
    }

    private void numericUpDown1_ValueChanged(object sender, EventArgs e)
    {
      pitchTrackBar.Value = (int)numericPitch.Value;
    }

    private void numericYaw_ValueChanged(object sender, EventArgs e)
    {
      yawTrackBar.Value = (int)numericYaw.Value;
    }

    private void yawTrackBar_Scroll(object sender, EventArgs e)
    {
      numericYaw.Value = yawTrackBar.Value;
    }
  }
}
