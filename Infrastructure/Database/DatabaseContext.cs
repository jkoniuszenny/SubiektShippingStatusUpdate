using Core.Models;
using Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Database
{
    public class DatabaseContext : DbContext
    {
        private readonly DatabaseSettings _databaseSettings;

        public DatabaseContext(DbContextOptions<DatabaseContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        public DatabaseContext(DatabaseSettings databaseSettings)
        {
            _databaseSettings = databaseSettings;
        }

        public DbSet<FlagDictionary> FlagDictionary { get; set; }
        public DbSet<FlagValue> FlagValue { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableDetailedErrors(true);
            //optionsBuilder.UseSqlServer("Server=localhost; Database=neopak2; Trusted_Connection=True; MultipleActiveResultSets=true");// _settings.ConnectionString, s => s.CommandTimeout(60));
            optionsBuilder.UseSqlServer(_databaseSettings.ConnectionString);
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FlagDictionary>(builder =>
            {
                builder.HasKey(e => e.flg_Id);
            });

            modelBuilder.Entity<FlagValue>(builder =>
            {
                builder.HasKey(e => e.flw_IdGrupyFlag);
                builder.HasKey(e => e.flw_TypObiektu);
                builder.HasKey(e => e.flw_IdObiektu);
            });
        }
    }

}