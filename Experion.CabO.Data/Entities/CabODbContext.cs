using Microsoft.EntityFrameworkCore;
using System;

namespace Experion.CabO.Data.Entities
{
    public class CabODbContext : DbContext
    {
        public CabODbContext()
        {
        }

        public CabODbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<RideRequestor> RideRequestor { get; set; }
        public DbSet<Rating> Rating { get; set; }
        public DbSet<Ride> Ride { get; set; }
        public DbSet<Cab> Cab { get; set; }
        public DbSet<Driver> Driver { get; set; }
        public DbSet<RideAssignment> RideAssignment { get; set; }
        public DbSet<RideStatus> RideStatus { get; set; }
        public DbSet<RideType> RideType { get; set; }
        public DbSet<Shift> Shift { get; set; }
        public DbSet<AvailableTime> AvailableTime { get; set; }
        public DbSet<OfficeCommutation> OfficeCommutation { get; set; }
        public DbSet<OfficeLocation> OfficeLocation { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ride>(entity => {
                entity.Property(x => x.CabType).HasMaxLength(10);
                entity.Property(x => x.From).HasMaxLength(300);
                entity.Property(x => x.To).HasMaxLength(300);
                entity.Property(x => x.ContactNo).HasMaxLength(10);
                entity.Property(x => x.Purpose).HasMaxLength(100);
                entity.Property(x => x.ProjectCode).HasMaxLength(200);
                entity.Property(x => x.ExternalCabName).HasMaxLength(100);
                entity.Property(x => x.CancelReason).HasMaxLength(100);
            });
            modelBuilder.Entity<Rating>().ToTable("Rating");
            modelBuilder.Entity<RideAssignment>().ToTable("RideAssignment");
            modelBuilder.Entity<AvailableTime>().ToTable("AvailableTime");
            modelBuilder.Entity<OfficeLocation>().ToTable("OfficeLocation");
            modelBuilder.Entity<OfficeCommutation>().ToTable("OfficeCommutation");
            modelBuilder.Entity<RideRequestor>().ToTable("RideRequestor");
            modelBuilder.Entity<RideStatus>(entity => {
                entity.Property(x => x.StatusName).HasMaxLength(30);
            });
            modelBuilder.Entity<RideType>(entity => {
                entity.Property(x => x.rideType).HasMaxLength(30);
            });
            modelBuilder.Entity<Shift>(entity => {
                entity.Property(x => x.ShiftName).HasMaxLength(50);
            });
            modelBuilder.Entity<Driver>(entity => {
                entity.Property(x => x.Name).HasMaxLength(50);
            });
            modelBuilder.Entity<Cab>(entity => {
                entity.Property(x => x.Model).HasMaxLength(50);
                entity.Property(x => x.VehicleNo).HasMaxLength(30);
            });
            modelBuilder.Entity<RideStatus>().HasData(
                new RideStatus { Id = 1, StatusName = "Approved" },
                new RideStatus { Id = 2, StatusName = "Pending" },
                new RideStatus { Id = 3, StatusName = "Rejected" },
                new RideStatus { Id = 4, StatusName = "Completed" },
                new RideStatus { Id = 5, StatusName = "Cancelled" }
            );
            modelBuilder.Entity<RideType>().HasData(
                new RideType { Id = 1, rideType = "CABO-OFFICE" },
                new RideType { Id = 2, rideType = "CABO-CLIENT" },
                new RideType { Id = 3, rideType = "CABO-OTHERS" },
                new RideType { Id = 4, rideType = "CABO-ADMIN" }
            );
            modelBuilder.Entity<Shift>().HasData(
                new Shift { Id = 1, ShiftName = "Day", ShiftStart = new DateTime(01,01,01,07,30,00), ShiftEnd = new DateTime(01, 01, 01, 17, 30, 00) },
                new Shift { Id = 2, ShiftName = "Night", ShiftStart = new DateTime(01, 01, 01, 17, 30, 00), ShiftEnd = new DateTime(01, 01, 01, 07, 30, 00) }
            );
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=exp-db.cmog3ma1ytnw.us-east-2.rds.amazonaws.com;Database=ExperionCabO;User Id=admin;Password=admin123!;MultipleActiveResultSets=true");
        }
    }
}
