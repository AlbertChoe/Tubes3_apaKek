using MySql.Data.MySqlClient;
using Services.Tools;
using System.Configuration;
using System.Windows;
using Services.Hash;
using Tubes3_apaKek.Models;

namespace Tubes3_apaKek.DataAccess
{
    public static class Database
    {
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

        private static List<string> allPath = new List<string>();
        private static Blowfish hash;
        public static MySqlConnection GetConnection()
        {
 
            return new MySqlConnection(connectionString);
        }

        public static Blowfish GetHash()
        {
            if (hash != null)
            {
                return hash;
            }
            
            hash = new Blowfish("aabb09182736ccdd");
            return hash;
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
            if (allPath.Count() > 0) {
                return allPath;
            }
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

                                string citra = GetHash().Decrypt(reader["berkas_citra"].ToString());
                                // Console.WriteLine(citra);
                                paths.Add(citra);
                            }
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Failed to retrieve fingerprint paths: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }

            allPath = paths;
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
                        command.Parameters.AddWithValue("@Path", GetHash().Encrypt(path));
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                realName = GetHash().Decrypt(reader["nama"].ToString());
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
                                alayNames.Add(GetHash().Decrypt(reader["nama"].ToString()));
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
                            string alayName = GetHash().Decrypt(reader["nama"].ToString());
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
                command.Parameters.AddWithValue("@Name", GetHash().Encrypt(name));
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
#pragma warning disable CS8601 // Possible null reference assignment.
                        biodata = new Biodata
                        {
                            NIK = GetHash().Decrypt(reader["NIK"].ToString()),
                            Nama = realName + " (" + name +")",
                            TempatLahir = GetHash().Decrypt(reader["tempat_lahir"].ToString()),
                            TanggalLahir = Convert.ToDateTime(reader["tanggal_lahir"]),
                            JenisKelamin = reader["jenis_kelamin"].ToString(),
                            GolonganDarah = GetHash().Decrypt(reader["golongan_darah"].ToString()),
                            Alamat = GetHash().Decrypt(reader["alamat"].ToString()),
                            Agama = GetHash().Decrypt(reader["agama"].ToString()),
                            StatusPerkawinan = reader["status_perkawinan"].ToString(),
                            Pekerjaan = GetHash().Decrypt(reader["pekerjaan"].ToString()),
                            Kewarganegaraan = GetHash().Decrypt(reader["kewarganegaraan"].ToString())
                        };
#pragma warning restore CS8601 // Possible null reference assignment.
                    }
                }
            }
            return biodata;
        }
    }
}
