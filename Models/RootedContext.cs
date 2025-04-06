using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;

namespace RootedWeb.Models
{
    public class RootedContext : DbContext
    {
        public RootedContext(DbContextOptions<RootedContext> options) : base(options) { }

        public DbSet<TaskItem> Tasks { get; set; }

        public DbSet<Streak> Streaks { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<MoodLog> MoodLogs { get; set; }

        public DbSet<TreePlanting> TreePlantings { get; set; }



    }
}
