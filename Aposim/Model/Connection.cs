using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aposim.Model
{
    internal class Connection
    {

        public MySqlCommand cmd; //Untuk menjalankan perintah SQL (SELECT, INSERT, UPDATE, DELETE).
        public DataSet ds; //Wadah untuk menyimpan data hasil query dari database.
        public MySqlDataAdapter da; //penghubung antara database dan DataTable.

        public MySqlConnection GetConn() //membuat connection ke db
        {
            return new MySqlConnection(
                "server=localhost;uid=root;pwd=;database=posinventory;"
            );
        }
    }
}