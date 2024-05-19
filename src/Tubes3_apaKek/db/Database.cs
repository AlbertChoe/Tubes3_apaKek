using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Windows; // Tambahkan ini
using Tubes3_apaKek.Models;

namespace Tubes3_apaKek.DataAccess
{
    public static class Database
    {
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        public static void TestConnection()
        {
            using (var connection = GetConnection())
            {
                try
                {
                    connection.Open();
                    MessageBox.Show("Connection to the database was successful!"); // Pastikan menggunakan System.Windows.MessageBox
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error connecting to the database: " + ex.Message);
                }
            }
        }

        public static List<string> GetAllFingerprintPaths()
        {
            List<string> paths = new List<string>();
            using (var connection = GetConnection())
            {
                try
                {
                    connection.Open();
                    var query = "SELECT berkas_citra FROM sidik_jari";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                paths.Add(reader["berkas_citra"].ToString());
                            }
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Failed to retrieve fingerprint paths: " + ex.Message);
                }
            }
            return paths;
        }

        public static string GetRealNameByPath(string path)
        {
            string realName = string.Empty;
            using (var connection = GetConnection())
            {
                try
                {
                    connection.Open();
                    var query = "SELECT nama FROM sidik_jari WHERE berkas_citra = @Path LIMIT 1";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Path", path);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                realName = reader["nama"].ToString();
                            }
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Failed to retrieve real name: " + ex.Message);
                }
            }
            return realName;
        }

        public static List<string> GetAllAlayNames()
        {
            List<string> alayNames = new List<string>();
            using (var connection = GetConnection())
            {
                try
                {
                    connection.Open();
                    var query = "SELECT nama FROM biodata";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                alayNames.Add(reader["nama"].ToString());
                            }
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Failed to retrieve alay names: " + ex.Message);
                }
            }
            return alayNames;
        }

        public static Biodata GetBiodataByAlayName(string alayName)
        {
            Biodata biodata = null;
            using (var connection = GetConnection())
            {
                try
                {
                    connection.Open();
                    var query = "SELECT * FROM biodata WHERE nama = @AlayName";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@AlayName", alayName);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                biodata = new Biodata
                                {
                                    NIK = reader["NIK"].ToString(),
                                    Nama = reader["nama"].ToString(),
                                    TempatLahir = reader["tempat_lahir"].ToString(),
                                    TanggalLahir = Convert.ToDateTime(reader["tanggal_lahir"]),
                                    JenisKelamin = reader["jenis_kelamin"].ToString(),
                                    GolonganDarah = reader["golongan_darah"].ToString(),
                                    Alamat = reader["alamat"].ToString(),
                                    Agama = reader["agama"].ToString(),
                                    StatusPerkawinan = reader["status_perkawinan"].ToString(),
                                    Pekerjaan = reader["pekerjaan"].ToString(),
                                    Kewarganegaraan = reader["kewarganegaraan"].ToString()
                                };
                            }
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Failed to retrieve biodata: " + ex.Message);
                }
            }
            return biodata;
        }



    }
}
