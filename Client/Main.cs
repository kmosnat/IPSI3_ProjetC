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
using System.Collections.Concurrent;

using Utils;
using System.Threading;
using System.Globalization;

namespace Client
{
    public partial class Client : Form
    {
        private readonly object imageLock = new object();
        private IPAddress m_ipAdrDistante;
        private int m_numPort;
        private ConcurrentDictionary<string, RobotObject> localObjects = new ConcurrentDictionary<string, RobotObject>();

        private readonly TimeSpan reconnectInterval = TimeSpan.FromSeconds(5);
        private readonly int maxReconnectAttempts = 0; // 0 pour illimité
        private CancellationTokenSource reconnectCancellationTokenSource;

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

        private async Task InitClientTCPAsync(CancellationToken cancellationToken)
        {
            if (m_ipAdrDistante == null)
            {
                tbCom.LogError("Adresse IP non définie. Veuillez entrer l'adresse IP du serveur.");

                

                return;
            }

            int attempt = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                TcpClient tcpClient = new TcpClient();
                try
                {
                    tbCom.LogInfo("Tentative de connexion au serveur...");
                    var connectTask = tcpClient.ConnectAsync(m_ipAdrDistante, m_numPort);
                    var timeoutTask = Task.Delay(TimeSpan.FromSeconds(10), cancellationToken); // Timeout de 10 secondes

                    var completedTask = await Task.WhenAny(connectTask, timeoutTask);
                    if (completedTask == timeoutTask)
                    {
                        throw new TimeoutException("Délai de connexion dépassé.");
                    }

                    await connectTask;
                    tbCom.LogInfo("Connexion établie");
                    UpdateStatus(true);

                    NetworkStream networkStream = tcpClient.GetStream();

                    string request = "GET_IMAGE\n";
                    byte[] requestBytes = Encoding.ASCII.GetBytes(request);
                    await networkStream.WriteAsync(requestBytes, 0, requestBytes.Length, cancellationToken);
                    await networkStream.FlushAsync(cancellationToken);
                    tbCom.LogInfo("Requête d'image envoyée : " + request);

                    const uint maxExpectedSize = 10_000_000;

                    while (tcpClient.Connected && !cancellationToken.IsCancellationRequested)
                    {
                        // Lecture de la taille de l'image
                        byte[] sizeBytes = new byte[4];
                        int totalRead = 0;
                        while (totalRead < 4)
                        {
                            int bytesRead = await networkStream.ReadAsync(sizeBytes, totalRead, 4 - totalRead, cancellationToken);
                            if (bytesRead == 0)
                            {
                                throw new Exception("Connexion fermée avant de recevoir la taille de l'image.");
                            }
                            totalRead += bytesRead;
                        }

                        uint imageSize = FromBigEndianBytes(sizeBytes);

                        if (imageSize == 0)
                        {
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
                            int bytesRead = await networkStream.ReadAsync(imageBytes, totalRead, (int)(imageSize - totalRead), cancellationToken);
                            if (bytesRead == 0)
                            {
                                throw new Exception("Connexion fermée avant de recevoir toute l'image.");
                            }
                            totalRead += bytesRead;
                        }

                        using (MemoryStream ms = new MemoryStream(imageBytes))
                        {
                            Image receivedImage = Image.FromStream(ms);
                            DisplayImage(receivedImage);
                        }

                        UpdateStatus(true);
                    }
                }
                catch (Exception ex)
                {
                    tbCom.LogError("Erreur : " + ex.Message);
                    UpdateStatus(false);

                    attempt++;
                    if (maxReconnectAttempts > 0 && attempt >= maxReconnectAttempts)
                    {
                        tbCom.LogError("Nombre maximal de tentatives de reconnexion atteint.");
                        break;
                    }

                    tbCom.LogInfo($"Nouvelle tentative de connexion dans {reconnectInterval.TotalSeconds} secondes...");
                    await Task.Delay(reconnectInterval, cancellationToken);
                }
                finally
                {
                    if (tcpClient != null)
                    {
                        tcpClient.Close();
                        tbCom.LogInfo("Connexion fermée.");
                    }
                }
            }
        }

        private void UpdateStatus(bool isConnected)
        {
            this.Invoke((MethodInvoker)(() =>
            {
                toolStripStatus.Text = isConnected ? "État : Connecté" : "État : Déconnecté";
                toolStripStatus.ForeColor = isConnected ? Color.Green : Color.Red;

            }));
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
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de l'affichage de l'image : " + ex.Message);
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

                    int couleur = (int)ClImage.valeurChamp(clImage.ClPtr, 0);   // Couleur détectée
                    int forme = (int)ClImage.valeurChamp(clImage.ClPtr, 1);     // Forme détectée
                    int posX = (int)ClImage.valeurChamp(clImage.ClPtr, 2);      // Position X
                    int posY = (int)ClImage.valeurChamp(clImage.ClPtr, 3);      // Position Y

                    // Affichage des résultats dans une zone locale (par exemple, `tbCom` dans le client)
                    tbCom.LogInfo("===== Résultats reçus du traitement =====.");
                    tbCom.LogInfo("Couleur détectée : {couleur}.");
                    tbCom.LogInfo("Forme détectée   : {forme}.");
                    tbCom.LogInfo("Position X       : {posX}.");
                    tbCom.LogInfo("Position Y       : {posY}.");

                }

