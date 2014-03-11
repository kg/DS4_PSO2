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
            this.tmrEnumerate = new System.Windows.Forms.Timer(this.components);
            this.tmrUpdate = new System.Windows.Forms.Timer(this.components);
            this.ttToolTips = new System.Windows.Forms.ToolTip(this.components);
            this.tbAccelZ = new System.Windows.Forms.TrackBar();
            this.tbAccelY = new System.Windows.Forms.TrackBar();
            this.tbAccelX = new System.Windows.Forms.TrackBar();
            this.tbGyroZ = new System.Windows.Forms.TrackBar();
            this.tbGyroY = new System.Windows.Forms.TrackBar();
            this.tbGyroX = new System.Windows.Forms.TrackBar();
            this.niTrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.cmsTrayIcon = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Tabs = new System.Windows.Forms.TabControl();
            this.SettingsPage = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkJoystickEnabled = new System.Windows.Forms.CheckBox();
            this.btnConfigureJoystick = new System.Windows.Forms.Button();
            this.nudJoystickNumber = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ShowGestureOverlay = new System.Windows.Forms.RadioButton();
            this.NoGestureOverlay = new System.Windows.Forms.RadioButton();
            this.StatusPage = new System.Windows.Forms.TabPage();
            this.pnlSensors = new System.Windows.Forms.Panel();
            this.txtDualShock4State = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.tbAccelZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbAccelY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbAccelX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbGyroZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbGyroY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbGyroX)).BeginInit();
            this.cmsTrayIcon.SuspendLayout();
            this.Tabs.SuspendLayout();
            this.SettingsPage.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudJoystickNumber)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.StatusPage.SuspendLayout();
            this.pnlSensors.SuspendLayout();
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
            // tmrEnumerate
            // 
            this.tmrEnumerate.Enabled = true;
            this.tmrEnumerate.Interval = 500;
            this.tmrEnumerate.Tick += new System.EventHandler(this.tmrEnumerate_Tick);
            // 
            // tmrUpdate
            // 
            this.tmrUpdate.Enabled = true;
            this.tmrUpdate.Interval = 33;
            this.tmrUpdate.Tick += new System.EventHandler(this.tmrUpdate_Tick);
            // 
            // tbAccelZ
            // 
            this.tbAccelZ.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbAccelZ.AutoSize = false;
            this.tbAccelZ.Enabled = false;
            this.tbAccelZ.Location = new System.Drawing.Point(3, 183);
            this.tbAccelZ.Maximum = 1280;
            this.tbAccelZ.Minimum = -1280;
            this.tbAccelZ.Name = "tbAccelZ";
            this.tbAccelZ.Size = new System.Drawing.Size(169, 30);
            this.tbAccelZ.TabIndex = 5;
            this.tbAccelZ.TickFrequency = 320;
            this.ttToolTips.SetToolTip(this.tbAccelZ, "Accelerometer (Z)");
            // 
            // tbAccelY
            // 
            this.tbAccelY.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbAccelY.AutoSize = false;
            this.tbAccelY.Enabled = false;
            this.tbAccelY.Location = new System.Drawing.Point(3, 147);
            this.tbAccelY.Maximum = 1280;
            this.tbAccelY.Minimum = -1280;
            this.tbAccelY.Name = "tbAccelY";
            this.tbAccelY.Size = new System.Drawing.Size(169, 30);
            this.tbAccelY.TabIndex = 4;
            this.tbAccelY.TickFrequency = 320;
            this.ttToolTips.SetToolTip(this.tbAccelY, "Accelerometer (Y)");
            // 
            // tbAccelX
            // 
            this.tbAccelX.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbAccelX.AutoSize = false;
            this.tbAccelX.Enabled = false;
            this.tbAccelX.Location = new System.Drawing.Point(3, 111);
            this.tbAccelX.Maximum = 1280;
            this.tbAccelX.Minimum = -1280;
            this.tbAccelX.Name = "tbAccelX";
            this.tbAccelX.Size = new System.Drawing.Size(169, 30);
            this.tbAccelX.TabIndex = 3;
            this.tbAccelX.TickFrequency = 320;
            this.ttToolTips.SetToolTip(this.tbAccelX, "Accelerometer (X)");
            // 
            // tbGyroZ
            // 
            this.tbGyroZ.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbGyroZ.AutoSize = false;
            this.tbGyroZ.Enabled = false;
            this.tbGyroZ.Location = new System.Drawing.Point(3, 75);
            this.tbGyroZ.Maximum = 1280;
            this.tbGyroZ.Minimum = -1280;
            this.tbGyroZ.Name = "tbGyroZ";
            this.tbGyroZ.Size = new System.Drawing.Size(169, 30);
            this.tbGyroZ.TabIndex = 2;
            this.tbGyroZ.TickFrequency = 320;
            this.ttToolTips.SetToolTip(this.tbGyroZ, "Gyroscope (Z)");
            // 
            // tbGyroY
            // 
            this.tbGyroY.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbGyroY.AutoSize = false;
            this.tbGyroY.Enabled = false;
            this.tbGyroY.Location = new System.Drawing.Point(3, 39);
            this.tbGyroY.Maximum = 1280;
            this.tbGyroY.Minimum = -1280;
            this.tbGyroY.Name = "tbGyroY";
            this.tbGyroY.Size = new System.Drawing.Size(169, 30);
            this.tbGyroY.TabIndex = 1;
            this.tbGyroY.TickFrequency = 320;
            this.ttToolTips.SetToolTip(this.tbGyroY, "Gyroscope (Y)");
            // 
            // tbGyroX
            // 
            this.tbGyroX.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbGyroX.AutoSize = false;
            this.tbGyroX.Enabled = false;
            this.tbGyroX.Location = new System.Drawing.Point(3, 3);
            this.tbGyroX.Maximum = 1280;
            this.tbGyroX.Minimum = -1280;
            this.tbGyroX.Name = "tbGyroX";
            this.tbGyroX.Size = new System.Drawing.Size(169, 30);
            this.tbGyroX.TabIndex = 0;
            this.tbGyroX.TickFrequency = 320;
            this.ttToolTips.SetToolTip(this.tbGyroX, "Gyroscope (X)");
            // 
            // niTrayIcon
            // 
            this.niTrayIcon.ContextMenuStrip = this.cmsTrayIcon;
            this.niTrayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("niTrayIcon.Icon")));
            this.niTrayIcon.Text = "DualShock 4 for PSO2";
            this.niTrayIcon.MouseDown += new System.Windows.Forms.MouseEventHandler(this.niTrayIcon_MouseDown);
            // 
            // cmsTrayIcon
            // 
            this.cmsTrayIcon.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.cmsTrayIcon.Name = "cmsTrayIcon";
            this.cmsTrayIcon.Size = new System.Drawing.Size(104, 54);
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.showToolStripMenuItem.Text = "&Show";
            this.showToolStripMenuItem.Click += new System.EventHandler(this.showToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(100, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // Tabs
            // 
            this.Tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Tabs.Controls.Add(this.SettingsPage);
            this.Tabs.Controls.Add(this.StatusPage);
            this.Tabs.Location = new System.Drawing.Point(16, 38);
            this.Tabs.Name = "Tabs";
            this.Tabs.SelectedIndex = 0;
            this.Tabs.Size = new System.Drawing.Size(416, 435);
            this.Tabs.TabIndex = 8;
            // 
            // SettingsPage
            // 
            this.SettingsPage.Controls.Add(this.groupBox2);
            this.SettingsPage.Controls.Add(this.groupBox1);
            this.SettingsPage.Location = new System.Drawing.Point(4, 22);
            this.SettingsPage.Name = "SettingsPage";
            this.SettingsPage.Padding = new System.Windows.Forms.Padding(3);
            this.SettingsPage.Size = new System.Drawing.Size(408, 409);
            this.SettingsPage.TabIndex = 0;
            this.SettingsPage.Text = "Settings";
            this.SettingsPage.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkJoystickEnabled);
            this.groupBox2.Controls.Add(this.btnConfigureJoystick);
            this.groupBox2.Controls.Add(this.nudJoystickNumber);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(6, 58);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(396, 75);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Virtual Joystick";
            // 
            // chkJoystickEnabled
            // 
            this.chkJoystickEnabled.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkJoystickEnabled.Checked = true;
            this.chkJoystickEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkJoystickEnabled.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkJoystickEnabled.Location = new System.Drawing.Point(6, 42);
            this.chkJoystickEnabled.Name = "chkJoystickEnabled";
            this.chkJoystickEnabled.Size = new System.Drawing.Size(175, 25);
            this.chkJoystickEnabled.TabIndex = 14;
            this.chkJoystickEnabled.Text = "Enable Joystick";
            this.chkJoystickEnabled.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkJoystickEnabled.UseVisualStyleBackColor = true;
            // 
            // btnConfigureJoystick
            // 
            this.btnConfigureJoystick.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnConfigureJoystick.Location = new System.Drawing.Point(187, 42);
            this.btnConfigureJoystick.Name = "btnConfigureJoystick";
            this.btnConfigureJoystick.Size = new System.Drawing.Size(175, 25);
            this.btnConfigureJoystick.TabIndex = 13;
            this.btnConfigureJoystick.Text = "Auto-configure vJoy";
            this.btnConfigureJoystick.UseVisualStyleBackColor = true;
            // 
            // nudJoystickNumber
            // 
            this.nudJoystickNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nudJoystickNumber.Location = new System.Drawing.Point(115, 16);
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
            this.nudJoystickNumber.Size = new System.Drawing.Size(275, 20);
            this.nudJoystickNumber.TabIndex = 12;
            this.nudJoystickNumber.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "vJoy Joystick #:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ShowGestureOverlay);
            this.groupBox1.Controls.Add(this.NoGestureOverlay);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(396, 46);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Gesture Overlay";
            // 
            // ShowGestureOverlay
            // 
            this.ShowGestureOverlay.AutoSize = true;
            this.ShowGestureOverlay.Location = new System.Drawing.Point(72, 19);
            this.ShowGestureOverlay.Name = "ShowGestureOverlay";
            this.ShowGestureOverlay.Size = new System.Drawing.Size(58, 17);
            this.ShowGestureOverlay.TabIndex = 15;
            this.ShowGestureOverlay.Text = "Enable";
            this.ShowGestureOverlay.UseVisualStyleBackColor = true;
            // 
            // NoGestureOverlay
            // 
            this.NoGestureOverlay.AutoSize = true;
            this.NoGestureOverlay.Checked = true;
            this.NoGestureOverlay.Location = new System.Drawing.Point(6, 19);
            this.NoGestureOverlay.Name = "NoGestureOverlay";
            this.NoGestureOverlay.Size = new System.Drawing.Size(60, 17);
            this.NoGestureOverlay.TabIndex = 14;
            this.NoGestureOverlay.TabStop = true;
            this.NoGestureOverlay.Text = "Disable";
            this.NoGestureOverlay.UseVisualStyleBackColor = true;
            // 
            // StatusPage
            // 
            this.StatusPage.Controls.Add(this.pnlSensors);
            this.StatusPage.Controls.Add(this.txtDualShock4State);
            this.StatusPage.Location = new System.Drawing.Point(4, 22);
            this.StatusPage.Name = "StatusPage";
            this.StatusPage.Padding = new System.Windows.Forms.Padding(3);
            this.StatusPage.Size = new System.Drawing.Size(408, 409);
            this.StatusPage.TabIndex = 1;
            this.StatusPage.Text = "Controller Status";
            this.StatusPage.UseVisualStyleBackColor = true;
            // 
            // pnlSensors
            // 
            this.pnlSensors.Controls.Add(this.tbAccelZ);
            this.pnlSensors.Controls.Add(this.tbAccelY);
            this.pnlSensors.Controls.Add(this.tbAccelX);
            this.pnlSensors.Controls.Add(this.tbGyroZ);
            this.pnlSensors.Controls.Add(this.tbGyroY);
            this.pnlSensors.Controls.Add(this.tbGyroX);
            this.pnlSensors.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlSensors.Location = new System.Drawing.Point(230, 3);
            this.pnlSensors.Name = "pnlSensors";
            this.pnlSensors.Size = new System.Drawing.Size(175, 403);
            this.pnlSensors.TabIndex = 8;
            // 
            // txtDualShock4State
            // 
            this.txtDualShock4State.Dock = System.Windows.Forms.DockStyle.Left;
            this.txtDualShock4State.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDualShock4State.Location = new System.Drawing.Point(3, 3);
            this.txtDualShock4State.Multiline = true;
            this.txtDualShock4State.Name = "txtDualShock4State";
            this.txtDualShock4State.ReadOnly = true;
            this.txtDualShock4State.Size = new System.Drawing.Size(221, 403);
            this.txtDualShock4State.TabIndex = 3;
            this.txtDualShock4State.WordWrap = false;
            // 
            // MainWindow
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(444, 485);
            this.Controls.Add(this.txtDualShock4Status);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Tabs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainWindow";
            this.Text = "DualShock 4 for PSO2";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.Shown += new System.EventHandler(this.MainWindow_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.tbAccelZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbAccelY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbAccelX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbGyroZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbGyroY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbGyroX)).EndInit();
            this.cmsTrayIcon.ResumeLayout(false);
            this.Tabs.ResumeLayout(false);
            this.SettingsPage.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudJoystickNumber)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.StatusPage.ResumeLayout(false);
            this.StatusPage.PerformLayout();
            this.pnlSensors.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtDualShock4Status;
        private System.Windows.Forms.Timer tmrEnumerate;
        private System.Windows.Forms.Timer tmrUpdate;
        private System.Windows.Forms.ToolTip ttToolTips;
        private System.Windows.Forms.NotifyIcon niTrayIcon;
        private System.Windows.Forms.ContextMenuStrip cmsTrayIcon;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.TabControl Tabs;
        private System.Windows.Forms.TabPage SettingsPage;
        private System.Windows.Forms.TabPage StatusPage;
        private System.Windows.Forms.Panel pnlSensors;
        private System.Windows.Forms.TrackBar tbAccelZ;
        private System.Windows.Forms.TrackBar tbAccelY;
        private System.Windows.Forms.TrackBar tbAccelX;
        private System.Windows.Forms.TrackBar tbGyroZ;
        private System.Windows.Forms.TrackBar tbGyroY;
        private System.Windows.Forms.TrackBar tbGyroX;
        private System.Windows.Forms.TextBox txtDualShock4State;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton ShowGestureOverlay;
        private System.Windows.Forms.RadioButton NoGestureOverlay;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkJoystickEnabled;
        private System.Windows.Forms.Button btnConfigureJoystick;
        private System.Windows.Forms.NumericUpDown nudJoystickNumber;
        private System.Windows.Forms.Label label2;
    }
}

