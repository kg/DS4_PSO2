namespace DS4_PSO2 {
    partial class MainWindow {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose (bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent () {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.label1 = new System.Windows.Forms.Label();
            this.txtDualShock4Status = new System.Windows.Forms.TextBox();
            this.txtDualShock4State = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.nudJoystickNumber = new System.Windows.Forms.NumericUpDown();
            this.btnConfigureJoystick = new System.Windows.Forms.Button();
            this.chkJoystickEnabled = new System.Windows.Forms.CheckBox();
            this.tmrEnumerate = new System.Windows.Forms.Timer(this.components);
            this.tmrUpdate = new System.Windows.Forms.Timer(this.components);
            this.pnlSensors = new System.Windows.Forms.Panel();
            this.tbAccelZ = new System.Windows.Forms.TrackBar();
            this.tbAccelY = new System.Windows.Forms.TrackBar();
            this.tbAccelX = new System.Windows.Forms.TrackBar();
            this.tbGyroZ = new System.Windows.Forms.TrackBar();
            this.tbGyroY = new System.Windows.Forms.TrackBar();
            this.tbGyroX = new System.Windows.Forms.TrackBar();
            this.ttToolTips = new System.Windows.Forms.ToolTip(this.components);
            this.niTrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.nudJoystickNumber)).BeginInit();
            this.pnlSensors.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbAccelZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbAccelY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbAccelX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbGyroZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbGyroY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbGyroX)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "DualShock 4 status:";
            // 
            // txtDualShock4Status
            // 
            this.txtDualShock4Status.Location = new System.Drawing.Point(122, 12);
            this.txtDualShock4Status.Name = "txtDualShock4Status";
            this.txtDualShock4Status.ReadOnly = true;
            this.txtDualShock4Status.Size = new System.Drawing.Size(150, 20);
            this.txtDualShock4Status.TabIndex = 1;
            // 
            // txtDualShock4State
            // 
            this.txtDualShock4State.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.txtDualShock4State.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDualShock4State.Location = new System.Drawing.Point(16, 38);
            this.txtDualShock4State.Multiline = true;
            this.txtDualShock4State.Name = "txtDualShock4State";
            this.txtDualShock4State.ReadOnly = true;
            this.txtDualShock4State.Size = new System.Drawing.Size(210, 378);
            this.txtDualShock4State.TabIndex = 2;
            this.txtDualShock4State.WordWrap = false;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 424);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "vJoy Joystick #:";
            // 
            // nudJoystickNumber
            // 
            this.nudJoystickNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.nudJoystickNumber.Location = new System.Drawing.Point(122, 422);
            this.nudJoystickNumber.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.nudJoystickNumber.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudJoystickNumber.Name = "nudJoystickNumber";
            this.nudJoystickNumber.Size = new System.Drawing.Size(150, 20);
            this.nudJoystickNumber.TabIndex = 4;
            this.nudJoystickNumber.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // btnConfigureJoystick
            // 
            this.btnConfigureJoystick.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnConfigureJoystick.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnConfigureJoystick.Location = new System.Drawing.Point(172, 448);
            this.btnConfigureJoystick.Name = "btnConfigureJoystick";
            this.btnConfigureJoystick.Size = new System.Drawing.Size(100, 25);
            this.btnConfigureJoystick.TabIndex = 5;
            this.btnConfigureJoystick.Text = "Configure";
            this.btnConfigureJoystick.UseVisualStyleBackColor = true;
            // 
            // chkJoystickEnabled
            // 
            this.chkJoystickEnabled.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkJoystickEnabled.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkJoystickEnabled.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkJoystickEnabled.Location = new System.Drawing.Point(16, 448);
            this.chkJoystickEnabled.Name = "chkJoystickEnabled";
            this.chkJoystickEnabled.Size = new System.Drawing.Size(150, 25);
            this.chkJoystickEnabled.TabIndex = 6;
            this.chkJoystickEnabled.Text = "Enable Joystick";
            this.chkJoystickEnabled.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkJoystickEnabled.UseVisualStyleBackColor = true;
            // 
            // tmrEnumerate
            // 
            this.tmrEnumerate.Enabled = true;
            this.tmrEnumerate.Interval = 500;
            this.tmrEnumerate.Tick += new System.EventHandler(this.tmrEnumerate_Tick);
            // 
            // tmrUpdate
            // 
            this.tmrUpdate.Enabled = true;
            this.tmrUpdate.Interval = 25;
            this.tmrUpdate.Tick += new System.EventHandler(this.tmrUpdate_Tick);
            // 
            // pnlSensors
            // 
            this.pnlSensors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlSensors.Controls.Add(this.tbAccelZ);
            this.pnlSensors.Controls.Add(this.tbAccelY);
            this.pnlSensors.Controls.Add(this.tbAccelX);
            this.pnlSensors.Controls.Add(this.tbGyroZ);
            this.pnlSensors.Controls.Add(this.tbGyroY);
            this.pnlSensors.Controls.Add(this.tbGyroX);
            this.pnlSensors.Location = new System.Drawing.Point(232, 38);
            this.pnlSensors.Name = "pnlSensors";
            this.pnlSensors.Size = new System.Drawing.Size(200, 378);
            this.pnlSensors.TabIndex = 7;
            // 
            // tbAccelZ
            // 
            this.tbAccelZ.AutoSize = false;
            this.tbAccelZ.Enabled = false;
            this.tbAccelZ.Location = new System.Drawing.Point(3, 183);
            this.tbAccelZ.Maximum = 1280;
            this.tbAccelZ.Minimum = -1280;
            this.tbAccelZ.Name = "tbAccelZ";
            this.tbAccelZ.Size = new System.Drawing.Size(194, 30);
            this.tbAccelZ.TabIndex = 5;
            this.tbAccelZ.TickFrequency = 320;
            this.ttToolTips.SetToolTip(this.tbAccelZ, "Accelerometer (Z)");
            // 
            // tbAccelY
            // 
            this.tbAccelY.AutoSize = false;
            this.tbAccelY.Enabled = false;
            this.tbAccelY.Location = new System.Drawing.Point(3, 147);
            this.tbAccelY.Maximum = 1280;
            this.tbAccelY.Minimum = -1280;
            this.tbAccelY.Name = "tbAccelY";
            this.tbAccelY.Size = new System.Drawing.Size(194, 30);
            this.tbAccelY.TabIndex = 4;
            this.tbAccelY.TickFrequency = 320;
            this.ttToolTips.SetToolTip(this.tbAccelY, "Accelerometer (Y)");
            // 
            // tbAccelX
            // 
            this.tbAccelX.AutoSize = false;
            this.tbAccelX.Enabled = false;
            this.tbAccelX.Location = new System.Drawing.Point(3, 111);
            this.tbAccelX.Maximum = 1280;
            this.tbAccelX.Minimum = -1280;
            this.tbAccelX.Name = "tbAccelX";
            this.tbAccelX.Size = new System.Drawing.Size(194, 30);
            this.tbAccelX.TabIndex = 3;
            this.tbAccelX.TickFrequency = 320;
            this.ttToolTips.SetToolTip(this.tbAccelX, "Accelerometer (X)");
            // 
            // tbGyroZ
            // 
            this.tbGyroZ.AutoSize = false;
            this.tbGyroZ.Enabled = false;
            this.tbGyroZ.Location = new System.Drawing.Point(3, 75);
            this.tbGyroZ.Maximum = 1280;
            this.tbGyroZ.Minimum = -1280;
            this.tbGyroZ.Name = "tbGyroZ";
            this.tbGyroZ.Size = new System.Drawing.Size(194, 30);
            this.tbGyroZ.TabIndex = 2;
            this.tbGyroZ.TickFrequency = 320;
            this.ttToolTips.SetToolTip(this.tbGyroZ, "Gyroscope (Z)");
            // 
            // tbGyroY
            // 
            this.tbGyroY.AutoSize = false;
            this.tbGyroY.Enabled = false;
            this.tbGyroY.Location = new System.Drawing.Point(3, 39);
            this.tbGyroY.Maximum = 1280;
            this.tbGyroY.Minimum = -1280;
            this.tbGyroY.Name = "tbGyroY";
            this.tbGyroY.Size = new System.Drawing.Size(194, 30);
            this.tbGyroY.TabIndex = 1;
            this.tbGyroY.TickFrequency = 320;
            this.ttToolTips.SetToolTip(this.tbGyroY, "Gyroscope (Y)");
            // 
            // tbGyroX
            // 
            this.tbGyroX.AutoSize = false;
            this.tbGyroX.Enabled = false;
            this.tbGyroX.Location = new System.Drawing.Point(3, 3);
            this.tbGyroX.Maximum = 1280;
            this.tbGyroX.Minimum = -1280;
            this.tbGyroX.Name = "tbGyroX";
            this.tbGyroX.Size = new System.Drawing.Size(194, 30);
            this.tbGyroX.TabIndex = 0;
            this.tbGyroX.TickFrequency = 320;
            this.ttToolTips.SetToolTip(this.tbGyroX, "Gyroscope (X)");
            // 
            // niTrayIcon
            // 
            this.niTrayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("niTrayIcon.Icon")));
            this.niTrayIcon.Text = "DualShock 4 for PSO2";
            this.niTrayIcon.Click += new System.EventHandler(this.niTrayIcon_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(444, 485);
            this.Controls.Add(this.pnlSensors);
            this.Controls.Add(this.chkJoystickEnabled);
            this.Controls.Add(this.btnConfigureJoystick);
            this.Controls.Add(this.nudJoystickNumber);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtDualShock4State);
            this.Controls.Add(this.txtDualShock4Status);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainWindow";
            this.Text = "DualShock 4 for PSO2";
            ((System.ComponentModel.ISupportInitialize)(this.nudJoystickNumber)).EndInit();
            this.pnlSensors.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tbAccelZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbAccelY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbAccelX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbGyroZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbGyroY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbGyroX)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtDualShock4Status;
        private System.Windows.Forms.TextBox txtDualShock4State;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudJoystickNumber;
        private System.Windows.Forms.Button btnConfigureJoystick;
        private System.Windows.Forms.CheckBox chkJoystickEnabled;
        private System.Windows.Forms.Timer tmrEnumerate;
        private System.Windows.Forms.Timer tmrUpdate;
        private System.Windows.Forms.Panel pnlSensors;
        private System.Windows.Forms.TrackBar tbGyroZ;
        private System.Windows.Forms.TrackBar tbGyroY;
        private System.Windows.Forms.TrackBar tbGyroX;
        private System.Windows.Forms.TrackBar tbAccelZ;
        private System.Windows.Forms.TrackBar tbAccelY;
        private System.Windows.Forms.TrackBar tbAccelX;
        private System.Windows.Forms.ToolTip ttToolTips;
        private System.Windows.Forms.NotifyIcon niTrayIcon;
    }
}

