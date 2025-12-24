using Aposim.Controller;
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
    public partial class FormUser : Form
    {
        PosController poscontroller;

        public FormUser()
        {
            InitializeComponent();
            poscontroller = new PosController();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            dgvUser.DataSource = poscontroller.Search(txtSearch.Text);
        }

        private void dgvUser_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void FormUser_Load(object sender, EventArgs e)
        {
            ShowTable();
        }


        bool ShowTable()
        {
            dgvUser.AutoGenerateColumns = true;

            dgvUser.DataSource = poscontroller.TampilkanUser(
                new MySqlCommand(
                    @"SELECT 
                u.user_id,
                u.nama,
                u.username,
                u.email,
                u.no_hp,
                u.role_id,  
                r.role_name,
                u.status
              FROM users u
              JOIN roles r ON u.role_id = r.role_id"
                )
            );

            if (dgvUser.Columns.Contains("user_id"))
                dgvUser.Columns["user_id"].HeaderText = "ID";
            if (dgvUser.Columns.Contains("nama"))
                dgvUser.Columns["nama"].HeaderText = "Nama";
            if (dgvUser.Columns.Contains("username"))
                dgvUser.Columns["username"].HeaderText = "Username";
            if (dgvUser.Columns.Contains("email"))
                dgvUser.Columns["email"].HeaderText = "Email";
            if (dgvUser.Columns.Contains("no_hp"))
                dgvUser.Columns["no_hp"].HeaderText = "No HP";
            if (dgvUser.Columns.Contains("role_name"))
                dgvUser.Columns["role_name"].HeaderText = "Role";
            if (dgvUser.Columns.Contains("status"))
                dgvUser.Columns["status"].HeaderText = "Status";

            if (dgvUser.Columns.Contains("role_id"))
                dgvUser.Columns["role_id"].Visible = false;

            dgvUser.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            return true;
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            FormAddUser addUser = new FormAddUser();
            addUser.ShowDialog();
            ShowTable();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvUser.CurrentRow == null || dgvUser.CurrentRow.IsNewRow)
            {
                MessageBox.Show("Pilih user terlebih dahulu!");
                return;
            }

            DataGridViewRow row = dgvUser.CurrentRow;

            int userId = Convert.ToInt32(row.Cells["user_id"].Value);
            string nama = row.Cells["nama"].Value?.ToString() ?? "";
            string username = row.Cells["username"].Value?.ToString() ?? "";
            string email = row.Cells["email"].Value?.ToString() ?? "";
            string NoHp = row.Cells["no_hp"].Value?.ToString() ?? "";

            FormEditUser frm = new FormEditUser(
                userId,
                nama,
                username,
                email,
                NoHp
            );

            frm.ShowDialog();
            ShowTable();
        }

        void LoadUser()
        {
            MySqlCommand cmd = new MySqlCommand(
       @"SELECT 
    u.user_id,
    u.nama,
    u.username,
    u.role_id,
    r.role_name,
    u.status
FROM users u
JOIN roles r ON u.role_id = r.role_id
"
   );

            // 2️⃣ PENTING: aktifkan auto generate kolom
            dgvUser.AutoGenerateColumns = true;

            // 3️⃣ Isi DataGridView dari controller
            dgvUser.DataSource = poscontroller.TampilkanUser(cmd);

            // 4️⃣ Rapikan tampilan
            dgvUser.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // 5️⃣ SEMBUNYIKAN kolom role_id (tapi datanya tetap ada)
            dgvUser.Columns["role_id"].Visible = false;
        }

        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            if (dgvUser.CurrentRow == null)
            {
                MessageBox.Show("Pilih user terlebih dahulu!");
                return;
            }

            int userId = Convert.ToInt32(dgvUser.CurrentRow.Cells["user_id"].Value);
            string username = dgvUser.CurrentRow.Cells["username"].Value.ToString();

            DialogResult confirm = MessageBox.Show(
                $"Reset password user {username} ke default?",
                "Konfirmasi",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirm == DialogResult.Yes)
            {
                bool result = poscontroller.ResetPassword(userId, username);

                if (result)
                    MessageBox.Show("Password berhasil direset ke: Username + 123");
                else
                    MessageBox.Show("Gagal reset password");
            }
        }

        private void btnNonaktif_Click(object sender, EventArgs e)
        {
            if (dgvUser.CurrentRow == null)
            {
                MessageBox.Show("Pilih user terlebih dahulu!");
                return;
            }

            int userId = Convert.ToInt32(dgvUser.CurrentRow.Cells["user_id"].Value);
            string nama = dgvUser.CurrentRow.Cells["nama"].Value.ToString();
            string status = dgvUser.CurrentRow.Cells["status"].Value.ToString();

            // Tentukan status baru
            string newStatus = status == "active" ? "inactive" : "active";

            DialogResult confirm = MessageBox.Show(
                $"Ubah status user {nama} menjadi {newStatus}?",
                "Konfirmasi",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirm == DialogResult.Yes)
            {
                bool result = poscontroller.ToggleUserStatus(userId, newStatus);

                if (result)
                {
                    MessageBox.Show("Status user berhasil diubah");
                    ShowTable(); // refresh tabel
                }
                else
                {
                    MessageBox.Show("Gagal mengubah status user");
                }
            }
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
