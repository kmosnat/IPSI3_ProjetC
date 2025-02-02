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
using System.Linq;
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
        //ajout pour charger l'image
        //ajout à enlever
        // ajout conversion couleur
        private Bitmap ConvertToColor(Bitmap grayscaleImage)
        {
            Bitmap colorImage = new Bitmap(grayscaleImage.Width, grayscaleImage.Height);
            for (int y = 0; y < grayscaleImage.Height; y++)
            {
                for (int x = 0; x < grayscaleImage.Width; x++)
                {
                    Color pixelColor = grayscaleImage.GetPixel(x, y);
                    int gray = pixelColor.R; // Puisque c'est en niveaux de gris, R=G=B
                    Color newColor = Color.FromArgb(gray, gray, gray); // Conversion en couleur
                    colorImage.SetPixel(x, y, newColor);
                }
            }
            return colorImage;
        }

        private string GetLatestImagePath(string folderPath)
        {
            try
            {
                if (!Directory.Exists(folderPath))
                {
                    tbCom.LogError("Le dossier d'images n'existe pas.");
                    return null;
                }

                var files = Directory.GetFiles(folderPath, "*.*")
                    .Where(f => f.EndsWith(".png") || f.EndsWith(".jpg") || f.EndsWith(".bmp"))
                    .OrderByDescending(f => File.GetLastWriteTime(f))
                    .ToList();

                if (files.Count == 0)
                {
                    tbCom.LogError("Aucune image trouvée dans le dossier.");
                    return null;
                }

                return files.First();
            }
            catch (Exception ex)
            {
                tbCom.LogError("Erreur lors du chargement de l'image : " + ex.Message);
                return null;
            }
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

      
        private void DisplayImage(Image img)
        {
            if (img == null)
            {
                tbCom.LogError("🔴 L'image est nulle.");
                return;
            }

            tbCom.LogInfo($"🖼️ Image affichée : {img.Width}x{img.Height} | Pixel (0,0) : {((Bitmap)img).GetPixel(0, 0)}");

            this.Invoke((MethodInvoker)(() =>
            {
                pbImage.Image = null;  // Nettoyer avant d'afficher une nouvelle image
                pbImage.BackColor = Color.White;  // Mettre un fond blanc pour forcer un rafraîchissement
                pbImage.Image = new Bitmap(img);
                pbImage.SizeMode = PictureBoxSizeMode.StretchImage;
                pbImage.Refresh();
                pbImage.Invalidate();
                pbImage.Update();
                this.Refresh();
            }));

            tbCom.LogInfo("✅ Image affichée avec succès.");
        }

        // 🔹 Déclarations des méthodes de la DLL
        [DllImport("libIHM.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr objetLibDataImg(int nbChamps, byte[] data, int stride, int nbLig, int nbCol);

        [DllImport("libIHM.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr processCap(IntPtr obj);

        [DllImport("libIHM.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void destroyClibIHM(IntPtr obj);

        [DllImport("libIHM.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr valeurObject(IntPtr pImg, int i);

        [DllImport("libIHM.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int valeurChamp(IntPtr pImg, int i);
        // fin ajout à enlever

        // Fonction qui traite l'image et affiche les résultats
        private Image ProcessImage(Image receivedImage)
        {
            tbCom.LogInfo(" ProcessImage() a bien été appelée !");

            // Charger l’image depuis le disque (si nécessaire)
            string imagePath = GetLatestImagePath(@"C:\Users\diarr\OneDrive\Documents\IPSI3\ProjetReconnaissanceCouleur\image");

            if (string.IsNullOrEmpty(imagePath))
            {
                tbCom.LogError("Impossible de charger l'image.");
                return receivedImage; // ⚠️ Retourne l'image d'origine pour éviter un crash
            }

            Bitmap bitmap = new Bitmap(imagePath);

            tbCom.LogInfo($"Image chargée depuis {imagePath} : {bitmap.Width}x{bitmap.Height}");

            // 🔹 Vérification si l'image passe bien dans la DLL (traitement)
            IntPtr objPtr = objetLibDataImg(1, null, 0, bitmap.Height, bitmap.Width);
            if (objPtr == IntPtr.Zero)
            {
                tbCom.LogError(" Échec de la création de l'objet ClibIHM.");
                return receivedImage;
            }

            objPtr = processCap(objPtr);
            if (objPtr == IntPtr.Zero)
            {
                tbCom.LogError(" Échec du traitement de l'image.");
                return receivedImage;
            }

            tbCom.LogInfo("Traitement terminé, image prête à être affichée.");

            return bitmap; // 🔥 Retourne bien l'image traitée
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

        private void AddRobotObject(string color, string shape, int x, int y)
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

        private void tbCom_TextChanged(object sender, EventArgs e)
        {

        }

        private void pbImage_Click(object sender, EventArgs e)
        {

        }

       
    }
}
