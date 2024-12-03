using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {
        private readonly object imageLock = new object();
        private IPAddress m_ipAdrLocal;
        private IPAddress m_ipAdrDistante;
        private int m_numPort;

        public Form1()
        {
            InitializeComponent();
            m_ipAdrLocal = IPAddress.Parse("192.168.1.54");  // Adresse locale
            m_ipAdrDistante = IPAddress.Parse("192.168.1.155");   // Adresse distante
            m_numPort = 8001;

            Task.Run(() => initClientTCP());
        }

        private void initClientTCP()
        {
            TcpClient tcpClient = null;
            try
            {
                tcpClient = new TcpClient();
                this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Connexion en cours...\r\n")));

                tcpClient.Connect(m_ipAdrDistante, m_numPort);
                this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Connexion établie\r\n")));

                NetworkStream networkStream = tcpClient.GetStream();

                // Envoyer la requête "START_STREAM"
                string request = "START_STREAM";
                byte[] requestBytes = Encoding.ASCII.GetBytes(request);
                networkStream.Write(requestBytes, 0, requestBytes.Length);
                this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Requête d'envoi d'image envoyée : " + request + "\r\n")));

                while (true)
                {
                    // Lire la taille de l'image (4 octets)
                    byte[] sizeBytes = new byte[4];
                    int totalRead = 0;
                    while (totalRead < 4)
                    {
                        int bytesRead = networkStream.Read(sizeBytes, totalRead, 4 - totalRead);
                        if (bytesRead == 0)
                        {
                            throw new Exception("Connexion fermée avant de recevoir la taille de l'image.");
                        }
                        totalRead += bytesRead;
                    }

                    int imageSize = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(sizeBytes, 0));
                    this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Taille de l'image à recevoir : " + imageSize + " octets.\r\n")));

                    // Lire les octets de l'image
                    byte[] imageBytes = new byte[imageSize];
                    totalRead = 0;
                    while (totalRead < imageSize)
                    {
                        int bytesRead = networkStream.Read(imageBytes, totalRead, imageSize - totalRead);
                        if (bytesRead == 0)
                        {
                            throw new Exception("Connexion fermée avant de recevoir toute l'image.");
                        }
                        totalRead += bytesRead;
                    }

                    this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Image reçue en " + totalRead + " octets.\r\n")));

                    // Convertir les octets en image et l'afficher
                    using (MemoryStream ms = new MemoryStream(imageBytes))
                    {
                        Image receivedImage = Image.FromStream(ms);

                        lock (imageLock) // Verrouiller l'accès à l'image pour éviter les conflits
                        {
                            this.pbImage.Invoke((MethodInvoker)(() =>
                            {
                                if (this.pbImage.Image != null)
                                {
                                    this.pbImage.Image.Dispose(); // Libérer les ressources de l'image précédente
                                }
                                this.pbImage.Image = (Image)receivedImage.Clone(); // Cloner l'image avant de l'afficher
                            }));
                        }

                        this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Image affichée.\r\n")));
                    }
                }
            }
            catch (Exception ex)
            {
                this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Erreur : " + ex.Message + "\r\n")));
            }
            finally
            {
                if (tcpClient != null)
                {
                    tcpClient.Close();
                    this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Connexion fermée.\r\n")));
                }
            }
        }
    }
}
