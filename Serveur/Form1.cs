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

namespace Serveur
{
    public partial class Form1 : Form
    {
        smcs.IDevice m_device;
        Rectangle m_rect;
        PixelFormat m_pixelFormat;
        UInt32 m_pixelType;

        private IPAddress m_ipAdrLocal;
        private int m_numPort;
        private bool isTCPRunning = false;
        private bool isAcquisitionRunning = false;

        private CancellationTokenSource tcpCancellationTokenSource;

        public Form1()
        {
            InitializeComponent();

            m_ipAdrLocal = IPAddress.Parse("192.168.1.54");
            m_numPort = 8001;

            btnStartAcquisition.Enabled = true;
            btnStopAcquisition.Enabled = false;
            btnStopTCP.Enabled = false;
            btnStartTCP.Enabled = true; 
        }

        private void initCamera()
        {
            try
            {
                bool cameraConnected = false;

                smcs.CameraSuite.InitCameraAPI();
                smcs.ICameraAPI smcsVisionApi = smcs.CameraSuite.GetCameraAPI();

                if (!smcsVisionApi.IsUsingKernelDriver())
                {
                    MessageBox.Show("Warning: Smartek Filter Driver not loaded.");
                }

                smcsVisionApi.FindAllDevices(3.0);
                smcs.IDevice[] devices = smcsVisionApi.GetAllDevices();

                if (devices.Length > 0)
                {
                    m_device = devices[0];

                    if (m_device != null && m_device.Connect())
                    {
                        this.Invoke((MethodInvoker)(() =>
                        {
                            lblConnection.BackColor = Color.LimeGreen;
                            lblConnection.Text = "Connexion établie";
                            lblAdrIP.BackColor = Color.LimeGreen;
                            lblAdrIP.Text = "Adresse IP : " + Common.IpAddrToString(m_device.GetIpAddress());
                            lblNomCamera.Text = m_device.GetManufacturerName() + " : " + m_device.GetModelName();
                        }));

                        bool status = m_device.SetStringNodeValue("TriggerMode", "Off");
                        status = m_device.SetStringNodeValue("AcquisitionMode", "Continuous");
                        status = m_device.SetIntegerNodeValue("TLParamsLocked", 1);
                        status = m_device.CommandNodeExecute("AcquisitionStart");
                        cameraConnected = true;

                        this.Invoke((MethodInvoker)(() =>
                        {
                            btnStartAcquisition.Enabled = true;
                            btnStartTCP.Enabled = true;
                        }));
                    }
                }

                if (!cameraConnected)
                {
                    MessageBox.Show("Aucune caméra détectée. L'acquisition se fera avec une image de test.");
                    this.Invoke((MethodInvoker)(() =>
                    {
                        lblAdrIP.BackColor = Color.Red;
                        lblAdrIP.Text = "Aucune caméra connectée";
                        lblConnection.Text = "Utilisation de l'image de test";
                        btnStartAcquisition.Enabled = true;
                        btnStartTCP.Enabled = true;
                    }));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Échec de l'initialisation de la caméra : " + ex.Message);
            }
        }

        private void initServerTCP()
        {
            if (isTCPRunning)
            {
                this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Le serveur TCP est déjà en cours d'exécution.\r\n")));
                return;
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
                    btnStartTCP.Enabled = true;
                    btnStopTCP.Enabled = false;
                }));
            }
        }

        private void HandleClient(Socket clientSocket)
        {
            try
            {
                using (NetworkStream networkStream = new NetworkStream(clientSocket))
                {
                    byte[] buffer = new byte[1024];
                    int bytesReceived = networkStream.Read(buffer, 0, buffer.Length);
                    string request = Encoding.ASCII.GetString(buffer, 0, bytesReceived).Trim();

                    this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Requête reçue : " + request + "\r\n")));

                    if (request.Equals("START_STREAM", StringComparison.OrdinalIgnoreCase))
                    {
                        while (clientSocket.Connected)
                        {
                            try
                            {
                                Bitmap bitmap = null;
                                if (m_device != null && m_device.IsConnected())
                                {
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
                                    bitmap = GenerateTestImage();
                                }

                                if (bitmap != null)
                                {
                                    byte[] imageBytes = ImageToByteArray(bitmap, ImageFormat.Jpeg);

                                    byte[] sizeBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(imageBytes.Length));
                                    networkStream.Write(sizeBytes, 0, sizeBytes.Length);
                                    this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Taille de l'image envoyée : " + imageBytes.Length + " octets.\r\n")));

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

        private byte[] ImageToByteArray(Image image, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                return ms.ToArray();
            }
        }

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

        private void btnSearchCameras_Click(object sender, EventArgs e)
        {
            Task.Run(() => initCamera());
        }

        private void btnStartTCP_Click(object sender, EventArgs e)
        {
            if (!isTCPRunning)
            {
                Task.Run(() => initServerTCP());
                btnStartTCP.Enabled = false;
                btnStopTCP.Enabled = true;
            }
        }

        private void btnStopTCP_Click(object sender, EventArgs e)
        {
            if (isTCPRunning)
            {
                tcpCancellationTokenSource.Cancel();
                isTCPRunning = false;
                btnStartTCP.Enabled = true;
                btnStopTCP.Enabled = false;
            }
        }

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
                    // Si la caméra n'est pas connectée, afficher une image de test
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            closeCamera();
            if (isTCPRunning)
            {
                tcpCancellationTokenSource.Cancel();
            }
        }

        private void quitBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