                            string color = parts[0].Trim();
                            string shape = parts[1].Trim();
                            string xStr = parts[2].Trim();
                            string yStr = parts[3].Trim();

                            tbCom.LogInfo($"Couleur: {color}");
                            tbCom.LogInfo($"Forme: {shape}");
                            tbCom.LogInfo($"X: {xStr}");
                            tbCom.LogInfo($"Y: {yStr}");

                            float x, y;

                            if (!float.TryParse(xStr, NumberStyles.Float, CultureInfo.InvariantCulture, out x))
                            {
                                tbCom.LogError($"Erreur de parsing de la coordonnée X : {xStr}");
                                continue;
                            }

                            if (!float.TryParse(yStr, NumberStyles.Float, CultureInfo.InvariantCulture, out y))
                            {
                                tbCom.LogError($"Erreur de parsing de la coordonnée Y : {yStr}");
                                continue;
                            }



        }


        private void serveurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ServerSelectionDialog dialog = new ServerSelectionDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    m_ipAdrDistante = dialog.SelectedIPAddress;
                    tbCom.LogInfo("Adresse IP du serveur mise à jour : " + m_ipAdrDistante);

                    reconnectCancellationTokenSource?.Cancel();

                    reconnectCancellationTokenSource = new CancellationTokenSource();
                    var token = reconnectCancellationTokenSource.Token;

                    Task.Run(() => InitClientTCPAsync(token), token);
                }
                else
                {
                    tbCom.LogWarning("Aucune adresse IP n'a été entrée. L'application ne peut pas continuer.");
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            reconnectCancellationTokenSource?.Cancel();
            base.OnFormClosing(e);
        }

        private void quitterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            reconnectCancellationTokenSource?.Cancel();
            this.Close();
        }

        private void AddRobotObject(string color, string shape, float x, float y)
        {
            var robotObject = new RobotObject(color, shape, x, y);
            string key = robotObject.Key;

            if (localObjects.TryAdd(key, robotObject))
            {
                tbCom.LogInfo($"Ajout de l'objet {color}, {shape}, {x}, {y} avec clé '{key}'.");

                string robotObjectJson = robotObject.ToString();
                tbCom.LogInfo($"Serialized RobotObject: {robotObjectJson}");

                string addObjectCommand = $"ADD_OBJECT, {robotObjectJson}\n";
                tbCom.LogInfo($"Commande ADD_OBJECT envoyée : {addObjectCommand.Trim()}");

                byte[] commandBytes = Encoding.UTF8.GetBytes(addObjectCommand);

                try
                {
                    using (TcpClient client = new TcpClient())
                    {
                        client.Connect(m_ipAdrDistante, m_numPort);
                        NetworkStream networkStream = client.GetStream();
                        networkStream.Write(commandBytes, 0, commandBytes.Length);
                        networkStream.Flush();

                        byte[] buffer = new byte[1024];
                        int bytesRead = networkStream.Read(buffer, 0, buffer.Length);
                        string response = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                        tbCom.LogInfo($"Réponse du serveur : {response}");

                        if (response.StartsWith("OBJET AJOUTÉ", StringComparison.OrdinalIgnoreCase))
                        {
                            tbCom.LogInfo($"Objet '{key}' ajouté avec succès.");
                        }
                        else if (response.StartsWith("OBJET EXISTE DÉJÀ", StringComparison.OrdinalIgnoreCase))
                        {
                            tbCom.LogInfo($"Objet '{key}' existe déjà.");
                        }
                        else
                        {
                            tbCom.LogError($"Erreur lors de l'ajout de l'objet '{key}' : {response}");
                            // Si l'ajout a échoué, retirer l'objet de la collection locale
                            localObjects.TryRemove(key, out _);
                        }
                    }
                }
                catch (Exception ex)
                {
                    tbCom.LogError($"Erreur lors de l'envoi de l'objet '{key}' : {ex.Message}");
                    // Si une erreur survient, retirer l'objet de la collection locale
                    localObjects.TryRemove(key, out _);
                }
            }
            else
            {
                //tbCom.LogInfo($"Objet {color}, {shape}, {x}, {y} avec clé '{key}' existe déjà. Ignoré.");
            }
        }


        private void testObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string color = "Rouge";
            string shape = "Triangle";
            int x = 300;
            int y = 200;
            AddRobotObject(color, shape, x, y);
        }
    }
}
