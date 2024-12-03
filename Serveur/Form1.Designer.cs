using System.Drawing;
using System.Windows.Forms;

namespace Serveur
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
            this.components = new System.ComponentModel.Container();
            this.gbCamera = new System.Windows.Forms.GroupBox();
            this.lblNomCamera = new System.Windows.Forms.Label();
            this.lblAdrIP = new System.Windows.Forms.Label();
            this.lblConnection = new System.Windows.Forms.Label();
            this.pbImage = new System.Windows.Forms.PictureBox();
            this.boutAcquisition = new System.Windows.Forms.Button();
            this.boutStop = new System.Windows.Forms.Button();
            this.timAcq = new System.Windows.Forms.Timer(this.components);
            this.Arduino = new System.IO.Ports.SerialPort(this.components);
            this.tbCom = new System.Windows.Forms.TextBox();
            this.btninit = new System.Windows.Forms.Button();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.quitBtn = new System.Windows.Forms.Button();
            this.gbCamera.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).BeginInit();
            this.SuspendLayout();
            // 
            // gbCamera
            // 
            this.gbCamera.Controls.Add(this.lblNomCamera);
            this.gbCamera.Controls.Add(this.lblAdrIP);
            this.gbCamera.Controls.Add(this.lblConnection);
            this.gbCamera.Location = new System.Drawing.Point(21, 17);
            this.gbCamera.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gbCamera.Name = "gbCamera";
            this.gbCamera.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gbCamera.Size = new System.Drawing.Size(491, 163);
            this.gbCamera.TabIndex = 0;
            this.gbCamera.TabStop = false;
            this.gbCamera.Text = "Caméra";
            // 
            // lblNomCamera
            // 
            this.lblNomCamera.AutoSize = true;
            this.lblNomCamera.Location = new System.Drawing.Point(26, 133);
            this.lblNomCamera.Name = "lblNomCamera";
            this.lblNomCamera.Size = new System.Drawing.Size(87, 25);
            this.lblNomCamera.TabIndex = 2;
            this.lblNomCamera.Text = "Caméra";
            // 
            // lblAdrIP
            // 
            this.lblAdrIP.AutoSize = true;
            this.lblAdrIP.Location = new System.Drawing.Point(26, 91);
            this.lblAdrIP.Name = "lblAdrIP";
            this.lblAdrIP.Size = new System.Drawing.Size(200, 25);
            this.lblAdrIP.TabIndex = 1;
            this.lblAdrIP.Text = "Adresse IP : 0.0.0.0";
            // 
            // lblConnection
            // 
            this.lblConnection.AutoSize = true;
            this.lblConnection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.lblConnection.Location = new System.Drawing.Point(26, 52);
            this.lblConnection.Name = "lblConnection";
            this.lblConnection.Size = new System.Drawing.Size(228, 25);
            this.lblConnection.TabIndex = 0;
            this.lblConnection.Text = "Connection en cours...";
            // 
            // pbImage
            // 
            this.pbImage.Location = new System.Drawing.Point(538, 30);
            this.pbImage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pbImage.Name = "pbImage";
            this.pbImage.Size = new System.Drawing.Size(1101, 682);
            this.pbImage.TabIndex = 1;
            this.pbImage.TabStop = false;
            // 
            // boutAcquisition
            // 
            this.boutAcquisition.Location = new System.Drawing.Point(52, 250);
            this.boutAcquisition.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.boutAcquisition.Name = "boutAcquisition";
            this.boutAcquisition.Size = new System.Drawing.Size(138, 36);
            this.boutAcquisition.TabIndex = 2;
            this.boutAcquisition.Text = "Acquisition";
            this.boutAcquisition.UseVisualStyleBackColor = true;
            this.boutAcquisition.Click += new System.EventHandler(this.boutAcquisition_Click);
            // 
            // boutStop
            // 
            this.boutStop.Location = new System.Drawing.Point(291, 197);
            this.boutStop.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.boutStop.Name = "boutStop";
            this.boutStop.Size = new System.Drawing.Size(138, 36);
            this.boutStop.TabIndex = 3;
            this.boutStop.Text = "Arret";
            this.boutStop.UseVisualStyleBackColor = true;
            // 
            // timAcq
            // 
            this.timAcq.Interval = 20;
            this.timAcq.Tick += new System.EventHandler(this.timAcq_Tick_1);
            // 
            // tbCom
            // 
            this.tbCom.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbCom.Location = new System.Drawing.Point(21, 303);
            this.tbCom.Multiline = true;
            this.tbCom.Name = "tbCom";
            this.tbCom.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbCom.Size = new System.Drawing.Size(491, 397);
            this.tbCom.TabIndex = 0;
            this.tbCom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btninit
            // 
            this.btninit.Location = new System.Drawing.Point(52, 197);
            this.btninit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btninit.Name = "btninit";
            this.btninit.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btninit.Size = new System.Drawing.Size(138, 36);
            this.btninit.TabIndex = 4;
            this.btninit.Text = "Init";
            this.btninit.UseVisualStyleBackColor = true;
            this.btninit.Click += new System.EventHandler(this.btninit_Click);
            // 
            // quitBtn
            // 
            this.quitBtn.Location = new System.Drawing.Point(291, 250);
            this.quitBtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.quitBtn.Name = "quitBtn";
            this.quitBtn.Size = new System.Drawing.Size(138, 36);
            this.quitBtn.TabIndex = 5;
            this.quitBtn.Text = "Quitter";
            this.quitBtn.UseVisualStyleBackColor = true;
            this.quitBtn.Click += new System.EventHandler(this.quitBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1683, 738);
            this.Controls.Add(this.quitBtn);
            this.Controls.Add(this.btninit);
            this.Controls.Add(this.tbCom);
            this.Controls.Add(this.boutStop);
            this.Controls.Add(this.boutAcquisition);
            this.Controls.Add(this.pbImage);
            this.Controls.Add(this.gbCamera);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.gbCamera.ResumeLayout(false);
            this.gbCamera.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.IO.Ports.SerialPort Arduino;
        private TextBox tbCom;
        private Button btninit;
        private System.IO.Ports.SerialPort serialPort1;
        private Button quitBtn;
    }
}

