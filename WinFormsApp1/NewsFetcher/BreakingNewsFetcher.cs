using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WinFormsApp1.NewsFetcher
{
    public class BreakingNewsFetcher
    {
        private static readonly HttpClient client = new HttpClient();

        /// <summary>
        /// Asynchronously fetches the latest breaking news items.
        /// </summary>
        /// <returns>A list of breaking news items.</returns>
        public static async Task<List<NewsEntry>> GetBreakingNewsAsync()
        {
            string url = "https://economictimes.indiatimes.com/etstatic/breakingnews/etjson_bnews.html";
            var result = new List<NewsEntry>();

            try
            {
                // Fetch raw response from the breaking news endpoint
                string response = await client.GetStringAsync(url);

                // Extract the JSON part from the JavaScript function call
                Match match = Regex.Match(response, @"breakingnews\s*\(\s*(\[.*?\])\s*\);", RegexOptions.Singleline);

                if (match.Success)
                {
                    string json = match.Groups[1].Value;
                    result = JsonConvert.DeserializeObject<List<NewsEntry>>(json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Breaking News Fetch Error: " + ex.Message);
            }

            return result;
        }
    }
}
