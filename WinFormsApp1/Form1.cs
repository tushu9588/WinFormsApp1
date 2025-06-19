using System.Configuration;
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
            this.Text = "Fetching news...";

            // ⏱ Fetch all news sources concurrently
            var breakingNewsTask = BreakingNewsFetcher.GetBreakingNewsAsync();
            var todayNewsTask = EcomicsTimesNewsFetcher.GetNewsEntrysAsync("https://m.economictimes.com/sitemap/today");
            var yesterdayNewsTask = EcomicsTimesNewsFetcher.GetNewsEntrysAsync("https://m.economictimes.com/sitemap/yesterday");
            var toiFetcher = new TimesOfIndiaSitemapFetcher();
            var toiNewsTask = toiFetcher.GetRecentNewsAsync();

            await Task.WhenAll(breakingNewsTask, todayNewsTask, yesterdayNewsTask, toiNewsTask);

            var allNews = new List<NewsEntry>();
            allNews.AddRange(breakingNewsTask.Result);
            allNews.AddRange(todayNewsTask.Result);
            allNews.AddRange(yesterdayNewsTask.Result);
            allNews.AddRange(toiNewsTask.Result);

            this.Text = $"📋 Total News Fetched: {allNews.Count}";

            if (allNews.Count > 0)
            {
                string connStr = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;
                var dbSaver = new NewsDatabaseSaver(connStr);

                // ⏳ Run DB saving off the UI thread to avoid UI freezing
                await Task.Run(() => dbSaver.SaveNewsToDatabase(allNews));
                this.Text = "News saved to database.";
            }
            else
            {
                this.Text = "No news found.";
            }
        }
    }
}
