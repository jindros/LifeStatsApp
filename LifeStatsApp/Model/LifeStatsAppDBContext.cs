using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeStatsApp.Model
{
    internal class LifeStatsAppDBContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; }

        public DbSet<SportingEvent> SportingEvents { get; set; }

        public DbSet<BoardGame> BoardGames { get; set; }

        public DbSet<Book> Books { get; set; }

        public LifeStatsAppDBContext() : base()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
    => options.UseSqlServer($"Server=(localdb)\\MSSQLLocalDB;Database=LifeStatsAppDB");

    }

}
