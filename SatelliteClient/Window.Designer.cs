namespace SatelliteClient
{
    partial class Window
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Window));
            this.groupBoxOrientation = new System.Windows.Forms.GroupBox();
            this.tbYaw = new System.Windows.Forms.TextBox();
            this.tbPitch = new System.Windows.Forms.TextBox();
            this.tbRoll = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBoxServos = new System.Windows.Forms.GroupBox();
            this.servoYaw = new System.Windows.Forms.TextBox();
            this.servoPitch = new System.Windows.Forms.TextBox();
            this.stabilizeCb = new System.Windows.Forms.CheckBox();
            this.yawTrackBar = new System.Windows.Forms.TrackBar();
            this.pitchTrackBar = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.connectBn = new System.Windows.Forms.Button();
            this.imageBn = new System.Windows.Forms.Button();
            this.disconnectBn = new System.Windows.Forms.Button();
            this.frameRateLabel = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.defaultKParamsBn = new System.Windows.Forms.Button();
            this.kpTextBox = new System.Windows.Forms.TextBox();
            this.kiTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.savePathBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.defCamParams = new System.Windows.Forms.Button();
            this.fpsTextBox = new System.Windows.Forms.TextBox();
            this.expTimeTextBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBoxOrientation.SuspendLayout();
            this.groupBoxServos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.yawTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pitchTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxOrientation
            // 
            this.groupBoxOrientation.Controls.Add(this.tbYaw);
            this.groupBoxOrientation.Controls.Add(this.tbPitch);
            this.groupBoxOrientation.Controls.Add(this.tbRoll);
            this.groupBoxOrientation.Controls.Add(this.label3);
            this.groupBoxOrientation.Controls.Add(this.label2);
            this.groupBoxOrientation.Controls.Add(this.label1);
            this.groupBoxOrientation.Location = new System.Drawing.Point(9, 10);
            this.groupBoxOrientation.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxOrientation.Name = "groupBoxOrientation";
            this.groupBoxOrientation.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxOrientation.Size = new System.Drawing.Size(112, 97);
            this.groupBoxOrientation.TabIndex = 0;
            this.groupBoxOrientation.TabStop = false;
            this.groupBoxOrientation.Text = "Orientation";
            // 
            // tbYaw
            // 
            this.tbYaw.Location = new System.Drawing.Point(40, 66);
            this.tbYaw.Margin = new System.Windows.Forms.Padding(2);
            this.tbYaw.Name = "tbYaw";
            this.tbYaw.Size = new System.Drawing.Size(61, 20);
            this.tbYaw.TabIndex = 5;
            // 
            // tbPitch
            // 
            this.tbPitch.Location = new System.Drawing.Point(40, 41);
            this.tbPitch.Margin = new System.Windows.Forms.Padding(2);
            this.tbPitch.Name = "tbPitch";
            this.tbPitch.Size = new System.Drawing.Size(61, 20);
            this.tbPitch.TabIndex = 4;
            // 
            // tbRoll
            // 
            this.tbRoll.Location = new System.Drawing.Point(40, 14);
            this.tbRoll.Margin = new System.Windows.Forms.Padding(2);
            this.tbRoll.Name = "tbRoll";
            this.tbRoll.Size = new System.Drawing.Size(61, 20);
            this.tbRoll.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 68);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(28, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Yaw";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 44);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Pitch";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 18);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Roll";
            // 
            // groupBoxServos
            // 
            this.groupBoxServos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxServos.Controls.Add(this.servoYaw);
            this.groupBoxServos.Controls.Add(this.servoPitch);
            this.groupBoxServos.Controls.Add(this.stabilizeCb);
            this.groupBoxServos.Controls.Add(this.yawTrackBar);
            this.groupBoxServos.Controls.Add(this.pitchTrackBar);
            this.groupBoxServos.Controls.Add(this.label4);
            this.groupBoxServos.Controls.Add(this.label5);
            this.groupBoxServos.Location = new System.Drawing.Point(125, 11);
            this.groupBoxServos.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxServos.Name = "groupBoxServos";
            this.groupBoxServos.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxServos.Size = new System.Drawing.Size(399, 97);
            this.groupBoxServos.TabIndex = 1;
            this.groupBoxServos.TabStop = false;
            this.groupBoxServos.Text = "Servos";
            // 
            // servoYaw
            // 
            this.servoYaw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.servoYaw.Location = new System.Drawing.Point(341, 68);
            this.servoYaw.Name = "servoYaw";
            this.servoYaw.Size = new System.Drawing.Size(53, 20);
            this.servoYaw.TabIndex = 7;
            this.servoYaw.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // servoPitch
            // 
            this.servoPitch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.servoPitch.Location = new System.Drawing.Point(341, 39);
            this.servoPitch.Name = "servoPitch";
            this.servoPitch.Size = new System.Drawing.Size(53, 20);
            this.servoPitch.TabIndex = 6;
            this.servoPitch.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // stabilizeCb
            // 
            this.stabilizeCb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.stabilizeCb.AutoSize = true;
            this.stabilizeCb.Location = new System.Drawing.Point(329, 13);
            this.stabilizeCb.Margin = new System.Windows.Forms.Padding(2);
            this.stabilizeCb.Name = "stabilizeCb";
            this.stabilizeCb.Size = new System.Drawing.Size(65, 17);
            this.stabilizeCb.TabIndex = 5;
            this.stabilizeCb.Text = "Stabilize";
            this.stabilizeCb.UseVisualStyleBackColor = true;
            this.stabilizeCb.CheckedChanged += new System.EventHandler(this.stabilizeCb_CheckedChanged);
            // 
            // yawTrackBar
            // 
            this.yawTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.yawTrackBar.AutoSize = false;
            this.yawTrackBar.LargeChange = 100;
            this.yawTrackBar.Location = new System.Drawing.Point(37, 64);
            this.yawTrackBar.Margin = new System.Windows.Forms.Padding(2);
            this.yawTrackBar.Maximum = 8000;
            this.yawTrackBar.Minimum = 4000;
            this.yawTrackBar.Name = "yawTrackBar";
            this.yawTrackBar.Size = new System.Drawing.Size(299, 29);
            this.yawTrackBar.SmallChange = 10;
            this.yawTrackBar.TabIndex = 4;
            this.yawTrackBar.TickFrequency = 100;
            this.yawTrackBar.Value = 6000;
            this.yawTrackBar.Scroll += new System.EventHandler(this.yawTrackBar_Scroll);
            // 
            // pitchTrackBar
            // 
            this.pitchTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pitchTrackBar.AutoSize = false;
            this.pitchTrackBar.LargeChange = 100;
            this.pitchTrackBar.Location = new System.Drawing.Point(37, 35);
            this.pitchTrackBar.Margin = new System.Windows.Forms.Padding(2);
            this.pitchTrackBar.Maximum = 8000;
            this.pitchTrackBar.Minimum = 4000;
            this.pitchTrackBar.Name = "pitchTrackBar";
            this.pitchTrackBar.Size = new System.Drawing.Size(299, 29);
            this.pitchTrackBar.SmallChange = 10;
            this.pitchTrackBar.TabIndex = 3;
            this.pitchTrackBar.TickFrequency = 100;
            this.pitchTrackBar.Value = 6000;
            this.pitchTrackBar.Scroll += new System.EventHandler(this.pitchTrackBar_Scroll);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 69);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(28, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Yaw";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 42);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Pitch";
            // 
            // pictureBox
            // 
            this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox.Location = new System.Drawing.Point(9, 111);
            this.pictureBox.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(768, 539);
            this.pictureBox.TabIndex = 2;
            this.pictureBox.TabStop = false;
            // 
            // connectBn
            // 
            this.connectBn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.connectBn.Location = new System.Drawing.Point(9, 654);
            this.connectBn.Margin = new System.Windows.Forms.Padding(2);
            this.connectBn.Name = "connectBn";
            this.connectBn.Size = new System.Drawing.Size(84, 24);
            this.connectBn.TabIndex = 6;
            this.connectBn.Text = "Connect";
            this.connectBn.UseVisualStyleBackColor = true;
            this.connectBn.Click += new System.EventHandler(this.connectBn_Click);
            // 
            // imageBn
            // 
            this.imageBn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.imageBn.Location = new System.Drawing.Point(693, 654);
            this.imageBn.Margin = new System.Windows.Forms.Padding(2);
            this.imageBn.Name = "imageBn";
            this.imageBn.Size = new System.Drawing.Size(84, 24);
            this.imageBn.TabIndex = 8;
            this.imageBn.Text = "Save Image";
            this.imageBn.UseVisualStyleBackColor = true;
            this.imageBn.Click += new System.EventHandler(this.saveImageButton_Click);
            // 
            // disconnectBn
            // 
            this.disconnectBn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.disconnectBn.Location = new System.Drawing.Point(99, 654);
            this.disconnectBn.Margin = new System.Windows.Forms.Padding(2);
            this.disconnectBn.Name = "disconnectBn";
            this.disconnectBn.Size = new System.Drawing.Size(84, 24);
            this.disconnectBn.TabIndex = 9;
            this.disconnectBn.Text = "Disconnect";
            this.disconnectBn.UseVisualStyleBackColor = true;
            this.disconnectBn.Click += new System.EventHandler(this.disconnectBn_Click);
            // 
            // frameRateLabel
            // 
            this.frameRateLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.frameRateLabel.AutoSize = true;
            this.frameRateLabel.Location = new System.Drawing.Point(188, 660);
            this.frameRateLabel.Name = "frameRateLabel";
            this.frameRateLabel.Size = new System.Drawing.Size(0, 13);
            this.frameRateLabel.TabIndex = 10;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.defaultKParamsBn);
            this.groupBox1.Controls.Add(this.kpTextBox);
            this.groupBox1.Controls.Add(this.kiTextBox);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Location = new System.Drawing.Point(664, 11);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(113, 96);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Stabilization";
            // 
            // defaultKParamsBn
            // 
            this.defaultKParamsBn.Location = new System.Drawing.Point(41, 67);
            this.defaultKParamsBn.Name = "defaultKParamsBn";
            this.defaultKParamsBn.Size = new System.Drawing.Size(64, 23);
            this.defaultKParamsBn.TabIndex = 5;
            this.defaultKParamsBn.Text = "Default";
            this.defaultKParamsBn.UseVisualStyleBackColor = true;
            this.defaultKParamsBn.Click += new System.EventHandler(this.defaultKParamsBn_Click);
            // 
            // kpTextBox
            // 
            this.kpTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.kpTextBox.Location = new System.Drawing.Point(45, 17);
            this.kpTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.kpTextBox.Name = "kpTextBox";
            this.kpTextBox.Size = new System.Drawing.Size(60, 20);
            this.kpTextBox.TabIndex = 4;
            // 
            // kiTextBox
            // 
            this.kiTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.kiTextBox.Location = new System.Drawing.Point(45, 43);
            this.kiTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.kiTextBox.Name = "kiTextBox";
            this.kiTextBox.Size = new System.Drawing.Size(60, 20);
            this.kiTextBox.TabIndex = 3;
            this.kiTextBox.TabStop = false;
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 20);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(20, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "Kp";
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(15, 46);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(16, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "Ki";
            // 
            // savePathBox
            // 
            this.savePathBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.savePathBox.Location = new System.Drawing.Point(356, 657);
            this.savePathBox.Name = "savePathBox";
            this.savePathBox.Size = new System.Drawing.Size(332, 20);
            this.savePathBox.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(288, 660);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Save path :";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.defCamParams);
            this.groupBox2.Controls.Add(this.fpsTextBox);
            this.groupBox2.Controls.Add(this.expTimeTextBox);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Location = new System.Drawing.Point(528, 11);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(132, 96);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Camera";
            // 
            // defCamParams
            // 
            this.defCamParams.Location = new System.Drawing.Point(60, 67);
            this.defCamParams.Name = "defCamParams";
            this.defCamParams.Size = new System.Drawing.Size(64, 23);
            this.defCamParams.TabIndex = 5;
            this.defCamParams.Text = "Default";
            this.defCamParams.UseVisualStyleBackColor = true;
            this.defCamParams.Click += new System.EventHandler(this.defCamParams_Click);
            // 
            // fpsTextBox
            // 
            this.fpsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fpsTextBox.Location = new System.Drawing.Point(58, 17);
            this.fpsTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.fpsTextBox.Name = "fpsTextBox";
            this.fpsTextBox.Size = new System.Drawing.Size(66, 20);
            this.fpsTextBox.TabIndex = 4;
            // 
            // expTimeTextBox
            // 
            this.expTimeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.expTimeTextBox.Location = new System.Drawing.Point(58, 43);
            this.expTimeTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.expTimeTextBox.Name = "expTimeTextBox";
            this.expTimeTextBox.Size = new System.Drawing.Size(66, 20);
            this.expTimeTextBox.TabIndex = 3;
            this.expTimeTextBox.TabStop = false;
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(26, 20);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(27, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "FPS";
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(4, 46);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(50, 13);
            this.label10.TabIndex = 0;
            this.label10.Text = "Exp. time";
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 688);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.savePathBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.frameRateLabel);
            this.Controls.Add(this.disconnectBn);
            this.Controls.Add(this.imageBn);
            this.Controls.Add(this.connectBn);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.groupBoxServos);
            this.Controls.Add(this.groupBoxOrientation);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(800, 726);
            this.Name = "Window";
            this.Text = "ClientApp";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Window_FormClosing);
            this.Load += new System.EventHandler(this.Window_Load);
            this.groupBoxOrientation.ResumeLayout(false);
            this.groupBoxOrientation.PerformLayout();
            this.groupBoxServos.ResumeLayout(false);
            this.groupBoxServos.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.yawTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pitchTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxOrientation;
        private System.Windows.Forms.TextBox tbYaw;
        private System.Windows.Forms.TextBox tbPitch;
        private System.Windows.Forms.TextBox tbRoll;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBoxServos;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar yawTrackBar;
        private System.Windows.Forms.TrackBar pitchTrackBar;
        private System.Windows.Forms.CheckBox stabilizeCb;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Button connectBn;
        private System.Windows.Forms.Button imageBn;
        private System.Windows.Forms.Button disconnectBn;
        private System.Windows.Forms.TextBox servoYaw;
        private System.Windows.Forms.TextBox servoPitch;
        private System.Windows.Forms.Label frameRateLabel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox kpTextBox;
        private System.Windows.Forms.TextBox kiTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox savePathBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button defaultKParamsBn;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button defCamParams;
        private System.Windows.Forms.TextBox fpsTextBox;
        private System.Windows.Forms.TextBox expTimeTextBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;

    }
}

