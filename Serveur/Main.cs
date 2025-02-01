using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace Serveur
{
    public partial class Main : Form
    {
        // Propriétés caméra (inchangées)
        private smcs.IDevice _device;
        private Rectangle _imageRect;
        private PixelFormat _pixelFormat;
        private UInt32 _pixelType;
        private Bitmap _customTestImage = null;

        // Gestion d'objets
        private ConcurrentQueue<RobotObject> objectBuffer = new ConcurrentQueue<RobotObject>();
        private RobotState robotState = RobotState.Wait;
        private RobotObject currentRobotObject = null;
        private Task moveTask;

        // Adresses & robot
        private string _ipRobot;
        private RobotModbusHelper robot;
        private bool _robotConnected = false;
        private CalibrationStep currentCalibrationStep = CalibrationStep.Point1;
        private bool isBlinking = false;

        // TCP
        private IPAddress _localIPAddress;
        private int _port;
        private bool _isTCPRunning = false;
        private CancellationTokenSource _tcpCancellationTokenSource;
        private TcpListener _tcpListener;
        private readonly object _tcpLock = new object();

        // Caméra / acquisition
        private bool _isAcquisitionRunning = false;
        private readonly object _deviceLock = new object();
        private CancellationTokenSource _cameraSearchCts = null;

        // Nouveaux contrôles UI et données
        private BindingList<RobotObject> robotObjectsList = new BindingList<RobotObject>();
        private ServerState serverState = ServerState.Wait;
        private List<(float xCam, float yCam, float xRob, float yRob)> calibrationPoints = new List<(float, float, float, float)>();
        private (float xCam, float yCam, float xRob, float yRob)? _calibPoint1 = null;
        private (float xCam, float yCam, float xRob, float yRob)? _calibPoint2 = null;
        private float _scaleX = 1f, _offsetX = 0f;
        private float _scaleY = 1f, _offsetY = 0f;
        private bool _calibrationDone = false;
        private float _refZ = 0.1f;
        private float _refRoll = 0f;
        private float _refPitch = 0f;
        private float _refYaw = 0f;
        private Bitmap _lastFrame = null;
        private readonly object _lastFrameLock = new object();
        private IPAddress _currentClientIPAddress = null;

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

            calibrationButton.Enabled = false;
            dgvObjects.AutoGenerateColumns = true;
            dgvObjects.DataSource = robotObjectsList;
        }

        private void Main_Load(object sender, EventArgs e)
        {
            ConnectRobot();
            NetworkSelection();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseCamera();
            StopTCPServer();
            DisconnectRobot();
        }

        private void OnClosed(object sender, FormClosedEventArgs e)
        {
            CloseCamera();
            StopTCPServer();
            DisconnectRobot();
        }

        #region Robot & Calibration

        private void StartCalibration()
        {
            currentCalibrationStep = CalibrationStep.Point1;
            calibrationPoints.Clear();
            _calibrationDone = false;
            serverState = ServerState.Calibration;
            tbCom.LogInfo("Calibration initiée. Cliquez sur 'Confirmer Point 1' pour commencer.", LogSource.Serveur);
            CalibRef1Butt.Enabled = true;
            CalibRef2Butt.Enabled = false;
        }

        private void ConnectRobot()
        {
            try
            {
                if (!_robotConnected && robot != null)
                {
                    robot.Connect();
                    _robotConnected = robot.IsConnected();
                    if (_robotConnected)
                        tbCom.LogInfo("Robot connecté avec succès.", LogSource.Serveur);
                    else
                        tbCom.LogError("Échec de connexion au robot.", LogSource.Serveur);
                }
            }
            catch (Exception ex)
            {
                tbCom.LogError($"Exception lors de la connexion au robot : {ex.Message}", LogSource.Serveur);
            }
        }

        private void DisconnectRobot()
        {
            try
            {
                if (_robotConnected && robot != null)
                {
                    robot.Disconnect();
                    tbCom.LogInfo("Robot déconnecté.", LogSource.Serveur);
                }
            }
            catch (Exception ex)
            {
                tbCom.LogError($"Exception lors de la déconnexion du robot : {ex.Message}", LogSource.Serveur);
            }
            finally
            {
                _robotConnected = false;
            }
        }

        private void calibrationButton_Click(object sender, EventArgs e)
        {
            if (!_robotConnected)
            {
                tbCom.LogError("Impossible de calibrer : robot non connecté.", LogSource.Serveur);
                return;
            }
            try
            {
                robot.calibrate();
                calibrationButton.Enabled = false;
            }
            catch (Exception ex)
            {
                tbCom.LogError("Erreur Modbus: " + ex.Message, LogSource.Serveur);
            }
        }

        private void CalibRef1Butt_Click(object sender, EventArgs e)
        {
            if (currentCalibrationStep != CalibrationStep.Point1)
            {
                tbCom.LogError("Aucune calibration point 1 en cours.", LogSource.Serveur);
                return;
            }
            serverState = ServerState.Calibration;
            try
            {
                var result = MessageBox.Show("Placez le robot au-dessus du premier objet détecté puis cliquez sur 'OK'.",
                                             "Calibration Point 1", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (result != DialogResult.OK)
                {
                    tbCom.LogInfo("Calibration Point 1 annulée par l'utilisateur.", LogSource.Serveur);
                    serverState = ServerState.Wait;
                    StartCalibration();
                    return;
                }
                robot.MoveToPose(-0.012f, -0.172f, 0.206f, 2.90f, 1.49f, 1.41f);
                tbCom.LogInfo("Robot déplacé à la position de calibration 1.", LogSource.Serveur);
                var (xCam, yCam) = RequestCoordinatesFromClient();
                RobotPose rp = robot.GetCurrentPose();
                _refZ = rp.Z; _refRoll = rp.Roll; _refPitch = rp.Pitch; _refYaw = rp.Yaw;
                _calibPoint1 = (xCam, yCam, rp.X, rp.Y);
                calibrationPoints.Add(_calibPoint1.Value);
                tbCom.LogInfo($"Point 1 enregistré : Caméra ({xCam}, {yCam}), Robot ({rp.X}, {rp.Y})", LogSource.Serveur);
                serverState = ServerState.Wait;
                currentCalibrationStep = CalibrationStep.Point2;
                tbCom.LogInfo("Calibration Point 2 : Cliquez sur 'Confirmer Point 2' après placement.", LogSource.Serveur);
                CalibRef2Butt.Enabled = true;
            }
            catch (Exception ex)
            {
                tbCom.LogError($"Erreur lors de la confirmation du point 1 : {ex.Message}", LogSource.Serveur);
            }
        }

        private void CalibRef2Butt_Click(object sender, EventArgs e)
        {
            if (currentCalibrationStep != CalibrationStep.Point2)
            {
                tbCom.LogError("Aucune calibration point 2 en cours.", LogSource.Serveur);
                return;
            }
            serverState = ServerState.Calibration;
            try
            {
                var result = MessageBox.Show("Placez le robot au-dessus du deuxième objet détecté puis cliquez sur 'OK'.",
                                             "Calibration Point 2", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (result != DialogResult.OK)
                {
                    tbCom.LogInfo("Calibration Point 2 annulée par l'utilisateur.", LogSource.Serveur);
                    serverState = ServerState.Wait;
                    StartCalibration();
                    return;
                }
                robot.MoveToPose(-0.012f, -0.172f, 0.206f, 2.90f, 1.49f, 1.41f);
                tbCom.LogInfo("Robot déplacé à la position de calibration 2.", LogSource.Serveur);
                var (xCam, yCam) = RequestCoordinatesFromClient();
                RobotPose rp = robot.GetCurrentPose();
                _calibPoint2 = (xCam, yCam, rp.X, rp.Y);
                calibrationPoints.Add(_calibPoint2.Value);
                tbCom.LogInfo($"Point 2 enregistré : Caméra ({xCam}, {yCam}), Robot ({rp.X}, {rp.Y})", LogSource.Serveur);
                var p1 = _calibPoint1.Value;
                var p2 = _calibPoint2.Value;
                if (Math.Abs(p2.xCam - p1.xCam) < 1e-6 || Math.Abs(p2.yCam - p1.yCam) < 1e-6)
                    throw new Exception("Points de calibration invalides ou trop proches.");
                _scaleX = (p2.xRob - p1.xRob) / (p2.xCam - p1.xCam);
                _offsetX = p1.xRob - _scaleX * p1.xCam;
                _scaleY = (p2.yRob - p1.yRob) / (p2.yCam - p1.yCam);
                _offsetY = p1.yRob - _scaleY * p1.yCam;
                _calibrationDone = true;
                serverState = ServerState.Ready;
                calibrationPoints.Clear();
                tbCom.LogInfo("Calibration effectuée avec succès.", LogSource.Serveur);
                currentCalibrationStep = CalibrationStep.None;
                CalibRef1Butt.Enabled = false;
                CalibRef2Butt.Enabled = false;
                tbCom.LogInfo("Le serveur est prêt à traiter les objets.", LogSource.Serveur);
            }
            catch (Exception ex)
            {
                tbCom.LogError($"Erreur lors de la confirmation du point 2 : {ex.Message}", LogSource.Serveur);
            }
        }

        #endregion

        #region Mise à jour de l'UI & État du Robot

        private void timerRobotState_Tick(object sender, EventArgs e)
        {
            UpdateRobotStateMachine();
            if (!_robotConnected)
            {
                calibrationStatus.Text = "Déconnecté";
                calibrationStatus.BackColor = Color.Red;
                if (isBlinking)
                {
                    calibrationStatus.Visible = true;
                    isBlinking = false;
                }
                lblX.Text = "X : ---";
                lblY.Text = "Y : ---";
                lblZ.Text = "Z : ---";
                lblRoll.Text = "Roll : ---";
                lblPitch.Text = "Pitch : ---";
                lblYaw.Text = "Yaw : ---";
                lblJoint1Position.Text = "Joint 1 Position : ---";
                lblJoint2Position.Text = "Joint 2 Position : ---";
                lblJoint3Position.Text = "Joint 3 Position : ---";
                lblJoint4Position.Text = "Joint 4 Position : ---";
                lblJoint5Position.Text = "Joint 5 Position : ---";
                lblJoint6Position.Text = "Joint 6 Position : ---";
                return;
            }
            try
            {
                if (!robot.IsConnected())
                {
                    _robotConnected = false;
                    return;
                }
                if (robot.calibrationNeeded())
                {
                    calibrationStatus.Text = "Calibration Nécessaire";
                    calibrationStatus.BackColor = Color.Orange;
                    calibrationButton.Enabled = true;
                    calibrationStatus.Visible = isBlinking ? !calibrationStatus.Visible : true;
                    isBlinking = true;
                }
                else
                {
                    calibrationStatus.Text = "Calibré";
                    calibrationStatus.BackColor = Color.Green;
                    calibrationStatus.Visible = true;
                    isBlinking = false;
                }
                RobotPose pos = robot.GetCurrentPose();
                lblX.Text = $"X : {pos.X:F3} mm";
                lblY.Text = $"Y : {pos.Y:F3} mm";
                lblZ.Text = $"Z : {pos.Z:F3} mm";
                lblRoll.Text = $"Roll : {pos.Roll:F2}°";
                lblPitch.Text = $"Pitch : {pos.Pitch:F2}°";
                lblYaw.Text = $"Yaw : {pos.Yaw:F2}°";
                float[] joints = robot.GetCurrentJointStates();
                if (joints != null && joints.Length >= 6)
                {
                    lblJoint1Position.Text = $"Joint 1 Position : {joints[0]:F2}°";
                    lblJoint2Position.Text = $"Joint 2 Position : {joints[1]:F2}°";
                    lblJoint3Position.Text = $"Joint 3 Position : {joints[2]:F2}°";
                    lblJoint4Position.Text = $"Joint 4 Position : {joints[3]:F2}°";
                    lblJoint5Position.Text = $"Joint 5 Position : {joints[4]:F2}°";
                    lblJoint6Position.Text = $"Joint 6 Position : {joints[5]:F2}°";
                }
                else
                {
                    lblJoint1Position.Text = "Joint 1 Position : N/A";
                    lblJoint2Position.Text = "Joint 2 Position : N/A";
                    lblJoint3Position.Text = "Joint 3 Position : N/A";
                    lblJoint4Position.Text = "Joint 4 Position : N/A";
                    lblJoint5Position.Text = "Joint 5 Position : N/A";
                    lblJoint6Position.Text = "Joint 6 Position : N/A";
                }
            }
            catch (Exception ex)
            {
                tbCom.LogError($"Error updating robot pose: {ex.Message}");
                lblX.Text = "X : Error";
                lblY.Text = "Y : Error";
                lblZ.Text = "Z : Error";
                lblRoll.Text = "Roll : Error";
                lblPitch.Text = "Pitch : Error";
                lblYaw.Text = "Yaw : Error";
                lblJoint1Position.Text = "Joint 1 Position : Error";
                lblJoint2Position.Text = "Joint 2 Position : Error";
                lblJoint3Position.Text = "Joint 3 Position : Error";
                lblJoint4Position.Text = "Joint 4 Position : Error";
                lblJoint5Position.Text = "Joint 5 Position : Error";
                lblJoint6Position.Text = "Joint 6 Position : Error";
            }
        }

        private void UpdateRobotStateMachine()
        {
            if (serverState == ServerState.Calibration || serverState == ServerState.Wait)
            {
                InvokeIfNeeded(() =>
                {
                    lblRobotState.Text = serverState == ServerState.Calibration ? "État Robot: Calibration en cours" : "État Robot: En attente de calibration";
                    lblRobotState.BackColor = serverState == ServerState.Calibration ? Color.Orange : Color.Gray;
                });
                return;
            }
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
                    moveTask = Task.Run(() =>
                    {
                        float xRobot = currentRobotObject.X;
                        float yRobot = currentRobotObject.Y;
                        robot.openGripper();
                        robot.MoveToPose(xRobot, yRobot, _refZ, _refRoll, _refPitch, _refYaw);
                        robot.MoveToPose(xRobot, yRobot, _refZ + 0.1f, _refRoll, _refPitch, _refYaw);
                        robot.closeGripper();
                        robot.MoveToPose(xRobot, yRobot, _refZ, _refRoll, _refPitch, _refYaw);
                        robot.MoveToPose(-0.012f, -0.172f, 0.206f, 2.90f, 1.49f, 1.41f);
                        robot.openGripper();
                    });
                    robotState = RobotState.RobotOnMoving;
                    break;
                case RobotState.RobotOnMoving:
                    if (_robotConnected && !robot.isMoving())
                    {
                        tbCom.LogInfo("Mouvement du robot terminé.", LogSource.Serveur);
                        var roToRemove = robotObjectsList.FirstOrDefault(ro => ro.Id == currentRobotObject.Id);
                        if (roToRemove != null)
                        {
                            robotObjectsList.Remove(roToRemove);
                        }
                        robotState = RobotState.Wait;
                    }
                    break;
            }
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

        #endregion

        #region Acquisition & Caméra

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
                    Bitmap testImage = GenerateTestImage();
                    SetPictureBoxImage(testImage);
                    return testImage;
                }
            }
            return null;
        }

        private Bitmap GenerateTestImage()
        {
            if (_customTestImage != null)
                return new Bitmap(_customTestImage);
            try
            {
                int width = 640, height = 480, circleRadius = 50, circleCount = 3;
                Random random = new Random();
                var bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.Clear(Color.Gray);
                    for (int i = 0; i < circleCount; i++)
                    {
                        int x = random.Next(circleRadius, width - circleRadius);
                        int y = random.Next(circleRadius, height - circleRadius);
                        g.FillEllipse(Brushes.White, x - circleRadius, y - circleRadius, circleRadius * 2, circleRadius * 2);
                    }
                }
                return bitmap;
            }
            catch
            {
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
                    lock (_lastFrameLock)
                    {
                        _lastFrame?.Dispose();
                        _lastFrame = (Bitmap)bitmap.Clone();
                    }
                }
                else
                {
                    Bitmap testImage = GenerateTestImage();
                    SetPictureBoxImage(testImage);
                }
            }
            catch (Exception ex)
            {
                tbCom.LogError("Erreur timAcq : " + ex.Message, LogSource.Serveur);
            }
        }

        private void SetPictureBoxImage(Bitmap bitmap)
        {
            if (pbImage.InvokeRequired)
                pbImage.Invoke(new MethodInvoker(() =>
                {
                    pbImage.Image?.Dispose();
                    pbImage.Image = (Bitmap)bitmap.Clone();
                }));
            else
            {
                pbImage.Image?.Dispose();
                pbImage.Image = (Bitmap)bitmap.Clone();
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
            if (_cameraSearchCts != null)
            {
                tbCom.LogInfo("Une recherche caméra est déjà en cours...", LogSource.Serveur);
                return;
            }
            _cameraSearchCts = new CancellationTokenSource();
            var token = _cameraSearchCts.Token;
            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    bool connected = TryConnectCamera();
                    if (connected)
                        break;
                    try { await Task.Delay(2000, token); } catch (TaskCanceledException) { break; }
                }
            }, token).ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    foreach (var ex in t.Exception.Flatten().InnerExceptions)
                        tbCom.LogError("Recherche caméra: " + ex.Message, LogSource.Serveur);
                }
                _cameraSearchCts = null;
            });
        }

        private bool TryConnectCamera()
        {
            bool cameraConnected = false;
            try
            {
                smcs.CameraSuite.InitCameraAPI();
                var smcsVisionApi = smcs.CameraSuite.GetCameraAPI();
                smcsVisionApi.FindAllDevices(3.0);
                var devices = smcsVisionApi.GetAllDevices();
                if (devices.Length > 0)
                {
                    _device = devices[0];
                    if (_device != null && _device.Connect())
                    {
                        cameraConnected = true;
                        InvokeIfNeeded(() =>
                        {
                            lblConnectionCamera.BackColor = Color.LimeGreen;
                            lblConnectionCamera.Text = "Connexion établie";
                            lblAdrIP.BackColor = Color.LimeGreen;
                            lblAdrIP.Text = "Adresse IP : " + Common.IpAddrToString(_device.GetIpAddress());
                            lblNomCamera.Text = _device.GetManufacturerName() + " : " + _device.GetModelName();
                            btnStartAcquisition.Enabled = true;
                            btnStartAcquisition.BackColor = Color.LightGreen;
                        });
                        _device.SetStringNodeValue("TriggerMode", "Off");
                        _device.SetStringNodeValue("AcquisitionMode", "Continuous");
                        _device.SetIntegerNodeValue("TLParamsLocked", 1);
                        _device.CommandNodeExecute("AcquisitionStart");
                        if (!_isTCPRunning)
                        {
                            Task.Run(() => StartServerAsync());
                        }
                    }
                }
                if (!cameraConnected)
                {
                    InvokeIfNeeded(() =>
                    {
                        lblAdrIP.BackColor = Color.Red;
                        lblAdrIP.Text = "Erreur de connexion!";
                    });
                }
            }
            catch (Exception ex)
            {
                InvokeIfNeeded(() =>
                {
                    tbCom.LogError("Erreur TryConnectCamera() : " + ex.Message, LogSource.Serveur);
                    lblAdrIP.BackColor = Color.Red;
                    lblAdrIP.Text = "Erreur de connexion!";
                });
            }
            return cameraConnected;
        }

        private void btnStartAcquisition_Click(object sender, EventArgs e)
        {
            _isAcquisitionRunning = true;
            timAcq.Start();
            btnStartAcquisition.Enabled = false;
            btnStopAcquisition.Enabled = true;
            btnStartAcquisition.BackColor = Color.LightGreen;
            btnStopAcquisition.BackColor = SystemColors.Control;
            tbCom.LogInfo("Acquisition démarrée.", LogSource.Serveur);
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
                tbCom.LogInfo("Acquisition arrêtée.", LogSource.Serveur);
            }
        }

        #endregion

        #region Serveur TCP

        private async Task StartServerAsync()
        {
            lock (_tcpLock)
            {
                if (_isTCPRunning)
                    return;
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
                _tcpListener?.Stop();
                _tcpListener = null;
                StopServerInternal();
            }
        }

        private void StopServerInternal()
        {
            lock (_tcpLock)
            {
                if (!_isTCPRunning)
                    return;
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
            StopServerInternal();
        }

        private void startTCP_Click(object sender, EventArgs e)
        {
            if (!_isTCPRunning)
            {
                Task.Run(() => StartServerAsync());
            }
        }

        private void stopTCP_Click(object sender, EventArgs e)
        {
            StopTCPServer();
        }

        private void HandleClient(Socket clientSocket, CancellationToken token)
        {
            try
            {
                using (var networkStream = new NetworkStream(clientSocket))
                {
                    _currentClientIPAddress = ((IPEndPoint)clientSocket.RemoteEndPoint).Address;
                    string request = ReadClientRequest(networkStream);
                    tbCom.LogInfo($"Requête reçue : {request}", LogSource.Client);
                    if (request.StartsWith("GET_IMAGE", StringComparison.OrdinalIgnoreCase))
                    {
                        HandleGetImage(networkStream, token, clientSocket);
                    }
                    else if (request.StartsWith("ADD_OBJECT", StringComparison.OrdinalIgnoreCase))
                    {
                        HandleAddObject(request, networkStream, clientSocket);
                    }
                    else
                    {
                        SendInvalidRequestResponse(networkStream);
                    }
                }
            }
            catch (Exception ex)
            {
                tbCom.LogError($"Erreur lors de la gestion du client {clientSocket.RemoteEndPoint} : {ex.Message}", LogSource.Client);
            }
            finally
            {
                try { clientSocket.Close(); } catch { }
            }
        }

        private void HandleGetImage(NetworkStream networkStream, CancellationToken token, Socket clientSocket)
        {
            try
            {
                while (clientSocket.Connected && !token.IsCancellationRequested)
                {
                    if (serverState == ServerState.Calibration)
                    {
                        tbCom.LogInfo("Mode Calibration actif : arrêt de l'envoi d'images.");
                        break;
                    }
                    Bitmap bitmap = GetNextFrame();
                    if (bitmap != null)
                    {
                        byte[] imageBytes = ImageToByteArray(bitmap, ImageFormat.Jpeg);
                        uint imageSize = (uint)imageBytes.Length;
                        byte[] sizeBytes = GetBigEndianBytes(imageSize);
                        networkStream.Write(sizeBytes, 0, sizeBytes.Length);
                        networkStream.Write(imageBytes, 0, imageBytes.Length);
                    }
                    else
                    {
                        byte[] sizeBytes = GetBigEndianBytes(0);
                        networkStream.Write(sizeBytes, 0, sizeBytes.Length);
                    }
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                tbCom.LogError($"Erreur lors de l'envoi d'image au client {clientSocket.RemoteEndPoint} : {ex.Message}", LogSource.Client);
            }
        }

        private void HandleAddObject(string request, NetworkStream networkStream, Socket clientSocket)
        {
            try
            {
                var parts = request.Split(new[] { ',' }, 2);
                if (parts.Length != 2)
                    throw new FormatException("Commande ADD_OBJECT mal formatée.");
                var objectData = parts[1].Trim();
                tbCom.LogInfo($"Données JSON reçues : {objectData}", LogSource.Serveur);
                var robotObject = RobotObject.FromString(objectData);
                if (serverState == ServerState.Calibration || serverState == ServerState.Wait)
                {
                    tbCom.LogInfo("En mode calibration ou en attente, l'objet est ignoré.", LogSource.Serveur);
                    string response = "CALIBRATION_IN_PROGRESS\n";
                    byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                    networkStream.Write(responseBytes, 0, responseBytes.Length);
                }
                else if (serverState == ServerState.Ready)
                {
                    float xRobot = (robotObject.X * _scaleX) + _offsetX;
                    float yRobot = (robotObject.Y * _scaleY) + _offsetY;
                    var calibratedObject = new RobotObject(robotObject.Color, robotObject.Shape, xRobot, yRobot);
                    objectBuffer.Enqueue(calibratedObject);
                    InvokeIfNeeded(() => robotObjectsList.Add(calibratedObject));
                    tbCom.LogInfo($"Objet calibré ajouté : {calibratedObject}", LogSource.Serveur);
                    Task.Run(() =>
                    {
                        try
                        {
                            var calibrationCoords = RequestCoordinatesFromClient();
                        }
                        catch (Exception ex)
                        {
                            tbCom.LogError($"Erreur lors de la demande des coordonnées au client : {ex.Message}", LogSource.Serveur);
                        }
                    });
                    string response = $"OBJET AJOUTÉ - Coordonnées calibrées : X={xRobot}, Y={yRobot}\n";
                    byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                    networkStream.Write(responseBytes, 0, responseBytes.Length);
                }
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
                if (ch == '\n')
                    break;
                requestBuilder.Append(ch);
            }
            return requestBuilder.ToString().Trim();
        }

        private void SendInvalidRequestResponse(NetworkStream networkStream)
        {
            string invalidRequest = "Requête invalide.\n";
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
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return bytes;
        }

        #endregion

        #region Réseau & UI

        private void ethernetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hotspotToolStripMenuItem.Enabled = true;
            ethernetToolStripMenuItem.Enabled = false;
            DisconnectRobot();
            _ipRobot = "169.254.200.200";
            robot = new RobotModbusHelper(_ipRobot, 5020);
            tbCom.LogInfo("Adresse IP du robot sélectionnée : " + _ipRobot, LogSource.Serveur);
            ConnectRobot();
        }

        private void hotspotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ethernetToolStripMenuItem.Enabled = true;
            hotspotToolStripMenuItem.Enabled = false;
            DisconnectRobot();
            _ipRobot = "10.10.10.10";
            robot = new RobotModbusHelper(_ipRobot, 5020);
            tbCom.LogInfo("Adresse IP du robot sélectionnée : " + _ipRobot, LogSource.Serveur);
            ConnectRobot();
        }

        private void quitterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseCamera();
            StopTCPServer();
            DisconnectRobot();
            Close();
        }

        private void NetworkInterfaceSelection_Click(object sender, EventArgs e)
        {
            NetworkSelection();
        }

        private void InitializeUIState()
        {
            btnStartAcquisition.Enabled = false;
            btnStopAcquisition.Enabled = false;
            startTCP.Enabled = true;
            stopTCP.Enabled = false;
            CalibRef1Butt.Enabled = true;
            CalibRef2Butt.Enabled = false;
            calibrationButton.Enabled = false;
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
                    MessageBox.Show("Aucune interface réseau sélectionnée. Le serveur utilisera toutes les interfaces.",
                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void InvokeIfNeeded(Action action)
        {
            if (InvokeRequired)
                Invoke(action);
            else
                action();
        }

        private (float xCam, float yCam) RequestCoordinatesFromClient()
        {
            if (_currentClientIPAddress == null)
                throw new InvalidOperationException("Adresse IP du client non définie.");
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    client.Connect(_currentClientIPAddress, 9000);
                    NetworkStream networkStream = client.GetStream();
                    string request = "GET_CURRENT_COORDINATES\n";
                    byte[] requestBytes = Encoding.ASCII.GetBytes(request);
                    networkStream.Write(requestBytes, 0, requestBytes.Length);
                    networkStream.Flush();
                    tbCom.LogInfo("Requête GET_CURRENT_COORDINATES envoyée au client.", LogSource.Serveur);
                    string response = ReadClientRequest(networkStream);
                    tbCom.LogInfo($"Réponse reçue du client : {response}", LogSource.Serveur);
                    var parts = response.Split(',');
                    if (parts.Length != 2)
                        throw new FormatException("Réponse de coordonnées mal formatée.");
                    if (!float.TryParse(parts[0].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float xCam))
                        throw new FormatException("Coordonnée X de la caméra invalide.");
                    if (!float.TryParse(parts[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float yCam))
                        throw new FormatException("Coordonnée Y de la caméra invalide.");
                    return (xCam, yCam);
                }
            }
            catch (Exception ex)
            {
                tbCom.LogError($"Erreur lors de la demande des coordonnées au client : {ex.Message}", LogSource.Serveur);
                throw;
            }
        }

        #endregion

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
                            "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void moveRobotTest_Click(object sender, EventArgs e)
        {
            if (!_robotConnected)
            {
                tbCom.LogError("Le robot n'est pas connecté, impossible de bouger.", LogSource.Serveur);
                return;
            }
            try
            {
                RobotPose currentPose = robot.GetCurrentPose();
                tbCom.LogInfo($"Pose courante => {currentPose}", LogSource.Serveur);
                robot.MoveToPose(-0.012f, -0.172f, 0.206f, 2.90f, 1.49f, 1.41f);
            }
            catch (Exception ex)
            {
                tbCom.LogError("Erreur Modbus: " + ex.Message, LogSource.Serveur);
            }
        }
    }
}
