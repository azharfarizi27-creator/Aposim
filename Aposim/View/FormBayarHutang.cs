using Aposim.Controller;
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
    public partial class FormBayarHutang : Form
    {
        int selectedPurchaseId = 0;
        int userId = 1;

        public FormBayarHutang()
        {
            InitializeComponent();
        }

        private void FormBayarHutang_Load(object sender, EventArgs e)
        {
            LoadPembelian();
        }

        void LoadPembelian()
        {
            dgvPembelian.DataSource =
                PembayaranController.GetPembelianBelumLunas();

            dgvPembelian.Columns["purchase_id"].Visible = false;
            dgvPembelian.Columns["invoice_number"].HeaderText = "No Faktur";
            dgvPembelian.Columns["purchase_date"].HeaderText = "Tanggal";
            dgvPembelian.Columns["total_amount"].HeaderText = "Total";

            dgvPembelian.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void dgvPembelian_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvPembelian.Rows[e.RowIndex];

                selectedPurchaseId =
                    Convert.ToInt32(row.Cells["purchase_id"].Value);

                txtNoFaktur.Text =
                    row.Cells["invoice_number"].Value.ToString();

                txtTotal.Text =
                    Convert.ToDecimal(row.Cells["total_amount"].Value)
                    .ToString("N0");
            }
        }

        private void btnBayar_Click(object sender, EventArgs e)
        {
            if (selectedPurchaseId == 0)
            {
                MessageBox.Show("Pilih pembelian terlebih dahulu");
                return;
            }

            if (MessageBox.Show(
                "Yakin ingin melunasi pembelian ini?",
                "Konfirmasi",
                MessageBoxButtons.YesNo
            ) == DialogResult.No) return;

            PembayaranController.BayarHutang(
                selectedPurchaseId,
                txtNoFaktur.Text,
                Convert.ToDecimal(txtTotal.Text),
                userId
            );

            MessageBox.Show("Pembayaran berhasil");
            LoadPembelian();

            txtNoFaktur.Clear();
            txtTotal.Clear();
            selectedPurchaseId = 0;
        }

        private void btnBatal_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtTotal_TextChanged(object sender, EventArgs e)
        {

        }

        private void paHeader_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dtTanggalBayar_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
