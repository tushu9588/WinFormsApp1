using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using WinFormsApp1;

public class TimesOfIndiaSitemapFetcher
{
    private const string TodaySitemapUrl = "https://timesofindia.indiatimes.com/sitemap/today";

    public async Task<List<NewsItem>> GetRecentNewsAsync()
    {
        var newsList = new List<NewsItem>();

        using (HttpClient client = new HttpClient())
        {
            string sitemapXml = await client.GetStringAsync(TodaySitemapUrl);
            XDocument xdoc = XDocument.Parse(sitemapXml);

            XNamespace ns = "http://www.google.com/schemas/sitemap-news/0.9";
            var newsItems = xdoc.Descendants(ns + "news");

            foreach (var item in newsItems)
            {
                string loc = item.Parent?.Element("loc")?.Value;
                string pubDateStr = item.Element(ns + "publication_date")?.Value;
                string title = item.Element(ns + "title")?.Value;

                if (!string.IsNullOrWhiteSpace(loc) &&
                    Uri.TryCreate(loc, UriKind.Absolute, out Uri url) &&
                    DateTime.TryParse(pubDateStr, out DateTime pubDate))
                {
                    // Filter by Today or Yesterday
                    DateTime today = DateTime.UtcNow.Date;
                    DateTime yesterday = today.AddDays(-1);
                    if (pubDate.Date == today || pubDate.Date == yesterday)
                    {
                        newsList.Add(new NewsItem
                        {
                            Title = title,
                            Url = url.ToString(),
                            PublicationDate = pubDate,
                            Type = "TOI"
                        });
                    }
                }
            }
        }

        return newsList.OrderByDescending(n => n.PublicationDate).ToList();
    }
}
