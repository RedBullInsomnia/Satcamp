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
    private Server _server;

    public Window()
    {
        InitializeComponent();
        _server = new Server(this.Handle.ToInt64(), pictureBox, false); // ideally, set to false in production to avoid lost of time due to image copying
    }

    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    protected override void WndProc(ref Message m)
    {
        // Listen for operating system messages
        switch (m.Msg)
        {
            case uc480.IS_UC480_MESSAGE:
                _server.passMessage(ref m);
                break;
        }
        base.WndProc(ref m);
    }

    private void Window_Load(object sender, EventArgs e)
    {
        _server.Start();
        Logger.instance().setTextBox(this.richTextBox1);
    }

    private void Window_FormClosing(object sender, FormClosingEventArgs e)
    {
        _server.Stop();
        for(int i = 0; i < 6; ++i) {
            System.Threading.Thread.Sleep(1000);
            Logger.instance().log("server stopping in " + (5 - i) + " second(s)...");
        }
    }

    private void richTextBox1_TextChanged(object sender, EventArgs e)
    {

    }

    private void displayCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        Invoke(new Action(() => {
            _server.SetPrintInBox(displayCheckBox.Checked);
        }));
    }
  }
}
