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
    internal class ReportController
    {


        Connection db = new Connection();

        // Produk
        public DataTable GetProductReport()
        {
            using (var conn = db.GetConn())
            {
                conn.Open();
                string sql = @"SELECT 
    p.product_code   AS 'Kode Produk',
    p.product_name   AS 'Nama Produk',
    c.category_name  AS 'Kategori',
    p.purchase_price AS 'Harga Beli',
    p.selling_price  AS 'Harga Jual',
    p.stock          AS 'Stok',
    p.status         AS 'Status'
FROM products p
LEFT JOIN categories c ON p.category_id = c.category_id";
                MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        // Penjualan / Pembelian
        public DataTable GetSalesReport()
        {
            using (var conn = db.GetConn())
            {
                conn.Open();
                string sql = @"

                    SELECT 
    s.invoice_number AS 'No Invoice',
    c.customer_name AS 'Customer',
    s.sale_date     AS 'Tanggal',
    s.total_amount  AS 'Total',
    s.payment_method AS 'Metode Bayar'
FROM sales s
LEFT JOIN customers c ON s.customer_id = c.customer_id";

                MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public DataTable GetPurchaseReport()
        {
            using (var conn = db.GetConn())
            {
                conn.Open();
                string sql = @"SELECT 
    p.invoice_number AS 'No Invoice',
    s.supplier_name AS 'Supplier',
    p.purchase_date AS 'Tanggal',
    p.total_amount  AS 'Total',
    p.payment_status AS 'Status Bayar'
FROM purchases p
LEFT JOIN suppliers s ON p.supplier_id = s.supplier_id";
                MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        // Kas / Keuangan
        public DataTable GetCashflowReport()
        {
            using (var conn = db.GetConn())
            {
                conn.Open();
                string sql = @"SELECT 
    cashflow_id AS 'ID',
    type        AS 'Tipe',
    description AS 'Keterangan',
    amount      AS 'Jumlah',
    created_at  AS 'Tanggal'
FROM cashflows";
                MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        //tansaksi

        public DataTable GetTransactionReport()
        {
            using (var conn = db.GetConn())
            {
                conn.Open();
                string sql = @"
        SELECT 
            'Penjualan' AS 'Tipe Transaksi',
            s.invoice_number AS 'No Invoice',
            c.customer_name AS 'Customer / Supplier',
            s.sale_date AS 'Tanggal',
            s.total_amount AS 'Total Transaksi'
        FROM sales s
        LEFT JOIN customers c ON s.customer_id = c.customer_id

        UNION ALL

        SELECT 
            'Pembelian' AS 'Tipe Transaksi',
            p.invoice_number AS 'No Invoice',
            sup.supplier_name AS 'Customer / Supplier',
            p.purchase_date AS 'Tanggal',
            p.total_amount AS 'Total Transaksi'
        FROM purchases p
        LEFT JOIN suppliers sup ON p.supplier_id = sup.supplier_id

        ORDER BY Tanggal DESC";

                MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
    }
}
