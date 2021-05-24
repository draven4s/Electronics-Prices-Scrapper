using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using insta_scrape.Classes;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;


namespace insta_scrape.Contexts
{
    public class ScrapeDbContext : DbContext
    {
        private const string connectionString = "server=88.222.149.150;port=3306;database=cad_pd;user=root;password=LUL";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(connectionString);
        }
        public virtual DbSet<User> users { get; set; }
        public virtual DbSet<Logins> logins { get; set; }
        public virtual DbSet<Phone> phones { get; set; }
        public virtual DbSet<Searches> searches { get; set; }
        public virtual DbSet<UserSearches> usersearches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Logins>().ToTable("logins");
            modelBuilder.Entity<User>()
                    .HasKey(e => e.Id);
            modelBuilder.Entity<Logins>()
                    .HasKey(e => e.Id);
            modelBuilder.Entity<Logins>().Property(u => u.Id).HasColumnType("int").UseMySqlIdentityColumn().IsRequired();
            modelBuilder.Entity<Logins>().Property(u => u.username).HasColumnType("nvarchar(50)").IsRequired();
            modelBuilder.Entity<Logins>().Property(u => u.password).HasColumnType("nvarchar(50)").IsRequired();
            modelBuilder.Entity<Logins>().Property(u => u.banned).HasColumnType("bool").IsRequired();
            modelBuilder.Entity<Logins>().HasIndex(p => p.Id).IsUnique();

        }

    }

}
