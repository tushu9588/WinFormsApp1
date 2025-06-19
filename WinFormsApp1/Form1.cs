using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // 🔄 Form Load Event
        private async void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                // 🔴 1. Fetch Breaking News
                this.Text = "🔴 Fetching Breaking News...";
                var breakingNews = await BreakingNewsFetcher.GetBreakingNewsAsync();

                foreach (var news in breakingNews)
                {
                    Console.WriteLine($"📰 {news.title}");
                    Console.WriteLine($"🔗 {news.link}");
                }

                if (breakingNews.Count > 0)
                {
                    string connStr = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;
                    var breakingNewsSaver = new BreakingNewsDatabaseSaver(connStr);
                    breakingNewsSaver.SaveBreakingNewsToDatabase(breakingNews);
                    this.Text = "✅ Breaking news saved to database.";
                }

                // 📰 2. Fetch Economic Times Sitemap News
                this.Text = "🔄 Fetching today’s Economic Times news...";
                var todayNews = await NewsFetcher.GetNewsItemsAsync("https://m.economictimes.com/sitemap/today");

                this.Text = "🔄 Fetching yesterday’s Economic Times news...";
                var yesterdayNews = await NewsFetcher.GetNewsItemsAsync("https://m.economictimes.com/sitemap/yesterday");

                var allNews = new List<NewsItem>();
                allNews.AddRange(todayNews);
                allNews.AddRange(yesterdayNews);

                this.Text = $"📋 Total ET News Fetched: {allNews.Count}";

                if (allNews.Count > 0)
                {
                    string connStr = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;
                    var dbSaver = new NewsDatabaseSaver(connStr);
                    dbSaver.SaveNewsToDatabase(allNews);
                    this.Text = "✅ Economic Times news saved to database.";
                }
                else
                {
                    this.Text = "⚠️ No Economic Times news found.";
                }
            }
            catch (Exception ex)
            {
                this.Text = "❌ Error: " + ex.Message;
            }
        }

        // 🆕 Button Click to Load Times of India News
        private async void btnLoadTimesNews_Click(object sender, EventArgs e)
        {
            try
            {
                this.Text = "🔄 Fetching Times of India News...";

                var fetcher = new TimesOfIndiaSitemapFetcher();
                var news = await fetcher.GetRecentNewsAsync();

                // Optional: display in ListView if you have one (else skip this block)
                /*
                listViewNews.Items.Clear();
                foreach (var item in news)
                {
                    var row = new ListViewItem(item.PublishDate.ToString("yyyy-MM-dd HH:mm"));
                    row.SubItems.Add(item.Title);
                    row.SubItems.Add(item.Url.ToString());
                    listViewNews.Items.Add(row);
                }
                */

                // ✅ Save to MySQL Database
                if (news.Count > 0)
                {
                    string connStr = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;
                    var saver = new NewsDatabaseSaver(connStr);
                    saver.SaveNewsToDatabase(news);
                    this.Text = $"✅ TOI News saved to DB: {news.Count} items.";
                }
                else
                {
                    this.Text = "⚠️ No Times of India news found.";
                }
            }
            catch (Exception ex)
            {
                this.Text = "❌ TOI Error: " + ex.Message;
            }
        }

    }
}

