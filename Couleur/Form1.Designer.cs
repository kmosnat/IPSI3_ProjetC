namespace Couleur
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
            gbCamera = new GroupBox();
            pictureBox1 = new PictureBox();
            boutAcquisition = new Button();
            boutStop = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // gbCamera
            // 
            gbCamera.Location = new Point(23, 22);
            gbCamera.Name = "gbCamera";
            gbCamera.Size = new Size(532, 283);
            gbCamera.TabIndex = 0;
            gbCamera.TabStop = false;
            gbCamera.Text = "Caméra";
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(583, 39);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(1193, 873);
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // boutAcquisition
            // 
            boutAcquisition.Location = new Point(73, 339);
            boutAcquisition.Name = "boutAcquisition";
            boutAcquisition.Size = new Size(150, 46);
            boutAcquisition.TabIndex = 2;
            boutAcquisition.Text = "Acquisition";
            boutAcquisition.UseVisualStyleBackColor = true;
            boutAcquisition.Click += boutAcquisition_Click;
            // 
            // boutStop
            // 
            boutStop.Location = new Point(323, 339);
            boutStop.Name = "boutStop";
            boutStop.Size = new Size(150, 46);
            boutStop.TabIndex = 3;
            boutStop.Text = "Arret";
            boutStop.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1823, 945);
            Controls.Add(boutStop);
            Controls.Add(boutAcquisition);
            Controls.Add(pictureBox1);
            Controls.Add(gbCamera);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox gbCamera;
        private PictureBox pictureBox1;
        private Button boutAcquisition;
        private Button boutStop;
    }
}