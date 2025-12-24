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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Aposim.View
{
    public partial class FormCustomer : Form
    {

        CustomerController customer;
        ValidasiController validasi;
        int selectedId = 0;

        public FormCustomer()
        {
            InitializeComponent();
        }

        private void guna2TextBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void FormCustomer_Load(object sender, EventArgs e)
        {
            customer = new CustomerController();
            validasi = new ValidasiController();
            LoadCustomer();
            SetAddMode();
        }

        void LoadCustomer()
        {
            dgvCustomer.DataSource = customer.GetCustomers();

            dgvCustomer.Columns["customer_id"].Visible = false;
            dgvCustomer.Columns["member_points"].Visible = false;
            dgvCustomer.Columns["created_at"].Visible = false;

            dgvCustomer.Columns["customer_name"].HeaderText = "Nama";
            dgvCustomer.Columns["phone"].HeaderText = "No HP";
            dgvCustomer.Columns["email"].HeaderText = "Email";
            dgvCustomer.Columns["address"].HeaderText = "Alamat";

            dgvCustomer.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
        }

        void SetAddMode()
        {
            selectedId = 0;
            btnAdd.Enabled = true;
            btnEdit.Enabled = false;
            btnDelete.Enabled = false;
        }

        void SetEditMode()
        {
            btnAdd.Enabled = false;
            btnEdit.Enabled = true;
            btnDelete.Enabled = true;
        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            string nama = txtNama.Text.Trim();
            string nohp = txtNoHp.Text.Trim();
            string email = txtEmail.Text.Trim();
            string alamat = txtAlamat.Text.Trim();

            if (!validasi.IsUsernameValid(nama))
            {
                MessageBox.Show("Nama Customer tidak valid!\nMinimal 5 karakter dan tanpa simbol.",
                    "Validasi Nama", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!validasi.IsPhoneValid(nohp))
            {
                MessageBox.Show("Nomor HP tidak valid!\nGunakan format 08xxxxxxxxxx (10–13 digit, tanpa simbol).",
                                   "Validasi Nomor HP", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!validasi.IsEmailValid(email))
            {
                MessageBox.Show("Email tidak valid!\nBagian sebelum '@' minimal 5 karakter dan tidak boleh mengandung simbol.",
                                    "Validasi Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!validasi.IsAlamatValid(alamat))
            {
                MessageBox.Show("Alamat tidak valid!\nMinimal 10 karakter dan hanya huruf, angka, spasi, koma, titik, garis miring, dan tanda hubung.",
                "Validasi Alamat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            bool result = customer.AddCustomer(
                txtNama.Text.Trim(),
                txtNoHp.Text.Trim(),
                txtEmail.Text.Trim(),
                txtAlamat.Text.Trim()
            );

            if (result)
            {
                MessageBox.Show("Customer berhasil ditambahkan");
                LoadCustomer();
                ClearForm();
            }

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            string nama = txtNama.Text.Trim();
            string nohp = txtNoHp.Text.Trim();
            string email = txtEmail.Text.Trim();
            string alamat = txtAlamat.Text.Trim();


            if (selectedId == 0)
            {
                MessageBox.Show("Pilih customer terlebih dahulu");
                return;
            }
            if (!validasi.IsUsernameValid(nama))
            {
                MessageBox.Show("Nama Customer tidak valid!\nMinimal 5 karakter dan tanpa simbol.",
                    "Validasi Nama", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!validasi.IsPhoneValid(nohp))
            {
                MessageBox.Show("Nomor HP tidak valid!\nGunakan format 08xxxxxxxxxx (10–13 digit, tanpa simbol).",
                                   "Validasi Nomor HP", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!validasi.IsEmailValid(email))
            {
                MessageBox.Show("Email tidak valid!\nBagian sebelum '@' minimal 5 karakter dan tidak boleh mengandung simbol.",
                                    "Validasi Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!validasi.IsAlamatValid(alamat))
            {
                MessageBox.Show("Alamat tidak valid!\nMinimal 10 karakter dan hanya huruf, angka, spasi, koma, titik, garis miring, dan tanda hubung.",
                "Validasi Alamat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            bool result = customer.UpdateCustomer(
                selectedId,
                txtNama.Text.Trim(),
                txtNoHp.Text.Trim(),
                txtEmail.Text.Trim(),
                txtAlamat.Text.Trim()
            );

            if (result)
            {
                MessageBox.Show(
                    "Customer berhasil diupdate",
                    "Sukses",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                LoadCustomer();
                ClearForm();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedId == 0)
            {
                MessageBox.Show("Pilih customer terlebih dahulu");
                return;
            }

            bool hasTrx = customer.HasTransaction(selectedId);

            if (hasTrx)
            {
                MessageBox.Show(
                    "Customer sudah memiliki transaksi dan tidak bisa dihapus",
                    "Info",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Hapus customer ini?",
                "Konfirmasi",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirm == DialogResult.Yes)
            {
                customer.DeleteCustomer(selectedId);
                LoadCustomer();
                ClearForm();
            }
        }

        private void dgvCustomer_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvCustomer.Rows[e.RowIndex];

                selectedId = Convert.ToInt32(row.Cells["customer_id"].Value);
                txtNama.Text = row.Cells["customer_name"].Value.ToString();
                txtNoHp.Text = row.Cells["phone"].Value.ToString();
                txtEmail.Text = row.Cells["email"].Value.ToString();
                txtAlamat.Text = row.Cells["address"].Value.ToString();
                SetEditMode();
            }
        }

        void ClearForm()
        {
            txtNama.Clear();
            txtNoHp.Clear();
            txtEmail.Clear();
            txtAlamat.Clear();
            selectedId = 0;

            SetAddMode(); // 🔥 wajib
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void btnkembali_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
