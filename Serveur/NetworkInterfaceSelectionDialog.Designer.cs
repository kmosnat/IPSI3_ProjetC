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
            this.listViewInterfaces = new System.Windows.Forms.ListView();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listViewInterfaces
            // 
            this.listViewInterfaces.HideSelection = false;
            this.listViewInterfaces.Location = new System.Drawing.Point(63, 45);
            this.listViewInterfaces.Name = "listViewInterfaces";
            this.listViewInterfaces.Size = new System.Drawing.Size(449, 337);
            this.listViewInterfaces.TabIndex = 0;
            this.listViewInterfaces.UseCompatibleStateImageBehavior = false;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(112, 413);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(124, 41);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(334, 413);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(137, 41);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Fermer";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // NetworkInterfaceSelectionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(563, 481);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.listViewInterfaces);
            this.Name = "NetworkInterfaceSelectionDialog";
            this.Text = "NetworkInterfaceSelectionDialog";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewInterfaces;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}