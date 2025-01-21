using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;           
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using EasyModbus;
using Utils;

namespace Serveur
{
    public partial class Main : Form
    {
        // ---- Propriétés caméra (inchangées) ----
        private smcs.IDevice _device;
        private Rectangle _imageRect;
        private PixelFormat _pixelFormat;
        private UInt32 _pixelType;
        private Bitmap _customTestImage = null;

        // ---- File d'objets et gestion d'état ----
        // Concurrency => pour la logique
        private ConcurrentQueue<RobotObject> objectBuffer = new ConcurrentQueue<RobotObject>();

        // État interne du robot
        private RobotState robotState = RobotState.Wait;

        // Objet courant en cours de traitement
        private RobotObject currentRobotObject = null;

        // Indique si un mouvement est en cours
        private bool isMoving = false;
        private Task moveTask;

        // ---- Adresses & robot ----
        private string _ipRobot;
        private RobotModbusHelper robot;

        // ---- TCP ----
        private IPAddress _localIPAddress;
        private int _port;
        private bool _isTCPRunning = false;
        private CancellationTokenSource _tcpCancellationTokenSource;
        private TcpListener _tcpListener;
        private readonly object _tcpLock = new object();

        // ---- Caméra / acquisition ----
        private bool _isAcquisitionRunning = false;
        private readonly object _deviceLock = new object();

        // == Nouveaux contrôles UI ==   
        private BindingList<RobotObject> robotObjectsList = new BindingList<RobotObject>();
        private TableLayoutPanel mainTableLayout;


        public Main()
        {
            InitializeComponent();

            _port = 8001;
            InitializeUIState();

            _ipRobot = "169.254.200.200";
            robot = new RobotModbusHelper(_ipRobot, 5020);

            ethernetToolStripMenuItem.Enabled = false;
            hotspotToolStripMenuItem.Enabled = true;

            var timerRobotState = new System.Windows.Forms.Timer();
            timerRobotState.Interval = 500;
            timerRobotState.Tick += timerRobotState_Tick;
            timerRobotState.Start();

            dgvObjects.AutoGenerateColumns = true;
            dgvObjects.DataSource = robotObjectsList;

        }

        private void Main_Load(object sender, EventArgs e)
        {
            NetworkSelection();
        }


        private void timerRobotState_Tick(object sender, EventArgs e)
        {
            UpdateRobotStateMachine();
        }

        private void UpdateRobotStateMachine()
        {
            switch (robotState)
            {
                case RobotState.Wait:
                    if (objectBuffer.TryDequeue(out RobotObject robotObject))
                    {
                        tbCom.LogInfo($"Objet à traiter : {robotObject}", LogSource.Serveur);

                        currentRobotObject = robotObject;
                        robotState = RobotState.OnProcess;
                    }
                    break;

                case RobotState.OnProcess:
                    tbCom.LogInfo("Envoi des informations au robot...", LogSource.Serveur);

                    isMoving = true;
                    moveTask = Task.Run(() =>
                    {
                        float x = currentRobotObject.X / 1000f;
                        float y = currentRobotObject.Y / 1000f;

                        moveRobot(x, y);
                        isMoving = false;
                    });

                    // Passage immédiat en "Moving"
                    robotState = RobotState.RobotOnMoving;
                    break;

                case RobotState.RobotOnMoving:
                    // On attend que la tâche de mouvement se termine.
                    if (!isMoving)
                    {
                        tbCom.LogInfo("Mouvement du robot terminé.", LogSource.Serveur);

                        // Une fois terminé, on peut retirer l’objet de la liste si on veut
                        if (currentRobotObject != null)
                        {
                            // Cherche l'objet dans la BindingList et le retire
                            var roToRemove = robotObjectsList.FirstOrDefault(ro => ro.Id == currentRobotObject.Id);
                            if (roToRemove != null)
                            {
                                robotObjectsList.Remove(roToRemove);
                            }
                        }

                        robotState = RobotState.Wait;
                    }
                    break;
            }

            // Mise à jour de l'affichage du Label d’état
            InvokeIfNeeded(() =>
            {
                switch (robotState)
                {
                    case RobotState.Wait:
                        lblRobotState.Text = "État Robot: En attente";
                        lblRobotState.BackColor = Color.LightGray;
                        break;
                    case RobotState.OnProcess:
                        lblRobotState.Text = "État Robot: Analyse objet…";
                        lblRobotState.BackColor = Color.Khaki;
                        break;
                    case RobotState.RobotOnMoving:
                        lblRobotState.Text = "État Robot: En mouvement";
                        lblRobotState.BackColor = Color.LightGreen;
                        break;
                }
            });
        }

