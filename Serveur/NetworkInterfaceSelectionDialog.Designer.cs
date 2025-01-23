using System.Windows.Forms;

namespace Serveur
{
    partial class NetworkInterfaceSelectionDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblInstruction = new System.Windows.Forms.Label();
            this.listViewInterfaces = new System.Windows.Forms.ListView();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();

            // 
            // Form propriétés
            // 
            this.Text = "Sélection de l’interface réseau";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new System.Drawing.Size(400, 300);
            // Permet d’avoir une taille minimale
            this.Size = new System.Drawing.Size(500, 350);
            // Taille de départ
            this.FormBorderStyle = FormBorderStyle.Sizable;
            // Rendre la fenêtre redimensionnable

            //
            // lblInstruction
            //
            this.lblInstruction.Text = "Veuillez sélectionner une interface réseau :";
            this.lblInstruction.AutoSize = true;
            this.lblInstruction.Location = new System.Drawing.Point(12, 9);
            // On n’ancre pas forcément ce label en bas ni à droite,
            // mais on peut le laisser en haut à gauche.

            //
            // listViewInterfaces
            //
            this.listViewInterfaces.Location = new System.Drawing.Point(12, 30);
            this.listViewInterfaces.Size = new System.Drawing.Size(460, 220);
            this.listViewInterfaces.Anchor = AnchorStyles.Top
                                           | AnchorStyles.Bottom
                                           | AnchorStyles.Left
                                           | AnchorStyles.Right;
            this.listViewInterfaces.FullRowSelect = true;
            this.listViewInterfaces.View = View.Details;
            this.listViewInterfaces.HideSelection = false;

            // Ajouter des colonnes pour un affichage en mode Details
            this.listViewInterfaces.Columns.Add("Interface", 250);
            // Ajuster la taille au contenu (si besoin)
            this.listViewInterfaces.Columns.Add("Adresse IP", 180);

            //
            // btnOk
            //
            this.btnOk.Text = "Valider";
            this.btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.Location = new System.Drawing.Point(316, 260);
            this.btnOk.Click += new System.EventHandler(this.btnOK_Click);

            //
            // btnCancel
            //
            this.btnCancel.Text = "Annuler";
            this.btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.Location = new System.Drawing.Point(397, 260);
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            //
            // Ajout des contrôles sur le formulaire
            //
            this.Controls.Add(this.lblInstruction);
            this.Controls.Add(this.listViewInterfaces);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
        }

        #endregion

        private System.Windows.Forms.ListView listViewInterfaces;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}