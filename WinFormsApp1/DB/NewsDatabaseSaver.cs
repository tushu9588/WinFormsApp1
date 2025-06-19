using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace WinFormsApp1.DB
{
    public class NewsDatabaseSaver
    {
        private readonly string connectionString;

        /// <summary>
        /// Initializes the database saver with the given connection string.
        /// </summary>
        /// <param name="connStr">MySQL connection string.</param>
        public NewsDatabaseSaver(string connStr)
        {
            connectionString = connStr ?? throw new ArgumentNullException(nameof(connStr));
        }

        /// <summary>
        /// Saves a list of news items to the database using INSERT IGNORE to avoid duplicates.
        /// Includes support for the 'Type' field (e.g., TOI, Regular, Breaking).
        /// </summary>
        /// <param name="newsList">List of news items to save.</param>
        public void SaveNewsToDatabase(List<NewsEntry> newsList)
        {
            if (newsList == null || newsList.Count == 0)
            {
                Console.WriteLine("⚠️ No news items to save.");
                return;
            }

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    var sb = new StringBuilder();
                    sb.Append("INSERT IGNORE INTO News (Url, Title, PublicationDate, Type) VALUES ");

                    // Build bulk insert placeholders
                    for (int i = 0; i < newsList.Count; i++)
                    {
                        sb.AppendFormat("(@Url{0}, @Title{0}, @PublicationDate{0}, @Type{0})", i);
                        if (i < newsList.Count - 1)
                            sb.Append(", ");
                    }

                    using (var cmd = new MySqlCommand(sb.ToString(), conn))
                    {
                        // Add parameters for each news item
                        for (int i = 0; i < newsList.Count; i++)
                        {
                            cmd.Parameters.AddWithValue($"@Url{i}", newsList[i].Url);
                            cmd.Parameters.AddWithValue($"@Title{i}", newsList[i].Title);
                            cmd.Parameters.AddWithValue($"@PublicationDate{i}", newsList[i].PublicationDate);
                            cmd.Parameters.AddWithValue($"@Type{i}", newsList[i].Type);
                        }

                        int rowsInserted = cmd.ExecuteNonQuery();
                        Console.WriteLine($"✅ Inserted {rowsInserted} new news item(s) into the database.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Failed to insert news into database:");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