        private void moveRobot(float x, float y)
        {
            try
            {
                robot.Connect();

                float[] joints = robot.GetCurrentJointStates();
                tbCom.LogInfo("[ROBOT] Lecture des Joints :", LogSource.Serveur);
                for (int i = 0; i < joints.Length; i++)
                {
                    tbCom.LogInfo($"Joint {i + 1} = {joints[i]:F4} rad", LogSource.Serveur);
                }

                RobotPose currentPose = robot.GetCurrentPose();
                tbCom.LogInfo($"Pose courante => {currentPose}", LogSource.Serveur);

                robot.MoveToPose(x, y, 0.1f);
                tbCom.LogInfo($"Déplacement du robot vers X={x}, Y={y}, Z=0.1 en cours...", LogSource.Serveur);
            }
            catch (Exception ex)
            {
                tbCom.LogError("Erreur Modbus: " + ex.Message, LogSource.Serveur);
            }
            finally
            {
                robot.Disconnect();
            }
        }

        private async Task InitCamera()
        {
            bool cameraConnected = false;

            smcs.CameraSuite.InitCameraAPI();
            var smcsVisionApi = smcs.CameraSuite.GetCameraAPI();

            await Task.Run(() =>
            {
                smcsVisionApi.FindAllDevices(3.0);
                var devices = smcsVisionApi.GetAllDevices();

                if (devices.Length > 0)
                {
                    _device = devices[0];

                    if (_device != null && _device.Connect())
                    {
                        UpdateUIOnCameraConnected();

                        bool status = _device.SetStringNodeValue("TriggerMode", "Off");
                        status = _device.SetStringNodeValue("AcquisitionMode", "Continuous");
                        status = _device.SetIntegerNodeValue("TLParamsLocked", 1);
                        status = _device.CommandNodeExecute("AcquisitionStart");
                        cameraConnected = true;

                        InvokeIfNeeded(() =>
                        {
                            btnStartAcquisition.Enabled = true;
                            btnStartAcquisition.BackColor = Color.LightGreen;
                        });

                        if (!_isTCPRunning)
                        {
                            _ = StartServerAsync();
                        }
                    }
                }
            });

            if (!cameraConnected)
            {
                InvokeIfNeeded(() =>
                {
                    lblAdrIP.BackColor = Color.Red;
                    lblAdrIP.Text = "Erreur de connexion!";
                });
            }
        }

        private void UpdateUIOnCameraConnected()
        {
            InvokeIfNeeded(() =>
            {
                lblConnectionCamera.BackColor = Color.LimeGreen;
                lblConnectionCamera.Text = "Connexion établie";
                lblAdrIP.BackColor = Color.LimeGreen;
                if (_device != null)
                    lblAdrIP.Text = "Adresse IP : " + Common.IpAddrToString(_device.GetIpAddress());
                if (_device != null)
                    lblNomCamera.Text = _device.GetManufacturerName() + " : " + _device.GetModelName();
            });
        }

        private Bitmap GetNextFrame()
        {
            lock (_deviceLock)
            {
                if (_device != null && _device.IsConnected())
                {
                    if (!_device.IsBufferEmpty())
                    {
                        smcs.IImageInfo imageInfo = null;
                        _device.GetImageInfo(ref imageInfo);
                        if (imageInfo != null)
                        {
                            Bitmap bitmap = null;
                            BitmapData bd = null;
                            ImageUtils.CopyToBitmap(imageInfo, ref bitmap, ref bd, ref _pixelFormat, ref _imageRect, ref _pixelType);

                            if (bd != null)
                                bitmap.UnlockBits(bd);

                            _device.PopImage(imageInfo);
                            return bitmap;
                        }
                    }
                }
                else
                {
                    // Génère une simple image de test si la caméra n’est pas connectée
                    return GenerateTestImage();
                }
            }
            return null;
        }

