namespace DS4_PSO2 {
    partial class GestureOverlay {
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
            this.TopmostHackTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // TopmostHackTimer
            // 
            this.TopmostHackTimer.Interval = 500;
            this.TopmostHackTimer.Tick += new System.EventHandler(this.TopmostHackTimer_Tick);
            // 
            // GestureOverlay
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(384, 189);
            this.ControlBox = false;
            this.Enabled = false;
            this.Font = new System.Drawing.Font("Candara", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "GestureOverlay";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Gesture Overlay";
            this.TopMost = true;
            this.Resize += new System.EventHandler(this.GestureOverlay_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer TopmostHackTimer;
    }
}