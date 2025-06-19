using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1
{
    public class BreakingNewsDatabaseSaver
    {
        private readonly string _connectionString;

        public BreakingNewsDatabaseSaver(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void SaveBreakingNewsToDatabase(List<BreakingNewsItem> newsList)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS BreakingNews (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    Title TEXT,
                    Type VARCHAR(10),
                    Link TEXT,
                    InsertedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                );";
                using (var cmd = new MySqlCommand(createTableQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                foreach (var item in newsList)
                {
                    string insertQuery = "INSERT INTO BreakingNews (Title, Type, Link) VALUES (@title, @type, @link)";
                    using (var cmd = new MySqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@title", item.title);
                        cmd.Parameters.AddWithValue("@type", item.type);
                        cmd.Parameters.AddWithValue("@link", item.link ?? "");
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}