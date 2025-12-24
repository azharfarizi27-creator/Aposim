using Aposim.Controller;
using Aposim.Model;
using Aposim.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aposim
{
    public partial class FormLogin : Form
    {
        ValidasiController validasi = new ValidasiController();
        public FormLogin()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            // 🧩 1️⃣ Cek field kosong
            if (!validasi.ValidateRequiredFields(
                (username, "Username"),
                (password, "Password")
            ))
            {
                return;
            }

            // 🧩 2️⃣ Cek format username
            if (!validasi.IsUsernameValid(username))
            {
                MessageBox.Show("Username tidak valid!\nMinimal 3 karakter dan hanya huruf/angka tanpa simbol atau spasi.",
                                "Validasi Username", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 🧩 3️⃣ Jalankan proses login
            var result = PosController.Login(username, password);

            if (!validasi.IsPasswordValid(password))
            {
                MessageBox.Show("Password tidak valid!\nMinimal 8 karakter, harus mengandung huruf dan angka.\nHanya simbol umum seperti @, #, !, _, - yang diperbolehkan.",
                   "Validasi Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 🧩 4️⃣ Jika login gagal
            if (!result.success)
            {
                MessageBox.Show("Login gagal!\nPeriksa kembali username atau password.",
                                "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 🧩 5️⃣ Jika login berhasil
            MessageBox.Show(
                $"Login berhasil!\nSelamat datang {result.nama} - {result.role}",
                "Sukses",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );


            // 🧩 6️⃣ Buka form sesuai role
            this.Hide();

            switch (result.role)
            {
                case "Admin":
                    FormAdmin admin = new FormAdmin(result.userId, result.nama, result.role);
                    admin.Show();
                    this.Hide();
                    break;
                case "Kasir":
                    FormKasir kasir = new FormKasir(result.userId, result.nama, result.role);
                    kasir.Show();
                    this.Hide();
                    break;
                case "Manager":
                    FormManajer manajer = new FormManajer(result.userId,result.nama, result.role);
                    manajer.Show();
                    this.Hide();
                    break;
                default:
                    MessageBox.Show("Role tidak dikenali!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Show();
                    break;
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
