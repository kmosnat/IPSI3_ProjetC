namespace Couleur
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            gbCamera = new GroupBox();
            lblNomCamera = new Label();
            lblAdrIP = new Label();
            lblConnection = new Label();
            pbImage = new PictureBox();
            boutAcquisition = new Button();
            boutStop = new Button();
            timAcq = new System.Windows.Forms.Timer(components);
            gbCamera.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbImage).BeginInit();
            SuspendLayout();
            // 
            // gbCamera
            // 
            gbCamera.Controls.Add(lblNomCamera);
            gbCamera.Controls.Add(lblAdrIP);
            gbCamera.Controls.Add(lblConnection);
            gbCamera.Location = new Point(23, 22);
            gbCamera.Name = "gbCamera";
            gbCamera.Size = new Size(532, 209);
            gbCamera.TabIndex = 0;
            gbCamera.TabStop = false;
            gbCamera.Text = "Caméra";
            // 
            // lblNomCamera
            // 
            lblNomCamera.AutoSize = true;
            lblNomCamera.Location = new Point(28, 170);
            lblNomCamera.Name = "lblNomCamera";
            lblNomCamera.Size = new Size(95, 32);
            lblNomCamera.TabIndex = 2;
            lblNomCamera.Text = "Caméra";
            // 
            // lblAdrIP
            // 
            lblAdrIP.AutoSize = true;
            lblAdrIP.Location = new Point(28, 117);
            lblAdrIP.Name = "lblAdrIP";
            lblAdrIP.Size = new Size(209, 32);
            lblAdrIP.TabIndex = 1;
            lblAdrIP.Text = "Adresse IP : 0.0.0.0";
            // 
            // lblConnection
            // 
            lblConnection.AutoSize = true;
            lblConnection.BackColor = Color.FromArgb(255, 128, 0);
            lblConnection.Location = new Point(28, 66);
            lblConnection.Name = "lblConnection";
            lblConnection.Size = new Size(250, 32);
            lblConnection.TabIndex = 0;
            lblConnection.Text = "Connection en cours...";
            // 
            // pbImage
            // 
            pbImage.Location = new Point(583, 39);
            pbImage.Name = "pbImage";
            pbImage.Size = new Size(1193, 873);
            pbImage.TabIndex = 1;
            pbImage.TabStop = false;
            // 
            // boutAcquisition
            // 
            boutAcquisition.Location = new Point(51, 293);
            boutAcquisition.Name = "boutAcquisition";
            boutAcquisition.Size = new Size(150, 46);
            boutAcquisition.TabIndex = 2;
            boutAcquisition.Text = "Acquisition";
            boutAcquisition.UseVisualStyleBackColor = true;
            boutAcquisition.Click += boutAcquisition_Click;
            // 
            // boutStop
            // 
            boutStop.Location = new Point(316, 293);
            boutStop.Name = "boutStop";
            boutStop.Size = new Size(150, 46);
            boutStop.TabIndex = 3;
            boutStop.Text = "Arret";
            boutStop.UseVisualStyleBackColor = true;
            boutStop.Click += boutStop_Click;
            // 
            // timAcq
            // 
            timAcq.Interval = 20;
            timAcq.Tick += timAcq_Tick;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1823, 945);
            Controls.Add(boutStop);
            Controls.Add(boutAcquisition);
            Controls.Add(pbImage);
            Controls.Add(gbCamera);
            Name = "Form1";
            Text = "Form1";
            gbCamera.ResumeLayout(false);
            gbCamera.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbImage).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox gbCamera;
        private PictureBox pbImage;
        private Button boutAcquisition;
        private Button boutStop;
        private Label lblAdrIP;
        private Label lblConnection;
        private Label lblNomCamera;
        private System.Windows.Forms.Timer timAcq;
    }
}