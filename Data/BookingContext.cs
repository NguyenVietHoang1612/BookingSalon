using BookingSalon.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookingSalon.Data
{
    public class BookingContext : IdentityDbContext<Users>
    {
        protected BookingContext()
        {
        }

        public BookingContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<StylistProfile> StylistProfiles { get; set; }
        public DbSet<StylistImage> StylistImages { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Combo> Combos { get; set; }
        public DbSet<ComboService> ComboServices { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<StylistTimeWork> stylistTimeWorks { get; set; }
        public DbSet<FixedTimeSlot> FixedTimeSlots { get; set; }
        public DbSet<BookingDetail> BookingDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasMany(b => b.BookingServices)
                      .WithOne(bs => bs.Booking)
                      .HasForeignKey(bs => bs.Booking_Id);
            });

            ConfigureUser(modelBuilder);
            ConfigureStylist(modelBuilder);
            ConfigureBranch(modelBuilder);
            ConfigureTimeSlot(modelBuilder);
            ConfigureService(modelBuilder);
            ConfigureBooking(modelBuilder);
            ConfigureBookingDetail(modelBuilder);
        }

        private void ConfigureUser(ModelBuilder builder)
        {
            builder.Entity<Users>(entity =>
            {
                entity.ToTable("Users");

                entity.Property(x => x.FullName)
                      .HasMaxLength(150);

                entity.Property(x => x.PasswordHash)
                      .HasMaxLength(150);

                entity.Property(x => x.Avatar_Name)
                      .HasMaxLength(255);

                entity.Property(x => x.Create_At)
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(x => x.Update_At)
                      .HasDefaultValueSql("GETDATE()");
            });
        }

        private void ConfigureStylist(ModelBuilder builder)
        {
            builder.Entity<StylistImage>(entity =>
            {
                entity.HasKey(x => x.ImageId);

                entity.Property(x => x.ImageUrl)
                      .HasMaxLength(255)
                      .IsRequired();

                entity.HasOne(x => x.StylistProfile)
                      .WithMany(s => s.StylistImages)
                      .HasForeignKey(x => x.StylistId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(x => x.Create_At)
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(x => x.Update_At)
                     .HasDefaultValueSql("GETDATE()");
            });

            builder.Entity<StylistProfile>(entity =>
            {
                entity.HasKey(x => x.StylistId);

                entity.Property(x => x.StylistSkill)
                      .HasMaxLength(255);

                entity.Property(x => x.Create_At)
                      .HasDefaultValueSql("GETDATE()");

                entity.HasOne(x => x.Stylist)
                      .WithOne(u => u.StylistProfile)
                      .HasForeignKey<StylistProfile>(x => x.StylistId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Branch)
                      .WithMany(b => b.StylistProfiles)
                      .HasForeignKey(x => x.Branch_Id)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigureBranch(ModelBuilder builder)
        {
            builder.Entity<Branch>(entity =>
            {
                entity.HasKey(x => x.BranchId);

                entity.Property(x => x.Branch_Name)
                      .HasMaxLength(150)
                      .IsRequired();

                entity.Property(x => x.Created_At)
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(x => x.Update_At)
                     .HasDefaultValueSql("GETDATE()");
            });
        }

        private void ConfigureTimeSlot(ModelBuilder builder)
        {
            builder.Entity<FixedTimeSlot>(entity =>
            {
                entity.HasKey(x => x.SlotId);

                entity.Property(x => x.TimeLabel)
                      .HasMaxLength(5)
                      .IsRequired();

                entity.HasIndex(x => x.Sort_Order)
                      .IsUnique();
            });

            builder.Entity<StylistTimeWork>(entity =>
            {
                entity.HasKey(x => x.Stylist_Time_Work_Id);

                entity.HasOne(x => x.Stylist)
                      .WithMany(u => u.StylistTimeWorks)
                      .HasForeignKey(x => x.Stylist_Id)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.TimeSlot)
                      .WithMany(s => s.StylistTimeWorks)
                      .HasForeignKey(x => x.Slot_Id)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new { x.Stylist_Id, x.Work_Date, x.Slot_Id })
                      .IsUnique();
            });
        }

        private void ConfigureService(ModelBuilder builder)
        {
            builder.Entity<Service>(entity =>
            {
                entity.HasKey(x => x.ServiceId);

                entity.Property(x => x.Service_Name)
                      .HasMaxLength(150)
                      .IsRequired();
            });

            builder.Entity<Combo>(entity =>
            {
                entity.HasKey(x => x.Combo_Id);

                entity.Property(x => x.Name_Combo)
                      .HasMaxLength(150)
                      .IsRequired();
            });

            builder.Entity<ComboService>(entity =>
            {
                entity.HasKey(x => new { x.ComboId, x.ServiceId });

                entity.HasOne(x => x.Combo)
                      .WithMany(c => c.ComboServices)
                      .HasForeignKey(x => x.ComboId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Service)
                      .WithMany(s => s.ComboServices)
                      .HasForeignKey(x => x.ServiceId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigureBooking(ModelBuilder builder)
        {
            builder.Entity<Booking>(entity =>
            {
                entity.HasKey(x => x.Booking_Id);

                entity.HasOne(x => x.Customer)
                      .WithMany(u => u.CustormerBookings)
                      .HasForeignKey(x => x.Customer_Id)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.StylistProfile)
                      .WithMany()
                      .HasForeignKey(x => x.Stylist_Profile_Id)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Branch)
                      .WithMany(b => b.Bookings)
                      .HasForeignKey(x => x.Branch_Id)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.StartSlot)
                      .WithMany()
                      .HasForeignKey(x => x.Slot_Id)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<BookingDetail>(entity =>
            {
                entity.HasKey(x => x.Booking_Detail_Id);

                entity.HasOne(x => x.Booking)
                      .WithMany(b => b.BookingServices)
                      .HasForeignKey(x => x.Booking_Id);
            });

            builder.Entity<Payment>(entity =>
            {
                entity.HasKey(x => x.PaymentId);

                entity.HasOne(x => x.Booking)
                      .WithOne(b => b.Payment)
                      .HasForeignKey<Payment>(x => x.BookingId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigureBookingDetail(ModelBuilder builder)
        {
            builder.Entity<BookingDetail>(entity =>
            {
                entity.HasKey(x => x.Booking_Detail_Id);

                builder.Entity<Service>()
                .HasMany(s => s.BookingDetail)
                .WithOne(d => d.Service)
                .HasForeignKey(d => d.Service_Id)
                .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<Combo>()
                .HasMany(c => c.BookingDetails)
                .WithOne(d => d.Combo)
                .HasForeignKey(d => d.ComboId)
                .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
