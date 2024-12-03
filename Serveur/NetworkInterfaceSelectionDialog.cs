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

        public NetworkInterfaceSelectionDialog()
        {
            InitializeComponent();

            // Initialiser la liste des interfaces réseau
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
                    // Ignorer les adresses de lien local
                    if (!ipInfo.Address.ToString().StartsWith("169.254"))
                    {
                        string interfaceName = $"{ni.Name} ({ipInfo.Address.ToString()})";
                        ListViewItem item = new ListViewItem(interfaceName);
                        item.Tag = new Tuple<IPAddress, string>(ipInfo.Address, ni.Name);
                        listViewInterfaces.Items.Add(item);
                    }
                }
            }

            // Si aucune interface n'est ajoutée, ajouter le loopback
            if (listViewInterfaces.Items.Count == 0)
            {
                string interfaceName = "Loopback (127.0.0.1)";
                ListViewItem item = new ListViewItem(interfaceName);
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
                MessageBox.Show("Veuillez sélectionner une interface réseau.", "Avertissement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }


    }
}
