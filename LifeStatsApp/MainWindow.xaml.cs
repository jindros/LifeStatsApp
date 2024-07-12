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
using Serilog;
using System.Xml.Linq;


namespace LifeStatsApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly LifeStatsAppDBContext _context = new LifeStatsAppDBContext();

        public MainWindow()
        {
            InitializeComponent();
        }


        #region movies

        private async Task<ObservableCollection<Movie>> GetMoviesDataAsync()
        {
            List<Movie> movies = new List<Movie>();

            int i = 1;

            while (true)
            {
                var response = CallUrl("https://www.csfd.cz/uzivatel/7063-jindros/hodnoceni/?page=" + i.ToString()).Result;

                List<Movie> moviesFromCurrentPage = ParseMoviesHtml(response);

                if (moviesFromCurrentPage.Count == 0)
                {
                    break;
                }
                i++;

                movies.AddRange(moviesFromCurrentPage);
            }

            return new ObservableCollection<Movie>(movies);
        }




        private List<Movie> ParseMoviesHtml(string html /*, ObservableCollection<Movie> movies */)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);


            // < a href = "/film/1420300-the-nerd-crew/" class="film-title-name">The Nerd Crew</a>
            // < span class="film-title-info"><span class="info">(2017)</span> <span class="info">(seriál)</span></span>
            // < span class="info">(2017)</span>

            var movies = new List<Movie>();

            var table = htmlDoc.QuerySelector(".striped");

            if (table == null)
                return movies;


            var trs = table.SelectNodes(".//tr");

            if (trs == null)
                return movies;

            foreach (var tr in trs)
            {

                var link = tr.QuerySelectorAll(".film-title-name").FirstOrDefault();

                if (link is null)
                    continue;

                int year = -1;

                var yearHtml = tr.QuerySelectorAll(".info").FirstOrDefault();

                if (yearHtml != null)
                {
                    string yearString = yearHtml.InnerHtml.Replace("(", string.Empty).Replace(")", string.Empty);
                    int.TryParse(yearString, out year);
                }


                var stars = tr.QuerySelector(".stars");

                string classStringValue = stars.Attributes["class"].Value;

                char starRating = classStringValue[classStringValue.Length - 1];

                int rating = 0;

                if (Char.IsNumber(starRating))
                    rating = starRating - '0';

                // star-rating
                // stars stars-5
                // date-only

                var dateString = tr.QuerySelector(".date-only").InnerText;

                DateTime dateOfRating = DateTime.Parse(dateString);

                movies.Add(new Movie() { Name = link.InnerHtml, Year = year, CsfdLink = link.Attributes["href"].Value, Rating = rating, DateOfRating = dateOfRating });

            }

            return movies;
        }

        #endregion



        #region board games


        

        private async Task<ObservableCollection<BoardGame>> GetBoardGamesDataAsync()
        {
            List<BoardGame> boardGames = new List<BoardGame>();


            HttpClient client = new HttpClient();


            string response = await client.GetStringAsync("https://boardgamegeek.com/xmlapi/collection/Jindros");


            var boardGamesXml = XDocument.Parse(response);



            var items = boardGamesXml.Element("items");


            if (items != null)
            {
                IEnumerable<XElement> itemsList = items.Elements("item");

                if (itemsList != null)
                {
                    foreach (XElement item in itemsList)
                    {
                        BoardGame boardGame = new BoardGame
                        {
                            Name = item.Element("name").Value,                            
                            Plays = Int32.Parse(item.Element("numplays").Value),                            
                            BGGLink = "https://boardgamegeek.com/boardgame/" + Int32.Parse(item.Attribute("objectid").Value)
                        };

                        var wishListCommentNode = item.Element("wishlistcomment");
                        if (wishListCommentNode != null)
                        {
                            boardGame.WishlistComment = wishListCommentNode.Value;
                        }


                        int year = -1;


                        var yearPublishedElement = item.Element("yearpublished");


                        if (yearPublishedElement != null)                        
                            Int32.TryParse(yearPublishedElement.Value, out year);                        


                        boardGame.Year = year;



                        string valueString = item.Element("stats").Element("rating").Attribute("value").Value;

                        float rating = -1.0f;

                        float.TryParse(valueString, out rating);                                                 

                        boardGame.Rating = rating;

                        boardGames.Add(boardGame);
                    }
                }
            }

            return new ObservableCollection<BoardGame>(boardGames);
        }


        #endregion


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
        }

        private async void MoviesButtonMouseUp(object sender, RoutedEventArgs e)
        {

            MoviesWebScrapButton.Content = "Loading...";

            ObservableCollection<Movie> movies = await Task.Run(() => GetMoviesDataAsync());


            DG1.DataContext = movies; 

            foreach (Movie movie in movies)
            {
                _context.Add(movie);
            }

            _context.SaveChanges();


            MoviesWebScrapButton.Content = "Done.";
        }

        private async void BoardGamesButtonMouseUp(object sender, RoutedEventArgs e)
        {

            // https://boardgamegeek.com/xmlapi/collection/Jindros


            ObservableCollection<BoardGame> bgs = await Task.Run(() => GetBoardGamesDataAsync());


        }            
    }
}










