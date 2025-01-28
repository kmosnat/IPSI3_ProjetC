using System.Windows.Forms;
using System.Drawing;

namespace Client
{
    partial class Client
    {
        private System.ComponentModel.IContainer components = null;

        // Contrôles
        private MenuStrip navBar;
        private ToolStripMenuItem serveurToolStripMenuItem;
        private ToolStripMenuItem testObjectToolStripMenuItem;
        private ToolStripMenuItem quitterToolStripMenuItem1;

        private TableLayoutPanel mainTableLayoutPanel;
        private Label lblLogs;
        private TextBox tbCom;
        private Label lblPreview;
        private PictureBox pbImage;

        private StatusStrip statusStrip;
        private ToolStripStatusLabel toolStripStatus;

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
            this.navBar = new System.Windows.Forms.MenuStrip();
            this.serveurToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testObjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitterToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.mainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.lblLogs = new System.Windows.Forms.Label();
            this.lblPreview = new System.Windows.Forms.Label();
            this.tbCom = new System.Windows.Forms.TextBox();
            this.pbImage = new System.Windows.Forms.PictureBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.navBar.SuspendLayout();
            this.mainTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // navBar
            // 
            this.navBar.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.navBar.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.navBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.serveurToolStripMenuItem,
            this.testObjectToolStripMenuItem,
            this.quitterToolStripMenuItem1});
            this.navBar.Location = new System.Drawing.Point(0, 0);
            this.navBar.Name = "navBar";
            this.navBar.Size = new System.Drawing.Size(1169, 40);
            this.navBar.TabIndex = 0;
            this.navBar.Text = "menuStrip1";
            // 
            // serveurToolStripMenuItem
            // 
            this.serveurToolStripMenuItem.Name = "serveurToolStripMenuItem";
            this.serveurToolStripMenuItem.Size = new System.Drawing.Size(115, 36);
            this.serveurToolStripMenuItem.Text = "Serveur";
            this.serveurToolStripMenuItem.Click += new System.EventHandler(this.serveurToolStripMenuItem_Click);
            // 
            // testObjectToolStripMenuItem
            // 
            this.testObjectToolStripMenuItem.Name = "testObjectToolStripMenuItem";
            this.testObjectToolStripMenuItem.Size = new System.Drawing.Size(153, 36);
            this.testObjectToolStripMenuItem.Text = "Test Object";
            this.testObjectToolStripMenuItem.Click += new System.EventHandler(this.testObjectToolStripMenuItem_Click);
            // 
            // quitterToolStripMenuItem1
            // 
            this.quitterToolStripMenuItem1.Name = "quitterToolStripMenuItem1";
            this.quitterToolStripMenuItem1.Size = new System.Drawing.Size(109, 36);
            this.quitterToolStripMenuItem1.Text = "Quitter";
            this.quitterToolStripMenuItem1.Click += new System.EventHandler(this.quitterToolStripMenuItem1_Click);
            // 
            // mainTableLayoutPanel
            // 
            this.mainTableLayoutPanel.ColumnCount = 2;
            this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.mainTableLayoutPanel.Controls.Add(this.lblLogs, 0, 0);
            this.mainTableLayoutPanel.Controls.Add(this.lblPreview, 1, 0);
            this.mainTableLayoutPanel.Controls.Add(this.tbCom, 0, 1);
            this.mainTableLayoutPanel.Controls.Add(this.pbImage, 1, 1);
            this.mainTableLayoutPanel.Controls.Add(this.statusStrip, 0, 2);
            this.mainTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTableLayoutPanel.Location = new System.Drawing.Point(0, 40);
            this.mainTableLayoutPanel.Margin = new System.Windows.Forms.Padding(4);
            this.mainTableLayoutPanel.Name = "mainTableLayoutPanel";
            this.mainTableLayoutPanel.RowCount = 3;
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.mainTableLayoutPanel.Size = new System.Drawing.Size(1169, 847);
            this.mainTableLayoutPanel.TabIndex = 1;
            // 
            // lblLogs
            // 
            this.lblLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLogs.Location = new System.Drawing.Point(4, 0);
            this.lblLogs.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLogs.Name = "lblLogs";
            this.lblLogs.Size = new System.Drawing.Size(459, 31);
            this.lblLogs.TabIndex = 0;
            this.lblLogs.Text = "Log";
            this.lblLogs.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPreview
            // 
            this.lblPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPreview.Location = new System.Drawing.Point(471, 0);
            this.lblPreview.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPreview.Name = "lblPreview";
            this.lblPreview.Size = new System.Drawing.Size(694, 31);
            this.lblPreview.TabIndex = 1;
            this.lblPreview.Text = "Aperçu de l\'image :";
            this.lblPreview.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbCom
            // 
            this.tbCom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbCom.Location = new System.Drawing.Point(4, 35);
            this.tbCom.Margin = new System.Windows.Forms.Padding(4);
            this.tbCom.Multiline = true;
            this.tbCom.Name = "tbCom";
            this.tbCom.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbCom.Size = new System.Drawing.Size(459, 778);
            this.tbCom.TabIndex = 2;
            // 
            // pbImage
            // 
            this.pbImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbImage.Location = new System.Drawing.Point(471, 35);
            this.pbImage.Margin = new System.Windows.Forms.Padding(4);
            this.pbImage.Name = "pbImage";
            this.pbImage.Size = new System.Drawing.Size(694, 778);
            this.pbImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbImage.TabIndex = 3;
            this.pbImage.TabStop = false;
            // 
            // statusStrip
            // 
            this.statusStrip.BackColor = System.Drawing.Color.LightGray;
            this.mainTableLayoutPanel.SetColumnSpan(this.statusStrip, 2);
            this.statusStrip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 817);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1169, 30);
            this.statusStrip.TabIndex = 4;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatus
            // 
            this.toolStripStatus.ForeColor = System.Drawing.Color.Red;
            this.toolStripStatus.Name = "toolStripStatus";
            this.toolStripStatus.Size = new System.Drawing.Size(201, 20);
            this.toolStripStatus.Text = "État : Déconnecté";
            // 
            // Client
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1169, 887);
            this.Controls.Add(this.mainTableLayoutPanel);
            this.Controls.Add(this.navBar);
            this.MainMenuStrip = this.navBar;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Client";
            this.Text = "Couleur - Client";
            this.navBar.ResumeLayout(false);
            this.navBar.PerformLayout();
            this.mainTableLayoutPanel.ResumeLayout(false);
            this.mainTableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

    }
}
