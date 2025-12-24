using Aposim.Model;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aposim.Controller
{
    internal class PembelianController
    {
        public static void SimpanPembelian(
                string noFaktur,
                int supplierId,
                DateTime tanggal,
                string statusBayar,
                int userId,
                DataTable detailBarang,
                decimal totalPembelian
            )
        {
            Connection db = new Connection();

            using (MySqlConnection conn = db.GetConn())
            {
                conn.Open();
                MySqlTransaction trx = conn.BeginTransaction();

                try
                {
                    // =============================
                    // 1. INSERT purchases
                    // =============================
                    string qPembelian = @"
                    INSERT INTO purchases
                    (supplier_id, invoice_number, purchase_date, total_amount, payment_status, created_by)
                    VALUES
                    (@supplier, @invoice, @tanggal, @total, @status, @user)";

                    MySqlCommand cmdPembelian = new MySqlCommand(qPembelian, conn, trx);
                    cmdPembelian.Parameters.AddWithValue("@supplier", supplierId);
                    cmdPembelian.Parameters.AddWithValue("@invoice", noFaktur);
                    cmdPembelian.Parameters.AddWithValue("@tanggal", tanggal);
                    cmdPembelian.Parameters.AddWithValue("@total", totalPembelian);
                    cmdPembelian.Parameters.AddWithValue("@status", statusBayar);
                    cmdPembelian.Parameters.AddWithValue("@user", userId);
                    cmdPembelian.ExecuteNonQuery();

                    long purchaseId = cmdPembelian.LastInsertedId;

                    // =============================
                    // 2. INSERT purchase_items + update stock
                    // =============================
                    foreach (DataRow row in detailBarang.Rows)
                    {
                        int productId = Convert.ToInt32(row["id_barang"]);
                        int qty = Convert.ToInt32(row["jumlah"]);
                        decimal harga = Convert.ToDecimal(row["harga"]);
                        decimal subtotal = Convert.ToDecimal(row["subtotal"]);

                        // purchase_items
                        string qItem = @"
                        INSERT INTO purchase_items
                        (purchase_id, product_id, quantity, price, subtotal)
                        VALUES
                        (@purchase, @product, @qty, @price, @subtotal)";

                        MySqlCommand cmdItem = new MySqlCommand(qItem, conn, trx);
                        cmdItem.Parameters.AddWithValue("@purchase", purchaseId);
                        cmdItem.Parameters.AddWithValue("@product", productId);
                        cmdItem.Parameters.AddWithValue("@qty", qty);
                        cmdItem.Parameters.AddWithValue("@price", harga);
                        cmdItem.Parameters.AddWithValue("@subtotal", subtotal);
                        cmdItem.ExecuteNonQuery();

                        // update products.stock
                        string qStock = @"
UPDATE products
SET stock = stock + @qty
WHERE product_id = @product";

                        MySqlCommand cmdStock = new MySqlCommand(qStock, conn, trx);
                        cmdStock.Parameters.AddWithValue("@qty", qty);
                        cmdStock.Parameters.AddWithValue("@product", productId);
                        cmdStock.ExecuteNonQuery();

                        string qUpdateHarga = @"
UPDATE products
SET purchase_price = @harga
WHERE product_id = @product";

                        MySqlCommand cmdHarga = new MySqlCommand(qUpdateHarga, conn, trx);
                        cmdHarga.Parameters.AddWithValue("@harga", harga);
                        cmdHarga.Parameters.AddWithValue("@product", productId);
                        cmdHarga.ExecuteNonQuery();

                        // stock_adjustments
                        string qAdjust = @"
                        INSERT INTO stock_adjustments
                        (product_id, adjustment_type, quantity, note, created_by)
                        VALUES
                        (@product, 'in', @qty, 'Pembelian barang', @user)";

                        MySqlCommand cmdAdjust = new MySqlCommand(qAdjust, conn, trx);
                        cmdAdjust.Parameters.AddWithValue("@product", productId);
                        cmdAdjust.Parameters.AddWithValue("@qty", qty);
                        cmdAdjust.Parameters.AddWithValue("@user", userId);
                        cmdAdjust.ExecuteNonQuery();
                    }

                    // =============================
                    // 3. cashflows (jika lunas)
                    // =============================
                    if (statusBayar == "lunas")
                    {
                        string qCash = @"
                        INSERT INTO cashflows
                        (type, description, amount, created_by)
                        VALUES
                        ('out', @desc, @amount, @user)";

                        MySqlCommand cmdCash = new MySqlCommand(qCash, conn, trx);
                        cmdCash.Parameters.AddWithValue("@desc", "Pembelian " + noFaktur);
                        cmdCash.Parameters.AddWithValue("@amount", totalPembelian);
                        cmdCash.Parameters.AddWithValue("@user", userId);
                        cmdCash.ExecuteNonQuery();
                    }

                    trx.Commit();
                }
                catch (Exception ex)
                {
                    trx.Rollback();
                    throw new Exception("Gagal menyimpan pembelian: " + ex.Message);
                }
            }
        }


        // ==============================
        // 1️⃣ Ambil semua daftar pembelian
        // ==============================
        public DataTable GetDaftarPembelian()
        {
            Connection db = new Connection();
            DataTable dt = new DataTable();

            try
            {
                using (MySqlConnection conn = db.GetConn())
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            p.invoice_number AS 'No Faktur',
                            s.supplier_name AS 'Nama Supplier',
                            p.purchase_date AS 'Tanggal Pembelian',
                            p.total_amount AS 'Total',
                            p.payment_status AS 'Status Pembayaran'
                        FROM purchases p
                        LEFT JOIN suppliers s ON p.supplier_id = s.supplier_id
                        ORDER BY p.purchase_date DESC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal mengambil daftar pembelian: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }


        public DataTable GetDetailPembelian(string invoiceNumber)
        {
            Connection db = new Connection();
            DataTable dt = new DataTable();

            try
            {
                using (MySqlConnection conn = db.GetConn())
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            pr.product_code AS 'Kode Barang',
                            pr.product_name AS 'Nama Barang',
                            pi.quantity AS 'Jumlah',
                            pi.price AS 'Harga Beli',
                            pi.subtotal AS 'Subtotal'
                        FROM purchases p
                        JOIN purchase_items pi ON p.purchase_id = pi.purchase_id
                        JOIN products pr ON pi.product_id = pr.product_id
                        WHERE p.invoice_number = @invoice";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@invoice", invoiceNumber);
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal mengambil detail pembelian: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }
    

    public DataTable CariPembelian(string keyword)
        {
            Connection db = new Connection();
            DataTable dt = new DataTable();

            try
            {
                using (MySqlConnection conn = db.GetConn())
                {
                    conn.Open();

                    string query = @"
                SELECT 
                    p.invoice_number AS 'No Faktur',
                    s.supplier_name AS 'Nama Supplier',
                    p.purchase_date AS 'Tanggal Pembelian',
                    p.total_amount AS 'Total',
                    p.payment_status AS 'Status Pembayaran'
                FROM purchases p
                LEFT JOIN suppliers s ON p.supplier_id = s.supplier_id
                WHERE p.invoice_number LIKE @keyword
                ORDER BY p.purchase_date DESC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@keyword", "%" + keyword + "%");
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal mencari data pembelian: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }

    }
}

