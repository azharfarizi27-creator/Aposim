using Aposim.Controller;
using Aposim.Model;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Windows.Forms;




namespace Aposim.View
{

    public partial class FormKasir : Form
    {

        PrintDocument printDoc = new PrintDocument();
        KasirController kasir = new KasirController();
        ValidasiController validasi = new ValidasiController();
        SettingController setting = new SettingController();

        decimal totalBayar = 0;
        int selectedRowIndex = -1;
        int memberPoint = 0;
        decimal diskon = 0;


        public int KasirId { get; set; }
        public string KasirNama { get; set; }
        public string KasirRole { get; set; }

        public FormKasir(int id, string nama, string role)
        {
            InitializeComponent();
            KasirId = id;
            KasirNama = nama;
            KasirRole = role;
        }

        private void FormKasir_Load(object sender, EventArgs e)
        {
            btnBayar.Enabled = false;
            dgvTransaksi.Columns.Clear();
            txtQty.Text = "1";
            LoadCustomer();


            dgvTransaksi.Columns.Add("product_id", "ID");
            dgvTransaksi.Columns.Add("product_code", "Kode");
            dgvTransaksi.Columns.Add("product_name", "Nama");
            dgvTransaksi.Columns.Add("price", "Harga");
            dgvTransaksi.Columns.Add("qty", "Qty");
            dgvTransaksi.Columns.Add("subtotal", "Subtotal");

            dgvTransaksi.Columns["product_id"].Visible = false;


            cbMetodeBayar.Items.Add("Cash");
            cbMetodeBayar.Items.Add("QRIS");
            cbMetodeBayar.Items.Add("Debit");
            cbMetodeBayar.SelectedIndex = 0;

            printDoc.PrintPage += PrintStruk;

        }
        private bool AdaItemTransaksi()
        {
            return dgvTransaksi.Rows
                .Cast<DataGridViewRow>()
                .Any(r => !r.IsNewRow);
        }

        void UpdateButtonBayar()
        {
            btnBayar.Enabled = dgvTransaksi.Rows.Count > 0;
        }

        void LoadCustomer()
        {
            using (MySqlConnection conn = new Connection().GetConn())
            {
                conn.Open();

                MySqlDataAdapter da = new MySqlDataAdapter(
                    "SELECT customer_id, customer_name FROM customers",
                    conn
                );

                DataTable dt = new DataTable();
                da.Fill(dt);

                // customer umum
                DataRow row = dt.NewRow();
                row["customer_id"] = DBNull.Value;
                row["customer_name"] = "Umum";
                dt.Rows.InsertAt(row, 0);

                cbCustomer.DataSource = dt;
                cbCustomer.DisplayMember = "customer_name";
                cbCustomer.ValueMember = "customer_id";
            }
        }


