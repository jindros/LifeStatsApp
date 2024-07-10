using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeStatsApp.Model
{
    internal class SportingEvent
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public string Sport { get; set; }

        public float Distance { get; set; }

        public string Sparing { get; set; }

        public float Time { get; set; }

        public DateTime DateTime { get; set; }
    }
}