        private Bitmap GenerateTestImage()
        {
            if (_customTestImage != null)
            {
                return new Bitmap(_customTestImage);
            }
            try
            {
                var bitmap = new Bitmap(640, 480, PixelFormat.Format24bppRgb);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.Clear(Color.Gray);
                    g.DrawString("Image de test", new Font("Arial", 24),
                                 Brushes.Black, new PointF(10, 10));
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

        private void timAcq_Tick(object sender, EventArgs e)
        {
            try
            {
                Bitmap bitmap = GetNextFrame();
                if (bitmap != null)
                {
                    SetPictureBoxImage(bitmap);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur dans timAcq_Tick : " + ex.Message);
            }
        }

        private void SetPictureBoxImage(Bitmap bitmap)
        {
            if (pbImage.InvokeRequired)
            {
                pbImage.Invoke(new MethodInvoker(() => pbImage.Image = bitmap));
            }
            else
            {
                pbImage.Image = bitmap;
            }
        }

        private void CloseCamera()
        {
            timAcq.Stop();
            _isAcquisitionRunning = false;
            btnStartAcquisition.Enabled = true;
            btnStopAcquisition.Enabled = false;

            if (_device != null && _device.IsConnected())
            {
                _device.CommandNodeExecute("AcquisitionStop");
                _device.SetIntegerNodeValue("TLParamsLocked", 0);
                _device.Disconnect();
            }

            smcs.CameraSuite.ExitCameraAPI();
        }

        private void btnSearchCamera_Click(object sender, EventArgs e)
        {
            try
            {
                _ = InitCamera();
            }
            catch (Exception ex)
            {
                tbCom.LogError($"Erreur lors de l'initialisation de la caméra : {ex.Message}",
                               LogSource.Serveur);
            }
        }

        private void btnStartAcquisition_Click(object sender, EventArgs e)
        {
            if (!_isAcquisitionRunning)
            {
                _isAcquisitionRunning = true;
                timAcq.Start();
                btnStartAcquisition.Enabled = false;
                btnStartAcquisition.BackColor = Color.Green;
                btnStopAcquisition.BackColor = Color.LightGray;
                btnStopAcquisition.Enabled = true;
            }
        }

        private void btnStopAcquisition_Click(object sender, EventArgs e)
        {
            if (_isAcquisitionRunning)
            {
                timAcq.Stop();
                _isAcquisitionRunning = false;
                btnStartAcquisition.Enabled = true;
                btnStartAcquisition.BackColor = Color.LightGreen;
                btnStopAcquisition.Enabled = false;
                btnStopAcquisition.BackColor = SystemColors.Control;
            }
        }

        private async void démarrerLeServeurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_isTCPRunning)
            {
                await StartServerAsync();
            }
        }

        private void arrêterLeServeurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopTCPServer();
        }

        private void sélectionnerUneCarteRéseauToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NetworkSelection();
        }

