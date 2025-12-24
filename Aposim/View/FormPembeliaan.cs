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
using System.Windows.Forms;

namespace Aposim.View
{
    public partial class FormPembeliaan : Form
    {
        DataTable dtBarang = new DataTable();
        ValidasiController validasi = new ValidasiController();

        int userId = 1;
        decimal totalPembelian = 0;
        public FormPembeliaan()
        {
            InitializeComponent();
        }

        private void FormPembeliaan_Load(object sender, EventArgs e)
        {
            GenerateNoFaktur();
            LoadSupplier();
            LoadProduk();
            InitTableBarang();
            LoadStatusBayar();

        }

        private void GenerateNoFaktur()
        {
            txtNoFaktur.Text = "PB-" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        private void LoadSupplier()
        {
            Connection db = new Connection();
            using (MySqlConnection conn = db.GetConn())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(
                    "SELECT supplier_id, supplier_name FROM suppliers", conn);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cbSupplier.DataSource = dt;
                cbSupplier.DisplayMember = "supplier_name";
                cbSupplier.ValueMember = "supplier_id";
                cbSupplier.SelectedIndex = -1;
            }
        }

        private void LoadProduk()
        {
            Connection db = new Connection();
            using (MySqlConnection conn = db.GetConn())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(
                    "SELECT product_id, product_name FROM products WHERE status='active'", conn);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cbBarang.DataSource = dt;
                cbBarang.DisplayMember = "product_name";
                cbBarang.ValueMember = "product_id";
                cbBarang.SelectedIndex = -1;
            }
        }

        private void InitTableBarang()
        {
            dtBarang.Columns.Add("id_barang", typeof(int));
            dtBarang.Columns.Add("nama_barang", typeof(string));
            dtBarang.Columns.Add("jumlah", typeof(int));
            dtBarang.Columns.Add("harga", typeof(decimal));
            dtBarang.Columns.Add("subtotal", typeof(decimal));

            dgvDafratBarang.DataSource = dtBarang;
        }

        private void LoadStatusBayar()
        {
            cbStatusBayar.Items.Clear();
            cbStatusBayar.Items.Add("Lunas");
            cbStatusBayar.Items.Add("Belum Lunas");
            cbStatusBayar.SelectedIndex = 0; // default Lunas
        }


        private void btnTambahBarang_Click(object sender, EventArgs e)
        {
            if (cbBarang.SelectedIndex == -1)
            {
                MessageBox.Show("Pilih produk terlebih dahulu!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2️⃣ Validasi jumlah barang
            if (!validasi.IsQtyValid(nudJumlah.Value.ToString()))
            {
                MessageBox.Show("Jumlah barang tidak valid!\nMasukkan angka lebih dari 0.",
                                "Validasi Qty", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3️⃣ Validasi harga beli
            if (!validasi.IsHargaValid(txtHargaBeli.Text))
            {
                MessageBox.Show("Harga beli tidak valid!\nMasukkan angka lebih dari 0.",
                                "Validasi Harga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int productId = Convert.ToInt32(cbBarang.SelectedValue);
            int qty = Convert.ToInt32(nudJumlah.Value);
            decimal harga = Convert.ToDecimal(txtHargaBeli.Text);
            decimal subtotal = qty * harga;            // Cek jika produk sudah ada

            foreach (DataRow row in dtBarang.Rows)
            {
                if (Convert.ToInt32(row["id_barang"]) == productId)
                {
                    row["jumlah"] = Convert.ToInt32(row["jumlah"]) + qty;
                    row["subtotal"] = Convert.ToInt32(row["jumlah"]) * harga;
                    HitungTotal();
                    return;
                }
            }

            // Jika belum ada
            dtBarang.Rows.Add(
                productId,
                cbBarang.Text,
                qty,
                harga,
                subtotal
            );

            HitungTotal();
        }

        private void HitungTotal()
        {
            totalPembelian = 0;
            foreach (DataRow row in dtBarang.Rows)
            {
                totalPembelian += Convert.ToDecimal(row["subtotal"]);
            }
            lblTotalPembeian.Text = totalPembelian.ToString("N0");
        }

        private void btnSimpanPembelian_Click(object sender, EventArgs e)
        {
            if (cbSupplier.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Supplier wajib dipilih!",
                    "Validasi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // 2️⃣ Validasi barang
            if (dtBarang.Rows.Count == 0)
            {
                MessageBox.Show(
                    "Belum ada barang yang ditambahkan!",
                    "Validasi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // 3️⃣ Ambil status bayar
            string statusBayar = cbStatusBayar.SelectedItem
                .ToString()
                .Trim()
                .ToLower() == "lunas"
                ? "lunas"
                : "belum lunas";

            try
            {
                // 4️⃣ Simpan pembelian
                PembelianController.SimpanPembelian(
                    txtNoFaktur.Text,
                    Convert.ToInt32(cbSupplier.SelectedValue),
                    dtTanggalBeli.Value,
                    statusBayar,
                    userId,
                    dtBarang,
                    totalPembelian
                );

                // 5️⃣ Notifikasi sukses
                MessageBox.Show(
                    "Pembelian berhasil disimpan",
                    "Sukses",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                // 6️⃣ RESET FORM & DATAGRIDVIEW
                ClearPembelian();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Terjadi kesalahan saat menyimpan pembelian:\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void lblBarang_Click(object sender, EventArgs e)
        {

        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lblTotalText_Click(object sender, EventArgs e)
        {

        }

        private void guna2GradientPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnBAtalPembelian_Click(object sender, EventArgs e)
        {
            if (dtBarang.Rows.Count == 0)
            {
                ClearPembelian();
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Batalkan pembelian ini?\nSemua data yang sudah diinput akan dihapus.",
                "Konfirmasi Batal",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirm == DialogResult.Yes)
            {
                ClearPembelian();
            }
        }
        void ClearPembelian()
        {
            dtBarang.Clear();
            totalPembelian = 0;

            lblTotalPembeian.Text = "0";
            cbSupplier.SelectedIndex = -1;
            cbBarang.SelectedIndex = -1;
            txtHargaBeli.Clear();
            nudJumlah.Value = 1;
            cbStatusBayar.SelectedIndex = 0;

            GenerateNoFaktur();
        }

    }
}
