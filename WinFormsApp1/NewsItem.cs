using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1
{
    public class NewsItem
    {
        /// <summary>
        /// Gets or sets the URL of the news article.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the title of the news article.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the publication date of the news article.
        /// </summary>
        public DateTime PublicationDate { get; set; }
        public string Type { get; set; } // Regular, Breaking, TOI
    }
}


