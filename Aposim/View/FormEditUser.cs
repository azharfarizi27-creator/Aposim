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
    public partial class FormEditUser : Form
    {
        int userId;
        PosController poscontroller;
        ValidasiController va;

        public FormEditUser(int id, string nama, string username, string email, string noHp)
        {
            InitializeComponent();
            poscontroller = new PosController();
            va = new ValidasiController();

            userId = id;

            txtNama.Text = nama;
            txtUsername.Text = username;
            txtEmail.Text = email;
            txtNoHp.Text = noHp;

        }

        private void FormEditUser_Load(object sender, EventArgs e)
        {

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string nama = txtNama.Text.Trim();
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string noHp = txtNoHp.Text.Trim();

            // 1️⃣ Validasi wajib isi
            if (!va.ValidateRequiredFields(
                (nama, "Nama"),
                (username, "Username"),
                (email, "Email"),
                (noHp, "No HP")
            ))
                return;

            // 2️⃣ Validasi nama
            if (!va.IsUsernameValid(nama))
            {
                MessageBox.Show(
                    "Nama tidak valid!\nMinimal 5 karakter dan hanya huruf/angka.",
                    "Validasi Nama",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // 3️⃣ Validasi username
            if (!va.IsUsernameValid(username))
            {
                MessageBox.Show(
                    "Username tidak valid!\nMinimal 5 karakter dan hanya huruf/angka.",
                    "Validasi Username",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // 4️⃣ Validasi email
            if (!va.IsEmailValid(email))
            {
                MessageBox.Show("Format email tidak valid!",
                    "Validasi Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 5️⃣ Validasi No HP
            if (!va.IsPhoneValid(noHp))
            {
                MessageBox.Show("No HP tidak valid! (hanya angka, min 10 digit)",
                    "Validasi No HP", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 6️⃣ Update ke database
            bool result = poscontroller.UpdateUser(
                userId,
                nama,
                username,
                email,
                noHp
            );

            if (result)
            {
                MessageBox.Show("User berhasil diupdate");
                this.Close();
            }
            else
            {
                MessageBox.Show("Gagal update user");
            }
        }

        private void lblNama_Click(object sender, EventArgs e)
        {

        }

        private void Username_Click(object sender, EventArgs e)
        {

        }

        private void lblRole_Click(object sender, EventArgs e)
        {

        }

        private void lblStatuus_Click(object sender, EventArgs e)
        {

        }

        private void txtNama_TextChanged(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnBatal_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
