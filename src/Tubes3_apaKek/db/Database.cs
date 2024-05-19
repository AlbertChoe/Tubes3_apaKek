using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Windows; // Tambahkan ini

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
    }
}
