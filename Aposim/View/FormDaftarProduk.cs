using Aposim.Controller;
using Aposim.Model;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Windows.Forms;

namespace Aposim.View
{
    public partial class FormDaftarProduk : Form
    {
        private int kasirId;
        private string kasirNama;
        private string kasirRole;

        PosController user = new PosController();
        ValidasiController validasi = new ValidasiController();

        public FormDaftarProduk(int id, string nama, string role)
        {
            InitializeComponent();
            kasirId = id;
            kasirNama = nama;
            kasirRole = role;
        }

        private void FormDaftarProduk_Load(object sender, EventArgs e)
        {
            LoadProdukAktif();
            lblNamak.Text = kasirNama;
            lblRo.Text = kasirRole;
        }

        void LoadProdukAktif()
        {
            using (MySqlConnection conn = new Connection().GetConn())
            {
                conn.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(
                    "SELECT product_code AS 'Kode', product_name AS 'Nama Produk', selling_price AS 'Harga', stock AS 'Stok' FROM products WHERE status='active'",
                    conn
                );
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvReport.DataSource = dt;
                dgvReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }

        private void btnGantiPassword_Click(object sender, EventArgs e)
        {
            FormGantiPassword gp = new FormGantiPassword(kasirId);
            gp.ShowDialog();
        }

        private void dgvReport_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            FormKasir admin = new FormKasir(kasirId, kasirNama, kasirRole);
            admin.Show();
            this.Close();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
        "Yakin ingin logout?",
        "Konfirmasi Logout",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question
    ) == DialogResult.No)
            {
                return;
            }

            PosController.Logout(kasirId, kasirNama, kasirRole);

            FormLogin login = new FormLogin();
            login.Show();

            this.Close();
        }

        private void guna2GradientPanel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
