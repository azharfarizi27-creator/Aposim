using Aposim.Controller;
using Aposim.Model;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aposim.View
{
    public partial class FormGantiPassword : Form
    {
        PosController po = new PosController();
        ValidasiController va = new ValidasiController();
        private int kasirId;
        public FormGantiPassword(int id)
        {
            InitializeComponent();
            kasirId = id;
        }

        private void FormGantiPassword_Load(object sender, EventArgs e)
        {

        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            // 1️⃣ Validasi wajib isi
            if (!va.ValidateRequiredFields(
                (txtPasswordLama.Text, "Password Lama"),
                (txtPasswordBaru.Text, "Password Baru"),
                (txtKonfirmasi.Text, "Konfirmasi Password")
            ))
                return;

            // 2️⃣ Password baru ≠ konfirmasi
            if (txtPasswordBaru.Text != txtKonfirmasi.Text)
            {
                MessageBox.Show("Password baru dan konfirmasi tidak sama!",
                    "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3️⃣ Password baru ≠ password lama
            if (txtPasswordLama.Text == txtPasswordBaru.Text)
            {
                MessageBox.Show("Password baru tidak boleh sama dengan password lama!",
                    "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 4️⃣ Validasi format password baru
            if (!va.IsPasswordValid(txtPasswordBaru.Text))
            {
                MessageBox.Show(
                    "Password harus:\n" +
                    "- 8–20 karakter\n" +
                    "- Mengandung huruf dan angka\n" +
                    "- Boleh simbol (@ # - _ ! dll)",
                    "Password Tidak Valid",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // 4. Hash password
            string passLamaHash = PosController.HashPassword(txtPasswordLama.Text);
            string passBaruHash = PosController.HashPassword(txtPasswordBaru.Text);

            // 5. Update ke DB
            bool sukses = po.GantiPassword(kasirId, passLamaHash, passBaruHash);

            if (!sukses)
            {
                MessageBox.Show("Password lama salah!",
                    "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Password berhasil diganti!",
                "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.Close();
        }

        // 🔑 HASH PASSWORD (SHA256)
        private void btnKembai_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void paheader_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtPasswordBaru_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
