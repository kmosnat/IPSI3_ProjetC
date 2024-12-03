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
using System.Threading;
using System.Net.NetworkInformation;

namespace Serveur
{
    public partial class Main : Form
    {
        // Variables pour la caméra
        smcs.IDevice m_device;
        Rectangle m_rect;
        PixelFormat m_pixelFormat;
        UInt32 m_pixelType;

        // Variables pour le serveur
        private IPAddress m_ipAdrLocal;
        private int m_numPort;
        private bool isTCPRunning = false;
        private bool isAcquisitionRunning = false;

        private CancellationTokenSource tcpCancellationTokenSource;

        public Main()
        {
            InitializeComponent();

            m_numPort = 8001;

            // États initiaux des boutons/menu
            btnStartAcquisition.Enabled = true;
            btnStopAcquisition.Enabled = false;

            // Options du serveur TCP au démarrage
            démarrerLeServeurToolStripMenuItem.Enabled = true;
            arrêterLeServeurToolStripMenuItem.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Ouvrir la boîte de dialogue de sélection des interfaces réseau
            SélectionnerCarteRéseau();
        }

        private void SélectionnerCarteRéseau()
        {
            using (NetworkInterfaceSelectionDialog dialog = new NetworkInterfaceSelectionDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    m_ipAdrLocal = dialog.SelectedIPAddress;
                    string interfaceName = dialog.SelectedInterfaceName;

                    afficherLAdresseIPToolStripMenuItem.Text = $"Adresse IP : {m_ipAdrLocal.ToString()}";
                }
                else
                {
                    // Si l'utilisateur annule, utiliser IPAddress.Any
                    m_ipAdrLocal = IPAddress.Any;
                    afficherLAdresseIPToolStripMenuItem.Text = $"Adresse IP : {m_ipAdrLocal.ToString()}";
                    MessageBox.Show("Aucune interface réseau sélectionnée. Le serveur utilisera toutes les interfaces.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        // Méthode pour initialiser la caméra
        private void initCamera()
        {
            bool cameraConnected = false;

            // initialize GigEVision API
            smcs.CameraSuite.InitCameraAPI();
            smcs.ICameraAPI smcsVisionApi = smcs.CameraSuite.GetCameraAPI();

            if (!smcsVisionApi.IsUsingKernelDriver())
            {
                MessageBox.Show("Warning: Smartek Filter Driver not loaded.");
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

        // Méthode pour initialiser le serveur TCP
        private void initServerTCP()
        {
            if (isTCPRunning)
            {
                this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Le serveur TCP est déjà en cours d'exécution.\r\n")));
                return;
            }

            if (m_ipAdrLocal == null)
            {
                m_ipAdrLocal = IPAddress.Any;
            }

            isTCPRunning = true;
            TcpListener tcpListener = null;
            tcpCancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = tcpCancellationTokenSource.Token;

            try
            {
                tcpListener = new TcpListener(m_ipAdrLocal, m_numPort);
                tcpListener.Start();
                this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Le serveur est en cours d'exécution...\r\n")));
                this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Point de terminaison local : " + tcpListener.LocalEndpoint.ToString() + "\r\n")));
                this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("En attente de connexions...\r\n")));

                while (!token.IsCancellationRequested)
                {
                    if (tcpListener.Pending())
                    {
                        Socket clientSocket = tcpListener.AcceptSocket();
                        this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Connexion acceptée de " + clientSocket.RemoteEndPoint.ToString() + "\r\n")));

                        Task.Run(() => HandleClient(clientSocket));
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
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
                isTCPRunning = false;
                this.Invoke((MethodInvoker)(() =>
                {
                    démarrerLeServeurToolStripMenuItem.Enabled = true;
                    arrêterLeServeurToolStripMenuItem.Enabled = false;
                }));
            }
        }

        // Méthode pour gérer la communication avec le client
        private void HandleClient(Socket clientSocket)
        {
            try
            {
                using (NetworkStream networkStream = new NetworkStream(clientSocket))
                {
                    // Recevoir la requête du client
                    byte[] buffer = new byte[1024];
                    int bytesReceived = networkStream.Read(buffer, 0, buffer.Length);
                    string request = Encoding.ASCII.GetString(buffer, 0, bytesReceived).Trim();

                    this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Requête reçue : " + request + "\r\n")));

                    if (request.Equals("START_STREAM", StringComparison.OrdinalIgnoreCase))
                    {
                        // Envoyer les images en continu
                        while (clientSocket.Connected)
                        {
                            try
                            {
                                Bitmap bitmap = null;
                                if (m_device != null && m_device.IsConnected())
                                {
                                    // Capture de l'image depuis la caméra
                                    if (!m_device.IsBufferEmpty())
                                    {
                                        smcs.IImageInfo imageInfo = null;
                                        m_device.GetImageInfo(ref imageInfo);
                                        if (imageInfo != null)
                                        {
                                            BitmapData bd = null;

                                            ImageUtils.CopyToBitmap(imageInfo, ref bitmap, ref bd, ref m_pixelFormat, ref m_rect, ref m_pixelType);

                                            if (bd != null)
                                                bitmap.UnlockBits(bd);

                                            m_device.PopImage(imageInfo);
                                        }
                                    }
                                }
                                else
                                {
                                    // Si la caméra n'est pas connectée, utiliser l'image de test
                                    bitmap = GenerateTestImage();
                                }

                                if (bitmap != null)
                                {
                                    // Convertir l'image en tableau d'octets (JPEG)
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

                                Thread.Sleep(100);
                            }
                            catch (Exception ex)
                            {
                                this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Erreur lors de l'envoi de l'image : " + ex.Message + "\r\n")));
                                break;
                            }
                        }
                    }
                    else
                    {
                        string invalidRequest = "Requête invalide.";
                        byte[] invalidBytes = Encoding.ASCII.GetBytes(invalidRequest);
                        networkStream.Write(invalidBytes, 0, invalidBytes.Length);
                        this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Requête invalide reçue et réponse envoyée.\r\n")));
                    }
                }

                clientSocket.Close();
                this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Connexion fermée avec le client.\r\n")));
            }
            catch (Exception ex)
            {
                this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Erreur lors de la gestion du client : " + ex.Message + "\r\n")));
            }
        }

        // Méthode pour convertir une Image en tableau d'octets
        private byte[] ImageToByteArray(Image image, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                return ms.ToArray();
            }
        }

        // Méthode pour générer une image de test
        private Bitmap GenerateTestImage()
        {
            try
            {
                Bitmap bitmap = new Bitmap(640, 480, PixelFormat.Format24bppRgb);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.Clear(Color.Gray);
                    g.DrawString("Image de test", new Font("Arial", 24), Brushes.Black, new PointF(10, 10));
                    g.DrawRectangle(Pens.Red, 5, 5, bitmap.Width - 10, bitmap.Height - 10);
                }
                return bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la génération de l'image de test : " + ex.Message);
                return null;
            }
        }

        // Événement clic sur le bouton Démarrer l'Acquisition
        private void btnStartAcquisition_Click(object sender, EventArgs e)
        {
            if (!isAcquisitionRunning)
            {
                isAcquisitionRunning = true;
                timAcq.Start();
                btnStartAcquisition.Enabled = false;
                btnStopAcquisition.Enabled = true;
            }
        }

        // Événement clic sur le bouton Arrêter l'Acquisition
        private void btnStopAcquisition_Click(object sender, EventArgs e)
        {
            if (isAcquisitionRunning)
            {
                timAcq.Stop();
                isAcquisitionRunning = false;
                btnStartAcquisition.Enabled = true;
                btnStopAcquisition.Enabled = false;
            }
        }

        // Événement clic sur le bouton Rechercher des Caméras
        private void btnSearchCamera_Click(object sender, EventArgs e)
        {
            Task.Run(() => initCamera());
        }

        // Événement clic sur le menu Démarrer le Serveur TCP
        private void démarrerLeServeurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!isTCPRunning)
            {
                Task.Run(() => initServerTCP());
                démarrerLeServeurToolStripMenuItem.Enabled = false;
                arrêterLeServeurToolStripMenuItem.Enabled = true;
            }
        }

        // Événement clic sur le menu Arrêter le Serveur TCP
        private void arrêterLeServeurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isTCPRunning)
            {
                tcpCancellationTokenSource.Cancel();
                isTCPRunning = false;
                démarrerLeServeurToolStripMenuItem.Enabled = true;
                arrêterLeServeurToolStripMenuItem.Enabled = false;
            }
        }

