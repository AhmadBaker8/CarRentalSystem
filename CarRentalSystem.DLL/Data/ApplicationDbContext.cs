using CarRentalSystem.DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.DAL.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Car> Cars { get; set; }
        public DbSet<CarImage> CarImages { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<DamageReport> DamageReports { get; set; }
        public DbSet<DamageImage> DamageImages { get; set; }
        public DbSet<Maintenance> Daintenances { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Car>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.PlateNumber).IsRequired();
                entity.HasIndex(c => c.PlateNumber).IsUnique();
                entity.HasQueryFilter(c => !c.IsDeleted);
            });

            builder.Entity<CarImage>(entity =>
            {
                entity.HasKey(ci => ci.Id);

                entity.HasOne(ci => ci.Car)
                    .WithMany(c => c.CarImages)
                    .HasForeignKey(ci => ci.CarId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasQueryFilter(ci => !ci.IsDeleted);
            });

            builder.Entity<Booking>(entity =>
            {
                entity.HasKey(ci => ci.Id);

                entity.HasOne(b => b.Car)
                    .WithMany(c => c.Bookings)
                    .HasForeignKey(b => b.CarId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.User)
                    .WithMany(u => u.Bookings)
                    .HasForeignKey(b => b.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(b => !b.IsDeleted);
            });

            builder.Entity<DamageReport>(entity =>
            {
                entity.HasKey(dr => dr.Id);

                entity.HasOne(dr => dr.Booking)
                    .WithMany(b => b.DamageReports)
                    .HasForeignKey(dr => dr.BookingId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasQueryFilter(dr => !dr.IsDeleted);
            });

            builder.Entity<DamageImage>(entity =>
            {
                entity.HasKey(di => di.Id);

                entity.HasOne(di => di.DamageReport)
                    .WithMany(dr => dr.DamageImages)
                    .HasForeignKey(di => di.DamageReportId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasQueryFilter(di => !di.IsDeleted);
            });

            builder.Entity<Maintenance>(entity =>
            {
                entity.HasKey(m => m.Id);

                entity.HasOne(m => m.Car)
                    .WithMany(c => c.MaintenanceRecords)
                    .HasForeignKey(m => m.CarId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasQueryFilter(m => !m.IsDeleted);
            });

            builder.Entity<Review>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.HasOne(r => r.Car)
                    .WithMany(c => c.Reviews)
                    .HasForeignKey(r => r.CarId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.User)
                    .WithMany(u => u.Reviews)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // المستخدم يقدر يقيم السيارة مرة وحدة بس
                entity.HasIndex(r => new { r.CarId, r.UserId }).IsUnique();

                entity.HasQueryFilter(r => !r.IsDeleted);
            });
        }
    }
}
