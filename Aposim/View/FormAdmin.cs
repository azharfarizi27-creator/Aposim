using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aposim.View
{
    public partial class FormAdmin : Form
    {
        private int userId;
        private string nama;
        private string role;
        public FormAdmin(int id, string namaUser, string roleUser)
        {
            InitializeComponent();
            userId = id;
            nama = namaUser;
            role = roleUser;    
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            FormUser user = new FormUser();
            user.ShowDialog();

        }

        private void btnProducts_Click(object sender, EventArgs e)
        {
            FormProduc produc = new FormProduc();
            produc.ShowDialog();
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            FormReport report = new FormReport();
            report.ShowDialog();
        }

        private void btnPembelian_Click(object sender, EventArgs e)
        {
            FormPembeliaan formPembeliaan = new FormPembeliaan();
            formPembeliaan.ShowDialog();
        }

        private void btnSupplier_Click(object sender, EventArgs e)
        {
            FormSupplier formSupplier = new FormSupplier();
            formSupplier.ShowDialog();
        }

        private void btnBayarUtang_Click(object sender, EventArgs e)
        {
            FormBayarHutang formBayarHutang = new FormBayarHutang();
            formBayarHutang.ShowDialog();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            using (FornSetting formSettings = new FornSetting(userId, nama, role))
            {
                this.Hide();
                var result = formSettings.ShowDialog();

                if (result == DialogResult.Abort)
                {
                    FormLogin login = new FormLogin();
                    login.Show();
                    this.Close(); // tutup admin
                    return;
                }

                this.Show();
            }
        }

        private void btnCustomer_Click(object sender, EventArgs e)
        {
            FormCustomer formCustomer = new FormCustomer();
            formCustomer.ShowDialog();
        }

        private void FormAdmin_Load(object sender, EventArgs e)
        {

        }

        private void btnDaftarPembelian_Click(object sender, EventArgs e)
        {
            FormDaftarPembelian fdp = new FormDaftarPembelian();
            fdp.ShowDialog();
        }

        private void lblAppname_Click(object sender, EventArgs e)
        {

        }

        private void guna2CirclePictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void btnLogout_Click(object sender, EventArgs e)
        {

        }
    }
}
