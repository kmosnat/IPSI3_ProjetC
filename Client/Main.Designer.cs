namespace Client
{
    partial class Main
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
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
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.pbImage = new System.Windows.Forms.PictureBox();
            this.tbCom = new System.Windows.Forms.TextBox();
            this.navBar = new System.Windows.Forms.MenuStrip();
            this.serveurToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitterToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.statusIndicator = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).BeginInit();
            this.navBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // pbImage
            // 
            this.pbImage.Location = new System.Drawing.Point(592, 125);
            this.pbImage.Name = "pbImage";
            this.pbImage.Size = new System.Drawing.Size(680, 516);
            this.pbImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbImage.TabIndex = 0;
            this.pbImage.TabStop = false;
            // 
            // tbCom
            // 
            this.tbCom.Location = new System.Drawing.Point(45, 125);
            this.tbCom.Multiline = true;
            this.tbCom.Name = "tbCom";
            this.tbCom.Size = new System.Drawing.Size(502, 516);
            this.tbCom.TabIndex = 1;
            // 
            // navBar
            // 
            this.navBar.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.navBar.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.navBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.serveurToolStripMenuItem,
            this.quitterToolStripMenuItem1});
            this.navBar.Location = new System.Drawing.Point(0, 0);
            this.navBar.Name = "navBar";
            this.navBar.Size = new System.Drawing.Size(1305, 40);
            this.navBar.TabIndex = 5;
            this.navBar.Text = "menuStrip1";
            // 
            // serveurToolStripMenuItem
            // 
            this.serveurToolStripMenuItem.Name = "serveurToolStripMenuItem";
            this.serveurToolStripMenuItem.Size = new System.Drawing.Size(115, 36);
            this.serveurToolStripMenuItem.Text = "Serveur";
            this.serveurToolStripMenuItem.Click += new System.EventHandler(this.serveurToolStripMenuItem_Click);
            // 
            // quitterToolStripMenuItem1
            // 
            this.quitterToolStripMenuItem1.Name = "quitterToolStripMenuItem1";
            this.quitterToolStripMenuItem1.Size = new System.Drawing.Size(109, 36);
            this.quitterToolStripMenuItem1.Text = "Quitter";
            this.quitterToolStripMenuItem1.Click += new System.EventHandler(this.quitterToolStripMenuItem1_Click);
            // 
            // statusIndicator
            // 
            this.statusIndicator.BackColor = System.Drawing.Color.Red;
            this.statusIndicator.Location = new System.Drawing.Point(1209, 60);
            this.statusIndicator.Name = "statusIndicator";
            this.statusIndicator.Size = new System.Drawing.Size(63, 59);
            this.statusIndicator.TabIndex = 6;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1305, 685);
            this.Controls.Add(this.statusIndicator);
            this.Controls.Add(this.tbCom);
            this.Controls.Add(this.pbImage);
            this.Controls.Add(this.navBar);
            this.MainMenuStrip = this.navBar;
            this.Name = "Main";
            this.Text = "Couleur - Client";
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).EndInit();
            this.navBar.ResumeLayout(false);
            this.navBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbImage;
        private System.Windows.Forms.TextBox tbCom;
        private System.Windows.Forms.MenuStrip navBar;
        private System.Windows.Forms.ToolStripMenuItem serveurToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitterToolStripMenuItem1;
        private System.Windows.Forms.Panel statusIndicator;
    }
}

