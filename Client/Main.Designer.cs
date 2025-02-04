using System.Windows.Forms;
using System.Drawing;

namespace Client
{
    partial class Client
    {
        private System.ComponentModel.IContainer components = null;

        // Contrôles existants
        private MenuStrip navBar;
        private ToolStripMenuItem serveurToolStripMenuItem;
        private ToolStripMenuItem quitterToolStripMenuItem1;

        private TableLayoutPanel mainTableLayoutPanel;
        private Label lblLogs;
        private TextBox tbCom;
        private Label lblPreview;
        private PictureBox pbImage;

        private StatusStrip statusStrip;
        private ToolStripStatusLabel toolStripStatus;

        // Nouveaux contrôles
        private TrackBar trackBarSeuil;
        private TrackBar trackBarFiltre;
        private ToolTip toolTip;
        private GroupBox groupBoxSliders;
        private TableLayoutPanel slidersTableLayoutPanel;
        private Label lblSeuil;
        private Label lblFiltre;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.navBar = new System.Windows.Forms.MenuStrip();
            this.serveurToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitterToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.mainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxSliders = new System.Windows.Forms.GroupBox();
            this.slidersTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.lblSeuil = new System.Windows.Forms.Label();
            this.trackBarSeuil = new System.Windows.Forms.TrackBar();
            this.lblFiltre = new System.Windows.Forms.Label();
            this.trackBarFiltre = new System.Windows.Forms.TrackBar();
            this.lblLogs = new System.Windows.Forms.Label();
            this.lblPreview = new System.Windows.Forms.Label();
            this.tbCom = new System.Windows.Forms.TextBox();
            this.pbImage = new System.Windows.Forms.PictureBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.navBar.SuspendLayout();
            this.mainTableLayoutPanel.SuspendLayout();
            this.groupBoxSliders.SuspendLayout();
            this.slidersTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSeuil)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarFiltre)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // navBar
            // 
            this.navBar.BackColor = System.Drawing.Color.White;
            this.navBar.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.navBar.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.navBar.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.navBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.serveurToolStripMenuItem,
            this.quitterToolStripMenuItem1});
            this.navBar.Location = new System.Drawing.Point(0, 0);
            this.navBar.Name = "navBar";
            this.navBar.Size = new System.Drawing.Size(1456, 49);
            this.navBar.TabIndex = 0;
            this.navBar.Text = "Menu principal";
            // 
            // serveurToolStripMenuItem
            // 
            this.serveurToolStripMenuItem.Name = "serveurToolStripMenuItem";
            this.serveurToolStripMenuItem.Size = new System.Drawing.Size(125, 45);
            this.serveurToolStripMenuItem.Text = "Serveur";
            this.serveurToolStripMenuItem.Click += new System.EventHandler(this.serveurToolStripMenuItem_Click);
            // 
            // quitterToolStripMenuItem1
            // 
            this.quitterToolStripMenuItem1.Name = "quitterToolStripMenuItem1";
            this.quitterToolStripMenuItem1.Size = new System.Drawing.Size(120, 45);
            this.quitterToolStripMenuItem1.Text = "Quitter";
            this.quitterToolStripMenuItem1.Click += new System.EventHandler(this.quitterToolStripMenuItem1_Click);
            // 
            // mainTableLayoutPanel
            // 
            this.mainTableLayoutPanel.BackColor = System.Drawing.Color.WhiteSmoke;
            this.mainTableLayoutPanel.ColumnCount = 2;
            this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.mainTableLayoutPanel.Controls.Add(this.groupBoxSliders, 0, 0);
            this.mainTableLayoutPanel.Controls.Add(this.lblLogs, 0, 1);
            this.mainTableLayoutPanel.Controls.Add(this.lblPreview, 1, 1);
            this.mainTableLayoutPanel.Controls.Add(this.tbCom, 0, 2);
            this.mainTableLayoutPanel.Controls.Add(this.pbImage, 1, 2);
            this.mainTableLayoutPanel.Controls.Add(this.statusStrip, 0, 3);
            this.mainTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTableLayoutPanel.Location = new System.Drawing.Point(0, 49);
            this.mainTableLayoutPanel.Name = "mainTableLayoutPanel";
            this.mainTableLayoutPanel.Padding = new System.Windows.Forms.Padding(10);
            this.mainTableLayoutPanel.RowCount = 4;
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 156F));
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 51F));
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.mainTableLayoutPanel.Size = new System.Drawing.Size(1456, 1116);
            this.mainTableLayoutPanel.TabIndex = 1;
            // 
            // groupBoxSliders
            // 
            this.mainTableLayoutPanel.SetColumnSpan(this.groupBoxSliders, 2);
            this.groupBoxSliders.Controls.Add(this.slidersTableLayoutPanel);
            this.groupBoxSliders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxSliders.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.groupBoxSliders.ForeColor = System.Drawing.Color.DimGray;
            this.groupBoxSliders.Location = new System.Drawing.Point(15, 15);
            this.groupBoxSliders.Margin = new System.Windows.Forms.Padding(5);
            this.groupBoxSliders.Name = "groupBoxSliders";
            this.groupBoxSliders.Size = new System.Drawing.Size(1426, 146);
            this.groupBoxSliders.TabIndex = 0;
            this.groupBoxSliders.TabStop = false;
            this.groupBoxSliders.Text = "Réglages";
            // 
            // slidersTableLayoutPanel
            // 
            this.slidersTableLayoutPanel.ColumnCount = 2;
            this.slidersTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 277F));
            this.slidersTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.slidersTableLayoutPanel.Controls.Add(this.lblSeuil, 0, 0);
            this.slidersTableLayoutPanel.Controls.Add(this.trackBarSeuil, 1, 0);
            this.slidersTableLayoutPanel.Controls.Add(this.lblFiltre, 0, 1);
            this.slidersTableLayoutPanel.Controls.Add(this.trackBarFiltre, 1, 1);
            this.slidersTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.slidersTableLayoutPanel.Location = new System.Drawing.Point(3, 39);
            this.slidersTableLayoutPanel.Name = "slidersTableLayoutPanel";
            this.slidersTableLayoutPanel.Padding = new System.Windows.Forms.Padding(10);
            this.slidersTableLayoutPanel.RowCount = 2;
            this.slidersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.slidersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.slidersTableLayoutPanel.Size = new System.Drawing.Size(1420, 104);
            this.slidersTableLayoutPanel.TabIndex = 0;
            // 
            // lblSeuil
            // 
            this.lblSeuil.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSeuil.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSeuil.Location = new System.Drawing.Point(13, 13);
            this.lblSeuil.Margin = new System.Windows.Forms.Padding(3);
            this.lblSeuil.Name = "lblSeuil";
            this.lblSeuil.Size = new System.Drawing.Size(271, 36);
            this.lblSeuil.TabIndex = 0;
            this.lblSeuil.Text = "Seuil (0 - 255) :";
            this.lblSeuil.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // trackBarSeuil
            // 
            this.trackBarSeuil.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackBarSeuil.LargeChange = 10;
            this.trackBarSeuil.Location = new System.Drawing.Point(290, 13);
            this.trackBarSeuil.Maximum = 255;
            this.trackBarSeuil.Name = "trackBarSeuil";
            this.trackBarSeuil.Size = new System.Drawing.Size(1117, 36);
            this.trackBarSeuil.TabIndex = 1;
            this.trackBarSeuil.TickFrequency = 5;
            this.toolTip.SetToolTip(this.trackBarSeuil, "Ajustez le seuil entre 0 et 255");
            this.trackBarSeuil.Value = 180;
            this.trackBarSeuil.Scroll += new System.EventHandler(this.trackBarSeuil_Scroll);
            // 
            // lblFiltre
            // 
            this.lblFiltre.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFiltre.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblFiltre.Location = new System.Drawing.Point(13, 55);
            this.lblFiltre.Margin = new System.Windows.Forms.Padding(3);
            this.lblFiltre.Name = "lblFiltre";
            this.lblFiltre.Size = new System.Drawing.Size(271, 36);
            this.lblFiltre.TabIndex = 2;
            this.lblFiltre.Text = "Filtre (1000 - 50000) :";
            this.lblFiltre.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // trackBarFiltre
            // 
            this.trackBarFiltre.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackBarFiltre.LargeChange = 1000;
            this.trackBarFiltre.Location = new System.Drawing.Point(290, 55);
            this.trackBarFiltre.Maximum = 50000;
            this.trackBarFiltre.Minimum = 1000;
            this.trackBarFiltre.Name = "trackBarFiltre";
            this.trackBarFiltre.Size = new System.Drawing.Size(1117, 36);
            this.trackBarFiltre.SmallChange = 100;
            this.trackBarFiltre.TabIndex = 3;
            this.trackBarFiltre.TickFrequency = 1000;
            this.toolTip.SetToolTip(this.trackBarFiltre, "Ajustez le filtre entre 1000 et 10000");
            this.trackBarFiltre.Value = 5000;
            this.trackBarFiltre.Scroll += new System.EventHandler(this.trackBarFiltre_Scroll);
            // 
            // lblLogs
            // 
            this.lblLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLogs.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblLogs.ForeColor = System.Drawing.Color.DimGray;
            this.lblLogs.Location = new System.Drawing.Point(15, 171);
            this.lblLogs.Margin = new System.Windows.Forms.Padding(5);
            this.lblLogs.Name = "lblLogs";
            this.lblLogs.Size = new System.Drawing.Size(564, 41);
            this.lblLogs.TabIndex = 1;
            this.lblLogs.Text = "Logs :";
            this.lblLogs.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPreview
            // 
            this.lblPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPreview.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblPreview.ForeColor = System.Drawing.Color.DimGray;
            this.lblPreview.Location = new System.Drawing.Point(589, 171);
            this.lblPreview.Margin = new System.Windows.Forms.Padding(5);
            this.lblPreview.Name = "lblPreview";
            this.lblPreview.Size = new System.Drawing.Size(852, 41);
            this.lblPreview.TabIndex = 2;
            this.lblPreview.Text = "Aperçu de l\'image :";
            this.lblPreview.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbCom
            // 
            this.tbCom.BackColor = System.Drawing.Color.White;
            this.tbCom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbCom.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbCom.Location = new System.Drawing.Point(15, 222);
            this.tbCom.Margin = new System.Windows.Forms.Padding(5);
            this.tbCom.Multiline = true;
            this.tbCom.Name = "tbCom";
            this.tbCom.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbCom.Size = new System.Drawing.Size(564, 824);
            this.tbCom.TabIndex = 3;
            // 
            // pbImage
            // 
            this.pbImage.BackColor = System.Drawing.Color.LightGray;
            this.pbImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbImage.Location = new System.Drawing.Point(589, 222);
            this.pbImage.Margin = new System.Windows.Forms.Padding(5);
            this.pbImage.Name = "pbImage";
            this.pbImage.Size = new System.Drawing.Size(852, 824);
            this.pbImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbImage.TabIndex = 4;
            this.pbImage.TabStop = false;
            // 
            // statusStrip
            // 
            this.statusStrip.BackColor = System.Drawing.Color.White;
            this.mainTableLayoutPanel.SetColumnSpan(this.statusStrip, 2);
            this.statusStrip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatus});
            this.statusStrip.Location = new System.Drawing.Point(10, 1051);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.statusStrip.Size = new System.Drawing.Size(1436, 55);
            this.statusStrip.TabIndex = 5;
            // 
            // toolStripStatus
            // 
            this.toolStripStatus.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripStatus.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.toolStripStatus.ForeColor = System.Drawing.Color.Red;
            this.toolStripStatus.Name = "toolStripStatus";
            this.toolStripStatus.Size = new System.Drawing.Size(240, 45);
            this.toolStripStatus.Text = "État : Déconnecté";
            // 
            // Client
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(1456, 1165);
            this.Controls.Add(this.mainTableLayoutPanel);
            this.Controls.Add(this.navBar);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = this.navBar;
            this.Name = "Client";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Couleur - Client";
            this.navBar.ResumeLayout(false);
            this.navBar.PerformLayout();
            this.mainTableLayoutPanel.ResumeLayout(false);
            this.mainTableLayoutPanel.PerformLayout();
            this.groupBoxSliders.ResumeLayout(false);
            this.slidersTableLayoutPanel.ResumeLayout(false);
            this.slidersTableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSeuil)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarFiltre)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
