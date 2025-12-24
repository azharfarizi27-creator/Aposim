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
    internal class SupplierController
    {

        Connection db = new Connection();

        // GET SUPPLIER
        public DataTable GetSuppliers()
        {
            using (MySqlConnection conn = db.GetConn())
            {
                conn.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(
                    "SELECT * FROM suppliers",
                    conn
                );
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        // ADD
        public bool AddSupplier(
                  string name,
                  string contact,
                  string phone,
                  string email,
                  string address
              )
        {
            using (MySqlConnection conn = db.GetConn())
            {
                conn.Open();

                // 🔍 CEK NAMA SUPPLIER
                string cekNamaSql = "SELECT COUNT(*) FROM suppliers WHERE supplier_name = @name";
                using (MySqlCommand cmdNama = new MySqlCommand(cekNamaSql, conn))
                {
                    cmdNama.Parameters.AddWithValue("@name", name);
                    int namaCount = Convert.ToInt32(cmdNama.ExecuteScalar());

                    if (namaCount > 0)
                    {
                        MessageBox.Show(
                            "Nama supplier sudah terdaftar!",
                            "Validasi Supplier",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return false;
                    }
                }

                // 🔍 CEK EMAIL SUPPLIER
                string cekEmailSql = "SELECT COUNT(*) FROM suppliers WHERE email = @email";
                using (MySqlCommand cmdEmail = new MySqlCommand(cekEmailSql, conn))
                {
                    cmdEmail.Parameters.AddWithValue("@email", email);
                    int emailCount = Convert.ToInt32(cmdEmail.ExecuteScalar());

                    if (emailCount > 0)
                    {
                        MessageBox.Show(
                            "Email supplier sudah terdaftar!",
                            "Validasi Supplier",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return false;
                    }
                }

                // ➕ INSERT DATA
                MySqlCommand cmd = new MySqlCommand(
                    @"INSERT INTO suppliers
                      (supplier_name, contact_name, phone, email, address)
                      VALUES
                      (@name, @contact, @phone, @email, @address)",
                    conn
                );

                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@contact", contact);
                cmd.Parameters.AddWithValue("@phone", phone);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@address", address);

                return cmd.ExecuteNonQuery() > 0;
            }
        }
        // UPDATE
        public bool UpdateSupplier(
                  int id,
                  string name,
                  string contact,
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
                                      FROM suppliers 
                                      WHERE supplier_name = @name
                                      AND supplier_id <> @id";

                using (MySqlCommand cmdNama = new MySqlCommand(cekNamaSql, conn))
                {
                    cmdNama.Parameters.AddWithValue("@name", name);
                    cmdNama.Parameters.AddWithValue("@id", id);

                    int namaCount = Convert.ToInt32(cmdNama.ExecuteScalar());
                    if (namaCount > 0)
                    {
                        MessageBox.Show(
                            "Nama supplier sudah digunakan supplier lain!",
                            "Validasi Supplier",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return false;
                    }
                }

                // 🔍 CEK EMAIL (KECUALI DATA INI)
                string cekEmailSql = @"SELECT COUNT(*) 
                                       FROM suppliers 
                                       WHERE email = @email
                                       AND supplier_id <> @id";

                using (MySqlCommand cmdEmail = new MySqlCommand(cekEmailSql, conn))
                {
                    cmdEmail.Parameters.AddWithValue("@email", email);
                    cmdEmail.Parameters.AddWithValue("@id", id);

                    int emailCount = Convert.ToInt32(cmdEmail.ExecuteScalar());
                    if (emailCount > 0)
                    {
                        MessageBox.Show(
                            "Email supplier sudah digunakan supplier lain!",
                            "Validasi Supplier",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return false;
                    }
                }

                // ✏️ UPDATE DATA
                MySqlCommand cmd = new MySqlCommand(
                    @"UPDATE suppliers SET
                        supplier_name = @name,
                        contact_name  = @contact,
                        phone         = @phone,
                        email         = @email,
                        address       = @address
                      WHERE supplier_id = @id",
                    conn
                );

                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@contact", contact);
                cmd.Parameters.AddWithValue("@phone", phone);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@address", address);

                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}

