using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {
        private IPAddress m_ipAdrServeur;
        private IPAddress m_ipAdrClient;
        private int m_numPort;


        public Form1()
        {
            InitializeComponent();
            m_ipAdrServeur = IPAddress.Parse("192.168.1.100");  // Adresse locale
            m_ipAdrClient = IPAddress.Parse("192.168.1.200");   // Adresse distante
            m_numPort = 8001;

            initClientTCP();
        }

        private async void initClientTCP()
        {
            TcpClient tcpClient = null;
            try
            {
                tcpClient = new TcpClient();
                this.tbCom.AppendText("Connexion en cours...\r\n");

                await tcpClient.ConnectAsync(m_ipAdrClient, m_numPort);
                this.tbCom.AppendText("Connexion établie\r\n");

                NetworkStream networkStream = tcpClient.GetStream();

                // Envoyer la requête "SEND_IMAGE"
                string request = "SEND_IMAGE";
                byte[] requestBytes = Encoding.ASCII.GetBytes(request);
                await networkStream.WriteAsync(requestBytes, 0, requestBytes.Length);
                this.tbCom.AppendText("Requête d'envoi d'image envoyée : " + request + "\r\n");

                // Lire la taille de l'image (4 octets)
                byte[] sizeBytes = new byte[4];
                int totalRead = 0;
                while (totalRead < 4)
                {
                    int bytesRead = await networkStream.ReadAsync(sizeBytes, totalRead, 4 - totalRead);
                    if (bytesRead == 0)
                    {
                        throw new Exception("Connexion fermée avant de recevoir la taille de l'image.");
                    }
                    totalRead += bytesRead;
                }

                int imageSize = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(sizeBytes, 0));
                this.tbCom.AppendText("Taille de l'image à recevoir : " + imageSize + " octets.\r\n");

                // Lire les octets de l'image
                byte[] imageBytes = new byte[imageSize];
                totalRead = 0;
                while (totalRead < imageSize)
                {
                    int bytesRead = await networkStream.ReadAsync(imageBytes, totalRead, imageSize - totalRead);
                    if (bytesRead == 0)
                    {
                        throw new Exception("Connexion fermée avant de recevoir toute l'image.");
                    }
                    totalRead += bytesRead;
                }

                this.tbCom.AppendText("Image reçue en " + totalRead + " octets.\r\n");

                // Convertir les octets en image
                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    Image receivedImage = Image.FromStream(ms);
                    this.pbImage.Image = receivedImage;
                    this.tbCom.AppendText("Image affichée.\r\n");
                }
            }
            catch (Exception ex)
            {
                this.tbCom.AppendText("Erreur : " + ex.Message + "\r\n");
            }
            finally
            {
                if (tcpClient != null)
                {
                    tcpClient.Close();
                    this.tbCom.AppendText("Connexion fermée.\r\n");
                }
            }
        }


    }
}
