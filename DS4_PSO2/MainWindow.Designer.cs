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
            this.label1 = new System.Windows.Forms.Label();
            this.txtDualShock4Status = new System.Windows.Forms.TextBox();
            this.txtDualShock4State = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.nudJoystickNumber = new System.Windows.Forms.NumericUpDown();
            this.btnConfigureJoystick = new System.Windows.Forms.Button();
            this.chkJoystickEnabled = new System.Windows.Forms.CheckBox();
            this.tmrEnumerate = new System.Windows.Forms.Timer(this.components);
            this.tmrUpdate = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.nudJoystickNumber)).BeginInit();
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
            this.txtDualShock4State.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDualShock4State.Location = new System.Drawing.Point(16, 38);
            this.txtDualShock4State.Multiline = true;
            this.txtDualShock4State.Name = "txtDualShock4State";
            this.txtDualShock4State.ReadOnly = true;
            this.txtDualShock4State.Size = new System.Drawing.Size(416, 346);
            this.txtDualShock4State.TabIndex = 2;
            this.txtDualShock4State.WordWrap = false;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 392);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "vJoy Joystick #:";
            // 
            // nudJoystickNumber
            // 
            this.nudJoystickNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.nudJoystickNumber.Location = new System.Drawing.Point(122, 390);
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
            this.btnConfigureJoystick.Location = new System.Drawing.Point(172, 416);
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
            this.chkJoystickEnabled.Location = new System.Drawing.Point(16, 416);
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
            this.tmrUpdate.Interval = 1;
            this.tmrUpdate.Tick += new System.EventHandler(this.tmrUpdate_Tick);
            // 
            // MainWindow
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(444, 453);
            this.Controls.Add(this.chkJoystickEnabled);
            this.Controls.Add(this.btnConfigureJoystick);
            this.Controls.Add(this.nudJoystickNumber);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtDualShock4State);
            this.Controls.Add(this.txtDualShock4Status);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainWindow";
            this.Text = "DualShock 4 for PSO2";
            ((System.ComponentModel.ISupportInitialize)(this.nudJoystickNumber)).EndInit();
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
    }
}

