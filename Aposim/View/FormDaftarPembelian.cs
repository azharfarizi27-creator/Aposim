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
    public partial class FormDaftarPembelian : Form
    {
        PembelianController pembeliancontroller = new PembelianController();
        public FormDaftarPembelian()
        {
            InitializeComponent();

        }

        private void dgvPembeian_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string noFaktur = dgvPembeian.CurrentRow.Cells["No Faktur"].Value.ToString();
                FormDetailPembelian detail = new FormDetailPembelian(noFaktur);
                detail.ShowDialog();
            }
        }

        private void btnTambahPembelian_Click(object sender, EventArgs e)
        {
            if (dgvPembeian.CurrentRow != null)
            {
                string noFaktur = dgvPembeian.CurrentRow.Cells["No Faktur"].Value.ToString();
                FormDetailPembelian detail = new FormDetailPembelian(noFaktur);
                detail.ShowDialog();
            }
            else
            {
                MessageBox.Show("Pilih data pembelian terlebih dahulu.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnCariPembelian_Click(object sender, EventArgs e)
        {
            string keyword = txtCariFaktur.Text.Trim();

            if (string.IsNullOrEmpty(keyword))
            {
                // kalau kosong, tampilkan semua pembelian lagi
                dgvPembeian.DataSource = pembeliancontroller.GetDaftarPembelian();
            }
            else
            {
                // kalau ada isi, cari berdasarkan faktur
                dgvPembeian.DataSource = pembeliancontroller.CariPembelian(keyword);
            }
        }

        private void FormDaftarPembelian_Load(object sender, EventArgs e)
        {
            dgvPembeian.DataSource = pembeliancontroller.GetDaftarPembelian();
        }

        private void txtCariFaktur_TextChanged(object sender, EventArgs e)
        {
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
