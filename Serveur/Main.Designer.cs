using System;
using System.Drawing;
using System.Windows.Forms;

namespace Serveur
{
    partial class Main
    {
        private System.ComponentModel.IContainer components = null;

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

        private TabPage RobotPage;
        private TableLayoutPanel tableLayoutRobot;

        private GroupBox gbJointInfo;
        private Label lblJoint1Position;
        private Label lblJoint2Position;
        private Label lblJoint3Position;
        private Label lblJoint4Position;
        private Label lblJoint5Position;
        private Label lblJoint6Position;

        private GroupBox gbRealPosition;
        private Label lblX;
        private Label lblY;
        private Label lblZ;
        private Label lblRoll;
        private Label lblPitch;
        private Label lblYaw;

        private GroupBox gbReferencePoints;
        private ListBox lbReferencePoints;
        private Button calibrationButton;
        private Label calibrationStatus;

        private GroupBox gbCalibration;
        private Button moveRobotTest;

        /// <summary>
        /// Nettoyage des ressources utilisées.
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
        /// Méthode requise pour la prise en charge du concepteur :
        /// Ne modifiez pas le contenu de cette méthode avec l'éditeur de code.
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
            this.imageTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitApp = new System.Windows.Forms.ToolStripMenuItem();
            this.gbCamera = new System.Windows.Forms.GroupBox();
            this.btnSearchCamera = new System.Windows.Forms.PictureBox();
            this.btnStartAcquisition = new System.Windows.Forms.PictureBox();
            this.lblNomCamera = new System.Windows.Forms.Label();
            this.btnStopAcquisition = new System.Windows.Forms.PictureBox();
            this.lblAdrIP = new System.Windows.Forms.Label();
            this.lblConnectionCamera = new System.Windows.Forms.Label();
            this.flowButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.lblRobotState = new System.Windows.Forms.Label();
            this.dgvObjects = new System.Windows.Forms.DataGridView();
            this.rightSplit = new System.Windows.Forms.SplitContainer();
            this.pbImage = new System.Windows.Forms.PictureBox();
            this.tabControlBottom = new System.Windows.Forms.TabControl();
            this.RobotPage = new System.Windows.Forms.TabPage();
            this.tableLayoutRobot = new System.Windows.Forms.TableLayoutPanel();
            this.gbJointInfo = new System.Windows.Forms.GroupBox();
            this.lblJoint1Position = new System.Windows.Forms.Label();
            this.lblJoint2Position = new System.Windows.Forms.Label();
            this.lblJoint3Position = new System.Windows.Forms.Label();
            this.lblJoint4Position = new System.Windows.Forms.Label();
            this.lblJoint5Position = new System.Windows.Forms.Label();
            this.lblJoint6Position = new System.Windows.Forms.Label();
            this.gbRealPosition = new System.Windows.Forms.GroupBox();
            this.lblX = new System.Windows.Forms.Label();
            this.lblY = new System.Windows.Forms.Label();
            this.lblZ = new System.Windows.Forms.Label();
            this.lblRoll = new System.Windows.Forms.Label();
            this.lblPitch = new System.Windows.Forms.Label();
            this.lblYaw = new System.Windows.Forms.Label();
            this.gbCalibration = new System.Windows.Forms.GroupBox();
            this.moveRobotTest = new System.Windows.Forms.Button();
            this.calibrationStatus = new System.Windows.Forms.Label();
            this.calibrationButton = new System.Windows.Forms.Button();
            this.gbReferencePoints = new System.Windows.Forms.GroupBox();
            this.CalibRef2Butt = new System.Windows.Forms.Button();
            this.CalibRef1Butt = new System.Windows.Forms.Button();
            this.lbReferencePoints = new System.Windows.Forms.ListBox();
            this.tabLogs = new System.Windows.Forms.TabPage();
            this.tbCom = new System.Windows.Forms.TextBox();
            this.timAcq = new System.Windows.Forms.Timer(this.components);
            this.mainSplit = new System.Windows.Forms.SplitContainer();
            this.navBar.SuspendLayout();
            this.gbCamera.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnSearchCamera)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnStartAcquisition)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnStopAcquisition)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvObjects)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightSplit)).BeginInit();
            this.rightSplit.Panel1.SuspendLayout();
            this.rightSplit.Panel2.SuspendLayout();
            this.rightSplit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).BeginInit();
            this.tabControlBottom.SuspendLayout();
            this.RobotPage.SuspendLayout();
            this.tableLayoutRobot.SuspendLayout();
            this.gbJointInfo.SuspendLayout();
            this.gbRealPosition.SuspendLayout();
            this.gbCalibration.SuspendLayout();
            this.gbReferencePoints.SuspendLayout();
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
            this.navBar.Size = new System.Drawing.Size(1686, 42);
            this.navBar.TabIndex = 0;
            // 
            // serveurTCPToolStripMenuItem
            // 
            this.serveurTCPToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startTCP,
            this.stopTCP});
            this.serveurTCPToolStripMenuItem.Name = "serveurTCPToolStripMenuItem";
            this.serveurTCPToolStripMenuItem.Size = new System.Drawing.Size(162, 38);
            this.serveurTCPToolStripMenuItem.Text = "Serveur TCP";
            // 
            // startTCP
            // 
            this.startTCP.Name = "startTCP";
            this.startTCP.Size = new System.Drawing.Size(361, 44);
            this.startTCP.Text = "Démarrer le Serveur";
            this.startTCP.Click += new System.EventHandler(this.startTCP_Click);
            // 
            // stopTCP
            // 
            this.stopTCP.Name = "stopTCP";
            this.stopTCP.Size = new System.Drawing.Size(361, 44);
            this.stopTCP.Text = "Arrêter le Serveur";
            this.stopTCP.Click += new System.EventHandler(this.stopTCP_Click);
            // 
            // réseauToolStripMenuItem
            // 
            this.réseauToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NetworkInterfaceSelection,
            this.afficherLAdresseIPToolStripMenuItem,
            this.robotToolStripMenuItem});
            this.réseauToolStripMenuItem.Name = "réseauToolStripMenuItem";
            this.réseauToolStripMenuItem.Size = new System.Drawing.Size(109, 38);
            this.réseauToolStripMenuItem.Text = "Réseau";
            // 
            // NetworkInterfaceSelection
            // 
            this.NetworkInterfaceSelection.Name = "NetworkInterfaceSelection";
            this.NetworkInterfaceSelection.Size = new System.Drawing.Size(473, 44);
            this.NetworkInterfaceSelection.Text = "Sélectionner une Carte Réseau";
            this.NetworkInterfaceSelection.Click += new System.EventHandler(this.NetworkInterfaceSelection_Click);
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
            this.hotspotToolStripMenuItem});
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
            // imageTestToolStripMenuItem
            // 
            this.imageTestToolStripMenuItem.Name = "imageTestToolStripMenuItem";
            this.imageTestToolStripMenuItem.Size = new System.Drawing.Size(100, 38);
            this.imageTestToolStripMenuItem.Text = "Image";
            this.imageTestToolStripMenuItem.Click += new System.EventHandler(this.imageTestToolStripMenuItem_Click);
            // 
            // exitApp
            // 
            this.exitApp.Name = "exitApp";
            this.exitApp.Size = new System.Drawing.Size(109, 38);
            this.exitApp.Text = "Quitter";
            this.exitApp.Click += new System.EventHandler(this.quitterToolStripMenuItem_Click);
            // 
            // gbCamera
            // 
            this.gbCamera.Controls.Add(this.btnSearchCamera);
            this.gbCamera.Controls.Add(this.btnStartAcquisition);
            this.gbCamera.Controls.Add(this.lblNomCamera);
            this.gbCamera.Controls.Add(this.btnStopAcquisition);
            this.gbCamera.Controls.Add(this.lblAdrIP);
            this.gbCamera.Controls.Add(this.lblConnectionCamera);
            this.gbCamera.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbCamera.Location = new System.Drawing.Point(0, 0);
            this.gbCamera.Margin = new System.Windows.Forms.Padding(4);
            this.gbCamera.Name = "gbCamera";
            this.gbCamera.Padding = new System.Windows.Forms.Padding(4);
            this.gbCamera.Size = new System.Drawing.Size(667, 195);
            this.gbCamera.TabIndex = 1;
            this.gbCamera.TabStop = false;
            this.gbCamera.Text = "Caméra";
            // 
            // btnSearchCamera
            // 
            this.btnSearchCamera.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSearchCamera.Image = global::Serveur.Properties.Resources.search;
            this.btnSearchCamera.Location = new System.Drawing.Point(13, 122);
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
            this.btnStartAcquisition.Location = new System.Drawing.Point(85, 122);
            this.btnStartAcquisition.Margin = new System.Windows.Forms.Padding(4);
            this.btnStartAcquisition.Name = "btnStartAcquisition";
            this.btnStartAcquisition.Size = new System.Drawing.Size(64, 60);
            this.btnStartAcquisition.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnStartAcquisition.TabIndex = 1;
            this.btnStartAcquisition.TabStop = false;
            this.btnStartAcquisition.Click += new System.EventHandler(this.btnStartAcquisition_Click);
            // 
            // lblNomCamera
            // 
            this.lblNomCamera.AutoSize = true;
            this.lblNomCamera.Font = new System.Drawing.Font("Segoe UI", 7F);
            this.lblNomCamera.Location = new System.Drawing.Point(366, 65);
            this.lblNomCamera.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNomCamera.Name = "lblNomCamera";
            this.lblNomCamera.Size = new System.Drawing.Size(170, 25);
            this.lblNomCamera.TabIndex = 0;
            this.lblNomCamera.Text = "Caméra : inconnue";
            // 
            // btnStopAcquisition
            // 
            this.btnStopAcquisition.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStopAcquisition.Image = global::Serveur.Properties.Resources.stop;
            this.btnStopAcquisition.Location = new System.Drawing.Point(157, 122);
            this.btnStopAcquisition.Margin = new System.Windows.Forms.Padding(4);
            this.btnStopAcquisition.Name = "btnStopAcquisition";
            this.btnStopAcquisition.Size = new System.Drawing.Size(64, 60);
            this.btnStopAcquisition.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnStopAcquisition.TabIndex = 2;
            this.btnStopAcquisition.TabStop = false;
            this.btnStopAcquisition.Click += new System.EventHandler(this.btnStopAcquisition_Click);
            // 
            // lblAdrIP
            // 
            this.lblAdrIP.AutoSize = true;
            this.lblAdrIP.Font = new System.Drawing.Font("Segoe UI", 7F);
            this.lblAdrIP.Location = new System.Drawing.Point(366, 129);
            this.lblAdrIP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAdrIP.Name = "lblAdrIP";
            this.lblAdrIP.Size = new System.Drawing.Size(165, 25);
            this.lblAdrIP.TabIndex = 1;
            this.lblAdrIP.Text = "Adresse IP : 0.0.0.0";
            // 
            // lblConnectionCamera
            // 
            this.lblConnectionCamera.AutoSize = true;
            this.lblConnectionCamera.BackColor = System.Drawing.Color.Red;
            this.lblConnectionCamera.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblConnectionCamera.ForeColor = System.Drawing.Color.White;
            this.lblConnectionCamera.Location = new System.Drawing.Point(29, 48);
            this.lblConnectionCamera.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblConnectionCamera.Name = "lblConnectionCamera";
            this.lblConnectionCamera.Padding = new System.Windows.Forms.Padding(6);
            this.lblConnectionCamera.Size = new System.Drawing.Size(180, 49);
            this.lblConnectionCamera.TabIndex = 2;
            this.lblConnectionCamera.Text = "Déconnecté";
            // 
            // flowButtons
            // 
            this.flowButtons.AutoSize = true;
            this.flowButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowButtons.Location = new System.Drawing.Point(0, 195);
            this.flowButtons.Margin = new System.Windows.Forms.Padding(4);
            this.flowButtons.Name = "flowButtons";
            this.flowButtons.Padding = new System.Windows.Forms.Padding(8, 6, 8, 6);
            this.flowButtons.Size = new System.Drawing.Size(667, 12);
            this.flowButtons.TabIndex = 2;
            // 
            // lblRobotState
            // 
            this.lblRobotState.AutoSize = true;
            this.lblRobotState.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lblRobotState.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblRobotState.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblRobotState.ForeColor = System.Drawing.Color.Black;
            this.lblRobotState.Location = new System.Drawing.Point(0, 207);
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
            this.dgvObjects.Location = new System.Drawing.Point(0, 264);
            this.dgvObjects.Margin = new System.Windows.Forms.Padding(4);
            this.dgvObjects.Name = "dgvObjects";
            this.dgvObjects.RowHeadersWidth = 82;
            this.dgvObjects.Size = new System.Drawing.Size(667, 821);
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
            this.rightSplit.Size = new System.Drawing.Size(1015, 1085);
            this.rightSplit.SplitterDistance = 515;
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
            this.pbImage.Size = new System.Drawing.Size(1015, 515);
            this.pbImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbImage.TabIndex = 0;
            this.pbImage.TabStop = false;
            // 
            // tabControlBottom
            // 
            this.tabControlBottom.Controls.Add(this.RobotPage);
            this.tabControlBottom.Controls.Add(this.tabLogs);
            this.tabControlBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlBottom.Location = new System.Drawing.Point(0, 0);
            this.tabControlBottom.Margin = new System.Windows.Forms.Padding(4);
            this.tabControlBottom.Name = "tabControlBottom";
            this.tabControlBottom.SelectedIndex = 0;
            this.tabControlBottom.Size = new System.Drawing.Size(1015, 564);
            this.tabControlBottom.TabIndex = 0;
            // 
            // RobotPage
            // 
            this.RobotPage.Controls.Add(this.tableLayoutRobot);
            this.RobotPage.Location = new System.Drawing.Point(8, 39);
            this.RobotPage.Name = "RobotPage";
            this.RobotPage.Padding = new System.Windows.Forms.Padding(3);
            this.RobotPage.Size = new System.Drawing.Size(999, 517);
            this.RobotPage.TabIndex = 1;
            this.RobotPage.Text = "Robot";
            this.RobotPage.UseVisualStyleBackColor = true;
            // 
            // tableLayoutRobot
            // 
            this.tableLayoutRobot.ColumnCount = 2;
            this.tableLayoutRobot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutRobot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutRobot.Controls.Add(this.gbJointInfo, 0, 0);
            this.tableLayoutRobot.Controls.Add(this.gbRealPosition, 1, 0);
            this.tableLayoutRobot.Controls.Add(this.gbCalibration, 0, 1);
            this.tableLayoutRobot.Controls.Add(this.gbReferencePoints, 1, 1);
            this.tableLayoutRobot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutRobot.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutRobot.Name = "tableLayoutRobot";
            this.tableLayoutRobot.RowCount = 2;
            this.tableLayoutRobot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutRobot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutRobot.Size = new System.Drawing.Size(993, 511);
            this.tableLayoutRobot.TabIndex = 0;
            // 
            // gbJointInfo
            // 
            this.gbJointInfo.Controls.Add(this.lblJoint1Position);
            this.gbJointInfo.Controls.Add(this.lblJoint2Position);
            this.gbJointInfo.Controls.Add(this.lblJoint3Position);
            this.gbJointInfo.Controls.Add(this.lblJoint4Position);
            this.gbJointInfo.Controls.Add(this.lblJoint5Position);
            this.gbJointInfo.Controls.Add(this.lblJoint6Position);
            this.gbJointInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbJointInfo.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.gbJointInfo.Location = new System.Drawing.Point(3, 3);
            this.gbJointInfo.Name = "gbJointInfo";
            this.gbJointInfo.Padding = new System.Windows.Forms.Padding(5);
            this.gbJointInfo.Size = new System.Drawing.Size(490, 198);
            this.gbJointInfo.TabIndex = 2;
            this.gbJointInfo.TabStop = false;
            this.gbJointInfo.Text = "Informations des Joints";
            // 
            // lblJoint1Position
            // 
            this.lblJoint1Position.AutoSize = true;
            this.lblJoint1Position.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblJoint1Position.Location = new System.Drawing.Point(19, 60);
            this.lblJoint1Position.Name = "lblJoint1Position";
            this.lblJoint1Position.Size = new System.Drawing.Size(194, 30);
            this.lblJoint1Position.TabIndex = 0;
            this.lblJoint1Position.Text = "Joint 1 Position : 0°";
            // 
            // lblJoint2Position
            // 
            this.lblJoint2Position.AutoSize = true;
            this.lblJoint2Position.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblJoint2Position.Location = new System.Drawing.Point(19, 100);
            this.lblJoint2Position.Name = "lblJoint2Position";
            this.lblJoint2Position.Size = new System.Drawing.Size(194, 30);
            this.lblJoint2Position.TabIndex = 1;
            this.lblJoint2Position.Text = "Joint 2 Position : 0°";
            // 
            // lblJoint3Position
            // 
            this.lblJoint3Position.AutoSize = true;
            this.lblJoint3Position.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblJoint3Position.Location = new System.Drawing.Point(19, 140);
            this.lblJoint3Position.Name = "lblJoint3Position";
            this.lblJoint3Position.Size = new System.Drawing.Size(194, 30);
            this.lblJoint3Position.TabIndex = 2;
            this.lblJoint3Position.Text = "Joint 3 Position : 0°";
            // 
            // lblJoint4Position
            // 
            this.lblJoint4Position.AutoSize = true;
            this.lblJoint4Position.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblJoint4Position.Location = new System.Drawing.Point(279, 60);
            this.lblJoint4Position.Name = "lblJoint4Position";
            this.lblJoint4Position.Size = new System.Drawing.Size(194, 30);
            this.lblJoint4Position.TabIndex = 3;
            this.lblJoint4Position.Text = "Joint 4 Position : 0°";
            // 
            // lblJoint5Position
            // 
            this.lblJoint5Position.AutoSize = true;
            this.lblJoint5Position.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblJoint5Position.Location = new System.Drawing.Point(279, 100);
            this.lblJoint5Position.Name = "lblJoint5Position";
            this.lblJoint5Position.Size = new System.Drawing.Size(194, 30);
            this.lblJoint5Position.TabIndex = 4;
            this.lblJoint5Position.Text = "Joint 5 Position : 0°";
            // 
            // lblJoint6Position
            // 
            this.lblJoint6Position.AutoSize = true;
            this.lblJoint6Position.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblJoint6Position.Location = new System.Drawing.Point(279, 140);
            this.lblJoint6Position.Name = "lblJoint6Position";
            this.lblJoint6Position.Size = new System.Drawing.Size(194, 30);
            this.lblJoint6Position.TabIndex = 5;
            this.lblJoint6Position.Text = "Joint 6 Position : 0°";
            // 
            // gbRealPosition
            // 
            this.gbRealPosition.Controls.Add(this.lblX);
            this.gbRealPosition.Controls.Add(this.lblY);
            this.gbRealPosition.Controls.Add(this.lblZ);
            this.gbRealPosition.Controls.Add(this.lblRoll);
            this.gbRealPosition.Controls.Add(this.lblPitch);
            this.gbRealPosition.Controls.Add(this.lblYaw);
            this.gbRealPosition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbRealPosition.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.gbRealPosition.Location = new System.Drawing.Point(499, 3);
            this.gbRealPosition.Name = "gbRealPosition";
            this.gbRealPosition.Padding = new System.Windows.Forms.Padding(5);
            this.gbRealPosition.Size = new System.Drawing.Size(491, 198);
            this.gbRealPosition.TabIndex = 3;
            this.gbRealPosition.TabStop = false;
            this.gbRealPosition.Text = "Positions Réelles";
            // 
            // lblX
            // 
            this.lblX.AutoSize = true;
            this.lblX.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblX.Location = new System.Drawing.Point(52, 51);
            this.lblX.Name = "lblX";
            this.lblX.Size = new System.Drawing.Size(128, 30);
            this.lblX.TabIndex = 0;
            this.lblX.Text = "X : 0.00 mm";
            // 
            // lblY
            // 
            this.lblY.AutoSize = true;
            this.lblY.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblY.Location = new System.Drawing.Point(52, 91);
            this.lblY.Name = "lblY";
            this.lblY.Size = new System.Drawing.Size(127, 30);
            this.lblY.TabIndex = 1;
            this.lblY.Text = "Y : 0.00 mm";
            // 
            // lblZ
            // 
            this.lblZ.AutoSize = true;
            this.lblZ.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblZ.Location = new System.Drawing.Point(52, 131);
            this.lblZ.Name = "lblZ";
            this.lblZ.Size = new System.Drawing.Size(128, 30);
            this.lblZ.TabIndex = 2;
            this.lblZ.Text = "Z : 0.00 mm";
            // 
            // lblRoll
            // 
            this.lblRoll.AutoSize = true;
            this.lblRoll.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblRoll.Location = new System.Drawing.Point(298, 51);
            this.lblRoll.Name = "lblRoll";
            this.lblRoll.Size = new System.Drawing.Size(126, 30);
            this.lblRoll.TabIndex = 3;
            this.lblRoll.Text = "Roll :   0.00°";
            // 
            // lblPitch
            // 
            this.lblPitch.AutoSize = true;
            this.lblPitch.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblPitch.Location = new System.Drawing.Point(298, 91);
            this.lblPitch.Name = "lblPitch";
            this.lblPitch.Size = new System.Drawing.Size(125, 30);
            this.lblPitch.TabIndex = 4;
            this.lblPitch.Text = "Pitch : 0.00°";
            // 
            // lblYaw
            // 
            this.lblYaw.AutoSize = true;
            this.lblYaw.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblYaw.Location = new System.Drawing.Point(298, 131);
            this.lblYaw.Name = "lblYaw";
            this.lblYaw.Size = new System.Drawing.Size(122, 30);
            this.lblYaw.TabIndex = 5;
            this.lblYaw.Text = "Yaw :  0.00°";
            // 
            // gbCalibration
            // 
            this.gbCalibration.Controls.Add(this.moveRobotTest);
            this.gbCalibration.Controls.Add(this.calibrationStatus);
            this.gbCalibration.Controls.Add(this.calibrationButton);
            this.gbCalibration.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbCalibration.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.gbCalibration.Location = new System.Drawing.Point(3, 207);
            this.gbCalibration.Name = "gbCalibration";
            this.gbCalibration.Padding = new System.Windows.Forms.Padding(10);
            this.gbCalibration.Size = new System.Drawing.Size(490, 301);
            this.gbCalibration.TabIndex = 6;
            this.gbCalibration.TabStop = false;
            this.gbCalibration.Text = "Calibration";
            // 
            // moveRobotTest
            // 
            this.moveRobotTest.Location = new System.Drawing.Point(170, 196);
            this.moveRobotTest.Name = "moveRobotTest";
            this.moveRobotTest.Size = new System.Drawing.Size(181, 60);
            this.moveRobotTest.TabIndex = 6;
            this.moveRobotTest.Text = "Test Mouvement";
            this.moveRobotTest.UseVisualStyleBackColor = true;
            this.moveRobotTest.Click += new System.EventHandler(this.moveRobotTest_Click);
            // 
            // calibrationStatus
            // 
            this.calibrationStatus.AutoSize = true;
            this.calibrationStatus.BackColor = System.Drawing.Color.Red;
            this.calibrationStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.calibrationStatus.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.calibrationStatus.ForeColor = System.Drawing.Color.White;
            this.calibrationStatus.Location = new System.Drawing.Point(194, 39);
            this.calibrationStatus.Name = "calibrationStatus";
            this.calibrationStatus.Padding = new System.Windows.Forms.Padding(10);
            this.calibrationStatus.Size = new System.Drawing.Size(157, 52);
            this.calibrationStatus.TabIndex = 5;
            this.calibrationStatus.Text = "Déconnecté";
            // 
            // calibrationButton
            // 
            this.calibrationButton.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.calibrationButton.Location = new System.Drawing.Point(151, 115);
            this.calibrationButton.Name = "calibrationButton";
            this.calibrationButton.Size = new System.Drawing.Size(231, 50);
            this.calibrationButton.TabIndex = 2;
            this.calibrationButton.Text = "Lancer Calibration";
            this.calibrationButton.UseVisualStyleBackColor = true;
            this.calibrationButton.Click += new System.EventHandler(this.calibrationButton_Click);
            // 
            // gbReferencePoints
            // 
            this.gbReferencePoints.Controls.Add(this.CalibRef2Butt);
            this.gbReferencePoints.Controls.Add(this.CalibRef1Butt);
            this.gbReferencePoints.Controls.Add(this.lbReferencePoints);
            this.gbReferencePoints.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbReferencePoints.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.gbReferencePoints.Location = new System.Drawing.Point(499, 207);
            this.gbReferencePoints.Name = "gbReferencePoints";
            this.gbReferencePoints.Size = new System.Drawing.Size(491, 301);
            this.gbReferencePoints.TabIndex = 4;
            this.gbReferencePoints.TabStop = false;
            this.gbReferencePoints.Text = "Points de Référence";
            // 
            // CalibRef2Butt
            // 
            this.CalibRef2Butt.Location = new System.Drawing.Point(279, 67);
            this.CalibRef2Butt.Name = "CalibRef2Butt";
            this.CalibRef2Butt.Size = new System.Drawing.Size(167, 58);
            this.CalibRef2Butt.TabIndex = 6;
            this.CalibRef2Butt.Text = "Réf 2";
            this.CalibRef2Butt.UseVisualStyleBackColor = true;
            this.CalibRef2Butt.Click += new System.EventHandler(this.CalibRef2Butt_Click);
            // 
            // CalibRef1Butt
            // 
            this.CalibRef1Butt.Location = new System.Drawing.Point(26, 67);
            this.CalibRef1Butt.Name = "CalibRef1Butt";
            this.CalibRef1Butt.Size = new System.Drawing.Size(167, 58);
            this.CalibRef1Butt.TabIndex = 5;
            this.CalibRef1Butt.Text = "Réf 1";
            this.CalibRef1Butt.UseVisualStyleBackColor = true;
            this.CalibRef1Butt.Click += new System.EventHandler(this.CalibRef1Butt_Click);
            // 
            // lbReferencePoints
            // 
            this.lbReferencePoints.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lbReferencePoints.FormattingEnabled = true;
            this.lbReferencePoints.ItemHeight = 30;
            this.lbReferencePoints.Location = new System.Drawing.Point(13, 140);
            this.lbReferencePoints.Name = "lbReferencePoints";
            this.lbReferencePoints.Size = new System.Drawing.Size(460, 154);
            this.lbReferencePoints.TabIndex = 4;
            // 
            // tabLogs
            // 
            this.tabLogs.Controls.Add(this.tbCom);
            this.tabLogs.Location = new System.Drawing.Point(8, 39);
            this.tabLogs.Margin = new System.Windows.Forms.Padding(4);
            this.tabLogs.Name = "tabLogs";
            this.tabLogs.Padding = new System.Windows.Forms.Padding(12, 10, 12, 10);
            this.tabLogs.Size = new System.Drawing.Size(999, 513);
            this.tabLogs.TabIndex = 0;
            this.tabLogs.Text = "Logs";
            this.tabLogs.UseVisualStyleBackColor = true;
            // 
            // tbCom
            // 
            this.tbCom.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbCom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbCom.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.tbCom.Location = new System.Drawing.Point(12, 10);
            this.tbCom.Margin = new System.Windows.Forms.Padding(4);
            this.tbCom.Multiline = true;
            this.tbCom.Name = "tbCom";
            this.tbCom.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbCom.Size = new System.Drawing.Size(975, 493);
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
            this.mainSplit.Location = new System.Drawing.Point(0, 42);
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
            this.mainSplit.Size = new System.Drawing.Size(1686, 1085);
            this.mainSplit.SplitterDistance = 667;
            this.mainSplit.TabIndex = 0;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1686, 1127);
            this.Controls.Add(this.mainSplit);
            this.Controls.Add(this.navBar);
            this.MainMenuStrip = this.navBar;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(1300, 900);
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
            ((System.ComponentModel.ISupportInitialize)(this.dgvObjects)).EndInit();
            this.rightSplit.Panel1.ResumeLayout(false);
            this.rightSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rightSplit)).EndInit();
            this.rightSplit.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).EndInit();
            this.tabControlBottom.ResumeLayout(false);
            this.RobotPage.ResumeLayout(false);
            this.tableLayoutRobot.ResumeLayout(false);
            this.gbJointInfo.ResumeLayout(false);
            this.gbJointInfo.PerformLayout();
            this.gbRealPosition.ResumeLayout(false);
            this.gbRealPosition.PerformLayout();
            this.gbCalibration.ResumeLayout(false);
            this.gbCalibration.PerformLayout();
            this.gbReferencePoints.ResumeLayout(false);
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

        private Button CalibRef2Butt;
        private Button CalibRef1Butt;
    }
}
