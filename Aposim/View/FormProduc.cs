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
    public partial class FormProduc : Form
    {
        ProducController product;
        ValidasiController validasi;
        int selectedId = 0;

        public FormProduc()
        {
            InitializeComponent();
        }

        private void txtStock_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtStock_Load(object sender, EventArgs e)
        {

        }

        private void FormProduc_Load(object sender, EventArgs e)
        {
            product = new ProducController();
            validasi = new ValidasiController();    
            LoadCategory();
            LoadProduct();
            SetAddMode();


        }

        void SetAddMode()
        {
            selectedId = 0;
            btnAdd.Enabled = true;
            btnEdit.Enabled = false;
            btnStatus.Enabled = false;
        }

        void SetEditMode()
        {
            btnAdd.Enabled = false;
            btnEdit.Enabled = true;
            btnStatus.Enabled = true;
        }

        void LoadCategory()
        {
            cbCategory.DataSource = product.GetCategories();
            cbCategory.DisplayMember = "category_name";
            cbCategory.ValueMember = "category_id";
            cbCategory.SelectedIndex = -1;
        }

        void LoadProduct()
        {
            dgvProduc.DataSource = product.GetProducts();

            dgvProduc.Columns["category_id"].Visible = false;

            dgvProduc.Columns["product_id"].HeaderText = "ID";
            dgvProduc.Columns["product_code"].HeaderText = "Kode";
            dgvProduc.Columns["product_name"].HeaderText = "Nama Produk";
            dgvProduc.Columns["category_name"].HeaderText = "Kategori";
            //dgvProduc.Columns["purchase_price"].HeaderText = "Harga Beli";
            dgvProduc.Columns["selling_price"].HeaderText = "Harga Jual";
            dgvProduc.Columns["stock"].HeaderText = "Stok";
            dgvProduc.Columns["status"].HeaderText = "Status";
            dgvProduc.Columns["min_stock"].HeaderText = "Min Stok";
            dgvProduc.Columns["unit"].HeaderText = "Satuan";


            dgvProduc.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string kode = txtCode.Text.Trim();
            string nama = txtProducName.Text.Trim();
            string harga = txtHargaJual.Text.Trim();
            string unit = txtUnit.Text.Trim();

            // 1️⃣ Validasi field wajib isi
            if (!validasi.ValidateRequiredFields(
                (kode, "Kode Produk"),
                (nama, "Nama Produk"),
                (harga, "Harga Jual"),
                (unit, "Satuan")
            ))
                return;

            if (!validasi.IsKodeValid(kode))
            {
                MessageBox.Show("Kode produk tidak valid!\nHanya huruf/angka, maksimal 5 karakter, tanpa simbol.",
                                "Validasi Kode", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!validasi.IsHargaValid(harga))
            {
                MessageBox.Show("Harga jual tidak valid!\nMasukkan angka lebih dari 0 tanpa simbol.",
                                "Validasi Harga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cbCategory.SelectedIndex == -1)
            {
                MessageBox.Show("Kategori wajib dipilih!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool result = product.AddProduct(
                txtCode.Text.Trim(),
                txtProducName.Text.Trim(),
                Convert.ToInt32(cbCategory.SelectedValue),
                //Convert.ToDecimal(txtHargaBeli.Text),
                Convert.ToDecimal(txtHargaJual.Text),
                txtUnit.Text.Trim()
            );

            if (result)
            {
                MessageBox.Show("Produk berhasil ditambahkan");
                LoadProduct();
                ClearForm();

            }
            else
            {
                MessageBox.Show("Gagal menambahkan produk");
            }
        }

        void ClearForm()
        {
            txtCode.Clear();
            txtProducName.Clear();
            txtHargaJual.Clear();
            txtUnit.Clear();
            cbCategory.SelectedIndex = -1;
            selectedId = 0;

            SetAddMode(); // 🔥 INI YANG KURANG
        }


        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (selectedId == 0)
            {
                MessageBox.Show("Pilih produk terlebih dahulu");
                return;
            }

            string kode = txtCode.Text.Trim();
            string nama = txtProducName.Text.Trim();
            string harga = txtHargaJual.Text.Trim();
            string unit = txtUnit.Text.Trim();


            if (!validasi.ValidateRequiredFields(
                 (kode, "Kode Produk"),
                 (nama, "Nama Produk"),
                 (harga, "Harga Jual"),
                 (unit, "Satuan")
             ))
                return;

            if (!validasi.IsKodeValid(kode))
            {
                MessageBox.Show("Kode produk tidak valid!\nHanya huruf/angka, maksimal 5 karakter, tanpa simbol.",
                                "Validasi Kode", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!validasi.IsHargaValid(harga))
            {
                MessageBox.Show("Harga jual tidak valid!\nMasukkan angka lebih dari 0 tanpa simbol.",
                                "Validasi Harga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            bool result = product.UpdateProduct(
                selectedId,
                txtCode.Text.Trim(),
                txtProducName.Text.Trim(),
                Convert.ToInt32(cbCategory.SelectedValue),
                //Convert.ToDecimal(txtHargaBeli.Text),
                Convert.ToDecimal(txtHargaJual.Text),
                txtUnit.Text.Trim(),
                "active"
            );

            if (result)
            {
                MessageBox.Show("Produk berhasil diperbarui");
                LoadProduct();
                ClearForm();
            }
            else
            {
                MessageBox.Show("Gagal memperbarui produk");
            }
        }

        private void dgvProduc_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvProduc.Rows[e.RowIndex];

                selectedId = Convert.ToInt32(row.Cells["product_id"].Value);
                txtCode.Text = row.Cells["product_code"].Value.ToString();
                txtProducName.Text = row.Cells["product_name"].Value.ToString();
                cbCategory.SelectedValue = row.Cells["category_id"].Value;
                //txtHargaBeli.Text = row.Cells["purchase_price"].Value.ToString();
                txtHargaJual.Text = row.Cells["selling_price"].Value.ToString();
                txtUnit.Text = row.Cells["unit"].Value.ToString();

                SetEditMode();
                // JANGAN isi harga & stok di Form Produk
                // Harga & stok hanya dari Form Pembelian / Stock Adjustment
            }
        }

        private void btnStatus_Click(object sender, EventArgs e)
        {
            if (selectedId == 0)
            {
                MessageBox.Show("Pilih produk terlebih dahulu");
                return;
            }

            // Ambil status saat ini dari DataGridView
            string currentStatus = dgvProduc.SelectedRows[0].Cells["status"].Value.ToString();
            string newStatus = (currentStatus == "active") ? "inactive" : "active";

            bool result = product.UpdateProductStatus(selectedId, newStatus);

            if (result)
            {
                MessageBox.Show($"Status berhasil diubah menjadi {newStatus}");
                LoadProduct(); // refresh DataGridView
            }
            else
            {
                MessageBox.Show("Gagal mengubah status produk");
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void txtHargaBeli_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void paHeader_Paint(object sender, PaintEventArgs e)
        {
                    }

        private void guna2GradientPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
