using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace Client
{
    public partial class Client : Form
    {
        // Variables
        private readonly object imageLock = new object();
        private IPAddress m_ipAdrDistante;
        private int m_numPort;
        private ConcurrentDictionary<string, RobotObject> localObjects = new ConcurrentDictionary<string, RobotObject>();
        private List<(string color, string shape, float x, float y)> _lastFrameDetections = new List<(string, string, float, float)>();

        private readonly TimeSpan reconnectInterval = TimeSpan.FromSeconds(5);
        private readonly int maxReconnectAttempts = 0;
        private CancellationTokenSource reconnectCancellationTokenSource;

        private TcpListener calibrationListener;
        private CancellationTokenSource calibrationListenerCts;

        private int seuilValue;   
        private int filtreValue;

        public Client()
        {
            InitializeComponent();
            m_numPort = 8001;
            seuilValue = trackBarSeuil.Value;
            filtreValue = trackBarFiltre.Value;
        }

        // Méthode pour initialiser les composants
        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            serveurToolStripMenuItem_Click(this, EventArgs.Empty);
            _ = StartCalibrationListenerAsync();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            reconnectCancellationTokenSource?.Cancel();
            calibrationListenerCts?.Cancel();
            calibrationListener?.Stop();
            base.OnFormClosing(e);
        }

        #region Calibration Listener
        // Méthode pour démarrer le listener de calibration
        private async Task StartCalibrationListenerAsync()
        {
            try
            {
                calibrationListener = new TcpListener(IPAddress.Any, 9000);
                calibrationListener.Start();
                tbCom.LogInfo("Calibration Listener démarré sur le port 9000.");

                calibrationListenerCts = new CancellationTokenSource();
                var token = calibrationListenerCts.Token;

                while (!token.IsCancellationRequested)
                {
                    // Attendre une connexion
                    TcpClient client = await calibrationListener.AcceptTcpClientAsync();
                    _ = Task.Run(() => HandleCalibrationClientAsync(client, token), token);
                }
            }
            catch (OperationCanceledException)
            {
                tbCom.LogInfo("Calibration Listener arrêté.");
            }
            catch (Exception ex)
            {
                tbCom.LogError($"Erreur Calibration Listener : {ex.Message}");
            }
        }

        // Méthode pour traiter les commandes de calibration
        private async Task HandleCalibrationClientAsync(TcpClient client, CancellationToken token)
        {
            try
            {
                using (client)
                using (var networkStream = client.GetStream())
                {
                    // Lire la commande
                    string command = await ReadLineAsync(networkStream, token);
                    tbCom.LogInfo($"Commande reçue du serveur : {command}");

                    // Traiter la commande
                    if (command.Equals("GET_CURRENT_COORDINATES", StringComparison.OrdinalIgnoreCase))
                    {
                        // Récupérer les coordonnées actuelles
                        var currentCoordinates = GetCurrentCoordinates();
                        string response = $"{currentCoordinates.x.ToString(CultureInfo.InvariantCulture)},{currentCoordinates.y.ToString(CultureInfo.InvariantCulture)}\n";
                        // Envoyer les coordonnées au serveur
                        byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                        await networkStream.WriteAsync(responseBytes, 0, responseBytes.Length, token);
                        tbCom.LogInfo("Coordonnées actuelles envoyées au serveur.");
                    }
                    else
                    {
                        tbCom.LogError($"Commande inconnue : {command}");
                    }
                }
            }
            catch (Exception ex)
            {
                tbCom.LogError($"Erreur lors du traitement de la commande de calibration : {ex.Message}");
            }
        }

        // Méthode pour lire une ligne depuis le stream
        private async Task<string> ReadLineAsync(NetworkStream stream, CancellationToken token)
        {
            var sb = new StringBuilder();
            byte[] buffer = new byte[1];
            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, 1, token);
                if (bytesRead == 0)
                    break;
                char ch = (char)buffer[0];
                if (ch == '\n')
                    break;
                sb.Append(ch);
            }
            return sb.ToString().Trim();
        }

        // Méthode pour récupérer les coordonnées actuelles
        private (float x, float y) GetCurrentCoordinates()
        {
            if (_lastFrameDetections.Count == 0)
            {
                tbCom.LogError("Aucun point de détection disponible pour la calibration.");
                return (0f, 0f);
            }
            float x = _lastFrameDetections[0].x;
            float y = _lastFrameDetections[0].y;
            tbCom.LogInfo($"Coordonnées actuelles : X={x}, Y={y}");
            return (x, y);
        }

        #endregion

        #region Communication avec le serveur (Images & Objets)
        // Méthode pour convertir un tableau de bytes en entier non signé
        private uint FromBigEndianBytes(byte[] bytes)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }

        // Méthode pour initialiser le client TCP
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
                    // Connexion au serveur
                    var connectTask = tcpClient.ConnectAsync(m_ipAdrDistante, m_numPort);
                    var timeoutTask = Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                    var completedTask = await Task.WhenAny(connectTask, timeoutTask);
                    if (completedTask == timeoutTask)
                        throw new TimeoutException("Délai de connexion dépassé.");
                    await connectTask;
                    tbCom.LogInfo("Connexion établie");
                    // Mettre à jour l'état de la connexion
                    UpdateStatus(true);

                    using (NetworkStream networkStream = tcpClient.GetStream())
                    {
                        // Envoyer la requête d'image
                        string request = "GET_IMAGE\n";
                        byte[] requestBytes = Encoding.ASCII.GetBytes(request);
                        await networkStream.WriteAsync(requestBytes, 0, requestBytes.Length, cancellationToken);
                        // Attendre que les données soient envoyées
                        await networkStream.FlushAsync(cancellationToken);
                        tbCom.LogInfo("Requête d'image envoyée : " + request);

                        const uint maxExpectedSize = 10_000_000;
                        // Lire les images
                        while (tcpClient.Connected && !cancellationToken.IsCancellationRequested)
                        {
                            byte[] sizeBytes = new byte[4];
                            int totalRead = 0;
                            while (totalRead < 4)
                            {
                                int bytesRead = await networkStream.ReadAsync(sizeBytes, totalRead, 4 - totalRead, cancellationToken);
                                if (bytesRead == 0)
                                    throw new Exception("Connexion fermée avant de recevoir la taille de l'image.");
                                totalRead += bytesRead;
                            }
                            uint imageSize = FromBigEndianBytes(sizeBytes);
                            if (imageSize == 0)
                            {
                                tbCom.LogError("Image de taille 0 reçue.");
                                continue;
                            }
                            if (imageSize > maxExpectedSize)
                                throw new Exception($"Taille d'image invalide reçue : {imageSize}");

                            byte[] imageBytes = new byte[imageSize];
                            totalRead = 0;
                            while (totalRead < imageSize)
                            {
                                int bytesRead = await networkStream.ReadAsync(imageBytes, totalRead, (int)(imageSize - totalRead), cancellationToken);
                                if (bytesRead == 0)
                                    throw new Exception("Connexion fermée avant de recevoir toute l'image.");
                                totalRead += bytesRead;
                            }
                            // Afficher l'image
                            await Task.Run(() =>
                            {
                                try
                                {
                                    using (MemoryStream ms = new MemoryStream(imageBytes))
                                    {
                                        // Convertir les bytes en image
                                        Image receivedImage = Image.FromStream(ms);
                                        // Afficher l'image
                                        DisplayImage(receivedImage);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    tbCom.LogError($"Erreur lors du traitement de l'image : {ex.Message}");
                                }
                            }, cancellationToken);
                        }
                    }
                }
                catch (Exception ex)
                {
                    tbCom.LogError("Erreur dans InitClientTCPAsync : " + ex.Message);
                    // Mettre à jour l'état de la connexion
                    UpdateStatus(false);
                    // Incrémenter le nombre de tentatives
                    attempt++;
                    if (maxReconnectAttempts > 0 && attempt >= maxReconnectAttempts)
                    {
                        tbCom.LogError("Nombre maximal de tentatives de reconnexion atteint.");
                        break;
                    }
                    tbCom.LogInfo($"Nouvelle tentative dans {reconnectInterval.TotalSeconds} secondes...");
                    // Attendre avant de retenter la connexion
                    await Task.Delay(reconnectInterval, cancellationToken);
                }
                finally
                {
                    // Fermer la connexion
                    tcpClient.Close();
                    tbCom.LogInfo("Connexion fermée.");
                }
            }
        }

        // Méthode pour mettre à jour l'état de la connexion
        private void UpdateStatus(bool isConnected)
        {
            this.Invoke((MethodInvoker)(() =>
            {
                toolStripStatus.Text = isConnected ? "État : Connecté" : "État : Déconnecté";
                toolStripStatus.ForeColor = isConnected ? Color.Green : Color.Red;
            }));
        }

        // Méthode pour afficher l'image
        private void DisplayImage(Image receivedImage)
        {
            try
            {
                // Traiter l'image
                Image processedImage = ProcessImage(receivedImage);
                this.Invoke((MethodInvoker)(() =>
                {
                    if (this.pbImage.Image != null)
                    {
                        // Libérer les ressources de l'image précédente
                        this.pbImage.Image.Dispose();
                    }
                    // Afficher l'image
                    this.pbImage.Image = processedImage;
                }));
            }
            catch (Exception ex)
            {
                tbCom.LogError("Erreur lors de l'affichage de l'image : " + ex.Message);
            }
            finally
            {
                receivedImage.Dispose();
            }
        }

        // Méthode pour traiter l'image
        private Image ProcessImage(Image inputImage)
        {
            // Convertir l'image en format 24bpp
            Bitmap bitmap = new Bitmap(inputImage.Width, inputImage.Height, PixelFormat.Format24bppRgb);
            // Copier l'image
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(inputImage, 0, 0);
            }
            BitmapData bitmapData = null;
            try
            {
                // Verrouiller les bits de l'image
                Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                int width = bitmapData.Width;
                int height = bitmapData.Height;
                int stride = bitmapData.Stride;
                int bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
                int packedStride = width * bytesPerPixel;
                byte[] imageData = new byte[height * packedStride];
                IntPtr scan0 = bitmapData.Scan0;
                // Copier les données de l'image
                unsafe
                {
                    for (int y = 0; y < height; y++)
                    {
                        Marshal.Copy(new IntPtr(((byte*)scan0.ToPointer()) + y * stride), imageData, y * packedStride, packedStride);
                    }
                }
                // Traiter l'image
                using (ClImage clImage = new ClImage())
                {
                    // Initialiser l'objet ClImage
                    clImage.ObjetLibDataImgPtr(3, Marshal.UnsafeAddrOfPinnedArrayElement(imageData, 0), packedStride, height, width);
                    // Traiter l'image
                    clImage.ProcessCapPtr(seuilValue, filtreValue);
                    _lastFrameDetections.Clear();

                    // Récupérer les objets détectés
                    int objectCount = (int)clImage.ObjetLibValeurChamp(0);
                    for (int i = 0; i < objectCount; i++)
                    {
                        try
                        {
                            // Extraire les informations de l'objet
                            string objectInfo = clImage.ObjetLibObjectChamp(i);
                            tbCom.LogInfo($"{clImage.ObjetLibValeurChamp(2)}");
                            var parts = objectInfo.Split(',');
                            if (parts.Length != 4)
                            {
                                tbCom.LogError($"Format d'objet invalide : {objectInfo}");
                                continue;
                            }
                            string color = parts[0].Trim().ToLowerInvariant();
                            string shape = parts[1].Trim().ToLowerInvariant();
                            if (!float.TryParse(parts[2].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float x))
                            {
                                tbCom.LogError($"Erreur de parsing X : {parts[2].Trim()}");
                                continue;
                            }
                            if (!float.TryParse(parts[3].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float y))
                            {
                                tbCom.LogError($"Erreur de parsing Y : {parts[3].Trim()}");
                                continue;
                            }
                            // Ajouter l'objet à la liste
                            _lastFrameDetections.Add((color, shape, x, y));
                            Task.Run(() => AddRobotObject(color, shape, x, y));
                        }
                        catch (Exception ex)
                        {
                            tbCom.LogError($"Erreur lors de l'extraction d'un objet : {ex.Message}");
                        }
                    }
                }
                unsafe
                {
                    for (int y = 0; y < height; y++)
                    {
                        Marshal.Copy(imageData, y * packedStride, new IntPtr(((byte*)bitmapData.Scan0.ToPointer()) + y * stride), packedStride);
                    }
                }
                // Déverrouiller les bits de l'image
                bitmap.UnlockBits(bitmapData);
                bitmapData = null;
                return bitmap;
            }
            catch
            {
                if (bitmapData != null)
                    bitmap.UnlockBits(bitmapData);
                bitmap.Dispose();
                throw;
            }
        }

        private void trackBarFiltre_Scroll(object sender, EventArgs e)
        {
            filtreValue = trackBarFiltre.Value;
        }

        private void trackBarSeuil_Scroll(object sender, EventArgs e)
        {
            seuilValue = trackBarSeuil.Value;
        }

        // Méthode pour afficher la fenêtre d'ajout d'adresse IP
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

        private void quitterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            reconnectCancellationTokenSource?.Cancel();
            this.Close();
        }

        // Méthode pour ajouter un objet au serveur
        private async void AddRobotObject(string color, string shape, float x, float y)
        {
            // Créer un objet RobotObject
            var robotObject = new RobotObject(color, shape, x, y);
            // Vérifier si l'objet existe déjà
            string key = robotObject.Key;
            if (!localObjects.TryAdd(key, robotObject))
            {
                return;
            }
            tbCom.LogInfo($"Ajout de l'objet {color}, {shape}, {x}, {y} avec clé '{key}'.");
            string robotObjectJson = robotObject.ToString();
            tbCom.LogInfo($"Serialized RobotObject: {robotObjectJson}");
            string addObjectCommand = $"ADD_OBJECT, {robotObjectJson}\n";
            tbCom.LogInfo($"Commande ADD_OBJECT envoyée : {addObjectCommand.Trim()}");
            byte[] commandBytes = Encoding.UTF8.GetBytes(addObjectCommand);
            try
            {
                // Connexion au serveur
                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync(m_ipAdrDistante, m_numPort);
                    using (NetworkStream networkStream = client.GetStream())
                    {
                        // Envoyer la commande
                        await networkStream.WriteAsync(commandBytes, 0, commandBytes.Length);
                        await networkStream.FlushAsync();
                        byte[] buffer = new byte[1024];
                        int bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                        string response = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                        // Traiter la réponse
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
                            // Supprimer l'objet de la liste locale
                            localObjects.TryRemove(key, out _);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                tbCom.LogError($"Erreur lors de l'envoi de l'objet '{key}' : {ex.Message}");
                localObjects.TryRemove(key, out _);
            }
        }


        #endregion
    }
}
