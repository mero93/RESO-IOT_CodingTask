using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Record>? Records { get; set; }

        public DbSet<Sensor>? Sensors { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Record>()
                .HasOne(x => x.Sensor)
                .WithMany(x => x.Records)
                .HasForeignKey(x => x.SensorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Sensor>()
                .HasIndex(x => x.ClientId);

            builder.Entity<Sensor>().Property(x => x.Latitude)
                .HasColumnType("decimal(12,3)");
            builder.Entity<Sensor>().Property(x => x.Longitude)
                .HasColumnType("decimal(12,3)");
            builder.Entity<Record>().Property(x => x.Latitude)
                .HasColumnType("decimal(12,3)");
            builder.Entity<Record>().Property(x => x.Longitude)
                .HasColumnType("decimal(12,3)");
            builder.Entity<Record>().Property(x => x.Illumination)
                .HasColumnType("decimal(8,1))");
            builder.Entity<Record>().Property(x => x.Temperature)
                .HasColumnType("decimal(3,1)");
        }
    }
}