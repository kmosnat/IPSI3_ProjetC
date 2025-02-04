using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Net;

namespace Client
{
    public partial class ServerSelectionDialog : Form
    {
        public IPAddress SelectedIPAddress { get; private set; }

        public ServerSelectionDialog()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string ipAddressText = txtIPAddress.Text.Trim();
            if (IsValidIPAddress(ipAddressText))
            {
                // Convertir l'adresse IP en objet IPAddress
                SelectedIPAddress = IPAddress.Parse(ipAddressText);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Veuillez entrer une adresse IP valide.", "Adresse IP invalide", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Vérifier si une adresse IP est valide
        private bool IsValidIPAddress(string ipAddress)
        {
            string pattern = @"^(([0-9]|[0-9]{2}|[0-1][0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[0-9]{2}|[0-1][0-9]{2}|2[0-4][0-9]|25[0-5])$";
            return Regex.IsMatch(ipAddress, pattern);
        }

        // Annuler la sélection
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Sélectionner l'adresse IP locale
        private void local_Click(object sender, EventArgs e)
        {
            SelectedIPAddress = IPAddress.Parse("127.0.0.1");
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
