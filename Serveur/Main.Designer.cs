using System;
using System.Drawing;
using System.Windows.Forms;

namespace Serveur
{
    partial class Main
    {
        private System.ComponentModel.IContainer components = null;

        // Contrôles déclarés
        private MenuStrip navBar;
        private ToolStripMenuItem serveurTCPToolStripMenuItem;
        private ToolStripMenuItem startTCP;
        private ToolStripMenuItem stopTCP;
        private ToolStripMenuItem réseauToolStripMenuItem;
        private ToolStripMenuItem NetworkInterfaceSelection;
        private ToolStripMenuItem afficherLAdresseIPToolStripMenuItem;
        private ToolStripMenuItem robotToolStripMenuItem;
        private ToolStripMenuItem ethernetToolStripMenuItem;
        private ToolStripMenuItem hotspotToolStripMenuItem;
        private ToolStripMenuItem imageTestToolStripMenuItem;
        private ToolStripMenuItem exitApp;

        private GroupBox gbCamera;
        private Label lblNomCamera;
        private Label lblAdrIP;
        private Label lblConnectionCamera;

        private TextBox tbCom;
        private PictureBox pbImage;
        private System.Windows.Forms.Timer timAcq;

        private PictureBox btnSearchCamera;
        private PictureBox btnStartAcquisition;
        private PictureBox btnStopAcquisition;

        private SplitContainer mainSplit;
        private SplitContainer rightSplit;

        private FlowLayoutPanel flowButtons;
        private Label lblRobotState;
        private DataGridView dgvObjects;
        private TabControl tabControlBottom;
        private TabPage tabLogs;

