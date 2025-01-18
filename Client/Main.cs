using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Log;

namespace Client
{
    public partial class Client : Form
    {
        private readonly object imageLock = new object();
        private IPAddress m_ipAdrDistante;
        private int m_numPort;

        public Client()
        {
            InitializeComponent();
            m_numPort = 8001;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            serveurToolStripMenuItem_Click(this, EventArgs.Empty);
        }

        private uint FromBigEndianBytes(byte[] bytes)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return BitConverter.ToUInt32(bytes, 0);
        }

        private void InitClientTCP()
        {
            if (m_ipAdrDistante == null)
            {
                AppendLog(LogSource.Client, LogLevel.WARNING, "Adresse IP non définie. Veuillez entrer l'adresse IP du serveur.");
                return;
            }

            TcpClient tcpClient = null;
            try
            {
                tcpClient = new TcpClient();

                AppendLog(LogSource.Client, LogLevel.INFO, "Connexion en cours...");
                tcpClient.Connect(m_ipAdrDistante, m_numPort);

                AppendLog(LogSource.Client, LogLevel.INFO, "Connexion établie");

                NetworkStream networkStream = tcpClient.GetStream();

                string request = "GET_IMAGE\n";
                byte[] requestBytes = Encoding.ASCII.GetBytes(request);
                networkStream.Write(requestBytes, 0, requestBytes.Length);
                networkStream.Flush();

                AppendLog(LogSource.Client, LogLevel.INFO, $"Requête d'image envoyée : {request}");

                const uint maxExpectedSize = 10_000_000;

                while (tcpClient.Connected)
                {
                    // Lecture de la taille de l'image (4 octets)
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

                    uint imageSize = FromBigEndianBytes(sizeBytes);
                    AppendLog(LogSource.Client, LogLevel.INFO, $"Taille de l'image à recevoir : {imageSize} octets.");

                    if (imageSize == 0)
                    {
                        // Le serveur indique qu'il y a eu une erreur de capture
                        AppendLog(LogSource.Client, LogLevel.ERROR, "Le serveur a signalé une erreur lors de la capture de l'image.");
                        continue;
                    }

                    if (imageSize > maxExpectedSize)
                    {
                        throw new Exception($"Taille d'image invalide reçue : {imageSize}");
                    }

                    // Lecture des données de l'image
                    byte[] imageBytes = new byte[imageSize];
                    totalRead = 0;
                    while (totalRead < imageSize)
                    {
                        int bytesRead = networkStream.Read(imageBytes, totalRead, (int)(imageSize - totalRead));
                        if (bytesRead == 0)
                        {
                            throw new Exception("Connexion fermée avant de recevoir toute l'image.");
                        }
                        totalRead += bytesRead;
                    }

                    AppendLog(LogSource.Client, LogLevel.INFO, $"Image reçue en {totalRead} octets.");

                    using (MemoryStream ms = new MemoryStream(imageBytes))
                    {
                        Image receivedImage = Image.FromStream(ms);
                        DisplayImage(receivedImage);
                    }

                    this.statusIndicator.Invoke((MethodInvoker)(() => this.statusIndicator.BackColor = Color.Green));
                }
            }
            catch (Exception ex)
            {
                AppendLog(LogSource.Client, LogLevel.ERROR, "Erreur : " + ex.Message);
                this.statusIndicator.Invoke((MethodInvoker)(() => this.statusIndicator.BackColor = Color.Red));
            }
            finally
            {
                if (tcpClient != null)
                {
                    tcpClient.Close();
                    AppendLog(LogSource.Client, LogLevel.INFO, "Connexion fermée.");
                }
            }
        }

        private void DisplayImage(Image receivedImage)
        {
            try
            {
                Image processedImage = ProcessImage(receivedImage);

                this.Invoke((MethodInvoker)(() =>
                {
                    if (this.pbImage.Image != null)
                    {
                        this.pbImage.Image.Dispose();
                    }
                    this.pbImage.Image = processedImage;
                }));

                AppendLog(LogSource.Client, LogLevel.INFO, "Image affichée.");
            }
            catch (Exception ex)
            {
                AppendLog(LogSource.Client, LogLevel.ERROR, "Erreur lors de l'affichage de l'image : " + ex.Message);
            }
            finally
            {
                receivedImage.Dispose();
            }
        }

        private Image ProcessImage(Image inputImage)
        {
            Bitmap bitmap = new Bitmap(inputImage.Width, inputImage.Height, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(inputImage, 0, 0);
            }

            return bitmap;
        }

        private void serveurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ServerSelectionDialog dialog = new ServerSelectionDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    m_ipAdrDistante = dialog.SelectedIPAddress;
                    AppendLog(LogSource.Client, LogLevel.INFO, "Adresse IP du serveur mise à jour : " + m_ipAdrDistante);

                    Task.Run(() => InitClientTCP());
                }
                else
                {
                    AppendLog(LogSource.Client, LogLevel.WARNING, "Aucune adresse IP n'a été entrée. L'application ne peut pas continuer.");
                }
            }
        }

        private void quitterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AppendLog(LogSource source, LogLevel level, string message)
        {
            Log.Log logEntry = new Log.Log(source, level, message);
            string content = logEntry.ToString().Replace("\n", Environment.NewLine);

            string finalMessage = "--------------------------" + Environment.NewLine
                                  + content
                                  + Environment.NewLine;

            if (tbCom.InvokeRequired)
            {
                tbCom.Invoke(new Action(() =>
                {
                    tbCom.AppendText(finalMessage);
                }));
            }
            else
            {
                tbCom.AppendText(finalMessage);
            }
        }
    }
}
