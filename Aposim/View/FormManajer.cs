using Aposim.Controller;
using Guna.UI2.AnimatorNS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Animation;

namespace Aposim.View
{
    public partial class FormManajer : Form
    {
        
        ManajerController manajerController = new ManajerController();

        private int userId;
        private string nama;
        private string role;
        public FormManajer(int id, string namaUser, string roleUser)
        {
            InitializeComponent();
            userId = id;
            nama = namaUser;
            role = roleUser;
        }

        private void FormManajer_Load(object sender, EventArgs e)
        {
            LoadDashboard();
        }

        private void LoadDashboard()
        {
            lblPenjualanHariIni.Text =
                "Rp " + manajerController.GetPenjualanHariIni().ToString("N0");

            lblPenjualanBulanIni.Text =
                "Rp " + manajerController.GetPenjualanBulanIni().ToString("N0");

            lblProfit.Text =
                "Rp " + manajerController.GetProfit().ToString("N0");

            lblSaldoKas.Text =
                "Rp " + manajerController.GetSaldoKasToko().ToString("N0");


            dgvStokMenipis.DataSource =
                manajerController.GetStokMenipis();

            dgvStokMenipis.ReadOnly = true;
            dgvStokMenipis.AllowUserToAddRows = false;
            dgvStokMenipis.AllowUserToDeleteRows = false;
            dgvStokMenipis.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void dgvStokMenipis_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void lblPenjualanHariIni_Click(object sender, EventArgs e)
        {

        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {

        }

        private void btnLaporan_Click(object sender, EventArgs e)
        {
            FormReport formReport = new FormReport();
            formReport.ShowDialog();
        }

        private void btnStok_Click(object sender, EventArgs e)
        {

        }

        private void btnStok_Click_1(object sender, EventArgs e)
        {

            dgvStokMenipis.DataSource = manajerController.GetStok();
            dgvStokMenipis.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void btnLog_Click(object sender, EventArgs e)
        {
            dgvStokMenipis.DataSource = manajerController.GetLog();
            dgvStokMenipis.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void btnLaporan_Click_1(object sender, EventArgs e)
        {
            FormReport report = new FormReport();
            report.ShowDialog();
        }

        private void btnGantiPass_Click(object sender, EventArgs e)
        {
            FormGantiPassword gp = new FormGantiPassword(userId);
            gp.ShowDialog();
        }

        private void paHeader_Paint(object sender, PaintEventArgs e)
        {

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
            PosController.Logout(userId, nama, role);

            FormLogin login = new FormLogin();
            login.Show();

            this.Close();
        }
    }
}