        /// <summary>
        ///  Nettoyage des ressources utilisées.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        ///  Méthode requise pour la prise en charge du concepteur :
        ///  Ne modifiez pas le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.navBar = new System.Windows.Forms.MenuStrip();
            this.serveurTCPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startTCP = new System.Windows.Forms.ToolStripMenuItem();
            this.stopTCP = new System.Windows.Forms.ToolStripMenuItem();
            this.réseauToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NetworkInterfaceSelection = new System.Windows.Forms.ToolStripMenuItem();
            this.afficherLAdresseIPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.robotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ethernetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hotspotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jointsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.positionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitApp = new System.Windows.Forms.ToolStripMenuItem();
            this.gbCamera = new System.Windows.Forms.GroupBox();
            this.lblNomCamera = new System.Windows.Forms.Label();
            this.lblAdrIP = new System.Windows.Forms.Label();
            this.lblConnectionCamera = new System.Windows.Forms.Label();
            this.btnSearchCamera = new System.Windows.Forms.PictureBox();
            this.btnStartAcquisition = new System.Windows.Forms.PictureBox();
            this.btnStopAcquisition = new System.Windows.Forms.PictureBox();
            this.flowButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.lblRobotState = new System.Windows.Forms.Label();
            this.dgvObjects = new System.Windows.Forms.DataGridView();
            this.rightSplit = new System.Windows.Forms.SplitContainer();
            this.pbImage = new System.Windows.Forms.PictureBox();
            this.tabControlBottom = new System.Windows.Forms.TabControl();
            this.tabLogs = new System.Windows.Forms.TabPage();
            this.tbCom = new System.Windows.Forms.TextBox();
            this.timAcq = new System.Windows.Forms.Timer(this.components);
            this.mainSplit = new System.Windows.Forms.SplitContainer();
            this.navBar.SuspendLayout();
            this.gbCamera.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnSearchCamera)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnStartAcquisition)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnStopAcquisition)).BeginInit();
            this.flowButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvObjects)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightSplit)).BeginInit();
            this.rightSplit.Panel1.SuspendLayout();
            this.rightSplit.Panel2.SuspendLayout();
            this.rightSplit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).BeginInit();
            this.tabControlBottom.SuspendLayout();
            this.tabLogs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplit)).BeginInit();
            this.mainSplit.Panel1.SuspendLayout();
            this.mainSplit.Panel2.SuspendLayout();
            this.mainSplit.SuspendLayout();
            this.SuspendLayout();
            // 
            // navBar
            // 
            this.navBar.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.navBar.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.navBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.serveurTCPToolStripMenuItem,
            this.réseauToolStripMenuItem,
            this.imageTestToolStripMenuItem,
            this.exitApp});
            this.navBar.Location = new System.Drawing.Point(0, 0);
            this.navBar.Name = "navBar";
            this.navBar.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.navBar.Size = new System.Drawing.Size(1924, 40);
            this.navBar.TabIndex = 0;
            // 
            // serveurTCPToolStripMenuItem
            // 
            this.serveurTCPToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startTCP,
            this.stopTCP});
            this.serveurTCPToolStripMenuItem.Name = "serveurTCPToolStripMenuItem";
            this.serveurTCPToolStripMenuItem.Size = new System.Drawing.Size(162, 36);
            this.serveurTCPToolStripMenuItem.Text = "Serveur TCP";
            // 
            // startTCP
            // 
            this.startTCP.Name = "startTCP";
            this.startTCP.Size = new System.Drawing.Size(361, 44);
            this.startTCP.Text = "Démarrer le Serveur";
            this.startTCP.Click += new System.EventHandler(this.démarrerLeServeurToolStripMenuItem_Click);
            // 
            // stopTCP
            // 
            this.stopTCP.Name = "stopTCP";
            this.stopTCP.Size = new System.Drawing.Size(361, 44);
            this.stopTCP.Text = "Arrêter le Serveur";
            this.stopTCP.Click += new System.EventHandler(this.arrêterLeServeurToolStripMenuItem_Click);
            // 
            // réseauToolStripMenuItem
            // 
            this.réseauToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NetworkInterfaceSelection,
            this.afficherLAdresseIPToolStripMenuItem,
            this.robotToolStripMenuItem});
            this.réseauToolStripMenuItem.Name = "réseauToolStripMenuItem";
            this.réseauToolStripMenuItem.Size = new System.Drawing.Size(109, 36);
            this.réseauToolStripMenuItem.Text = "Réseau";
            // 
            // NetworkInterfaceSelection
            // 
            this.NetworkInterfaceSelection.Name = "NetworkInterfaceSelection";
            this.NetworkInterfaceSelection.Size = new System.Drawing.Size(473, 44);
            this.NetworkInterfaceSelection.Text = "Sélectionner une Carte Réseau";
            this.NetworkInterfaceSelection.Click += new System.EventHandler(this.sélectionnerUneCarteRéseauToolStripMenuItem_Click);
            // 
            // afficherLAdresseIPToolStripMenuItem
            // 
            this.afficherLAdresseIPToolStripMenuItem.Enabled = false;
            this.afficherLAdresseIPToolStripMenuItem.Name = "afficherLAdresseIPToolStripMenuItem";
            this.afficherLAdresseIPToolStripMenuItem.Size = new System.Drawing.Size(473, 44);
            this.afficherLAdresseIPToolStripMenuItem.Text = "Adresse IP :";
            // 
            // robotToolStripMenuItem
            // 
            this.robotToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ethernetToolStripMenuItem,
            this.hotspotToolStripMenuItem,
            this.jointsToolStripMenuItem,
            this.positionToolStripMenuItem});
            this.robotToolStripMenuItem.Name = "robotToolStripMenuItem";
            this.robotToolStripMenuItem.Size = new System.Drawing.Size(473, 44);
            this.robotToolStripMenuItem.Text = "Robot";
            // 
            // ethernetToolStripMenuItem
            // 
            this.ethernetToolStripMenuItem.Name = "ethernetToolStripMenuItem";
            this.ethernetToolStripMenuItem.Size = new System.Drawing.Size(237, 44);
            this.ethernetToolStripMenuItem.Text = "Ethernet";
            this.ethernetToolStripMenuItem.Click += new System.EventHandler(this.ethernetToolStripMenuItem_Click);
            // 
            // hotspotToolStripMenuItem
            // 
            this.hotspotToolStripMenuItem.Name = "hotspotToolStripMenuItem";
            this.hotspotToolStripMenuItem.Size = new System.Drawing.Size(237, 44);
            this.hotspotToolStripMenuItem.Text = "Hotspot";
            this.hotspotToolStripMenuItem.Click += new System.EventHandler(this.hotspotToolStripMenuItem_Click);
            // 
            // jointsToolStripMenuItem
            // 
            this.jointsToolStripMenuItem.Name = "jointsToolStripMenuItem";
            this.jointsToolStripMenuItem.Size = new System.Drawing.Size(237, 44);
            this.jointsToolStripMenuItem.Text = "Joints";
            this.jointsToolStripMenuItem.Click += new System.EventHandler(this.jointsToolStripMenuItem_Click);
            // 
            // positionToolStripMenuItem
            // 
            this.positionToolStripMenuItem.Name = "positionToolStripMenuItem";
            this.positionToolStripMenuItem.Size = new System.Drawing.Size(237, 44);
            this.positionToolStripMenuItem.Text = "Position";
            this.positionToolStripMenuItem.Click += new System.EventHandler(this.positionToolStripMenuItem_Click);
            // 
            // imageTestToolStripMenuItem
            // 
            this.imageTestToolStripMenuItem.Name = "imageTestToolStripMenuItem";
            this.imageTestToolStripMenuItem.Size = new System.Drawing.Size(100, 36);
            this.imageTestToolStripMenuItem.Text = "Image";
            this.imageTestToolStripMenuItem.Click += new System.EventHandler(this.imageTestToolStripMenuItem_Click);
            // 
            // exitApp
            // 
            this.exitApp.Name = "exitApp";
            this.exitApp.Size = new System.Drawing.Size(109, 36);
            this.exitApp.Text = "Quitter";
            this.exitApp.Click += new System.EventHandler(this.quitterToolStripMenuItem_Click);
            // 
            // gbCamera
            // 
            this.gbCamera.Controls.Add(this.lblNomCamera);
            this.gbCamera.Controls.Add(this.lblAdrIP);
            this.gbCamera.Controls.Add(this.lblConnectionCamera);
            this.gbCamera.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbCamera.Location = new System.Drawing.Point(0, 0);
            this.gbCamera.Margin = new System.Windows.Forms.Padding(4);
            this.gbCamera.Name = "gbCamera";
            this.gbCamera.Padding = new System.Windows.Forms.Padding(4);
            this.gbCamera.Size = new System.Drawing.Size(640, 125);
            this.gbCamera.TabIndex = 1;
            this.gbCamera.TabStop = false;
            this.gbCamera.Text = "Caméra";
            // 
            // lblNomCamera
            // 
            this.lblNomCamera.AutoSize = true;
            this.lblNomCamera.Location = new System.Drawing.Point(28, 90);
            this.lblNomCamera.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNomCamera.Name = "lblNomCamera";
            this.lblNomCamera.Size = new System.Drawing.Size(193, 25);
            this.lblNomCamera.TabIndex = 0;
            this.lblNomCamera.Text = "Caméra : inconnue";
            // 
            // lblAdrIP
            // 
            this.lblAdrIP.AutoSize = true;
            this.lblAdrIP.Location = new System.Drawing.Point(28, 62);
            this.lblAdrIP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAdrIP.Name = "lblAdrIP";
            this.lblAdrIP.Size = new System.Drawing.Size(200, 25);
            this.lblAdrIP.TabIndex = 1;
            this.lblAdrIP.Text = "Adresse IP : 0.0.0.0";
            // 
            // lblConnectionCamera
            // 
            this.lblConnectionCamera.AutoSize = true;
            this.lblConnectionCamera.BackColor = System.Drawing.Color.Red;
            this.lblConnectionCamera.Location = new System.Drawing.Point(28, 31);
            this.lblConnectionCamera.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblConnectionCamera.Name = "lblConnectionCamera";
            this.lblConnectionCamera.Size = new System.Drawing.Size(127, 25);
            this.lblConnectionCamera.TabIndex = 2;
            this.lblConnectionCamera.Text = "Déconnecté";
            // 
            // btnSearchCamera
            // 
            this.btnSearchCamera.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSearchCamera.Image = global::Serveur.Properties.Resources.search;
            this.btnSearchCamera.Location = new System.Drawing.Point(12, 10);
            this.btnSearchCamera.Margin = new System.Windows.Forms.Padding(4);
            this.btnSearchCamera.Name = "btnSearchCamera";
            this.btnSearchCamera.Size = new System.Drawing.Size(64, 60);
            this.btnSearchCamera.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnSearchCamera.TabIndex = 0;
            this.btnSearchCamera.TabStop = false;
            this.btnSearchCamera.Click += new System.EventHandler(this.btnSearchCamera_Click);
            // 
            // btnStartAcquisition
            // 
            this.btnStartAcquisition.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStartAcquisition.Image = global::Serveur.Properties.Resources.play;
            this.btnStartAcquisition.Location = new System.Drawing.Point(84, 10);
            this.btnStartAcquisition.Margin = new System.Windows.Forms.Padding(4);
            this.btnStartAcquisition.Name = "btnStartAcquisition";
            this.btnStartAcquisition.Size = new System.Drawing.Size(64, 60);
            this.btnStartAcquisition.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnStartAcquisition.TabIndex = 1;
            this.btnStartAcquisition.TabStop = false;
            this.btnStartAcquisition.Click += new System.EventHandler(this.btnStartAcquisition_Click);
            // 
            // btnStopAcquisition
            // 
            this.btnStopAcquisition.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStopAcquisition.Image = global::Serveur.Properties.Resources.stop;
            this.btnStopAcquisition.Location = new System.Drawing.Point(156, 10);
            this.btnStopAcquisition.Margin = new System.Windows.Forms.Padding(4);
            this.btnStopAcquisition.Name = "btnStopAcquisition";
            this.btnStopAcquisition.Size = new System.Drawing.Size(64, 60);
            this.btnStopAcquisition.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnStopAcquisition.TabIndex = 2;
            this.btnStopAcquisition.TabStop = false;
            this.btnStopAcquisition.Click += new System.EventHandler(this.btnStopAcquisition_Click);
            // 
            // flowButtons
            // 
            this.flowButtons.AutoSize = true;
            this.flowButtons.Controls.Add(this.btnSearchCamera);
            this.flowButtons.Controls.Add(this.btnStartAcquisition);
            this.flowButtons.Controls.Add(this.btnStopAcquisition);
            this.flowButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowButtons.Location = new System.Drawing.Point(0, 125);
            this.flowButtons.Margin = new System.Windows.Forms.Padding(4);
            this.flowButtons.Name = "flowButtons";
            this.flowButtons.Padding = new System.Windows.Forms.Padding(8, 6, 8, 6);
            this.flowButtons.Size = new System.Drawing.Size(640, 80);
            this.flowButtons.TabIndex = 2;
            // 
            // lblRobotState
            // 
            this.lblRobotState.AutoSize = true;
            this.lblRobotState.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lblRobotState.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblRobotState.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblRobotState.ForeColor = System.Drawing.Color.Black;
            this.lblRobotState.Location = new System.Drawing.Point(0, 205);
            this.lblRobotState.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRobotState.Name = "lblRobotState";
            this.lblRobotState.Padding = new System.Windows.Forms.Padding(12, 10, 12, 10);
            this.lblRobotState.Size = new System.Drawing.Size(295, 57);
            this.lblRobotState.TabIndex = 1;
            this.lblRobotState.Text = "État Robot: Inconnu";
            // 
            // dgvObjects
            // 
            this.dgvObjects.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvObjects.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvObjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvObjects.Location = new System.Drawing.Point(0, 262);
            this.dgvObjects.Margin = new System.Windows.Forms.Padding(4);
            this.dgvObjects.Name = "dgvObjects";
            this.dgvObjects.RowHeadersWidth = 82;
            this.dgvObjects.Size = new System.Drawing.Size(640, 760);
            this.dgvObjects.TabIndex = 0;
            // 
            // rightSplit
            // 
            this.rightSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightSplit.Location = new System.Drawing.Point(0, 0);
            this.rightSplit.Margin = new System.Windows.Forms.Padding(4);
            this.rightSplit.Name = "rightSplit";
            this.rightSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // rightSplit.Panel1
            // 
            this.rightSplit.Panel1.Controls.Add(this.pbImage);
            // 
            // rightSplit.Panel2
            // 
            this.rightSplit.Panel2.Controls.Add(this.tabControlBottom);
            this.rightSplit.Size = new System.Drawing.Size(1280, 1022);
            this.rightSplit.SplitterDistance = 511;
            this.rightSplit.SplitterWidth = 6;
            this.rightSplit.TabIndex = 0;
            // 
            // pbImage
            // 
            this.pbImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbImage.Location = new System.Drawing.Point(0, 0);
            this.pbImage.Margin = new System.Windows.Forms.Padding(4);
            this.pbImage.Name = "pbImage";
            this.pbImage.Size = new System.Drawing.Size(1280, 511);
            this.pbImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbImage.TabIndex = 0;
            this.pbImage.TabStop = false;
            // 
            // tabControlBottom
            // 
            this.tabControlBottom.Controls.Add(this.tabLogs);
            this.tabControlBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlBottom.Location = new System.Drawing.Point(0, 0);
            this.tabControlBottom.Margin = new System.Windows.Forms.Padding(4);
            this.tabControlBottom.Name = "tabControlBottom";
            this.tabControlBottom.SelectedIndex = 0;
            this.tabControlBottom.Size = new System.Drawing.Size(1280, 505);
            this.tabControlBottom.TabIndex = 0;
            // 
            // tabLogs
            // 
            this.tabLogs.Controls.Add(this.tbCom);
            this.tabLogs.Location = new System.Drawing.Point(8, 39);
            this.tabLogs.Margin = new System.Windows.Forms.Padding(4);
            this.tabLogs.Name = "tabLogs";
            this.tabLogs.Padding = new System.Windows.Forms.Padding(12, 10, 12, 10);
            this.tabLogs.Size = new System.Drawing.Size(1264, 458);
            this.tabLogs.TabIndex = 0;
            this.tabLogs.Text = "Logs";
            // 
            // tbCom
            // 
            this.tbCom.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbCom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbCom.Location = new System.Drawing.Point(12, 10);
            this.tbCom.Margin = new System.Windows.Forms.Padding(4);
            this.tbCom.Multiline = true;
            this.tbCom.Name = "tbCom";
            this.tbCom.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbCom.Size = new System.Drawing.Size(1240, 438);
            this.tbCom.TabIndex = 0;
            // 
            // timAcq
            // 
            this.timAcq.Interval = 20;
            this.timAcq.Tick += new System.EventHandler(this.timAcq_Tick);
            // 
            // mainSplit
            // 
            this.mainSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplit.Location = new System.Drawing.Point(0, 40);
            this.mainSplit.Margin = new System.Windows.Forms.Padding(4);
            this.mainSplit.Name = "mainSplit";
            // 
            // mainSplit.Panel1
            // 
            this.mainSplit.Panel1.Controls.Add(this.dgvObjects);
            this.mainSplit.Panel1.Controls.Add(this.lblRobotState);
            this.mainSplit.Panel1.Controls.Add(this.flowButtons);
            this.mainSplit.Panel1.Controls.Add(this.gbCamera);
            // 
            // mainSplit.Panel2
            // 
            this.mainSplit.Panel2.Controls.Add(this.rightSplit);
            this.mainSplit.Size = new System.Drawing.Size(1924, 1022);
            this.mainSplit.SplitterDistance = 640;
            this.mainSplit.TabIndex = 0;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1924, 1062);
            this.Controls.Add(this.mainSplit);
            this.Controls.Add(this.navBar);
            this.MainMenuStrip = this.navBar;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Main";
            this.Text = "Couleur - Serveur";
            this.Load += new System.EventHandler(this.Main_Load);
            this.navBar.ResumeLayout(false);
            this.navBar.PerformLayout();
            this.gbCamera.ResumeLayout(false);
            this.gbCamera.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnSearchCamera)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnStartAcquisition)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnStopAcquisition)).EndInit();
            this.flowButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvObjects)).EndInit();
            this.rightSplit.Panel1.ResumeLayout(false);
            this.rightSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rightSplit)).EndInit();
            this.rightSplit.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).EndInit();
            this.tabControlBottom.ResumeLayout(false);
            this.tabLogs.ResumeLayout(false);
            this.tabLogs.PerformLayout();
            this.mainSplit.Panel1.ResumeLayout(false);
            this.mainSplit.Panel1.PerformLayout();
            this.mainSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainSplit)).EndInit();
            this.mainSplit.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ToolStripMenuItem jointsToolStripMenuItem;
        private ToolStripMenuItem positionToolStripMenuItem;
    }
}
