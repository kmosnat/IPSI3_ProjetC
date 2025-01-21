using System.Windows.Forms;

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
        private ToolStripStatusLabel toolStripStatusLabel;

        // On déclare un Panel "classique"
        private Panel statusIndicator;
        // On déclare également le ToolStripControlHost
        private ToolStripControlHost statusIndicatorHost;

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
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();

            // --- Création et configuration du Panel pour la pastille de couleur ---
            this.statusIndicator = new System.Windows.Forms.Panel();
            this.statusIndicator.BackColor = System.Drawing.Color.Red;
            // On définit par exemple une taille de 15x15 pour la pastille
            this.statusIndicator.Size = new System.Drawing.Size(15, 15);

            // --- Création du ToolStripControlHost qui va contenir le Panel ---
            this.statusIndicatorHost = new System.Windows.Forms.ToolStripControlHost(this.statusIndicator);

            // Si vous voulez aligner la pastille à droite, vous pouvez faire :
            // this.statusIndicatorHost.Alignment = ToolStripItemAlignment.Right;

            // Ou si vous voulez gérer les marges...
            this.statusIndicatorHost.Margin = new System.Windows.Forms.Padding(10, 3, 0, 3);

            // Ajout du ToolStripControlHost à la StatusStrip
            // (il apparaîtra à côté du toolStripStatusLabel, selon l'ordre d'ajout)
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.toolStripStatusLabel,
                this.statusIndicatorHost
            });

            // 
            // navBar
            // 
            this.navBar.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.navBar.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.navBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.serveurToolStripMenuItem,
                this.testObjectToolStripMenuItem,
                this.quitterToolStripMenuItem1
            });
            this.navBar.Location = new System.Drawing.Point(0, 0);
            this.navBar.Name = "navBar";
            this.navBar.Size = new System.Drawing.Size(1300, 40);
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
            this.mainTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTableLayoutPanel.Location = new System.Drawing.Point(0, 40);
            this.mainTableLayoutPanel.Name = "mainTableLayoutPanel";
            this.mainTableLayoutPanel.RowCount = 2;
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTableLayoutPanel.Size = new System.Drawing.Size(1300, 580);
            this.mainTableLayoutPanel.TabIndex = 1;

            // 
            // lblLogs
            // 
            this.lblLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLogs.Location = new System.Drawing.Point(3, 0);
            this.lblLogs.Name = "lblLogs";
            this.lblLogs.Size = new System.Drawing.Size(514, 30);
            this.lblLogs.TabIndex = 0;
            this.lblLogs.Text = "Log";
            this.lblLogs.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // 
            // lblPreview
            // 
            this.lblPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPreview.Location = new System.Drawing.Point(523, 0);
            this.lblPreview.Name = "lblPreview";
            this.lblPreview.Size = new System.Drawing.Size(774, 30);
            this.lblPreview.TabIndex = 1;
            this.lblPreview.Text = "Aperçu de l'image :";
            this.lblPreview.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // 
            // tbCom
            // 
            this.tbCom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbCom.Location = new System.Drawing.Point(3, 33);
            this.tbCom.Multiline = true;
            this.tbCom.Name = "tbCom";
            this.tbCom.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbCom.Size = new System.Drawing.Size(514, 544);
            this.tbCom.TabIndex = 2;

            // 
            // pbImage
            // 
            this.pbImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbImage.Location = new System.Drawing.Point(523, 33);
            this.pbImage.Name = "pbImage";
            this.pbImage.Size = new System.Drawing.Size(774, 544);
            this.pbImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbImage.TabIndex = 3;
            this.pbImage.TabStop = false;

            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.toolStripStatusLabel
            });
            this.statusStrip.Location = new System.Drawing.Point(0, 620);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1300, 42);
            this.statusStrip.TabIndex = 2;

            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(201, 32);
            this.toolStripStatusLabel.Text = "État : Déconnecté";

            // 
            // Client
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1300, 662);
            this.Controls.Add(this.mainTableLayoutPanel);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.navBar);
            this.MainMenuStrip = this.navBar;
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
