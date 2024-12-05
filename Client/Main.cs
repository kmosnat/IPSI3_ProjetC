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

namespace Client
{
    public partial class Main : Form
    {
        private readonly object imageLock = new object();
        private IPAddress m_ipAdrDistante;
        private int m_numPort;
        private System.Windows.Forms.Timer imageTimer;

        public Main()
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
                this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Adresse IP non définie. Veuillez entrer l'adresse IP du serveur.\r\n")));
                return;
            }

            TcpClient tcpClient = null;
            try
            {
                tcpClient = new TcpClient();
                this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Connexion en cours...\r\n")));

                tcpClient.Connect(m_ipAdrDistante, m_numPort);
                this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Connexion établie\r\n")));

                NetworkStream networkStream = tcpClient.GetStream();

                string request = "GET_IMAGE\n";
                byte[] requestBytes = Encoding.ASCII.GetBytes(request);
                networkStream.Write(requestBytes, 0, requestBytes.Length);
                networkStream.Flush();
                this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Requête d'image envoyée : " + request + "\r\n")));

                const uint maxExpectedSize = 10_000_000;

                while (tcpClient.Connected)
                {
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
                    this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Taille de l'image à recevoir : " + imageSize + " octets.\r\n")));

                    if (imageSize == 0)
                    {
                        this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Le serveur a signalé une erreur lors de la capture de l'image.\r\n")));
                        continue;
                    }

                    if (imageSize > maxExpectedSize)
                    {
                        throw new Exception($"Taille d'image invalide reçue : {imageSize}");
                    }

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

                    this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Image reçue en " + totalRead + " octets.\r\n")));

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
                this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Erreur : " + ex.Message + "\r\n")));

                this.statusIndicator.Invoke((MethodInvoker)(() => this.statusIndicator.BackColor = Color.Red));
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

                this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Image affichée.\r\n")));
            }
            catch (Exception ex)
            {
                this.tbCom.Invoke((MethodInvoker)(() => this.tbCom.AppendText("Erreur lors de l'affichage de l'image : " + ex.Message + "\r\n")));
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

            BitmapData bitmapData = null;

            try
            {
                Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                int width = bitmapData.Width;
                int height = bitmapData.Height;
                int stride = bitmapData.Stride;
                int bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;

                int packedStride = width * bytesPerPixel;
                byte[] imageData = new byte[height * packedStride];

                IntPtr scan0 = bitmapData.Scan0;

                unsafe
                {
                    byte* sourcePtr = (byte*)scan0.ToPointer();

                    for (int y = 0; y < height; y++)
                    {
                        Marshal.Copy(new IntPtr(sourcePtr + y * stride), imageData, y * packedStride, packedStride);
                    }
                }

                using (ClImage clImage = new ClImage())
                {
                    clImage.ObjetLibDataImgPtr(
                        nbChamps: 3,
                        data: Marshal.UnsafeAddrOfPinnedArrayElement(imageData, 0),
                        stride: packedStride,
                        nbLig: height,
                        nbCol: width);

                    clImage.ProcessCapPtr();
                }

                unsafe
                {
                    byte* destPtr = (byte*)scan0.ToPointer();

                    for (int y = 0; y < height; y++)
                    {
                        Marshal.Copy(imageData, y * packedStride, new IntPtr(destPtr + y * stride), packedStride);
                    }
                }

                bitmap.UnlockBits(bitmapData);
                bitmapData = null;

                return bitmap;
            }
            catch
            {
                if (bitmapData != null)
                {
                    bitmap.UnlockBits(bitmapData);
                }
                bitmap.Dispose();
                throw;
            }
        }

        private void serveurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ServerSelectionDialog dialog = new ServerSelectionDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    m_ipAdrDistante = dialog.SelectedIPAddress;
                    this.tbCom.AppendText("Adresse IP du serveur mise à jour : " + m_ipAdrDistante.ToString() + "\r\n");

                    Task.Run(() => InitClientTCP());
                }
                else
                {
                    this.tbCom.AppendText("Aucune adresse IP n'a été entrée. L'application ne peut pas continuer.\r\n");
                }
            }
        }


        private void quitterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
