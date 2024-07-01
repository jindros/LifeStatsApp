using HtmlAgilityPack;
using LifeStatsApp.Data;
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

            ObservableCollection<Movie> custdata = GetData();

 
            DG1.DataContext = custdata;

        }

        private ObservableCollection<Movie> GetData()
        {
            ObservableCollection<Movie> returnValue = new ObservableCollection<Movie>();

            string url = "https://en.wikipedia.org/wiki/List_of_programmers";

            // string url = "https://www.csfd.cz/uzivatel/7063-jindros/hodnoceni/";

            string response = CallUrl(url).Result;

            var linkList = ParseHtml(response);

            foreach (string s in linkList)
            {
                returnValue.Add(new Movie() { Name = s });
            }

            //WriteToCsv(linkList);

            //return View();

        

            //for (int i = 0; i < 200; i++)
            //{
            //    returnValue.Add(new Movie() { CsfdLink = "fdf", Name = "", Year = "fdf" });
            //}

            return returnValue;
        }


        private List<string> ParseHtml(string html)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var programmerLinks = htmlDoc.DocumentNode.Descendants("li")
                .Where(node => !node.GetAttributeValue("class", "").Contains("tocsection"))
                .ToList();

            List<string> wikiLink = new List<string>();

            foreach (var link in programmerLinks)
            {
                if (link.FirstChild.Attributes.Count > 0)
                    wikiLink.Add("https://en.wikipedia.org/" + link.FirstChild.Attributes[0].Value);
            }

            return wikiLink;
        }



        private static /* async */ Task<string> CallUrl(string fullUrl)
        {
            HttpClient client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
            client.DefaultRequestHeaders.Accept.Clear();
            var response = client.GetStringAsync(fullUrl);
            return /* await */ response;
        }




        private void pnlMainGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // MessageBox.Show("You clicked me at " + e.GetPosition(this).ToString());
        }

        void ButtonMouseUp(object sender, RoutedEventArgs e)
        {
            hah.Background = Brushes.LightBlue;
        }
    }
}
