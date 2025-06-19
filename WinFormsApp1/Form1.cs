using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsApp1.DB;
using WinFormsApp1.NewsFetcher;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private async void Form1_Load(object sender, EventArgs e)
        {
            // 🔴 1. Fetch Breaking News
            this.Text = "Fetching Breaking News...";
            var breakingNews = await BreakingNewsFetcher.GetBreakingNewsAsync();

            this.Text = "Fetching today’s Economic Times news...";
            var todayNews = await EcomicsTimesNewsFetcher.GetNewsEntrysAsync("https://m.economictimes.com/sitemap/today");

            this.Text = "Fetching yesterday’s Economic Times news...";
            var yesterdayNews = await EcomicsTimesNewsFetcher.GetNewsEntrysAsync("https://m.economictimes.com/sitemap/yesterday");

            var fetcher = new TimesOfIndiaSitemapFetcher();
            var news = await fetcher.GetRecentNewsAsync();


            var allNews = new List<NewsEntry>();
            allNews.AddRange(news);
            allNews.AddRange(breakingNews);
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
    }
}

