using System.Drawing;
using System.Windows.Forms;

namespace Serveur
{
    partial class Main
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
            this.timAcq = new System.Windows.Forms.Timer(this.components);
            this.Arduino = new System.IO.Ports.SerialPort(this.components);
            this.tbCom = new System.Windows.Forms.TextBox();
            this.navBar = new System.Windows.Forms.MenuStrip();
            this.serveurTCPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.démarrerLeServeurToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.arrêterLeServeurToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.réseauToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sélectionnerUneCarteRéseauToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.afficherLAdresseIPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSearchCamera = new System.Windows.Forms.PictureBox();
            this.btnStartAcquisition = new System.Windows.Forms.PictureBox();
            this.btnStopAcquisition = new System.Windows.Forms.PictureBox();
            this.gbCamera.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).BeginInit();
            this.navBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnSearchCamera)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnStartAcquisition)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnStopAcquisition)).BeginInit();
            this.SuspendLayout();
            // 
            // gbCamera
            // 
            this.gbCamera.Controls.Add(this.lblNomCamera);
            this.gbCamera.Controls.Add(this.lblAdrIP);
            this.gbCamera.Controls.Add(this.lblConnection);
            this.gbCamera.Location = new System.Drawing.Point(21, 72);
            this.gbCamera.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gbCamera.Name = "gbCamera";
            this.gbCamera.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gbCamera.Size = new System.Drawing.Size(491, 211);
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
            this.pbImage.Location = new System.Drawing.Point(545, 72);
            this.pbImage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pbImage.Name = "pbImage";
            this.pbImage.Size = new System.Drawing.Size(1101, 716);
            this.pbImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbImage.TabIndex = 1;
            this.pbImage.TabStop = false;
            // 
            // timAcq
            // 
            this.timAcq.Interval = 20;
            this.timAcq.Tick += new System.EventHandler(this.timAcq_Tick);
            // 
            // tbCom
            // 
            this.tbCom.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbCom.Location = new System.Drawing.Point(21, 381);
            this.tbCom.Multiline = true;
            this.tbCom.Name = "tbCom";
            this.tbCom.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbCom.Size = new System.Drawing.Size(491, 412);
            this.tbCom.TabIndex = 0;
            this.tbCom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // navBar
            // 
            this.navBar.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.navBar.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.navBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.serveurTCPToolStripMenuItem,
            this.réseauToolStripMenuItem,
            this.quitterToolStripMenuItem});
            this.navBar.Location = new System.Drawing.Point(0, 0);
            this.navBar.Name = "navBar";
            this.navBar.Size = new System.Drawing.Size(1683, 40);
            this.navBar.TabIndex = 9;
            this.navBar.Text = "menuStrip1";
            // 
            // serveurTCPToolStripMenuItem
            // 
            this.serveurTCPToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.démarrerLeServeurToolStripMenuItem,
            this.arrêterLeServeurToolStripMenuItem});
            this.serveurTCPToolStripMenuItem.Name = "serveurTCPToolStripMenuItem";
            this.serveurTCPToolStripMenuItem.Size = new System.Drawing.Size(162, 36);
            this.serveurTCPToolStripMenuItem.Text = "Serveur TCP";
            // 
            // démarrerLeServeurToolStripMenuItem
            // 
            this.démarrerLeServeurToolStripMenuItem.Name = "démarrerLeServeurToolStripMenuItem";
            this.démarrerLeServeurToolStripMenuItem.Size = new System.Drawing.Size(361, 44);
            this.démarrerLeServeurToolStripMenuItem.Text = "Démarrer le Serveur";
            this.démarrerLeServeurToolStripMenuItem.Click += new System.EventHandler(this.démarrerLeServeurToolStripMenuItem_Click);
            // 
            // arrêterLeServeurToolStripMenuItem
            // 
            this.arrêterLeServeurToolStripMenuItem.Name = "arrêterLeServeurToolStripMenuItem";
            this.arrêterLeServeurToolStripMenuItem.Size = new System.Drawing.Size(361, 44);
            this.arrêterLeServeurToolStripMenuItem.Text = "Arrêter le Serveur";
            this.arrêterLeServeurToolStripMenuItem.Click += new System.EventHandler(this.arrêterLeServeurToolStripMenuItem_Click);
            // 
            // réseauToolStripMenuItem
            // 
            this.réseauToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sélectionnerUneCarteRéseauToolStripMenuItem,
            this.afficherLAdresseIPToolStripMenuItem});
            this.réseauToolStripMenuItem.Name = "réseauToolStripMenuItem";
            this.réseauToolStripMenuItem.Size = new System.Drawing.Size(109, 36);
            this.réseauToolStripMenuItem.Text = "Réseau";
            // 
            // sélectionnerUneCarteRéseauToolStripMenuItem
            // 
            this.sélectionnerUneCarteRéseauToolStripMenuItem.Name = "sélectionnerUneCarteRéseauToolStripMenuItem";
            this.sélectionnerUneCarteRéseauToolStripMenuItem.Size = new System.Drawing.Size(473, 44);
            this.sélectionnerUneCarteRéseauToolStripMenuItem.Text = "Sélectionner une Carte Réseau";
            this.sélectionnerUneCarteRéseauToolStripMenuItem.Click += new System.EventHandler(this.sélectionnerUneCarteRéseauToolStripMenuItem_Click);
            // 
            // afficherLAdresseIPToolStripMenuItem
            // 
            this.afficherLAdresseIPToolStripMenuItem.Enabled = false;
            this.afficherLAdresseIPToolStripMenuItem.Name = "afficherLAdresseIPToolStripMenuItem";
            this.afficherLAdresseIPToolStripMenuItem.Size = new System.Drawing.Size(473, 44);
            this.afficherLAdresseIPToolStripMenuItem.Text = "Adresse IP :";
            // 
            // quitterToolStripMenuItem
            // 
            this.quitterToolStripMenuItem.Name = "quitterToolStripMenuItem";
            this.quitterToolStripMenuItem.Size = new System.Drawing.Size(109, 36);
            this.quitterToolStripMenuItem.Text = "Quitter";
            this.quitterToolStripMenuItem.Click += new System.EventHandler(this.quitterToolStripMenuItem_Click);
            // 
            // btnSearchCamera
            // 
            this.btnSearchCamera.Image = global::Serveur.Properties.Resources.search;
            this.btnSearchCamera.Location = new System.Drawing.Point(77, 304);
            this.btnSearchCamera.Name = "btnSearchCamera";
            this.btnSearchCamera.Size = new System.Drawing.Size(66, 64);
            this.btnSearchCamera.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnSearchCamera.TabIndex = 11;
            this.btnSearchCamera.TabStop = false;
            this.btnSearchCamera.Click += new System.EventHandler(this.btnSearchCamera_Click);
            // 
            // btnStartAcquisition
            // 
            this.btnStartAcquisition.Image = global::Serveur.Properties.Resources.play;
            this.btnStartAcquisition.Location = new System.Drawing.Point(228, 304);
            this.btnStartAcquisition.Name = "btnStartAcquisition";
            this.btnStartAcquisition.Size = new System.Drawing.Size(66, 64);
            this.btnStartAcquisition.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnStartAcquisition.TabIndex = 12;
            this.btnStartAcquisition.TabStop = false;
            this.btnStartAcquisition.Click += new System.EventHandler(this.btnStartAcquisition_Click);
            // 
            // btnStopAcquisition
            // 
            this.btnStopAcquisition.Image = global::Serveur.Properties.Resources.stop;
            this.btnStopAcquisition.Location = new System.Drawing.Point(380, 304);
            this.btnStopAcquisition.Name = "btnStopAcquisition";
            this.btnStopAcquisition.Size = new System.Drawing.Size(66, 64);
            this.btnStopAcquisition.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnStopAcquisition.TabIndex = 13;
            this.btnStopAcquisition.TabStop = false;
            this.btnStopAcquisition.Click += new System.EventHandler(this.btnStopAcquisition_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1683, 827);
            this.Controls.Add(this.btnStopAcquisition);
            this.Controls.Add(this.btnStartAcquisition);
            this.Controls.Add(this.btnSearchCamera);
            this.Controls.Add(this.tbCom);
            this.Controls.Add(this.pbImage);
            this.Controls.Add(this.gbCamera);
            this.Controls.Add(this.navBar);
            this.MainMenuStrip = this.navBar;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Main";
            this.Text = "Couleur - Serveur";
            this.gbCamera.ResumeLayout(false);
            this.gbCamera.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).EndInit();
            this.navBar.ResumeLayout(false);
            this.navBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnSearchCamera)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnStartAcquisition)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnStopAcquisition)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GroupBox gbCamera;
        private PictureBox pbImage;
        private Label lblAdrIP;
        private Label lblConnection;
        private Label lblNomCamera;
        private System.Windows.Forms.Timer timAcq;
        private System.IO.Ports.SerialPort Arduino;
        private TextBox tbCom;
        private MenuStrip navBar;
        private ToolStripMenuItem serveurTCPToolStripMenuItem;
        private ToolStripMenuItem démarrerLeServeurToolStripMenuItem;
        private ToolStripMenuItem arrêterLeServeurToolStripMenuItem;
        private ToolStripMenuItem réseauToolStripMenuItem;
        private ToolStripMenuItem sélectionnerUneCarteRéseauToolStripMenuItem;
        private ToolStripMenuItem afficherLAdresseIPToolStripMenuItem;
        private ToolStripMenuItem quitterToolStripMenuItem;
        private PictureBox btnSearchCamera;
        private PictureBox btnStartAcquisition;
        private PictureBox btnStopAcquisition;
    }
}

