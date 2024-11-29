using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Serveur
{
    public partial class Form1 : Form
    {
        smcs.IDevice m_device;
        Rectangle m_rect;
        PixelFormat m_pixelFormat;
        UInt32 m_pixelType;

        private IPAddress m_ipAdrLocal;
        private IPAddress m_ipAdrDistante;
        private int m_numPort;

        public Form1()
        {
            InitializeComponent();

            m_ipAdrLocal = IPAddress.Parse("192.168.1.100");  // Adresse locale
            m_ipAdrDistante = IPAddress.Parse("192.168.1.200");   // Adresse distante
            m_numPort = 8001;

            initCamera();
            Task.Run(() => initServerTCP()); // Exécuter le serveur dans un thread séparé
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

        private void initServerTCP()
        {
            TcpListener tcpListener = null;
            try
            {
                tcpListener = new TcpListener(m_ipAdrLocal, m_numPort);
                tcpListener.Start();
                this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Le serveur est en cours d'exécution...\r\n")));
                this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Point de terminaison local : " + tcpListener.LocalEndpoint.ToString() + "\r\n")));
                this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("En attente de connexion...\r\n")));

                while (true)
                {
                    // Accepter une connexion entrante
                    Socket clientSocket = tcpListener.AcceptSocket();
                    this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Connexion acceptée de " + clientSocket.RemoteEndPoint.ToString() + "\r\n")));

                    // Gérer la connexion dans un thread séparé
                    Task.Run(() => HandleClient(clientSocket));
                }
            }
            catch (Exception ex)
            {
                this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Erreur serveur : " + ex.Message + "\r\n")));
            }
            finally
            {
                if (tcpListener != null)
                {
                    tcpListener.Stop();
                    this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Serveur arrêté.\r\n")));
                }
            }
        }

        private void HandleClient(Socket clientSocket)
        {
            try
            {
                NetworkStream networkStream = new NetworkStream(clientSocket);

                // Recevoir la requête du client
                byte[] buffer = new byte[1024];
                int bytesReceived = networkStream.Read(buffer, 0, buffer.Length);
                string request = Encoding.ASCII.GetString(buffer, 0, bytesReceived).Trim();

                this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Requête reçue : " + request + "\r\n")));

                if (request.Equals("SEND_IMAGE", StringComparison.OrdinalIgnoreCase))
                {
                    // Capture de l'image depuis la caméra
                    Bitmap bitmap = CaptureImageFromCamera();
                    if (bitmap != null)
                    {
                        // Convertir l'image en octets (JPEG)
                        byte[] imageBytes = ImageToByteArray(bitmap, ImageFormat.Jpeg);

                        // Envoyer la taille de l'image (4 octets, Big Endian)
                        byte[] sizeBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(imageBytes.Length));
                        networkStream.Write(sizeBytes, 0, sizeBytes.Length);
                        this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Taille de l'image envoyée : " + imageBytes.Length + " octets.\r\n")));

                        // Envoyer les octets de l'image
                        networkStream.Write(imageBytes, 0, imageBytes.Length);
                        this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Image envoyée au client.\r\n")));
                    }
                    else
                    {
                        string errorMsg = "Erreur de capture d'image.";
                        byte[] errorBytes = Encoding.ASCII.GetBytes(errorMsg);
                        networkStream.Write(errorBytes, 0, errorBytes.Length);
                        this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Erreur lors de la capture de l'image.\r\n")));
                    }
                }
                else
                {
                    string invalidRequest = "Requête invalide.";
                    byte[] invalidBytes = Encoding.ASCII.GetBytes(invalidRequest);
                    networkStream.Write(invalidBytes, 0, invalidBytes.Length);
                    this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Requête invalide reçue et réponse envoyée.\r\n")));
                }

                networkStream.Close();
                clientSocket.Close();
                this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Connexion fermée avec le client.\r\n")));
            }
            catch (Exception ex)
            {
                this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Erreur lors de la gestion du client : " + ex.Message + "\r\n")));
            }
        }

        private Bitmap CaptureImageFromCamera()
        {
            if (m_device != null && m_device.IsConnected())
            {
                if (!m_device.IsBufferEmpty())
                {
                    smcs.IImageInfo imageInfo = null;
                    m_device.GetImageInfo(ref imageInfo);
                    if (imageInfo != null)
                    {
                        Bitmap bitmap = new Bitmap(1, 1); // Initialisation temporaire
                        BitmapData bd = null;

                        ImageUtils.CopyToBitmap(imageInfo, ref bitmap, ref bd, ref m_pixelFormat, ref m_rect, ref m_pixelType);

                        // Débloquer les bits si nécessaire
                        if (bd != null)
                        {
                            bitmap.UnlockBits(bd);
                        }

                        // Mettre à jour le PictureBox
                        this.pbImage.Invoke((MethodInvoker)(() => this.pbImage.Image = bitmap));
                        this.pbImage.Invalidate();

                        // Retirer l'image du buffer
                        m_device.PopImage(imageInfo);
                        m_device.ClearImageBuffer();

                        return bitmap;
                    }
                }
            }
            return null;
        }

        private byte[] ImageToByteArray(Image image, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                return ms.ToArray();
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
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
