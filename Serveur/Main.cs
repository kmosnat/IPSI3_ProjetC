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
using System.IO.Ports;

using Log;

namespace Serveur
{
    public enum RobotState
    {
        Wait,
        OnProcess,
        RobotOnMoving
    }

    public partial class Main : Form
    {
        private smcs.IDevice _device;
        private Rectangle _imageRect;
        private PixelFormat _pixelFormat;
        private UInt32 _pixelType;

        private Queue<string> objectBuffer = new Queue<string>();
        private RobotState robotState = RobotState.Wait;

        private IPAddress _localIPAddress;
        private int _port;
        private bool _isTCPRunning = false;
        private bool _isAcquisitionRunning = false;
        private readonly object _deviceLock = new object();

        private CancellationTokenSource _tcpCancellationTokenSource;

        public Main()
        {
            InitializeComponent();
            _port = 8001;
            InitializeUIState();

            objectBuffer.Enqueue("Objet1");
            objectBuffer.Enqueue("Objet2");

            var timerRobotState = new System.Windows.Forms.Timer();
            timerRobotState.Interval = 500;
            timerRobotState.Tick += timerRobotState_Tick;
            timerRobotState.Start();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            NetworkSelection();

            string[] ports = SerialPort.GetPortNames();

            cbCom.Items.AddRange(ports);

            if (cbCom.Items.Count > 0)
            {
                cbCom.SelectedIndex = 0;
            }
            else
            {
                lblConnectionArduino.Text = "Aucun port série disponible.";
            }

            if (arduinoPort != null)
                arduinoPort.DataReceived += ArduinoPort_DataReceived;
        }

        private void ArduinoPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string line = arduinoPort.ReadLine().Trim();
                AppendLog(LogSource.Arduino, LogLevel.INFO, line);
                if (line == "DONE")
                {
                    this.Invoke(new Action(() =>
                    {
                        robotState = RobotState.Wait;
                        AppendLog(LogSource.Serveur, LogLevel.INFO, "Reçu DONE du robot, état => Wait.");

                        UpdateRobotStateMachine();
                    }));
                }
            }
            catch (Exception ex)
            {
                AppendLog(LogSource.Arduino, LogLevel.ERROR, ex.Message);
            }
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
                    if (objectBuffer.Count > 0 && arduinoPort != null && arduinoPort.IsOpen)
                    {
                        var obj = objectBuffer.Dequeue();
                        arduinoPort.WriteLine("RUN");
                        AppendLog(LogSource.Serveur, LogLevel.INFO, $"Objet en cours de traitement : {obj}");
                        robotState = RobotState.OnProcess;
                    }
                    break;

                case RobotState.OnProcess:
                    break;

