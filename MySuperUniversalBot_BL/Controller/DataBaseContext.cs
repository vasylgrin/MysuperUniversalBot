using Microsoft.EntityFrameworkCore;
using MySuperUniversalBot_BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySuperUniversalBot_BL.Controller
{
    public class DataBaseContext : DbContext
    {
        public DbSet<Reminder> Reminders => Set<Reminder>();

        public DataBaseContext()=> Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=helloapp.db");
        }

    }
}
