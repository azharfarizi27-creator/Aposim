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
    public partial class FormDetailPembelian : Form
    {
        private string invoice;
        private PembelianController pembelianController = new PembelianController();
        public FormDetailPembelian(string invoice)
        {
            InitializeComponent();
            this.invoice = invoice;
        }

        private void FormDetailPembelian_Load(object sender, EventArgs e)
        {
            dgvDetailBarang.DataSource = pembelianController.GetDetailPembelian(invoice);
            lblInvoice.Text = "Invoice: " + invoice;
        }

        private void dgvDetailBarang_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnTutupDetail_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void guna2CirclePictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void lblAppname_Click(object sender, EventArgs e)
        {

        }
    }
}