        // Événement clic sur le menu Sélectionner une Carte Réseau
        private void sélectionnerUneCarteRéseauToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SélectionnerCarteRéseau();
        }

        // Événement clic sur le menu Quitter
        private void quitterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Événement du timer pour l'acquisition
        private void timAcq_Tick(object sender, EventArgs e)
        {
            try
            {
                if (m_device != null && m_device.IsConnected())
                {
                    if (!m_device.IsBufferEmpty())
                    {
                        smcs.IImageInfo imageInfo = null;
                        m_device.GetImageInfo(ref imageInfo);
                        if (imageInfo != null)
                        {
                            Bitmap bitmap = null;
                            BitmapData bd = null;

                            ImageUtils.CopyToBitmap(imageInfo, ref bitmap, ref bd, ref m_pixelFormat, ref m_rect, ref m_pixelType);

                            if (bitmap != null)
                            {
                                if (pbImage.InvokeRequired)
                                {
                                    pbImage.Invoke(new MethodInvoker(delegate
                                    {
                                        pbImage.Image = bitmap;
                                    }));
                                }
                                else
                                {
                                    pbImage.Image = bitmap;
                                }
                            }

                            if (bd != null)
                                bitmap.UnlockBits(bd);

                            m_device.PopImage(imageInfo);
                        }
                    }
                }
                else
                {
                    Bitmap bitmap = GenerateTestImage();
                    if (bitmap != null)
                    {
                        if (pbImage.InvokeRequired)
                        {
                            pbImage.Invoke(new MethodInvoker(delegate
                            {
                                pbImage.Image = bitmap;
                                pbImage.Refresh();
                            }));
                        }
                        else
                        {
                            pbImage.Image = bitmap;
                            pbImage.Refresh();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur dans timAcq_Tick : " + ex.Message);
            }
        }

        // Méthode pour fermer les connexions de la caméra
        private void closeCamera()
        {
            timAcq.Stop();
            isAcquisitionRunning = false;
            btnStartAcquisition.Enabled = true;
            btnStopAcquisition.Enabled = false;

            if (m_device != null && m_device.IsConnected())
            {
                m_device.CommandNodeExecute("AcquisitionStop");
                m_device.SetIntegerNodeValue("TLParamsLocked", 0);
                m_device.Disconnect();
            }

            smcs.CameraSuite.ExitCameraAPI();
        }

        // Événement de fermeture du formulaire
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            closeCamera();
            if (isTCPRunning)
            {
                tcpCancellationTokenSource.Cancel();
            }
        }

    }
}