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
    internal class CustomerController
    {

        Connection db = new Connection();

        // =========================
        // GET ALL CUSTOMER
        // =========================
        public DataTable GetCustomers()
        {
            using (MySqlConnection conn = db.GetConn())
            {
                conn.Open();

                MySqlDataAdapter da = new MySqlDataAdapter(
                    @"SELECT 
                        customer_id,
                        customer_name,
                        phone,
                        email,
                        address,
                        member_points,
                        created_at
                      FROM customers",
                    conn
                );

                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        // =========================
        // ADD CUSTOMER
        // =========================
        public bool AddCustomer(
                   string name,
                   string phone,
                   string email,
                   string address
               )
        {
            using (MySqlConnection conn = db.GetConn())
            {
                conn.Open();

                // 🔍 CEK NAMA CUSTOMER
                string cekNamaSql = "SELECT COUNT(*) FROM customers WHERE customer_name = @name";
                using (MySqlCommand cmdNama = new MySqlCommand(cekNamaSql, conn))
                {
                    cmdNama.Parameters.AddWithValue("@name", name);
                    int namaCount = Convert.ToInt32(cmdNama.ExecuteScalar());

                    if (namaCount > 0)
                    {
                        MessageBox.Show(
                            "Nama customer sudah terdaftar!",
                            "Validasi Customer",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return false;
                    }
                }

                // 🔍 CEK EMAIL CUSTOMER
                string cekEmailSql = "SELECT COUNT(*) FROM customers WHERE email = @email";
                using (MySqlCommand cmdEmail = new MySqlCommand(cekEmailSql, conn))
                {
                    cmdEmail.Parameters.AddWithValue("@email", email);
                    int emailCount = Convert.ToInt32(cmdEmail.ExecuteScalar());

                    if (emailCount > 0)
                    {
                        MessageBox.Show(
                            "Email customer sudah terdaftar!",
                            "Validasi Customer",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return false;
                    }
                }

                // ➕ INSERT CUSTOMER
                MySqlCommand cmd = new MySqlCommand(
                    @"INSERT INTO customers
                      (customer_name, phone, email, address)
                      VALUES (@name, @phone, @email, @address)",
                    conn
                );

                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@phone", phone);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@address", address);

                return cmd.ExecuteNonQuery() > 0;
            }
        }
        // =========================
        // UPDATE CUSTOMER
        // =========================
        public bool UpdateCustomer(
                  int id,
                  string name,
                  string phone,
                  string email,
                  string address
              )
        {
            using (MySqlConnection conn = db.GetConn())
            {
                conn.Open();

                // 🔍 CEK NAMA (KECUALI DATA INI)
                string cekNamaSql = @"SELECT COUNT(*) 
                                      FROM customers 
                                      WHERE customer_name = @name
                                      AND customer_id <> @id";

                using (MySqlCommand cmdNama = new MySqlCommand(cekNamaSql, conn))
                {
                    cmdNama.Parameters.AddWithValue("@name", name);
                    cmdNama.Parameters.AddWithValue("@id", id);

                    int namaCount = Convert.ToInt32(cmdNama.ExecuteScalar());
                    if (namaCount > 0)
                    {
                        MessageBox.Show(
                            "Nama customer sudah digunakan customer lain!",
                            "Validasi Customer",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return false;
                    }
                }

                // 🔍 CEK EMAIL (KECUALI DATA INI)
                string cekEmailSql = @"SELECT COUNT(*) 
                                       FROM customers 
                                       WHERE email = @email
                                       AND customer_id <> @id";

                using (MySqlCommand cmdEmail = new MySqlCommand(cekEmailSql, conn))
                {
                    cmdEmail.Parameters.AddWithValue("@email", email);
                    cmdEmail.Parameters.AddWithValue("@id", id);

                    int emailCount = Convert.ToInt32(cmdEmail.ExecuteScalar());
                    if (emailCount > 0)
                    {
                        MessageBox.Show(
                            "Email customer sudah digunakan customer lain!",
                            "Validasi Customer",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return false;
                    }
                }

                // ✏️ UPDATE CUSTOMER
                MySqlCommand cmd = new MySqlCommand(
                    @"UPDATE customers SET
                        customer_name = @name,
                        phone = @phone,
                        email = @email,
                        address = @address
                      WHERE customer_id = @id",
                    conn
                );

                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@phone", phone);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@address", address);

                return cmd.ExecuteNonQuery() > 0;
            }
        }
        // =========================
        // CEK TRANSAKSI
        // =========================
        public bool HasTransaction(int customerId)
        {
            using (MySqlConnection conn = db.GetConn())
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(
                    "SELECT COUNT(*) FROM sales WHERE customer_id = @id",
                    conn
                );

                cmd.Parameters.AddWithValue("@id", customerId);
                int total = Convert.ToInt32(cmd.ExecuteScalar());

                return total > 0;
            }
        }

        // =========================
        // DELETE CUSTOMER (PERMANEN)
        // =========================
        public bool DeleteCustomer(int customerId)
        {
            using (MySqlConnection conn = db.GetConn())
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(
                    "DELETE FROM customers WHERE customer_id = @id",
                    conn
                );

                cmd.Parameters.AddWithValue("@id", customerId);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}
