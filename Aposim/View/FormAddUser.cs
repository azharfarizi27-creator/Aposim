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
    public partial class FormAddUser : Form
    {
        PosController poscontroller;
        ValidasiController validasicontroller;
        public FormAddUser()
        {
            InitializeComponent();
            poscontroller = new PosController(); // 🔥 WAJIB
            validasicontroller = new ValidasiController();
            LoadRole();
        }

        private void FormAddUser_Load(object sender, EventArgs e)
        {
          
        }

        private void cbRole_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            string nama = txtNama.Text.Trim();
            string username = txtUsername.Text.Trim();
            string role = cbRole.Text.Trim();

            if (!validasicontroller.ValidateRequiredFields( (nama, "Nama"), (username, "Username"), (role, "Role")))
            {
                MessageBox.Show("Nama, Username, Role tidak boleh Kosong!",
                                "Validasi Username", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!validasicontroller.IsUsernameValid(username))
            {
                MessageBox.Show("Username tidak valid!\nMinimal 5 karakter dan tanpa simbol/spasi.",
                                "Validasi Username", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!validasicontroller.IsUsernameValid(nama))
            {
                MessageBox.Show("Nama tidak valid!\nMinimal 5 karakter dan tanpa simbol/spasi.",
                "Validasi Username", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;

            }

            bool result = poscontroller.AddUser(
                txtNama.Text,
                txtUsername.Text,
                cbRole.Text
            );

            if (result)
            {
                MessageBox.Show("User berhasil ditambahkan");
                this.Close();
            }
            else
            {
                MessageBox.Show("Gagal menambahkan user");
            }
        }

        void LoadRole()
        {
            cbRole.DataSource = poscontroller.GetRoles();
            cbRole.DisplayMember = "role_name";
            cbRole.ValueMember = "role_id";
            cbRole.SelectedIndex = -1;
        }

        private void btnKembai_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtNama_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
