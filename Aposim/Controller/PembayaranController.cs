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
    internal class PembayaranController
    {

        public static DataTable GetPembelianBelumLunas()
        {
            Connection db = new Connection();
            DataTable dt = new DataTable();

            using (MySqlConnection conn = db.GetConn())
            {
                conn.Open();
                string query = @"
                SELECT
                    purchase_id,
                    invoice_number,
                    purchase_date,
                    total_amount
                FROM purchases
                WHERE payment_status = 'belum lunas'";

                MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                da.Fill(dt);
            }

            return dt;
        }

        public static void BayarHutang(
            int purchaseId,
            string noFaktur,
            decimal total,
            int userId
        )
        {
            Connection db = new Connection();

            using (MySqlConnection conn = db.GetConn())
            {
                conn.Open();
                MySqlTransaction trx = conn.BeginTransaction();

                try
                {
                    // 1. update purchases
                    string qUpdate = @"
                    UPDATE purchases
                    SET payment_status = 'lunas'
                    WHERE purchase_id = @id";

                    MySqlCommand cmdUpdate = new MySqlCommand(qUpdate, conn, trx);
                    cmdUpdate.Parameters.AddWithValue("@id", purchaseId);
                    cmdUpdate.ExecuteNonQuery();

                    // 2. cashflow
                    string qCash = @"
                    INSERT INTO cashflows
                    (type, description, amount, created_by)
                    VALUES
                    ('out', @desc, @amount, @user)";

                    MySqlCommand cmdCash = new MySqlCommand(qCash, conn, trx);
                    cmdCash.Parameters.AddWithValue("@desc", "Pelunasan " + noFaktur);
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
    }
}
