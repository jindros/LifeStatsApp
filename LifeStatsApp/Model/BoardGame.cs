using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeStatsApp.Model
{
    internal class BoardGame
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public int Year { get; set; }

        public int Plays { get; set; }

        public string WishlistComment { get; set; }

        public string BGGLink { get; set; }

        public float Rating { get; set; }
    }
}
