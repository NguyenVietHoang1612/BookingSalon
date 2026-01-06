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
        public DbSet<TypeOfService> TypeOfServices { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<StylistTimeWork> stylistTimeWorks { get; set; }
        public DbSet<FixedTimeSlot> FixedTimeSlots { get; set; }
        public DbSet<BookingDetail> BookingDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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

                entity.Property(x => x.PhoneNumber)
                     .HasMaxLength(15);

                entity.Property(x => x.Address)
                      .HasMaxLength(255);

                entity.Property(x => x.PasswordHash)
                      .HasMaxLength(150);

                entity.HasIndex(u => u.Email)
                      .IsUnique();

                entity.HasIndex(p => p.FullName);

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
                      .HasMaxLength(255);

                entity.HasOne(x => x.StylistProfile)
                      .WithMany(s => s.StylistImages)
                      .HasForeignKey(x => x.StylistProfileId)
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

                entity.Property(x => x.StylistExperience)
                      .HasMaxLength(50);

                entity.Property(x => x.Create_At)
                      .HasDefaultValueSql("GETDATE()");

                entity.HasOne(x => x.Stylist)
                      .WithOne()
                      .HasForeignKey<StylistProfile>(x => x.StylistId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Branch)
                      .WithMany()
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
                      .HasMaxLength(150);

                entity.Property(x => x.Address)
                      .HasMaxLength(255);

                entity.Property(x => x.Phone)
                      .HasMaxLength(15);

                entity.HasIndex(x => x.Branch_Name);

                entity.Property(s => s.Created_At)
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

                entity.Property(s => s.Create_At)
                    .HasDefaultValueSql("GETDATE()");

                entity.HasIndex(x => x.Sort_Order)
                      .IsUnique();
            });

            builder.Entity<StylistTimeWork>(entity =>
            {
                entity.HasKey(x => x.Stylist_Time_Work_Id);

                entity.Property(s => s.Create_At)
                    .HasDefaultValueSql("GETDATE()");

                entity.HasOne(x => x.Stylist)
                      .WithMany()
                      .HasForeignKey(x => x.Stylist_Id)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.TimeSlot)
                      .WithMany()
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
                entity.HasKey(s => s.ServiceId);

                entity.Property(s => s.Service_Name)
                      .HasMaxLength(150)
                      .IsRequired();

                entity.Property(s => s.DurationInMinutes)
                     .HasMaxLength(10);

                entity.Property(s => s.ImageName)
                     .HasMaxLength(200);

                entity.Property(s => s.description)
                     .HasMaxLength(500);

                entity.Property(s => s.Create_At)
                    .HasDefaultValueSql("GETDATE()");

                entity.HasOne(s => s.TypeOfService)
                      .WithMany(t => t.Services)
                      .HasForeignKey(s => s.Type_Service_Id)
                      .OnDelete(DeleteBehavior.Restrict);

            });

            builder.Entity<TypeOfService>(entity =>
            {
                entity.HasKey(ts => ts.TypeOfServiceId);

                entity.Property(ts => ts.Type_Service_Name)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(s => s.Created_At)
                      .HasDefaultValueSql("GETDATE()");
            });
        }

        private void ConfigureBooking(ModelBuilder builder)
        {
            builder.Entity<Booking>(entity =>
            {
                entity.HasKey(x => x.Booking_Id);

                entity.HasOne(x => x.Customer)
                      .WithMany()
                      .HasForeignKey(x => x.Customer_Id)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Stylist)
                      .WithMany()
                      .HasForeignKey(x => x.Stylist_Id)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Branch)
                      .WithMany()
                      .HasForeignKey(x => x.Branch_Id)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.StartSlot)
                      .WithMany()
                      .HasForeignKey(x => x.Slot_Id)
                      .OnDelete(DeleteBehavior.Restrict);
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

                entity.Property(x => x.Created_At)
                      .HasDefaultValueSql("GETDATE()");

                entity.HasOne(bd => bd.Booking)
                    .WithMany(b => b.BookingDetails)
                    .HasForeignKey(bd => bd.Booking_Id)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(bd => bd.Service)
                    .WithMany(s => s.BookingDetails)
                    .HasForeignKey(bd => bd.Service_Id)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
