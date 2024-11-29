using System;
using System.Drawing;
using System.Drawing.Imaging;

using System.Windows.Forms;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;
using System.IO.Ports;


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
            SerialPort port = new SerialPort("COM3", 9600);
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
                        BitmapData bd = null;
                        ImageUtils.CopyToBitmap(imageInfo, ref bitmap, ref bd, ref m_pixelFormat, ref m_rect, ref m_pixelType);

                        if (bitmap != null)
                        {
                            this.pbImage.Image = bitmap;
                        }

                        try
                        {
                            // Préparer les paramètres pour la bibliothèque
                            int nbChamps = GetNumberOfChannels(m_pixelFormat);
                            int stride = bd.Stride;
                            int nbLig = bitmap.Height;
                            int nbCol = bitmap.Width;

                            // Obtenir un pointeur vers les données de l'image
                            IntPtr ptrData = bd.Scan0;

                            // Instancier la classe ClImage
                            using (ClImage img = new ClImage())
                            {
                                // Appeler la fonction de la bibliothèque pour initialiser ClPtr
                                IntPtr clibPtr = img.ObjetLibDataImgPtr(nbChamps, ptrData, stride, nbLig, nbCol);

                                if (clibPtr == IntPtr.Zero)
                                {
                                    throw new Exception("Échec de l'initialisation de l'objet ClibIHM.");
                                }

                                // Appliquer un filtre
                                //img.FilterPtr(

                                // Optionnel : récupérer une valeur de champ pour vérification
                                double valeur = img.ObjetLibValeurChamp(0);
                                Console.WriteLine($"Valeur du champ 0: {valeur}");
                            }
                        }
                        catch (Exception ex)
                        {
                            // Gérer les exceptions et afficher un message d'erreur
                            MessageBox.Show($"Erreur lors de l'appel à la bibliothèque : {ex.Message}");
                        }
                        finally
                        {
                            // Déverrouiller les bits de l'image
                            if (bd != null)
                                bitmap.UnlockBits(bd);
                        }

                        this.pbImage.Invalidate();
                    }
                    // Retirer l'image du buffer
                    m_device.PopImage(imageInfo);
                    // Vider le buffer
                    m_device.ClearImageBuffer();
                }
            }
        }


        private int GetNumberOfChannels(PixelFormat format)
        {
            switch (format)
            {
                case PixelFormat.Format8bppIndexed:
                    return 1;
                case PixelFormat.Format24bppRgb:
                    return 3;
                case PixelFormat.Format32bppArgb:
                    return 4;
                default:
                    throw new NotSupportedException("Format de pixel non supporté");
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