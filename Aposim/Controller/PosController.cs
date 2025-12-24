using Aposim.Model;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Aposim.Controller
{
    internal class PosController
    {

        Connection db = new Connection();

        public static string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
        public static (bool success, string nama, string role, int userId) Login(string username, string password)
        {
            Connection db = new Connection();

            using (var conn = db.GetConn())
            {
                conn.Open();

                string passwordHash = HashPassword(password); // 🔐 HASH DI SINI

                string sql = @"
        SELECT u.user_id, u.nama, r.role_name
        FROM users u
        JOIN roles r ON u.role_id = r.role_id
        WHERE u.username = @username
        AND u.password_hash = @password
        AND u.status = 'active'
        ";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", passwordHash); // ✅ HASH

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int userId = Convert.ToInt32(reader["user_id"]);
                        string nama = reader["nama"].ToString();
                        string role = reader["role_name"].ToString();

                        InsertLog(userId, $"User '{nama}' berhasil login sebagai {role}");

                        return (true, nama, role, userId);
                    }
                }
            }

            return (false, "", "", 0);
        }


        ///ini untuk menampilkan data di admin->users
        ///

        public DataTable TampilkanUser(MySqlCommand command)
        {
            DataTable dt = new DataTable();

            using (MySqlConnection conn = db.GetConn())
            {
                command.Connection = conn;
                MySqlDataAdapter da = new MySqlDataAdapter(command);
                da.Fill(dt);
            }

            return dt;
        }



        public DataTable Search(string keyword)
        {
            MySqlCommand cmd = new MySqlCommand(
                @"SELECT u.user_id, u.nama, u.username, r.role_name, u.status
                  FROM users u
                  JOIN roles r ON u.role_id = r.role_id
                  WHERE u.nama LIKE @key OR u.username LIKE @key"
            );
            cmd.Parameters.AddWithValue("@key", "%" + keyword + "%");

            return TampilkanUser(cmd);
        }

        ///ini buat add user baru di admin
        ///

        public bool AddUser(string nama, string username, string roleName)
        {
            using (MySqlConnection conn = new Connection().GetConn())
            {
                conn.Open();

                // 🔒 CEK USERNAME DUPLIKAT
                if (IsUsernameExist(username))
                {
                    MessageBox.Show(
                        "Username sudah digunakan.\nSilakan gunakan username lain.",
                        "Validasi Username",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return false;
                }

                // ambil role_id
                string getRole = "SELECT role_id FROM roles WHERE role_name=@role";
                MySqlCommand cmdRole = new MySqlCommand(getRole, conn);
                cmdRole.Parameters.AddWithValue("@role", roleName);
                int roleId = Convert.ToInt32(cmdRole.ExecuteScalar());

                // password default
                string passwordDefault = username + "123";
                string passwordHash = HashPassword(passwordDefault);

                string sql = @"INSERT INTO users 
                       (nama, username, password_hash, role_id, status)
                       VALUES (@nama, @username, @pass, @role, 'active')";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@nama", nama);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@pass", passwordHash);
                cmd.Parameters.AddWithValue("@role", roleId);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        /// ngambil data dari database role
        public DataTable GetRoles()
        {
            DataTable dt = new DataTable();

            using (MySqlConnection conn = db.GetConn())
            {
                conn.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(
                    "SELECT role_id, role_name FROM roles", conn
                );
                da.Fill(dt);
            }

            return dt;
        }


        ///utuk update data ures
        ///
        public bool UpdateUser(int id, string nama, string username, string email, string noHp)
        {
            using (MySqlConnection conn = db.GetConn())
            {
                conn.Open();

                // 1️⃣ CEK USERNAME SUDAH DIPAKAI USER LAIN?
                string cekSql = @"SELECT COUNT(*) 
                      FROM users 
                      WHERE username = @username 
                      AND user_id <> @id";

                using (MySqlCommand cekCmd = new MySqlCommand(cekSql, conn))
                {
                    cekCmd.Parameters.AddWithValue("@username", username);
                    cekCmd.Parameters.AddWithValue("@id", id);

                    int count = Convert.ToInt32(cekCmd.ExecuteScalar());

                    if (count > 0)
                    {
                        // ❌ username sudah dipakai
                        MessageBox.Show(
                            "Username sudah digunakan oleh user lain!",
                            "Validasi Username",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return false;
                    }
                }

                // 2️⃣ UPDATE DATA USER
                string sql = @"UPDATE users 
                   SET nama=@nama, username=@username, email=@email, no_hp=@nohp
                   WHERE user_id=@id";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@nama", nama);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@nohp", noHp);
                    cmd.Parameters.AddWithValue("@id", id);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
        

        ///ini untuk ngereset paswor akun ke defauld
        ///
        public bool ResetPassword(int userId, string username)
        {
            using (MySqlConnection conn = db.GetConn())
            {
                conn.Open();

                string defaultPass = username + "123";
                string hash = HashPassword(defaultPass); // 🔐 HASH

                MySqlCommand cmd = new MySqlCommand(
                    "UPDATE users SET password_hash = @pass WHERE user_id = @id",
                    conn
                );

                cmd.Parameters.AddWithValue("@pass", hash); // ✅ HASH
                cmd.Parameters.AddWithValue("@id", userId);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        /// ini untuk aktif dan non aktifin akun
        /// 
        public bool ToggleUserStatus(int userId, string newStatus)
        {
            using (MySqlConnection conn = db.GetConn())
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(
                    "UPDATE users SET status = @status WHERE user_id = @id",
                    conn
                );

                cmd.Parameters.AddWithValue("@status", newStatus);
                cmd.Parameters.AddWithValue("@id", userId);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static void InsertLog(int userId, string action)
        {
            try
            {
                Connection db = new Connection();
                using (var conn = db.GetConn())
                {
                    conn.Open();

                    string sql = "INSERT INTO logs (user_id, action) VALUES (@userId, @action)";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@action", action);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menyimpan log: " + ex.Message);
            }
        }

        public static void Logout(int userId, string nama, string role)
        {
            InsertLog(userId, $"User '{nama}' logout dari role {role}");
        }


        public bool GantiPassword(int userId, string passLamaHash, string passBaruHash)
        {
            using (MySqlConnection conn = new Connection().GetConn())
            {
                conn.Open();

                string cek = @"SELECT COUNT(*) 
                               FROM users 
                               WHERE user_id=@id 
                               AND password_hash=@pass";

                MySqlCommand cmdCek = new MySqlCommand(cek, conn);
                cmdCek.Parameters.AddWithValue("@id", userId);
                cmdCek.Parameters.AddWithValue("@pass", passLamaHash);

                int valid = Convert.ToInt32(cmdCek.ExecuteScalar());
                if (valid == 0)
                    return false;

                string update = @"UPDATE users 
                                  SET password_hash=@newpass 
                                  WHERE user_id=@id";

                MySqlCommand cmdUpdate = new MySqlCommand(update, conn);
                cmdUpdate.Parameters.AddWithValue("@newpass", passBaruHash);
                cmdUpdate.Parameters.AddWithValue("@id", userId);
                cmdUpdate.ExecuteNonQuery();

                return true;
            }
        }

        bool IsUsernameExist(string username)
        {
            using (MySqlConnection conn = db.GetConn())
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(
                    "SELECT COUNT(*) FROM users WHERE username = @username",
                    conn
                );

                cmd.Parameters.AddWithValue("@username", username);

                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }




    }
}
