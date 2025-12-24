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
    public partial class FormSupplier : Form
    {
        SupplierController supplier;
        ValidasiController va;
        int selectedId = 0;
        public FormSupplier()
        {
            InitializeComponent();
        }

        private void FormSupplier_Load(object sender, EventArgs e)
        {
            supplier = new SupplierController();
            va = new ValidasiController();
            LoadSupplier();
            SetAddMode();
        }

        void LoadSupplier()
        {
            dgvSupplier.DataSource = supplier.GetSuppliers();
            dgvSupplier.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        void ClearForm()
        {
            txtNamaSupp.Clear();
            txtNamaKontak.Clear();
            txtNoHp.Clear();
            txtEmail.Clear();
            txtAlamat.Clear();
            selectedId = 0;

            SetAddMode(); // 🔥 PENTING
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string nama = txtNamaSupp.Text.Trim();
            string kontak = txtNamaKontak.Text.Trim();
            string nohp = txtNoHp.Text.Trim();
            string email = txtEmail.Text.Trim();
            string alamat = txtAlamat.Text.Trim();

            if (!va.ValidateRequiredFields((nama, "Nama Supplier")))
                return;

            // 2️⃣ Validasi nomor HP (boleh kosong tapi kalau diisi harus valid)
            if (!string.IsNullOrEmpty(nohp) && !va.IsPhoneValid(nohp))
            {
                MessageBox.Show("Nomor HP tidak valid!\nGunakan format 08xxxxxxxxxx (10–13 digit, tanpa simbol).",
                                "Validasi Nomor HP", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3️⃣ Validasi email (boleh kosong tapi kalau diisi harus valid)
            if (!string.IsNullOrEmpty(email) && !va.IsEmailValid(email))
            {
                MessageBox.Show("Email tidak valid!\nBagian sebelum '@' minimal 5 karakter dan tanpa simbol.",
                                "Validasi Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 4️⃣ Validasi alamat
            if (!string.IsNullOrEmpty(alamat) && !va.IsAlamatValid(alamat))
            {
                MessageBox.Show("Alamat tidak valid!\nMinimal 10 karakter dan hanya huruf, angka, spasi, koma, titik, garis miring, dan tanda hubung.",
                                "Validasi Alamat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            bool result = supplier.AddSupplier(
                txtNamaSupp.Text,
                txtNamaKontak.Text,
                txtNoHp.Text,
                txtEmail.Text,
                txtAlamat.Text
            );

            if (result)
            {
                MessageBox.Show("Supplier berhasil ditambahkan");
                LoadSupplier();
                ClearForm();
            }
            else
            {
                MessageBox.Show("Gagal menambahkan supplier");
            }

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (selectedId == 0)
            {
                MessageBox.Show("Pilih supplier terlebih dahulu");
                return;
            }

            string nama = txtNamaSupp.Text.Trim();
            string kontak = txtNamaKontak.Text.Trim();
            string nohp = txtNoHp.Text.Trim();
            string email = txtEmail.Text.Trim();
            string alamat = txtAlamat.Text.Trim();

            if (!va.ValidateRequiredFields((nama, "Nama Supplier")))
                return;

            if (!string.IsNullOrEmpty(nohp) && !va.IsPhoneValid(nohp))
            {
                MessageBox.Show("Nomor HP tidak valid!\nGunakan format 08xxxxxxxxxx (10–13 digit, tanpa simbol).",
                                "Validasi Nomor HP", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!string.IsNullOrEmpty(email) && !va.IsEmailValid(email))
            {
                MessageBox.Show("Email tidak valid!\nBagian sebelum '@' minimal 5 karakter dan tanpa simbol.",
                                "Validasi Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!string.IsNullOrEmpty(alamat) && !va.IsAlamatValid(alamat))
            {
                MessageBox.Show("Alamat tidak valid!\nMinimal 10 karakter dan hanya huruf, angka, spasi, koma, titik, garis miring, dan tanda hubung.",
                                "Validasi Alamat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool result = supplier.UpdateSupplier(
                selectedId,
                txtNamaSupp.Text,
                txtNamaKontak.Text,
                txtNoHp.Text,
                txtEmail.Text,
                txtAlamat.Text
            );

            if (result)
            {
                MessageBox.Show("Supplier berhasil diupdate");
                LoadSupplier();
                ClearForm();
            }
            else
            {
                MessageBox.Show("Gagal update supplier");
            }

        }

        private void dgvSupplier_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvSupplier.Rows[e.RowIndex];
                selectedId = Convert.ToInt32(row.Cells["supplier_id"].Value);
                txtNamaSupp.Text = row.Cells["supplier_name"].Value.ToString();
                txtNamaKontak.Text = row.Cells["contact_name"].Value.ToString();
                txtNoHp.Text = row.Cells["phone"].Value.ToString();
                txtEmail.Text = row.Cells["email"].Value.ToString();
                txtAlamat.Text = row.Cells["address"].Value.ToString();
                SetEditMode();
            }

        }

        void SetAddMode()
        {
            selectedId = 0;
            btnAdd.Enabled = true;
            btnEdit.Enabled = false;
        }

        void SetEditMode()
        {
            btnAdd.Enabled = false;
            btnEdit.Enabled = true;
        }


        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtNamaKontak_TextChanged(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