        private void btnTambah_Click(object sender, EventArgs e)
        {

            string kode = txtKodeBarang.Text.Trim();
            string qtyStr = txtQty.Text.Trim();

            if (!validasi.ValidateRequiredFields(
                (kode, "Kode Barang"),
                (qtyStr, "Qty")
            ))
                return;

            if (!validasi.IsKodeValid(kode))
            {
                MessageBox.Show("Kode barang tidak valid!\nHanya huruf/angka, maksimal 5 karakter, tanpa simbol.",
                "Validasi Kode", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            DataTable dt = kasir.GetProductByCode(txtKodeBarang.Text);

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("Produk tidak ditemukan");
                return;
            }

            DataRow row = dt.Rows[0];

            int productId = Convert.ToInt32(row["product_id"]);
            string code = row["product_code"].ToString();
            string name = row["product_name"].ToString();
            decimal price = Convert.ToDecimal(row["selling_price"]);

            if (!validasi.IsQtyValid(qtyStr))
            {
                MessageBox.Show("Qty tidak valid!\nMasukkan angka lebih dari 0.",
                                "Validasi Qty", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtQty.Text = "1";
                txtQty.Focus();
                return;
            }
            int qty = Convert.ToInt32(qtyStr);
            decimal subtotal = price * qty;

            dgvTransaksi.Rows.Add(
                productId,
                code,
                name,
                price,
                qty,
                subtotal
            );
            HitungTotal();
            UpdateButtonBayar();


            txtKodeBarang.Clear();
            txtQty.Text = "1";
            txtKodeBarang.Focus();
        }



        private void HitungTotal()
        {
            decimal subtotal = 0;

            foreach (DataGridViewRow row in dgvTransaksi.Rows)
            {
                if (row.IsNewRow) continue;
                subtotal += Convert.ToDecimal(row.Cells["subtotal"].Value);
            }

            //  RESET DISKON
            diskon = 0;

            //  LOGIKA DISKON MEMBER
            if (memberPoint >= 10)
            {
                int kelipatan = memberPoint / 10;
                diskon = Math.Min(kelipatan * 0.05m, 0.20m);
            }

            decimal potongan = subtotal * diskon;
            totalBayar = subtotal - potongan;

            lblTotal.Text = totalBayar.ToString("N0");

            // 🏷 TAMPILKAN DISKON
            lblDiskon.Text = diskon > 0
                ? $"Diskon {(diskon * 100)}% (-{potongan:N0})"
                : "Diskon 0%";
        }

        private void lblKembalian_Click(object sender, EventArgs e)
        {

        }

        private void txtBayar_TextChanged(object sender, EventArgs e)
        {
            if (cbMetodeBayar.Text.Trim().ToLower() != "cash")
            {
                lblKembalian.Text = "0";
                return;
            }

            if (decimal.TryParse(txtBayar.Text, out decimal uang))
            {
                decimal kembali = uang - totalBayar;
                lblKembalian.Text = kembali > 0 ? kembali.ToString("N0") : "0";
            }
            else
            {
                lblKembalian.Text = "0";
            }
            //if (dgvTransaksi.Rows.Count == 0)
            //    return;

            //decimal bayar, kembalian;

            //if (cbMetodeBayar.Text == "Cash")
            //{
            //    if (string.IsNullOrWhiteSpace(txtBayar.Text))
            //    {
            //        MessageBox.Show("Masukkan uang bayar");
            //        return;
            //    }

            //    bayar = Convert.ToDecimal(txtBayar.Text);
            //    if (bayar < totalBayar)
            //    {
            //        MessageBox.Show("Uang kurang");
            //        return;
            //    }
            //    kembalian = bayar - totalBayar;
            //}
            //else
            //{
            //    bayar = totalBayar;
            //    kembalian = 0;
            //}

            //int userId = KasirId;

            //try
            //{
            //    int? customerId =
            //        cbCustomer.SelectedValue == DBNull.Value
            //        ? (int?)null
            //        : Convert.ToInt32(cbCustomer.SelectedValue);

            //    kasir.SaveSaleWithItems(
            //        dgvTransaksi,
            //        totalBayar,
            //        cbMetodeBayar.Text.ToLower(),
            //        bayar,
            //        kembalian,
            //        userId,
            //        customerId
            //    );

            //    // 🔔 NOTIF + PILIHAN CETAK
            //    DialogResult res = MessageBox.Show(
            //        "Pembayaran berhasil!\n\nCetak struk?",
            //        "Sukses",
            //        MessageBoxButtons.YesNo,
            //        MessageBoxIcon.Information
            //    );

            //    if (res == DialogResult.Yes)
            //    {
            //        printDoc.PrinterSettings.PrinterName = "Microsoft Print to PDF";
            //        printDoc.Print();
            //    }

            //    ResetKasir();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message, "Error");
            //}
        }

        private void cbMetodeBayar_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbMetodeBayar.Text != "Cash")
            {
                txtBayar.Text = lblTotal.Text;
                txtBayar.Enabled = false;
                lblKembalian.Text = "0";
            }
            else
            {
                txtBayar.Clear();
                txtBayar.Enabled = true;
            }
        }

