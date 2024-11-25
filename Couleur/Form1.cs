using System;
using System.Drawing;
using System.Drawing.Imaging;

using System.Windows.Forms;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;


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
            initCamera();
        }

        private void initCamera()
        {
            bool cameraConnected = false;

            // initialize GigEVision API
            smcs.CameraSuite.InitCameraAPI();
            smcs.ICameraAPI smcsVisionApi = smcs.CameraSuite.GetCameraAPI();

            if (!smcsVisionApi.IsUsingKernelDriver())
            {
                //MessageBox.Show("Warning: Smartek Filter Driver not loaded.");
            }

            // discover all devices on network
            smcsVisionApi.FindAllDevices(3.0);
            smcs.IDevice[] devices = smcsVisionApi.GetAllDevices();
            //MessageBox.Show(devices.Length.ToString());
            if (devices.Length > 0)
            {
                // take first device in list
                m_device = devices[0];

                // uncomment to use specific model
                //for (int i = 0; i < devices.Length; i++)
                //{
                //    if (devices[i].GetModelName() == "GC652M")
                //    {
                //        m_device = devices[i];
                //    }
                //}

                // to change number of images in image buffer from default 10 images 
                // call SetImageBufferFrameCount() method before Connect() method
                //m_device.SetImageBufferFrameCount(20);

                if (m_device != null && m_device.Connect())
                {
                    this.lblConnection.BackColor = Color.LimeGreen;
                    this.lblConnection.Text = "Connection établie";
                    this.lblAdrIP.BackColor = Color.LimeGreen;
                    this.lblAdrIP.Text = "Adresse IP : " + Common.IpAddrToString(m_device.GetIpAddress());
                    this.lblNomCamera.Text = m_device.GetManufacturerName() + " : " + m_device.GetModelName();

                    // disable trigger mode
                    bool status = m_device.SetStringNodeValue("TriggerMode", "Off");
                    // set continuous acquisition mode
                    status = m_device.SetStringNodeValue("AcquisitionMode", "Continuous");
                    // start acquisition
                    status = m_device.SetIntegerNodeValue("TLParamsLocked", 1);
                    status = m_device.CommandNodeExecute("AcquisitionStart");
                    cameraConnected = true;
                }
            }

            if (!cameraConnected)
            {
                this.lblAdrIP.BackColor = Color.Red;
                this.lblAdrIP.Text = "Erreur de connection!";
            }
        }

        private void boutAcquisition_Click(object sender, EventArgs e)
        {
            timAcq.Start();
        }

        private void boutStop_Click(object sender, EventArgs e)
        {
            timAcq.Stop();
        }

        private void timAcq_Tick(object sender, EventArgs e)
        {
            if (m_device != null && m_device.IsConnected())
            {
                if (!m_device.IsBufferEmpty())
                {
                    smcs.IImageInfo imageInfo = null;
                    m_device.GetImageInfo(ref imageInfo);
                    if (imageInfo != null)
                    {
                        Bitmap bitmap = (Bitmap)this.pbImage.Image;
                        Bitmap bmpGt = (Bitmap)this.pbImage.Image;
                        ClImage Img = new ClImage();
                        ClImage ImgGT = new ClImage();
                        BitmapData bd = null;

                        ImageUtils.CopyToBitmap(imageInfo, ref bitmap, ref bd, ref m_pixelFormat, ref m_rect, ref m_pixelType);

                        var bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                        var bmpDataGt = bmpGt.LockBits(new Rectangle(0, 0, bmpGt.Width, bmpGt.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);    // blocage des pixels de l'image 

                        Img.objetLibDataImgPtr(2, bmpData.Scan0, bmpData.Stride, bitmap.Height, bitmap.Width);
                        Img.processPtr(ImgGT.objetLibDataImgPtr(0, bmpDataGt.Scan0, bmpDataGt.Stride, bmpGt.Height, bmpGt.Width));

                        //-------------------------------------------------------------------
                        //if (m_pixelFormat == PixelFormat.Format8bppIndexed)
                        //{
                        //    // set palette
                        //    ColorPalette palette = bitmap.Palette;
                        //    for (int i = 0; i < 256; i++)
                        //    {
                        //        palette.Entries[i] = Color.FromArgb(255 - i, 255 - i, 255 - i);
                        //    }
                        //    bitmap.Palette = palette;
                        //}
                        //-------------------------------------------------------------------
                        if (bitmap != null)
                        {
                            //this.pbImage.Height = bitmap.Height;
                            //this.pbImage.Width = bitmap.Width;
                            this.pbImage.Image = bitmap;
                        }

                        // display image
                        if (bd != null)
                            bitmap.UnlockBits(bd);

                        this.pbImage.Invalidate();
                    }
                    // remove (pop) image from image buffer
                    m_device.PopImage(imageInfo);
                    // empty buffer
                    m_device.ClearImageBuffer();
                }
            }
        }

        private void Form1_FormClosing(object sender, EventArgs e)
        {
            timAcq.Stop();
            if (m_device != null && m_device.IsConnected())
            {
                m_device.CommandNodeExecute("AcquisitionStop");
                m_device.SetIntegerNodeValue("TLParamsLocked", 0);
                m_device.Disconnect();
            }

            smcs.CameraSuite.ExitCameraAPI();

            this.Close();
        }
    }
}