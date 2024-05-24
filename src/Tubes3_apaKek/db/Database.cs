﻿using MySql.Data.MySqlClient;
using Services.Tools;
using System.Configuration;
using System.Windows; 
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
                    MessageBox.Show("Connection to the database was successful!"); 
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

        public static Biodata GetBiodataByRealName(string realName)
        {
            string? matchedName = null;

            using var connection = GetConnection();
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
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                            string alayName = reader["nama"].ToString();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

#pragma warning disable CS8604 // Possible null reference argument.
                            if (AlayChecker.IsAlayMatch(alayName, realName))
                            {
                                matchedName = alayName;
                                break;
                            }
#pragma warning restore CS8604 // Possible null reference argument.
                        }
                    }
                }

                if (matchedName != null)
                {
                    return GetBiodataByName(matchedName,realName, connection);
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Failed to retrieve biodata: " + ex.Message);
            }

            return null;
        }

        private static Biodata GetBiodataByName(string name,string realName, MySqlConnection connection)
        {
            Biodata biodata = null;
            var query = "SELECT * FROM biodata WHERE nama = @Name";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", name);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
#pragma warning disable CS8601 // Possible null reference assignment.
                        biodata = new Biodata
                        {
                            NIK = reader["NIK"].ToString(),
                            Nama = realName,
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
#pragma warning restore CS8601 // Possible null reference assignment.
                    }
                }
            }
            return biodata;
        }
    }
}