        private async Task StartServerAsync()
        {
            lock (_tcpLock)
            {
                if (_isTCPRunning) return;
                _isTCPRunning = true;
            }

            _tcpCancellationTokenSource = new CancellationTokenSource();
            var token = _tcpCancellationTokenSource.Token;

            try
            {
                _tcpListener = new TcpListener(_localIPAddress, _port);
                _tcpListener.Start();

                tbCom.LogInfo($"Serveur démarré sur {_localIPAddress}:{_port}", LogSource.Serveur);

                InvokeIfNeeded(() =>
                {
                    startTCP.Enabled = false;
                    stopTCP.Enabled = true;
                });

                while (!token.IsCancellationRequested)
                {
                    var acceptTask = _tcpListener.AcceptSocketAsync();
                    var completedTask = await Task.WhenAny(acceptTask, Task.Delay(Timeout.Infinite, token));

                    if (completedTask == acceptTask)
                    {
                        var clientSocket = acceptTask.Result;
                        if (clientSocket != null)
                        {
                            tbCom.LogInfo($"Connexion acceptée de {clientSocket.RemoteEndPoint}", LogSource.Serveur);
                            _ = Task.Run(() => HandleClient(clientSocket, token), token);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                tbCom.LogInfo("Serveur annulé.", LogSource.Serveur);
            }
            catch (Exception ex)
            {
                tbCom.LogError($"Erreur serveur : {ex.Message}", LogSource.Serveur);
            }
            finally
            {
                if (_tcpListener != null)
                {
                    _tcpListener.Stop();
                    _tcpListener = null;
                }
                StopServerInternal();
            }
        }

        private void StopServerInternal()
        {
            lock (_tcpLock)
            {
                if (!_isTCPRunning) return;
                _isTCPRunning = false;
            }

            _tcpCancellationTokenSource?.Cancel();
            _tcpCancellationTokenSource?.Dispose();
            _tcpCancellationTokenSource = null;

            _tcpListener?.Stop();
            _tcpListener = null;

            InvokeIfNeeded(() =>
            {
                startTCP.Enabled = true;
                stopTCP.Enabled = false;
                tbCom.LogInfo("Le serveur a été arrêté.", LogSource.Serveur);
            });
        }

        private void StopTCPServer()
        {
            lock (_tcpLock)
            {
                if (!_isTCPRunning) return;
                _isTCPRunning = false;
            }

            _tcpCancellationTokenSource?.Cancel();
            _tcpCancellationTokenSource?.Dispose();
            _tcpCancellationTokenSource = null;

            _tcpListener?.Stop();
            _tcpListener = null;

            InvokeIfNeeded(() =>
            {
                startTCP.Enabled = true;
                stopTCP.Enabled = false;
                tbCom.LogInfo("Le serveur a été arrêté.", LogSource.Serveur);
            });
        }

        private void HandleClient(Socket clientSocket, CancellationToken token)
        {
            try
            {
                using (var networkStream = new NetworkStream(clientSocket))
                {
                    string request = ReadClientRequest(networkStream);
                    tbCom.LogInfo($"Requête reçue : {request}", LogSource.Client);

                    if (request.StartsWith("GET_IMAGE", StringComparison.OrdinalIgnoreCase))
                    {
                        HandleGetImage(networkStream, token, clientSocket);
                    }
                    else if (request.StartsWith("ADD_OBJECT", StringComparison.OrdinalIgnoreCase))
                    {
                        HandleAddObject(request, networkStream);
                    }
                    else
                    {
                        SendInvalidRequestResponse(networkStream);
                    }
                }

                clientSocket.Close();
            }
            catch (Exception ex)
            {
                tbCom.LogError(ex.Message, LogSource.Client);
            }
        }

        private void HandleGetImage(NetworkStream networkStream, CancellationToken token, Socket clientSocket)
        {
            while (clientSocket.Connected && !token.IsCancellationRequested)
            {
                Bitmap bitmap = GetNextFrame();
                if (bitmap != null)
                {
                    try
                    {
                        byte[] imageBytes = ImageToByteArray(bitmap, ImageFormat.Jpeg);

                        uint imageSize = (uint)imageBytes.Length;
                        byte[] sizeBytes = GetBigEndianBytes(imageSize);
                        networkStream.Write(sizeBytes, 0, sizeBytes.Length);

                        networkStream.Write(imageBytes, 0, imageBytes.Length);
                    }
                    catch (Exception ex)
                    {
                        tbCom.LogError(ex.Message, LogSource.Client);
                    }
                }
                else
                {
                    uint imageSize = 0;
                    byte[] sizeBytes = GetBigEndianBytes(imageSize);
                    networkStream.Write(sizeBytes, 0, sizeBytes.Length);
                }
                Thread.Sleep(100);
            }
        }

        private void HandleAddObject(string request, NetworkStream networkStream)
        {
            try
            {
                // Format attendu : ADD_OBJECT, {"Id":"...", "Color":"...", "Shape":"...", "X":..., "Y":...}
                var parts = request.Split(new[] { ',' }, 2);
                if (parts.Length != 2)
                    throw new FormatException("Commande ADD_OBJECT mal formatée.");

                var objectData = parts[1].Trim();
                tbCom.LogInfo($"Données JSON reçues : {objectData}", LogSource.Serveur);

                var robotObject = RobotObject.FromString(objectData);

                // 1) On l'ajoute à la queue => logique
                objectBuffer.Enqueue(robotObject);

                // 2) On l'ajoute à la liste liée au DataGridView => affichage
                robotObjectsList.Add(robotObject);

                float x = robotObject.X / 1000f;
                float y = robotObject.Y / 1000f;

                tbCom.LogInfo($"Objet ajouté : ID={robotObject.Id}, Color={robotObject.Color}, Shape={robotObject.Shape}, X={x}, Y={y}",
                              LogSource.Serveur);

                string response = $"OBJET AJOUTÉ - Coordonnées reçues : X={x}, Y={y}\n";
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                networkStream.Write(responseBytes, 0, responseBytes.Length);
            }
            catch (Exception ex)
            {
                tbCom.LogError($"Erreur lors de l'ajout de l'objet : {ex.Message}", LogSource.Serveur);
                string errorResponse = $"ERREUR: {ex.Message}\n";
                byte[] errorBytes = Encoding.UTF8.GetBytes(errorResponse);
                networkStream.Write(errorBytes, 0, errorBytes.Length);
            }
        }

        private string ReadClientRequest(NetworkStream networkStream)
        {
            var requestBuilder = new StringBuilder();
            int readByte;
            while ((readByte = networkStream.ReadByte()) != -1)
            {
                char ch = (char)readByte;
                if (ch == '\n') break;
                requestBuilder.Append(ch);
            }
            return requestBuilder.ToString().Trim();
        }

        private void SendInvalidRequestResponse(NetworkStream networkStream)
        {
            string invalidRequest = "Requête invalide.";
            byte[] invalidBytes = Encoding.ASCII.GetBytes(invalidRequest);
            networkStream.Write(invalidBytes, 0, invalidBytes.Length);
            tbCom.LogInfo("Requête invalide reçue et réponse envoyée.", LogSource.Client);
        }

        private byte[] ImageToByteArray(Image image, ImageFormat format)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, format);
                return ms.ToArray();
            }
        }