        private void btnBayar_Click(object sender, EventArgs e)
        {

            if (!AdaItemTransaksi())
            {
                MessageBox.Show(
                    "Belum ada item yang dibeli",
                    "Informasi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            string metode = cbMetodeBayar.Text.Trim().ToLower();
            decimal bayar = 0;
            decimal kembalian = 0;

            // 🔔 KONFIRMASI KHUSUS NON-CASH
            if (metode != "cash")
            {
                DialogResult confirm = MessageBox.Show(
                    $"Pastikan pembayaran via {cbMetodeBayar.Text} sudah berhasil.\n\nLanjutkan transaksi?",
                    "Konfirmasi Pembayaran",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirm == DialogResult.No)
                    return;

                bayar = totalBayar;
                kembalian = 0;
            }
            else
            {
                // 💵 CASH WAJIB ISI UANG
                if (string.IsNullOrWhiteSpace(txtBayar.Text))
                {
                    MessageBox.Show(
                        "Masukkan jumlah uang bayar",
                        "Validasi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    txtBayar.Focus();
                    return;
                }

                if (!decimal.TryParse(txtBayar.Text, out bayar))
                {
                    MessageBox.Show("Format uang tidak valid");
                    return;
                }

                if (bayar < totalBayar)
                {
                    MessageBox.Show("Uang kurang");
                    return;
                }

                kembalian = bayar - totalBayar;
            }

            int userId = KasirId;

            try
            {
                int? customerId =
                    cbCustomer.SelectedValue == DBNull.Value
                    ? (int?)null
                    : Convert.ToInt32(cbCustomer.SelectedValue);

                kasir.SaveSaleWithItems(
                    dgvTransaksi,
                    totalBayar,
                    metode,
                    bayar,
                    kembalian,
                    userId,
                    customerId
                );
                if (customerId != null)
                {
                    int poinBaru = (int)(totalBayar / 10000); // 10rb = 1 poin

                    using (MySqlConnection conn = new Connection().GetConn())
                    {
                        conn.Open();
                        MySqlCommand cmd = new MySqlCommand(
                            "UPDATE customers SET member_points = member_points + @poin WHERE customer_id=@id",
                            conn
                        );
                        cmd.Parameters.AddWithValue("@poin", poinBaru);
                        cmd.Parameters.AddWithValue("@id", customerId);
                        cmd.ExecuteNonQuery();
                    }
                }
                DialogResult res = MessageBox.Show(
                    "Pembayaran berhasil!\n\nCetak struk?",
                    "Sukses",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information
                );

                if (res == DialogResult.Yes)
                {
                    printDoc.PrinterSettings.PrinterName = "Microsoft Print to PDF";
                    printDoc.Print();
                }

                ResetKasir();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }


        void ResetKasir()
        {
            dgvTransaksi.Rows.Clear();
            txtBayar.Clear();
            lblTotal.Text = "0";
            lblKembalian.Text = "0";
            totalBayar = 0;
            UpdateButtonBayar();
        }
        private void dgvTransaksi_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            dgvTransaksi.ReadOnly = true;
            if (e.RowIndex >= 0)
            {
                selectedRowIndex = e.RowIndex;
            }

        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {


        }

        private DataTable GetItemTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("product_id", typeof(int));
            dt.Columns.Add("qty", typeof(int));
            dt.Columns.Add("price", typeof(decimal));
            dt.Columns.Add("subtotal", typeof(decimal));

            foreach (DataGridViewRow row in dgvTransaksi.Rows)
            {
                if (row.IsNewRow) continue;
                if (row.Cells["product_id"].Value == null) continue;

                dt.Rows.Add(
                    Convert.ToInt32(row.Cells["product_id"].Value),
                    Convert.ToInt32(row.Cells["qty"].Value),
                    Convert.ToDecimal(row.Cells["price"].Value),
                    Convert.ToDecimal(row.Cells["subtotal"].Value)
                );
            }

            return dt;
        }

        private void btnBatal_Click(object sender, EventArgs e)
        {
            if (dgvTransaksi.Rows.Count == 0)
            {
                MessageBox.Show("Tidak ada transaksi untuk dibatalkan");
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Yakin ingin membatalkan transaksi?",
                "Konfirmasi",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirm == DialogResult.Yes)
            {
                ResetKasir();
                MessageBox.Show("Transaksi dibatalkan");
            }
        }

        private void txtKodeBarang_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnBatalProduk_Click(object sender, EventArgs e)
        {
            if (selectedRowIndex < 0)
            {
                MessageBox.Show("Pilih item yang ingin dibatalkan");
                return;
            }

            dgvTransaksi.Rows.RemoveAt(selectedRowIndex);
            selectedRowIndex = -1;

            HitungTotal();
        }

        //private void guna2Button1_Click(object sender, EventArgs e)
        //{
        //    printDoc.PrinterSettings.PrinterName = "Microsoft Print to PDF";
        //    printDoc.Print();

        //    ResetKasir();           // reset setelah cetak
        //    //btnStruk.Enabled = false;                                      // 
        //}

        ///print to pdf
        ///
        private void PrintStruk(object sender, PrintPageEventArgs e)
        {
            // Font
            Font fontJudul = new Font("Arial", 10, FontStyle.Bold);
            Font fontIsi = new Font("Arial", 8);
            int y = 10;

            // 🏪 Ambil info toko dari tabel settings
            DataTable dtSetting = setting.GetSetting();
            string namaToko = "TOKO APOSIM";
            string alamatToko = "";
            string telpToko = "";

            if (dtSetting.Rows.Count > 0)
            {
                DataRow r = dtSetting.Rows[0];
                namaToko = r["store_name"].ToString();
                alamatToko = r["store_address"].ToString();
                telpToko = r["store_phone"].ToString();
            }

            // 🧾 Header Struk
            e.Graphics.DrawString(namaToko, fontJudul, Brushes.Black, 10, y);
            y += 15;
            if (!string.IsNullOrEmpty(alamatToko))
            {
                e.Graphics.DrawString(alamatToko, fontIsi, Brushes.Black, 10, y);
                y += 12;
            }
            if (!string.IsNullOrEmpty(telpToko))
            {
                e.Graphics.DrawString("Telp: " + telpToko, fontIsi, Brushes.Black, 10, y);
                y += 12;
            }

            y += 5;
            e.Graphics.DrawString("--------------------------------", fontIsi, Brushes.Black, 10, y);
            y += 15;

            e.Graphics.DrawString(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), fontIsi, Brushes.Black, 10, y);
            y += 15;
            e.Graphics.DrawString("--------------------------------", fontIsi, Brushes.Black, 10, y);
            y += 15;

            // 💰 Daftar item
            foreach (DataGridViewRow row in dgvTransaksi.Rows)
            {
                if (row.IsNewRow) continue;

                string nama = row.Cells["product_name"].Value.ToString();
                int qty = Convert.ToInt32(row.Cells["qty"].Value);
                decimal harga = Convert.ToDecimal(row.Cells["price"].Value);
                decimal subtotal = Convert.ToDecimal(row.Cells["subtotal"].Value);

                e.Graphics.DrawString(nama, fontIsi, Brushes.Black, 10, y);
                y += 12;
                e.Graphics.DrawString($"{qty} x {harga:N0} = {subtotal:N0}", fontIsi, Brushes.Black, 10, y);
                y += 15;
            }

            e.Graphics.DrawString("--------------------------------", fontIsi, Brushes.Black, 10, y);
            y += 15;

            // 🔢 Total, bayar, kembalian
            e.Graphics.DrawString($"TOTAL   : {lblTotal.Text}", fontIsi, Brushes.Black, 10, y);
            y += 15;
            e.Graphics.DrawString($"BAYAR   : {txtBayar.Text}", fontIsi, Brushes.Black, 10, y);
            y += 15;
            e.Graphics.DrawString($"KEMBALI : {lblKembalian.Text}", fontIsi, Brushes.Black, 10, y);
            y += 15;

            e.Graphics.DrawString($"METODE  : {cbMetodeBayar.Text}", fontIsi, Brushes.Black, 10, y);
            y += 20;

            if (cbCustomer.SelectedValue != DBNull.Value)
            {
                e.Graphics.DrawString($"Poin Member : {memberPoint}", fontIsi, Brushes.Black, 10, y);
                y += 15;

                e.Graphics.DrawString($"Diskon      : {(diskon * 100)}%", fontIsi, Brushes.Black, 10, y);
                y += 15;
            }


            e.Graphics.DrawString("Terima kasih atas kunjungannya ", fontIsi, Brushes.Black, 10, y);
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            FormDaftarProduk settingKasir = new FormDaftarProduk(KasirId, KasirNama, KasirRole);
            settingKasir.ShowDialog();
            this.Close();
        }

        private void cbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbCustomer.SelectedValue == null ||
         cbCustomer.SelectedValue == DBNull.Value ||
         cbCustomer.SelectedValue is DataRowView)
            {
                memberPoint = 0;
                diskon = 0;
                HitungTotal();
                return;
            }

            int customerId = Convert.ToInt32(cbCustomer.SelectedValue);

            using (MySqlConnection conn = new Connection().GetConn())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(
                    "SELECT member_points FROM customers WHERE customer_id = @id",
                    conn
                );
                cmd.Parameters.AddWithValue("@id", customerId);

                object result = cmd.ExecuteScalar();
                memberPoint = result != null ? Convert.ToInt32(result) : 0;
            }

            HitungTotal();
        }
    }
}
