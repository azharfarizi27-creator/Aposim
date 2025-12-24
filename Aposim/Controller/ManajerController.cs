using Aposim.Model;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aposim.Controller
{
    internal class ManajerController
    {

        Connection db = new Connection();

        // Total penjualan hari ini
        public decimal GetPenjualanHariIni()
        {
            using (MySqlConnection conn = db.GetConn())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(
                    "SELECT IFNULL(SUM(total_amount),0) FROM sales WHERE DATE(sale_date)=CURDATE()",
                    conn
                );
                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }

        public decimal GetPenjualanBulanIni()
        {
            using (MySqlConnection conn = db.GetConn())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(
                    @"SELECT IFNULL(SUM(total_amount),0)
                      FROM sales
                      WHERE MONTH(sale_date)=MONTH(CURDATE())
                      AND YEAR(sale_date)=YEAR(CURDATE())",
                    conn
                );
                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }

        public decimal GetProfit()
        {
            using (MySqlConnection conn = db.GetConn())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(
                    @"SELECT
                      (SELECT IFNULL(SUM(total_amount),0) FROM sales) -
                      (SELECT IFNULL(SUM(total_amount),0) FROM purchases)",
                    conn
                );
                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }

        public DataTable GetStokMenipis()
        {
            using (MySqlConnection conn = db.GetConn())
            {
                conn.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(
                    @"SELECT product_code, product_name, stock, min_stock, unit
                      FROM products
                      WHERE stock <= min_stock AND STATUS='active'",
                    conn
                );

                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public decimal GetSaldoKasToko()
        {
            using (MySqlConnection conn = db.GetConn())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(
                    @"SELECT IFNULL(SUM(
                      CASE WHEN type='in' THEN amount ELSE -amount END
                      ),0) FROM cashflows",
                    conn
                );

                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }

        public DataTable GetStok()
        {
            using (var conn = db.GetConn())
            {
                conn.Open();
                string sql = @"
                    SELECT 
                        p.product_code AS 'Kode',
                        p.product_name AS 'Produk',
                        c.category_name AS 'Kategori',
                        p.stock AS 'Stok',
                        p.min_stock AS 'Min Stok'
                    FROM products p
                    LEFT JOIN categories c ON p.category_id = c.category_id
                    WHERE p.status = 'active'
                ";

                MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public DataTable GetLog()
        {
            using (var conn = db.GetConn())
            {
                conn.Open();
                string sql = @"
                    SELECT 
                        u.nama AS 'User',
                        l.action AS 'Aktivitas',
                        l.log_time AS 'Waktu'
                    FROM logs l
                    LEFT JOIN users u ON l.user_id = u.user_id
                    ORDER BY l.log_time DESC
                ";

                MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

    }
}