        private byte[] GetBigEndianBytes(uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return bytes;
        }

        private void imageTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Fichiers images|*.jpg;*.jpeg;*.png;*.bmp|Tous les fichiers|*.*";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _customTestImage = new Bitmap(ofd.FileName);
                        pbImage.Image = _customTestImage;

                        tbCom.LogInfo($"Image de test chargée depuis {ofd.FileName}.", LogSource.Serveur);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erreur lors du chargement de l'image : " + ex.Message,
                                        "Erreur",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ethernetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hotspotToolStripMenuItem.Enabled = true;
            ethernetToolStripMenuItem.Enabled = false;

            _ipRobot = "169.254.200.200";
            tbCom.LogInfo("Adresse IP du robot sélectionnée : " + _ipRobot, LogSource.Serveur);
        }

        private void hotspotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ethernetToolStripMenuItem.Enabled = true;
            hotspotToolStripMenuItem.Enabled = false;

            _ipRobot = "10.10.10.10";
            tbCom.LogInfo("Adresse IP du robot sélectionnée : " + _ipRobot, LogSource.Serveur);
        }

        private void quitterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseCamera();
            StopTCPServer();
            Close();
        }

        private void InitializeUIState()
        {
            btnStartAcquisition.Enabled = false;
            btnStopAcquisition.Enabled = false;

            startTCP.Enabled = true;
            stopTCP.Enabled = false;
        }

        private void NetworkSelection()
        {
            using (var dialog = new NetworkInterfaceSelectionDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _localIPAddress = dialog.SelectedIPAddress;
                    afficherLAdresseIPToolStripMenuItem.Text = $"Adresse IP : {_localIPAddress}";

                    if (_isTCPRunning)
                    {
                        StopTCPServer();
                        Task.Run(() => StartServerAsync());
                    }
                }
                else
                {
                    _localIPAddress = IPAddress.Any;
                    afficherLAdresseIPToolStripMenuItem.Text = $"Adresse IP : {_localIPAddress}";
                    MessageBox.Show(
                        "Aucune interface réseau sélectionnée. Le serveur utilisera toutes les interfaces.",
                        "Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseCamera();
            StopTCPServer();
        }

        private void InvokeIfNeeded(Action action)
        {
            if (InvokeRequired)
                Invoke(action);
            else
                action();
        }

    }
}
