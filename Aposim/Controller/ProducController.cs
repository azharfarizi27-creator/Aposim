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
    internal class ProducController
    {
        Connection db = new Connection();

        // =========================
        // GET PRODUCTS
        // =========================
        public DataTable GetProducts()
        {
            using (MySqlConnection conn = db.GetConn())
            {
                conn.Open();

                MySqlDataAdapter da = new MySqlDataAdapter(
                    @"SELECT 
    p.product_id,
    p.product_code,
    p.product_name,
    p.category_id,
    c.category_name,
    p.selling_price,
    p.stock,
    p.min_stock,
    p.unit,
    p.status
FROM products p
LEFT JOIN categories c ON p.category_id = c.category_id",
                    conn
                );

                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        // =========================
        // GET CATEGORIES
        // =========================
        public DataTable GetCategories()
        {
            using (MySqlConnection conn = db.GetConn())
            {
                conn.Open();

                MySqlDataAdapter da = new MySqlDataAdapter(
                    "SELECT category_id, category_name FROM categories",
                    conn
                );

                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
        // =========================
        // ADD PRODUCT
        // =========================
        public bool AddProduct(
            string code,
            string name,
            int categoryId,
            decimal sellingPrice,
            string unit
        )
        {
            using (MySqlConnection conn = db.GetConn())
            {
                conn.Open();

                // 🔍 CEK PRODUCT CODE SUDAH ADA?
                string cekSql = "SELECT COUNT(*) FROM products WHERE product_code = @code";
                using (MySqlCommand cekCmd = new MySqlCommand(cekSql, conn))
                {
                    cekCmd.Parameters.AddWithValue("@code", code);
                    int count = Convert.ToInt32(cekCmd.ExecuteScalar());

                    if (count > 0)
                    {
                        MessageBox.Show(
                            "Kode produk sudah digunakan!",
                            "Validasi Produk",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return false;
                    }
                }

                // ➕ INSERT PRODUK
                MySqlCommand cmd = new MySqlCommand(
                    @"INSERT INTO products
              (product_code, product_name, category_id,
               selling_price, stock, min_stock, unit, status)
              VALUES
              (@code, @name, @cat,
               @selling, 0, 1, @unit, 'active')",
                    conn
                );

                cmd.Parameters.AddWithValue("@code", code);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@cat", categoryId);
                cmd.Parameters.AddWithValue("@selling", sellingPrice);
                cmd.Parameters.AddWithValue("@unit", unit);

                return cmd.ExecuteNonQuery() > 0;
            }
        }
        //ini update produk
        ///

        public bool UpdateProduct(
            int id,
            string code,
            string name,
            int categoryId,
            decimal sellingPrice,
            string unit,
            string status
        )
        {
            using (MySqlConnection conn = db.GetConn())
            {
                conn.Open();

                // 🔍 CEK DUPLIKASI CODE (KECUALI PRODUK INI)
                string cekSql = @"SELECT COUNT(*) 
                          FROM products 
                          WHERE product_code = @code 
                          AND product_id <> @id";

                using (MySqlCommand cekCmd = new MySqlCommand(cekSql, conn))
                {
                    cekCmd.Parameters.AddWithValue("@code", code);
                    cekCmd.Parameters.AddWithValue("@id", id);

                    int count = Convert.ToInt32(cekCmd.ExecuteScalar());
                    if (count > 0)
                    {
                        MessageBox.Show(
                            "Kode produk sudah dipakai produk lain!",
                            "Validasi Produk",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return false;
                    }
                }

                // ✏️ UPDATE PRODUK
                MySqlCommand cmd = new MySqlCommand(
                    @"UPDATE products SET
                product_code  = @code,
                product_name  = @name,
                category_id   = @cat,
                selling_price = @selling,
                unit          = @unit,
                status        = @status
              WHERE product_id = @id",
                    conn
                );

                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@code", code);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@cat", categoryId);
                cmd.Parameters.AddWithValue("@selling", sellingPrice);
                cmd.Parameters.AddWithValue("@unit", unit);
                cmd.Parameters.AddWithValue("@status", status);

                return cmd.ExecuteNonQuery() > 0;
            }
        }
        /// ini buat update status dari produk
        /// 

        public bool UpdateProductStatus(int id, string status)
        {
            using (MySqlConnection conn = db.GetConn())
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(
                    "UPDATE products SET status=@status WHERE product_id=@id",
                    conn
                );

                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@status", status);

                return cmd.ExecuteNonQuery() > 0;
            }
        }




    }
}


