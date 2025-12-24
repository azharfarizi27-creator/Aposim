using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.IO;
using Aposim.Controller;

namespace Aposim.View
{
    public partial class FornSetting : Form
    {
        SettingController setting = new SettingController();
        ValidasiController va = new ValidasiController();
        private int userId;
        private string nama;
        private string role;
        public FornSetting(int id, string namaUser, string roleUser)
        {
            InitializeComponent();
            userId = id;
            nama = namaUser;
            role = roleUser;
        }

        private void FornSetting_Load(object sender, EventArgs e)
        {

            LoadPrinter();
            LoadSetting();
        }

        void LoadPrinter()
        {
            foreach (string printer in PrinterSettings.InstalledPrinters);
        }

        void LoadSetting()
        {
            DataTable dt = setting.GetSetting();
            if (dt.Rows.Count == 0) return;

            DataRow r = dt.Rows[0];

            txtNamaToko.Text = r["store_name"].ToString();
            txtAlamat.Text = r["store_address"].ToString();
            txtNotel.Text = r["store_phone"].ToString();
            txtNPWP.Text = r["npwp"].ToString();

            // LOGO
            if (!string.IsNullOrEmpty(r["store_logo"].ToString()) &&
                File.Exists(r["store_logo"].ToString()))
            {
                using (var fs = new FileStream(r["store_logo"].ToString(), FileMode.Open, FileAccess.Read))
                {
                    pbLogo.Image = Image.FromStream(fs);
                }
                txtLogoPath.Text = r["store_logo"].ToString();
            }
            else
            {
                pbLogo.Image = null;
                txtLogoPath.Clear();
            }
        }

        //private void btnSaveSetting_Click(object sender, EventArgs e)
        //{
        //    setting.SaveSetting(
        //        txtNamaToko.Text,
        //        txtAlamat.Text,
        //        txtNotel.Text,
        //        txtLogoPath.Text,
        //        txtNPWP.Text,
        //        cbTema.Text,
        //        cbPrinter.Text
        //    );

        //    MessageBox.Show("Pengaturan berhasil disimpan");
        //}

        private void btnPicture_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.png;*.jpg;*.jpeg";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtLogoPath.Text = ofd.FileName;
                pbLogo.ImageLocation = ofd.FileName;
            }
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            string nama = txtNamaToko.Text.Trim();
            string alamat = txtAlamat.Text.Trim();
            string notel = txtNotel.Text.Trim();
            string npwp = txtNPWP.Text.Trim();
            string logoPath = txtLogoPath.Text.Trim();

            // 1️⃣ Validasi wajib isi
            if (!va.ValidateRequiredFields((nama, "Nama Toko"), (alamat, "Alamat")))
                return;

            // 2️⃣ Validasi alamat
            if (!va.IsAlamatValid(alamat))
            {
                MessageBox.Show("Alamat tidak valid!\nMinimal 10 karakter dan hanya huruf, angka, spasi, koma, titik, garis miring, dan tanda hubung.",
                                "Validasi Alamat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3️⃣ Validasi nomor telepon
            if (!string.IsNullOrEmpty(notel) && !va.IsPhoneValid(notel))
            {
                MessageBox.Show("Nomor telepon tidak valid!\nGunakan format 08xxxxxxxxxx (10–13 digit, tanpa simbol).",
                                "Validasi Telepon", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 4️⃣ Validasi NPWP (jika diisi)
            if (!string.IsNullOrEmpty(npwp))
            {
                if (npwp.Length < 15 || npwp.Length > 20 || !npwp.All(char.IsDigit))
                {
                    MessageBox.Show("NPWP tidak valid!\nHanya angka, 15–20 digit.",
                                    "Validasi NPWP", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (!string.IsNullOrEmpty(logoPath) && File.Exists(logoPath))
            {
                string folder = Path.Combine(Application.StartupPath, "Assets");
                Directory.CreateDirectory(folder);

                string newPath = Path.Combine(folder, Path.GetFileName(logoPath));

                if (!string.Equals(logoPath, newPath, StringComparison.OrdinalIgnoreCase))
                {
                    byte[] bytes = File.ReadAllBytes(logoPath);
                    File.WriteAllBytes(newPath, bytes);
                }

                logoPath = newPath;
            }

            setting.SaveSetting(
                txtNamaToko.Text,
                txtAlamat.Text,
                txtNotel.Text,
                logoPath,
                txtNPWP.Text
            );

            MessageBox.Show("Pengaturan berhasil disimpan");
        }

        private void btnHapusLogo_Click(object sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show(
                    "Yakin ingin menghapus logo?",
                    "Konfirmasi",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

            if (confirm != DialogResult.Yes) return;

            if (setting.HapusLogo())
            {
                pbLogo.Image = null;
                txtLogoPath.Clear();

                MessageBox.Show("Logo berhasil dihapus");
            }
            else
            {
                MessageBox.Show("Gagal menghapus logo");
            }
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            //FormAdmin admin = new FormAdmin(userId, nama, role);
            //admin.Show();
            this.Close();
        }

        private void btnUbahPAssword_Click(object sender, EventArgs e)
        {
            FormGantiPassword gp = new FormGantiPassword(userId);
            gp.ShowDialog();
        }

        private void btnKeluar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
        "Yakin ingin logout?",
        "Konfirmasi Logout",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question
    ) == DialogResult.No)
                return;

            PosController.Logout(userId, nama, role);

            this.DialogResult = DialogResult.Abort;
            this.Close();

        }

        private void guna2GradientPanel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
