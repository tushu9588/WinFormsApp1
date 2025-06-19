using System.Xml.Linq;

namespace WinFormsApp1.NewsFetcher
{
    public class EcomicsTimesNewsFetcher
    {
        private static readonly HttpClient client = new HttpClient();

        /// <summary>
        /// Asynchronously retrieves news articles from the specified sitemap URL.
        /// </summary>
        /// <param name="sitemapUrl">The URL of the XML sitemap to fetch news from.</param>
        /// <returns>A list of NewsEntry objects representing the news articles.</returns>
        public static async Task<List<NewsEntry>> GetNewsEntrysAsync(string sitemapUrl)
        {
            var newsList = new List<NewsEntry>();

            try
            {
                string response = await client.GetStringAsync(sitemapUrl);
                var xmlDoc = XDocument.Parse(response);

                XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
                XNamespace newsNs = "http://www.google.com/schemas/sitemap-news/0.9";

                foreach (var item in xmlDoc.Descendants(ns + "url"))
                {
                    string url = item.Element(ns + "loc")?.Value;
                    string title = item.Element(newsNs + "news")?.Element(newsNs + "title")?.Value;
                    string pubDateStr = item.Element(newsNs + "news")?.Element(newsNs + "publication_date")?.Value;

                    if (DateTime.TryParse(pubDateStr, out DateTime pubDate))
                    {
                        // Filter to include only today's and yesterday's news
                        if (pubDate.Date == DateTime.Today || pubDate.Date == DateTime.Today.AddDays(-1))
                        {
                            newsList.Add(new NewsEntry
                            {
                                Url = url.ToString(),
                                Title = title,
                                PublicationDate = pubDate
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return newsList;
        }
    }
}
