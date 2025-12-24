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
    internal class SettingController
    {

        Connection db = new Connection();

        public DataTable GetSetting()
        {
            using (MySqlConnection conn = db.GetConn())
            {
                conn.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(
                    "SELECT * FROM settings WHERE setting_id = 1 LIMIT 1",
                    conn
                );

                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public bool SaveSetting(
            string name,
            string address,
            string phone,
            string logo,
            string npwp
        )
        {
            using (MySqlConnection conn = db.GetConn())
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(@"
            UPDATE settings SET
                store_name = @name,
                store_address = @address,
                store_phone = @phone,
                store_logo = @logo,
                npwp = @npwp
            WHERE setting_id = 1
        ", conn);

                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@address", address);
                cmd.Parameters.AddWithValue("@phone", phone);
                cmd.Parameters.AddWithValue("@logo",
                string.IsNullOrEmpty(logo) ? DBNull.Value : (object)logo);
                cmd.Parameters.AddWithValue("@npwp", npwp);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool HapusLogo()
        {
            using (MySqlConnection conn = db.GetConn())
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(
                    "UPDATE settings SET store_logo = NULL WHERE setting_id = 1",
                    conn
                );

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public string GetStoreName()
        {
            using (MySqlConnection conn = new Connection().GetConn())
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(
                    "SELECT store_name FROM settings LIMIT 1",
                    conn
                );

                object result = cmd.ExecuteScalar();
                return result == null ? "TOKO" : result.ToString();
            }
        }


    }
}
