using HtmlAgilityPack;
using LifeStatsApp.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using static System.Net.WebRequestMethods;
using static System.Net.Mime.MediaTypeNames;

namespace LifeStatsApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async Task<ObservableCollection<Movie>> GetMoviesDataAsync()
        {
            


            List<Movie> movies = new List<Movie>();

            int i = 1;

            while (true)
            {
                var response = CallUrl("https://www.csfd.cz/uzivatel/7063-jindros/hodnoceni/?page=" + i.ToString()).Result;

                List<Movie> moviesFromCurrentPage = ParseHtml(response);

                if (moviesFromCurrentPage.Count == 0)
                {
                    break;
                }
                i++;

                movies.AddRange(moviesFromCurrentPage);
            }

            return new ObservableCollection<Movie>(movies);

        }




        private List<Movie> ParseHtml(string html /*, ObservableCollection<Movie> movies */)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);


            // < a href = "/film/1420300-the-nerd-crew/" class="film-title-name">The Nerd Crew</a>
            // < span class="film-title-info"><span class="info">(2017)</span> <span class="info">(seriál)</span></span>
            // < span class="info">(2017)</span>


            var tds = htmlDoc.QuerySelectorAll(".name");

            var movies = new List<Movie>();

            foreach (var td in tds)
            {
                var link = td.QuerySelectorAll(".film-title-name").FirstOrDefault();

                if (link is null)
                    continue;

                int year = -1;

                var yearHtml = td.QuerySelectorAll(".info").FirstOrDefault();

                if (yearHtml != null)

                {
                    string yearString = yearHtml.InnerHtml.Replace("(", string.Empty).Replace(")", string.Empty);
                    int.TryParse(yearString, out year);
                }

                movies.Add(new Movie() { Name = link.InnerHtml, Year = year });

            }

            return movies;
        }



        private static async Task<string> CallUrl(string fullUrl)
        {
            HttpClient client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
            client.DefaultRequestHeaders.Accept.Clear();

            string response = await client.GetStringAsync(fullUrl);

            return response;
        }




        private void pnlMainGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // MessageBox.Show("You clicked me at " + e.GetPosition(this).ToString());
        }

        private async void ButtonMouseUp(object sender, RoutedEventArgs e)
        {
            WebScrapButton.Content = "Loading...";

            DG1.DataContext = new ObservableCollection<Movie>(await Task.Run(() => GetMoviesDataAsync()));

            WebScrapButton.Content = "Done.";
        }
    }
}


