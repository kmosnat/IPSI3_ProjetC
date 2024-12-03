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
            this.btnStartAcquisition = new System.Windows.Forms.Button();
            this.timAcq = new System.Windows.Forms.Timer(this.components);
            this.Arduino = new System.IO.Ports.SerialPort(this.components);
            this.tbCom = new System.Windows.Forms.TextBox();
            this.btnStartTCP = new System.Windows.Forms.Button();
            this.btnStopTCP = new System.Windows.Forms.Button();
            this.btnStopAcquisition = new System.Windows.Forms.Button();
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
            this.pbImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbImage.TabIndex = 1;
            this.pbImage.TabStop = false;
            // 
            // btnStartAcquisition
            // 
            this.btnStartAcquisition.Location = new System.Drawing.Point(21, 197);
            this.btnStartAcquisition.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnStartAcquisition.Name = "btnStartAcquisition";
            this.btnStartAcquisition.Size = new System.Drawing.Size(138, 36);
            this.btnStartAcquisition.TabIndex = 2;
            this.btnStartAcquisition.Text = "Start Acquisition";
            this.btnStartAcquisition.UseVisualStyleBackColor = true;
            this.btnStartAcquisition.Click += new System.EventHandler(this.btnStartAcquisition_Click);
            // 
            // timAcq
            // 
            this.timAcq.Interval = 20;
            this.timAcq.Tick += new System.EventHandler(this.timAcq_Tick);
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
            // btnStartTCP
            // 
            this.btnStartTCP.Location = new System.Drawing.Point(21, 250);
            this.btnStartTCP.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnStartTCP.Name = "btnStartTCP";
            this.btnStartTCP.Size = new System.Drawing.Size(138, 36);
            this.btnStartTCP.TabIndex = 6;
            this.btnStartTCP.Text = "Start TCP";
            this.btnStartTCP.UseVisualStyleBackColor = true;
            this.btnStartTCP.Click += new System.EventHandler(this.btnStartTCP_Click);
            // 
            // btnStopTCP
            // 
            this.btnStopTCP.Location = new System.Drawing.Point(223, 250);
            this.btnStopTCP.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnStopTCP.Name = "btnStopTCP";
            this.btnStopTCP.Size = new System.Drawing.Size(138, 36);
            this.btnStopTCP.TabIndex = 7;
            this.btnStopTCP.Text = "Stop TCP";
            this.btnStopTCP.UseVisualStyleBackColor = true;
            this.btnStopTCP.Click += new System.EventHandler(this.btnStopTCP_Click);
            // 
            // btnStopAcquisition
            // 
            this.btnStopAcquisition.Location = new System.Drawing.Point(208, 197);
            this.btnStopAcquisition.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnStopAcquisition.Name = "btnStopAcquisition";
            this.btnStopAcquisition.Size = new System.Drawing.Size(138, 36);
            this.btnStopAcquisition.TabIndex = 8;
            this.btnStopAcquisition.Text = "Stop";
            this.btnStopAcquisition.UseVisualStyleBackColor = true;
            this.btnStopAcquisition.Click += new System.EventHandler(this.btnStopAcquisition_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1683, 738);
            this.Controls.Add(this.btnStopAcquisition);
            this.Controls.Add(this.btnStopTCP);
            this.Controls.Add(this.btnStartTCP);
            this.Controls.Add(this.tbCom);
            this.Controls.Add(this.btnStartAcquisition);
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
        private Button btnStartAcquisition;
        private Label lblAdrIP;
        private Label lblConnection;
        private Label lblNomCamera;
        private System.Windows.Forms.Timer timAcq;
        private System.IO.Ports.SerialPort Arduino;
        private TextBox tbCom;
        private Button btnStartTCP;
        private Button btnStopTCP;
        private Button btnStopAcquisition;
    }
}

