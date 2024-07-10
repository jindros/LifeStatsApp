﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeStatsApp.Model
{
    internal class Movie
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Year { get; set; }

        public string CsfdLink { get; set; } 
        
        public int Rating { get; set; }

        public DateTime DateOfRating { get; set; }
    }
}