                case RobotState.RobotOnMoving:
                    break;
            }
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
                    MessageBox.Show("Aucune interface réseau sélectionnée. Le serveur utilisera toutes les interfaces.",
                                    "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private async void InitCamera()
        {
            bool cameraConnected = false;

            smcs.CameraSuite.InitCameraAPI();
            var smcsVisionApi = smcs.CameraSuite.GetCameraAPI();

            if (!smcsVisionApi.IsUsingKernelDriver())
            {
                MessageBox.Show("Warning: Smartek Filter Driver not loaded.");
            }

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
                        await StartServerAsync();
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

        private void UpdateUIOnCameraConnected()
        {
            InvokeIfNeeded(() =>
            {
                lblConnectionCamera.BackColor = Color.LimeGreen;
                lblConnectionCamera.Text = "Connection établie";
                lblAdrIP.BackColor = Color.LimeGreen;
                lblAdrIP.Text = "Adresse IP : " + Common.IpAddrToString(_device.GetIpAddress());
                lblNomCamera.Text = _device.GetManufacturerName() + " : " + _device.GetModelName();
            });
        }

        private async Task StartServerAsync()
        {
            if (_isTCPRunning) return;

            if (_localIPAddress == null)
                _localIPAddress = IPAddress.Any;

            _isTCPRunning = true;
            _tcpCancellationTokenSource = new CancellationTokenSource();
            var token = _tcpCancellationTokenSource.Token;

            try
            {
                var tcpListener = new TcpListener(_localIPAddress, _port);
                tcpListener.Start();
                AppendLog(LogSource.Serveur, LogLevel.INFO, "Le serveur est en cours d'exécution...");
                AppendLog(LogSource.Serveur, LogLevel.INFO, $"Point de terminaison local : {tcpListener.LocalEndpoint}");
                AppendLog(LogSource.Serveur, LogLevel.INFO, $"Adresse IP : {_localIPAddress}");
                AppendLog(LogSource.Serveur, LogLevel.INFO, $"Port : {_port}");
                AppendLog(LogSource.Serveur, LogLevel.INFO, $"En attente de connexions...");

                startTCP.Enabled = false;
                stopTCP.Enabled = true;

                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        var acceptTask = tcpListener.AcceptSocketAsync();
                        var completedTask = await Task.WhenAny(acceptTask, Task.Delay(100, token));

                        if (completedTask == acceptTask && acceptTask.Result != null)
                        {
                            var clientSocket = acceptTask.Result;
                            AppendLog(LogSource.Serveur, LogLevel.INFO, $"Connexion acceptée de {clientSocket.RemoteEndPoint}");

                            _ = Task.Run(() => HandleClient(clientSocket, token), token);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        AppendLog(LogSource.Serveur, LogLevel.ERROR, $"Erreur lors de l'acceptation d'un client : {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                AppendLog(LogSource.Serveur, LogLevel.ERROR, ex.Message);
            }
            finally
            {
                StopServer();
            }
        }

        private void StopServer()
        {
            _isTCPRunning = false;
            InvokeIfNeeded(() =>
            {
                startTCP.Enabled = true;
                stopTCP.Enabled = false;
                AppendLog(LogSource.Serveur, LogLevel.INFO, "Le serveur a été arrêté.");
            });
        }

        private void HandleClient(Socket clientSocket, CancellationToken token)
        {
            try
            {
                using (var networkStream = new NetworkStream(clientSocket))
                {
                    string request = ReadClientRequest(networkStream);
                    AppendLog(LogSource.Client, LogLevel.INFO, $"Requête reçue : {request}");

                    if (request.Equals("GET_IMAGE", StringComparison.OrdinalIgnoreCase))
                    {
                        SendImagesContinuously(clientSocket, networkStream, token);
                    }
                    else
                    {
                        SendInvalidRequestResponse(networkStream);
                    }
                }

                clientSocket.Close();
                AppendLog(LogSource.Client, LogLevel.INFO, "Connexion fermée avec le client.");
            }
            catch (Exception ex)
            {
                AppendLog(LogSource.Client, LogLevel.ERROR, ex.Message);
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

        private void SendImagesContinuously(Socket clientSocket, NetworkStream networkStream, CancellationToken token)
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
                        AppendLog(LogSource.Client, LogLevel.INFO, $"Taille de l'image envoyée : {imageSize} octets.");
                    }
                    catch (Exception ex)
                    {
                        AppendLog(LogSource.Client, LogLevel.ERROR, ex.Message);
                    }
                }
                else
                {
                    uint imageSize = 0;
                    byte[] sizeBytes = GetBigEndianBytes(imageSize);
                    networkStream.Write(sizeBytes, 0, sizeBytes.Length);
                    AppendLog(LogSource.Client, LogLevel.ERROR, "Erreur lors de la capture de l'image");
                }

                Thread.Sleep(100);
            }
        }

        private void SendInvalidRequestResponse(NetworkStream networkStream)
        {
            string invalidRequest = "Requête invalide.";
            byte[] invalidBytes = Encoding.ASCII.GetBytes(invalidRequest);
            networkStream.Write(invalidBytes, 0, invalidBytes.Length);
            AppendLog(LogSource.Client, LogLevel.INFO, "Requête invalide reçue et réponse envoyée.");
        }

        private byte[] GetBigEndianBytes(uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return bytes;
        }

        private byte[] ImageToByteArray(Image image, ImageFormat format)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, format);
                return ms.ToArray();
            }
        }

        private Bitmap GenerateTestImage()
        {
            try
            {
                var bitmap = new Bitmap(640, 480, PixelFormat.Format24bppRgb);
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
                    return GenerateTestImage();
                }
            }

            return null;
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

        private async void btnSearchCamera_Click(object sender, EventArgs e)
        {
            await Task.Run(() => InitCamera());
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

        private void StopTCPServer()
        {
            if (_isTCPRunning && _tcpCancellationTokenSource != null)
            {
                _tcpCancellationTokenSource.Cancel();
                _isTCPRunning = false;
                startTCP.Enabled = true;
                stopTCP.Enabled = false;
            }
        }

        private void sélectionnerUneCarteRéseauToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NetworkSelection();
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseCamera();
            StopTCPServer();
        }

        private void quitterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseCamera();
            StopTCPServer();
            this.Close();
        }

        private void AppendLog(LogSource source, LogLevel level, string message)
        {
            Log.Log logEntry = new Log.Log(source, level, message);

            string content = logEntry.ToString().Replace("\n", Environment.NewLine);

            string finalMessage = "--------------------------" + Environment.NewLine + content + Environment.NewLine ;

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


        private void InvokeIfNeeded(Action action)
        {
            if (InvokeRequired)
                Invoke(action);
            else
                action();
        }

        private void btnArduinoConnect_Click(object sender, EventArgs e)
        {
            if (cbCom.SelectedItem == null)
            {
                MessageBox.Show("Veuillez sélectionner un port série avant de vous connecter.");
                return;
            }

            string selectedPort = cbCom.SelectedItem.ToString();

            try
            {
                if (arduinoPort != null && arduinoPort.IsOpen)
                {
                    arduinoPort.Close();
                }

                arduinoPort = new SerialPort(selectedPort, 9600);
                arduinoPort.NewLine = "\r\n";
                arduinoPort.DataBits = 8;
                arduinoPort.Parity = Parity.None;
                arduinoPort.StopBits = StopBits.One;
                arduinoPort.Handshake = Handshake.None;

                arduinoPort.DataReceived += ArduinoPort_DataReceived;

                arduinoPort.Open();

                if (arduinoPort.IsOpen)
                {
                    lblConnectionArduino.Text = $"Connecté sur {selectedPort}";
                    lblConnectionArduino.BackColor = Color.LimeGreen;
                }
            }
            catch (Exception ex)
            {
                lblConnectionArduino.Text = "Erreur de connexion";
                MessageBox.Show("Impossible de se connecter au port sélectionné.\n" + ex.Message,
                                "Erreur",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void btnArduinoDeconnect_Click(object sender, EventArgs e)
        {
            if (arduinoPort != null && arduinoPort.IsOpen)
            {
                arduinoPort.Close();
                lblConnectionArduino.Text = "Déconnecté";
                lblConnectionArduino.BackColor = Color.Red;
            }
        }
    }
}
