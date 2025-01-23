using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using school_api.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection.Metadata;

namespace school_api.Data
{
    public class AppDbContext:IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Log>().HasKey(n => n.Id);

            //modelBuilder.Entity<School>()
            //  .HasMany(e => e.Session)
            //        .WithOne(e => e.School)
            //        .HasForeignKey(e => e.SchoolId)
            //        .IsRequired();

            //modelBuilder.Entity<Session>()
            //.HasMany(e => e.GameSession)
            //      .WithOne(e => e.Session)
            //       .HasForeignKey("SessionId")
            //      .IsRequired(false);

            // modelBuilder.Entity<GameSession>()
            //.HasOne(e => e.Avatar)
            //.WithOne(e => e.GameSession)
            //.HasForeignKey<Avatar>(e => e.GameSessionId)
            //.IsRequired();


            base.OnModelCreating(modelBuilder);

        }

        public DbSet<Log> Logs { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Transaction> Transaction { get; set; }
        public DbSet<School> School { get; set; }
        public DbSet<AdminTransaction> AdminTransaction { get; set; }
        public DbSet<Session> Session { get; set; }
        public DbSet<GameSession> GameSession { get; set; }
       // public DbSet<Player> Player { get; set; }
        public DbSet<Avatar> Avatar { get; set; }
        public DbSet<UserAccount> UserAccount { get; set; }

    }
}
