using System;
using System.Drawing;
using System.Drawing.Imaging;

using System.Windows.Forms;
using System.Threading;


namespace Couleur
{
    public partial class Form1 : Form
    {

        smcs.IDevice m_device;
        Rectangle m_rect;
        PixelFormat m_pixelFormat;
        UInt32 m_pixelType;

        public Form1()
        {
            InitializeComponent();
        }

        private void boutAcquisition_Click(object sender, EventArgs e)
        {

        }
    }
}