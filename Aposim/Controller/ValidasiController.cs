using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aposim.Controller
{
    internal class ValidasiController
    {

        // 🧩 Validasi email: username minimal 5 karakter dan tanpa simbol
        public bool IsEmailValid(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            // Cek format dasar email
            if (!Regex.IsMatch(email, @"^[a-zA-Z0-9]+@[a-zA-Z0-9]+\.[a-zA-Z]{2,}$"))
                return false;

            // Pisahkan bagian sebelum dan sesudah '@'
            string[] parts = email.Split('@');
            if (parts.Length != 2)
                return false;

            string username = parts[0]; // bagian sebelum @

            // Username minimal 5 karakter
            if (username.Length < 5)
                return false;

            // Username tidak boleh mengandung simbol (hanya huruf dan angka)
            if (!Regex.IsMatch(username, @"^[a-zA-Z0-9]+$"))
                return false;

            return true;
        }

        // 🧩 Validasi nomor telepon Indonesia (08xxxxxxxxxx)
        public bool IsPhoneValid(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            // Harus angka semua
            if (!Regex.IsMatch(phone, @"^[0-9]+$"))
                return false;

            // Harus mulai dengan 08
            if (!phone.StartsWith("08"))
                return false;

            // Panjang minimal 10 dan maksimal 13 digit
            if (phone.Length < 10 || phone.Length > 13)
                return false;

            return true;
        }

        // 🧩 Validasi kode: maksimal 5 karakter, tanpa simbol
        public bool IsKodeValid(string kode)
        {
            if (string.IsNullOrWhiteSpace(kode))
                return false;

            // Tidak boleh lebih dari 5 karakter
            if (kode.Length > 6)
                return false;

            // Hanya huruf dan angka (tidak boleh simbol)
            if (!Regex.IsMatch(kode, @"^[a-zA-Z0-9]+$"))
                return false;

            return true;
        }

        // 🧩 Validasi harga barang: tidak boleh 0 atau negatif
        public bool IsHargaValid(string harga)
        {
            if (string.IsNullOrWhiteSpace(harga))
                return false;

            // Pastikan bisa dikonversi ke decimal
            if (!decimal.TryParse(harga, out decimal value))
                return false;

            // Harga harus lebih dari 0
            if (value <= 0)
                return false;

            return true;
        }


        // 🧩 Validasi username: tidak boleh kosong, tidak boleh simbol, minimal 3 karakter
        public bool IsUsernameValid(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            // Minimal 5 karakter
            if (username.Length < 5)
                return false;

            // Hanya huruf dan angka (tanpa simbol atau spasi)
            if (!Regex.IsMatch(username, @"^[a-zA-Z0-9]+$"))
                return false;

            return true;
        }

        // 🧩 Validasi alamat: tidak boleh kosong, minimal 10 karakter, dan hanya karakter umum alamat
        public bool IsAlamatValid(string alamat)
        {
            if (string.IsNullOrWhiteSpace(alamat))
                return false;

            // Minimal 10 karakter
            if (alamat.Length < 10)
                return false;

            // Hanya huruf, angka, spasi, koma, titik, garis miring, dan tanda hubung
            if (!Regex.IsMatch(alamat, @"^[a-zA-Z0-9\s,./-]+$"))
                return false;

            return true;
        }

        // 🧩 Validasi quantity: tidak boleh 0, negatif, atau kosong
        public bool IsQtyValid(string qty)
        {
            if (string.IsNullOrWhiteSpace(qty))
                return false;

            // Pastikan bisa dikonversi ke integer
            if (!int.TryParse(qty, out int value))
                return false;

            // QTY harus lebih dari 0
            if (value <= 0)
                return false;

            return true;
        }

        // 🧩 Validasi umum: cek apakah ada field yang kosong
        public bool ValidateRequiredFields(params (string value, string fieldName)[] fields)
        {
            foreach (var (value, fieldName) in fields)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    System.Windows.Forms.MessageBox.Show($"{fieldName} tidak boleh kosong!",
                        "Validasi", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                    return false;
                }
            }
            return true;
        }

        // 🧩 Validasi password umum: minimal 8 karakter, ada huruf dan angka
        public bool IsPasswordValid(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            // Panjang minimal 8, maksimal 20 karakter
            if (password.Length < 8 || password.Length > 20)
                return false;

            // Harus mengandung setidaknya satu huruf
            if (!Regex.IsMatch(password, @"[A-Za-z]"))
                return false;

            // Harus mengandung setidaknya satu angka
            if (!Regex.IsMatch(password, @"[0-9]"))
                return false;

            // Hanya boleh huruf, angka, dan simbol umum berikut
            if (!Regex.IsMatch(password, @"^[A-Za-z0-9@#\-_!$%^&*]+$"))
                return false;

            return true;
        }

        string HashPassword(string password)
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





    }
}
