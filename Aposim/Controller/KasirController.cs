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
    internal class KasirController
    {

        private Connection db = new Connection();

        public DataTable GetProductByCode(string code)
        {
            using (MySqlConnection conn = db.GetConn())
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(
                    @"SELECT 
                    product_id,
                    product_code,
                    product_name,
                    selling_price,
                    stock
                  FROM products
                  WHERE product_code = @code
                  AND status = 'active'",
                    conn
                );

                cmd.Parameters.AddWithValue("@code", code);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }

        public int SaveSale(decimal total, string paymentMethod, decimal paid, decimal change, int userId)
        {
            using (MySqlConnection conn = db.GetConn())
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(
                    @"INSERT INTO sales
              VALUES (CONCAT('INV-', DATE_FORMAT(NOW(), '%Y%m%d%H%i%s')),
                      NOW(), @total, @method, @paid, @change, @user);
              SELECT LAST_INSERT_ID();",
                    conn
                );

                cmd.Parameters.AddWithValue("@total", total);
                cmd.Parameters.AddWithValue("@method", paymentMethod);
                cmd.Parameters.AddWithValue("@paid", paid);
                cmd.Parameters.AddWithValue("@change", change);
                cmd.Parameters.AddWithValue("@user", userId);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }


        public void ReduceStock(int productId, int qty, MySqlConnection conn, MySqlTransaction trx)
        {
            MySqlCommand cmd = new MySqlCommand(@"
        UPDATE products
        SET stock = stock - @qty
        WHERE product_id = @pid
    ", conn, trx);

            cmd.Parameters.AddWithValue("@qty", qty);
            cmd.Parameters.AddWithValue("@pid", productId);

            cmd.ExecuteNonQuery();
        }

        public int GetStock(int productId, MySqlConnection conn, MySqlTransaction trx)
        {
            MySqlCommand cmd = new MySqlCommand(
                "SELECT stock FROM products WHERE product_id = @pid",
                conn, trx
            );
            cmd.Parameters.AddWithValue("@pid", productId);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }


        public void SaveSaleWithItems(


            DataGridView dgv,
            decimal total,
            string paymentMethod,
            decimal paid,
            decimal change,
            int userId,
            int? customerId
        )
        {
            using (MySqlConnection conn = db.GetConn())
            {
                conn.Open();
                MySqlTransaction trx = conn.BeginTransaction();

                try
                {
                    // 1️⃣ insert sales
                    MySqlCommand cmdSale = new MySqlCommand(@"
                        INSERT INTO sales
        (invoice_number, sale_date, total_amount, payment_method,
     paid_amount, change_amount, created_by, customer_id)
    VALUES (
        CONCAT('INV-', DATE_FORMAT(NOW(), '%Y%m%d%H%i%s')),
        NOW(), @total, @method, @paid, @change, @user, @customer
    );
    SELECT LAST_INSERT_ID();
", conn, trx);

                    cmdSale.Parameters.AddWithValue("@total", total);
                    cmdSale.Parameters.AddWithValue("@method", paymentMethod);
                    cmdSale.Parameters.AddWithValue("@paid", paid);
                    cmdSale.Parameters.AddWithValue("@change", change);
                    cmdSale.Parameters.AddWithValue("@user", userId);

                    // ⬇️ TAMBAHKAN INI
                    if (customerId.HasValue)
                        cmdSale.Parameters.AddWithValue("@customer", customerId.Value);
                    else
                        cmdSale.Parameters.AddWithValue("@customer", DBNull.Value);

                    int saleId = Convert.ToInt32(cmdSale.ExecuteScalar());

                    // 2️⃣ loop item
                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        if (row.IsNewRow) continue;

                        int productId = Convert.ToInt32(row.Cells["product_id"].Value);
                        int qty = Convert.ToInt32(row.Cells["qty"].Value);
                        decimal price = Convert.ToDecimal(row.Cells["price"].Value);
                        decimal subtotal = Convert.ToDecimal(row.Cells["subtotal"].Value);

                        // 🔍 cek stok
                        int stock = GetStock(productId, conn, trx);
                        if (stock < qty)
                            throw new Exception("Stok tidak cukup");

                        // insert sale_items
                        MySqlCommand cmdItem = new MySqlCommand(@"
                    INSERT INTO sale_items
                    (sale_id, product_id, quantity, price, subtotal)
                    VALUES (@sid, @pid, @qty, @price, @subtotal)
                ", conn, trx);

                        cmdItem.Parameters.AddWithValue("@sid", saleId);
                        cmdItem.Parameters.AddWithValue("@pid", productId);
                        cmdItem.Parameters.AddWithValue("@qty", qty);
                        cmdItem.Parameters.AddWithValue("@price", price);
                        cmdItem.Parameters.AddWithValue("@subtotal", subtotal);
                        cmdItem.ExecuteNonQuery();

                        // kurangi stok
                        ReduceStock(productId, qty, conn, trx);
                    }

                    // 3️⃣ cashflow (kas masuk)
                    MySqlCommand cmdCash = new MySqlCommand(@"
                INSERT INTO cashflows
                (type, description, amount, created_by)
                VALUES ('in', @desc, @amount, @user)
            ", conn, trx);

                    cmdCash.Parameters.AddWithValue(
                        "@desc",
                        "Penjualan - " + paymentMethod.ToUpper()
                    );
                    cmdCash.Parameters.AddWithValue("@amount", total);
                    cmdCash.Parameters.AddWithValue("@user", userId);
                    cmdCash.ExecuteNonQuery();

                    trx.Commit();
                }
                catch
                {
                    trx.Rollback();
                    throw;
                }
            }
        }
        ///untuk kurangi stok
        ///




    }

}
