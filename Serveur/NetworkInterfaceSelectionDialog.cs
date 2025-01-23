using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System.Windows.Forms;

namespace Serveur
{
    public partial class NetworkInterfaceSelectionDialog : Form
    {
        public IPAddress SelectedIPAddress { get; private set; }
        public string SelectedInterfaceName { get; private set; }

        private Label lblInstruction;
        private Button btnOk;

        public NetworkInterfaceSelectionDialog()
        {
            InitializeComponent();
            PopulateNetworkInterfaces();
        }

        private void PopulateNetworkInterfaces()
        {
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces().ToList();

            foreach (var ni in networkInterfaces)
            {
                var ipProps = ni.GetIPProperties();
                var ipInfos = ipProps.UnicastAddresses
                    .Where(ip => ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

                foreach (var ipInfo in ipInfos)
                {
                    // Ignorer les adresses APIPA (169.254.x.x)
                    if (!ipInfo.Address.ToString().StartsWith("169.254"))
                    {
                        // Préparer l’info à afficher
                        string interfaceName = ni.Name;
                        string ipAddress = ipInfo.Address.ToString();

                        // Créer un item pour le ListView
                        ListViewItem item = new ListViewItem(interfaceName);
                        item.SubItems.Add(ipAddress);

                        // Mettre en Tag l’info utile pour la suite
                        item.Tag = new Tuple<IPAddress, string>(ipInfo.Address, ni.Name);

                        listViewInterfaces.Items.Add(item);
                    }
                }
            }

            // Si aucune interface n’est trouvée, ajouter la Loopback
            if (listViewInterfaces.Items.Count == 0)
            {
                string interfaceName = "Loopback";
                string ipAddress = "127.0.0.1";
                ListViewItem item = new ListViewItem(interfaceName);
                item.SubItems.Add(ipAddress);
                item.Tag = new Tuple<IPAddress, string>(IPAddress.Loopback, "Loopback");
                listViewInterfaces.Items.Add(item);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (listViewInterfaces.SelectedItems.Count > 0)
            {
                var selectedItem = listViewInterfaces.SelectedItems[0];
                var tuple = (Tuple<IPAddress, string>)selectedItem.Tag;
                SelectedIPAddress = tuple.Item1;
                SelectedInterfaceName = tuple.Item2;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show(
                    "Veuillez sélectionner une interface réseau.",
                    "Avertissement",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
