using BookingSalon.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace BookingSalon.Data
{
    public class BookingContext : IdentityDbContext<UsersModel>
    {
        public BookingContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<UsersModel> Users { get; set; }
        public DbSet<StaffProfileModel> StaffProfile { get; set; }
        public DbSet<StaffPortfolioModel> StaffPortfolio { get; set; }
        public DbSet<ServiceModel> Services { get; set; }
        public DbSet<TypeOfServiceModel> TypeOfServices { get; set; }
        public DbSet<BranchModel> Branch { get; set; }
        public DbSet<BookingModel> Bookings { get; set; }
        public DbSet<BookingImageModel> BookingImages { get; set; }
        public DbSet<BookingDetailModel> BookingDetails { get; set; }
        public DbSet<StaffScheduleModel> StaffSchedule { get; set; }
        public DbSet<FixedTimeSlotModel> FixedTimeSlots { get; set; }
        public DbSet<NewsModel> News { get; set; }
        public DbSet<ProvinceModel> Province { get; set; }
        public DbSet<DistrictModel> District { get; set; }
        public DbSet<WardModel> Ward { get; set; }
        public DbSet<ReviewModel> Reviews { get; set; }
        public DbSet<RankModel> Ranks { get; set; }
        public DbSet<CouponModel> Coupon { get; set; }
        public DbSet<CustomerRankModel> CustomerRank { get; set; }
        public DbSet<PaymentModel> Payment { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureUser(modelBuilder);
            ConfigureStaff(modelBuilder);
            ConfigureBranch(modelBuilder);
            ConfigureTimeSlot(modelBuilder);
            ConfigureService(modelBuilder);
            ConfigureBooking(modelBuilder);
            ConfigureBookingDetail(modelBuilder);
            ConfigureNews(modelBuilder);
            ConfigureLocation(modelBuilder);
            ConfigureLoyalty(modelBuilder);
            ConfigureReview(modelBuilder);
            ConfigureCoupon(modelBuilder);
            ConfigureCouponUsage(modelBuilder);
            ConfigurePayment(modelBuilder);
            ConfigureBookingImage(modelBuilder);
        }

        private void ConfigureUser(ModelBuilder builder)
        {
            builder.Entity<UsersModel>(entity =>
            {
                entity.ToTable("Users");

                entity.Property(x => x.FullName)
                      .HasMaxLength(50);

                entity.Property(x => x.Avatar_Name)
                      .HasMaxLength(100);

                entity.Property(x => x.PhoneNumber)
                     .HasMaxLength(15);

                entity.Property(u => u.DateOfBirth)
                    .HasColumnType("date");

                entity.Property(u => u.RoleId)
                    .HasMaxLength(450)
                    .IsRequired(false);

                entity.Property(x => x.Address)
                      .HasMaxLength(150);

                entity.Property(x => x.PasswordHash)
                      .HasMaxLength(100);

                entity.Property(u => u.Email)
                     .HasMaxLength(100);

                entity.HasIndex(p => p.FullName);

                entity.Property(x => x.Create_At)
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(x => x.Update_At)
                      .HasDefaultValueSql("GETDATE()");

                entity.HasOne(u => u.Ward)
                   .WithMany()
                   .HasForeignKey(u => u.WardId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Restrict);

            });
        }

        private void ConfigureStaff(ModelBuilder builder)
        {
            builder.Entity<StaffPortfolioModel>(entity =>
            {
                entity.HasKey(x => x.ImageId);

                entity.Property(x => x.StaffProfileId)
                      .HasMaxLength(450);

                entity.Property(x => x.Title)
                      .HasMaxLength(50);

                entity.Property(x => x.ImageUrl)
                      .HasMaxLength(100);

                entity.HasOne(x => x.StaffProfile)
                      .WithMany(s => s.StaffPortfolio)
                      .HasForeignKey(x => x.StaffProfileId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(x => x.Create_At)
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(x => x.Update_At)
                     .HasDefaultValueSql("GETDATE()");
            });

            builder.Entity<StaffProfileModel>(entity =>
            {
                entity.HasKey(x => x.StaffId);

                entity.Property(x => x.StaffId)
                     .HasMaxLength(450);

                entity.Property(x => x.Staff_Bio)
                      .HasMaxLength(200);

                entity.Property(x => x.Start_Work_Time)
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(x => x.End_Work_Time)
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(x => x.Create_At)
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(x => x.Update_At)
                      .HasDefaultValueSql("GETDATE()");

                entity.HasOne(x => x.Staff)
                      .WithOne()
                      .HasForeignKey<StaffProfileModel>(x => x.StaffId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Branch)
                      .WithMany()
                      .HasForeignKey(x => x.Branch_Id)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigureBranch(ModelBuilder builder)
        {
            builder.Entity<BranchModel>(entity =>
            {
                entity.HasKey(x => x.BranchId);

                entity.Property(x => x.Branch_Name)
                      .HasMaxLength(60);

                entity.Property(x => x.Address)
                      .HasMaxLength(150);

                entity.Property(x => x.PhoneNumber)
                      .HasMaxLength(15);

                entity.Property(x => x.Branch_Image)
                      .HasMaxLength(100);

                entity.HasIndex(x => x.Branch_Name);

                entity.Property(s => s.Created_At)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(x => x.Update_At)
                     .HasDefaultValueSql("GETDATE()");

                entity.HasOne(b => b.Ward)
                   .WithMany()
                   .HasForeignKey(b => b.WardId)
                   .OnDelete(DeleteBehavior.Restrict);

            });
        }

        private void ConfigureTimeSlot(ModelBuilder builder)
        {
            builder.Entity<FixedTimeSlotModel>(entity =>
            {
                entity.HasKey(x => x.SlotId);

                entity.Property(s => s.TimeLabel)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(s => s.Create_At)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(s => s.Update_At)
                    .HasDefaultValueSql("GETDATE()");

                entity.HasIndex(x => x.TimeLabel)
                      .IsUnique();
            });

            builder.Entity<StaffScheduleModel>(entity =>
            {
                entity.HasKey(x => x.ScheduleId);

                entity.Property(x => x.ScheduleId)
                 .ValueGeneratedOnAdd();

                entity.Property(x => x.Staff_Id)
                      .HasMaxLength(450);

                entity.Property(s => s.Create_At)
                   .HasDefaultValueSql("GETDATE()");

                entity.Property(s => s.Update_At)
                    .HasDefaultValueSql("GETDATE()");

                entity.HasIndex(x => new { x.Staff_Id, x.Work_Date, x.Slot_Id })
                 .IsUnique()
                 .HasDatabaseName("IX_Unique_Stylist_Slot_Per_Day");

                entity.HasOne(x => x.StaffProfile)
                      .WithMany()
                      .HasForeignKey(x => x.Staff_Id)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Booking)
                      .WithMany()
                      .HasForeignKey(x => x.Booking_Id)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.TimeSlot)
                      .WithMany()
                      .HasForeignKey(x => x.Slot_Id)
                      .OnDelete(DeleteBehavior.Restrict);

            });
        }

        private void ConfigureService(ModelBuilder builder)
        {
            builder.Entity<ServiceModel>(entity =>
            {
                entity.HasKey(s => s.ServiceId);

                entity.Property(s => s.Service_Name)
                      .HasMaxLength(60)
                      .IsRequired();

                entity.Property(s => s.ImageName)
                     .HasMaxLength(100);

                entity.Property(s => s.Base_Price)
                      .HasColumnType("decimal(12,2)");

                entity.Property(s => s.Promotion_Price)
                      .HasColumnType("decimal(12,2)");

                entity.Property(s => s.description)
                     .HasMaxLength(2000);

                entity.Property(s => s.IsPromotionActive)
                     .HasComputedColumnSql(@"CASE 
                            WHEN [Promotion_Price] IS NOT NULL 
                                 AND [Promotion_Price] > 0
                                 AND [Promotion_Start] <= GETDATE() 
                                 AND [Promotion_End] >= GETDATE() THEN CAST(1 AS BIT) 
                            ELSE CAST(0 AS BIT) 
                     END");

                entity.Property(s => s.Promotion_Start)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(s => s.Promotion_End)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(s => s.Create_At)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(s => s.Update_At)
                    .HasDefaultValueSql("GETDATE()");

                entity.HasOne(s => s.TypeOfService)
                      .WithMany(t => t.Services)
                      .HasForeignKey(s => s.Type_Service_Id)
                      .OnDelete(DeleteBehavior.Restrict);

            });

            builder.Entity<TypeOfServiceModel>(entity =>
            {
                entity.HasKey(ts => ts.TypeOfServiceId);

                entity.Property(ts => ts.Type_Service_Name)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(ts => ts.AppliedStaffRoleId)
                      .HasMaxLength(450);

                entity.Property(s => s.Created_At)
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(s => s.Update_At)
                      .HasDefaultValueSql("GETDATE()");
            });
        }

        private void ConfigureBooking(ModelBuilder builder)
        {
            builder.Entity<BookingModel>(entity =>
            {
                entity.HasKey(x => x.Booking_Id);


                entity.Property(b => b.Customer_Id)
                      .HasMaxLength(450);

                entity.Property(b => b.Stylist_Id)
                      .HasMaxLength(450);

                entity.Property(b => b.Skinner_Id)
                     .HasMaxLength(450);

                entity.Property(s => s.CouponCodeSnapshot)
                     .HasMaxLength(15);

                entity.Property(b => b.DiscountValueSnapshot)
                      .HasColumnType("decimal(12,2)");

                

                entity.Property(b => b.TotalPrice)
                      .HasColumnType("decimal(12,2)");

                entity.Property(b => b.DiscountAmount)
                      .HasColumnType("decimal(12,2)");

                entity.Property(b => b.FinalPrice)
                      .HasColumnType("decimal(12,2)");

                entity.Property(b => b.Note)
                      .HasMaxLength(250);

                entity.Property(b => b.UpdatedById)
                      .HasMaxLength(450);

                entity.Property(b => b.Status)
                      .HasColumnType("tinyint");

                entity.Property(b => b.Create_At)
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(b => b.Update_At)
                      .HasDefaultValueSql("GETDATE()");

                entity.HasOne(b => b.Customer)
                      .WithMany()
                      .HasForeignKey(b => b.Customer_Id)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.StylistProfile)
                      .WithMany()
                      .HasForeignKey(b => b.Stylist_Id)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.SkinnerProfile)
                      .WithMany()
                      .HasForeignKey(b => b.Skinner_Id)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.Branch)
                      .WithMany()
                      .HasForeignKey(b => b.Branch_Id)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.StartSlot)
                      .WithMany()
                      .HasForeignKey(s => s.Start_Slot_Id)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.EndSlot)
                      .WithMany()
                      .HasForeignKey(s => s.End_Slot_Id)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.Coupon)
                      .WithMany()
                      .HasForeignKey(c => c.Coupon_Id)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.UpdatedBy)
                      .WithMany()
                      .HasForeignKey(u => u.UpdatedById)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigureBookingDetail(ModelBuilder builder)
        {
            builder.Entity<BookingDetailModel>(entity =>
            {
                entity.HasKey(x => x.Booking_Detail_Id);

                entity.Property(x => x.Created_At)
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(x => x.Update_At)
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

        private void ConfigureNews(ModelBuilder builder)
        {
            builder.Entity<NewsModel>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Title)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(x => x.Summary)
                      .HasMaxLength(400);

                entity.Property(x => x.Content)
                      .IsRequired();

                entity.Property(x => x.Thumbnail)
                      .HasMaxLength(150);

                entity.Property(x => x.AuthorId)
                      .HasMaxLength(450);

                entity.HasOne(x => x.Author)
                      .WithMany() 
                      .HasForeignKey(x => x.AuthorId)
                      .OnDelete(DeleteBehavior.Restrict); 
               
                entity.Property(x => x.PublishedDate)
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(x => x.Created_At)
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(x => x.Updated_At)
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(x => x.IsActive)
                      .HasDefaultValue(true);

                entity.HasIndex(x => x.Title);
            });
        }

        private void ConfigureLocation(ModelBuilder builder)
        {
            builder.Entity<ProvinceModel>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(100);
            });

            builder.Entity<DistrictModel>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(100);
                entity.HasOne(d => d.Province)
                      .WithMany(p => p.Districts)
                      .HasForeignKey(d => d.ProvinceId)
                      .OnDelete(DeleteBehavior.Restrict); 
            });

            builder.Entity<WardModel>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(100);
                entity.HasOne(w => w.District)
                      .WithMany(d => d.Wards)
                      .HasForeignKey(w => w.DistrictId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            
        }

        private void ConfigureLoyalty(ModelBuilder builder)
        {
            builder.Entity<RankModel>(entity => {
                entity.HasKey(e => e.RankId);
                entity.Property(e => e.RankName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.DiscountPercent).HasColumnType("decimal(5,2)");
            });

            builder.Entity<CustomerRankModel>(entity => {
                entity.HasKey(e => e.Customer_Id);

                entity.HasOne(cr => cr.Customer)
                      .WithOne()
                      .HasForeignKey<CustomerRankModel>(cr => cr.Customer_Id)
                      .OnDelete(DeleteBehavior.Cascade); 

                entity.HasOne(cr => cr.Rank)
                      .WithMany()
                      .HasForeignKey(cr => cr.RankId)
                      .OnDelete(DeleteBehavior.Restrict); 
            });
        }

        private void ConfigureReview(ModelBuilder builder)
        {
            builder.Entity<ReviewModel>(entity => {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Customer_Id)
                      .HasMaxLength(450);

                entity.Property(e => e.RepliedById)
                      .HasMaxLength(450);

                entity.Property(e => e.Staff_Id)
                      .HasMaxLength(450);

                entity.Property(e => e.Comment)
                      .HasMaxLength(250);

                entity.Property(e => e.Reply_Comment)
                      .HasMaxLength(250);

                entity.Property(x => x.Created_At)
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(x => x.Updated_At)
                      .HasDefaultValueSql("GETDATE()");

                entity.HasOne(r => r.Booking)
                      .WithOne() 
                      .HasForeignKey<ReviewModel>(r => r.Booking_Id)
                      .OnDelete(DeleteBehavior.Cascade); 

                entity.HasOne(r => r.Staff)
                      .WithMany()
                      .HasForeignKey(r => r.Staff_Id)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Customer)
                      .WithMany()
                      .HasForeignKey(r => r.Customer_Id)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigureCoupon(ModelBuilder builder)
        {
            builder.Entity<CouponModel>(entity => {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Code)
                      .HasMaxLength(15);

                entity.Property(s => s.Discount_value)
                     .HasColumnType("decimal(12,2)");

                entity.Property(s => s.Min_Order_Amount)
                      .HasColumnType("decimal(12,2)");

                entity.Property(x => x.Created_At)
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(x => x.Updated_At)
                      .HasDefaultValueSql("GETDATE()");
            });
        }

        private void ConfigureCouponUsage(ModelBuilder builder)
        {
            builder.Entity<CouponUsage>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.UserId)
                      .HasMaxLength(450);

                entity.HasOne(e => e.Coupon)
                      .WithMany() 
                      .HasForeignKey(e => e.CouponId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                      .WithMany() 
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.UserId)
                      .HasMaxLength(450)
                      .IsRequired();

                entity.Property(e => e.UsedAt)
                      .HasDefaultValueSql("GETDATE()");

                entity.HasIndex(e => new { e.CouponId, e.UserId });
            });
        }

        private void ConfigurePayment(ModelBuilder builder)
        {
            builder.Entity<PaymentModel>(entity =>
            {
                entity.ToTable("Payments");

                entity.HasKey(x => x.PaymentId);

                entity.Property(x => x.Amount)
                      .HasColumnType("decimal(12,2)")
                      .IsRequired();

                entity.Property(x => x.PaymentMethod)
                      .HasMaxLength(20) 
                      .IsRequired();

                entity.Property(x => x.OrderInfo)
                      .HasMaxLength(250);

                entity.Property(x => x.ProcessedBy)
                      .HasMaxLength(450); 

                entity.Property(x => x.CreatedDate)
                      .HasDefaultValueSql("GETDATE()");


                entity.HasOne(p => p.Booking)
                      .WithMany() 
                      .HasForeignKey(p => p.BookingId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.StaffId)
                      .WithMany()
                      .HasForeignKey(p => p.ProcessedBy)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigureBookingImage(ModelBuilder builder)
        {
            builder.Entity<BookingImageModel>(entity =>
            {
                entity.ToTable("BookingImages");
                entity.HasKey(x => x.Image_Id);

                entity.Property(x => x.ImageUrl)
                      .IsRequired()
                      .HasMaxLength(150); 

                entity.Property(x => x.Created_At)
                      .HasDefaultValueSql("GETDATE()");

                entity.HasOne(x => x.Booking)            
                      .WithMany(b => b.BookingImages)   
                      .HasForeignKey(x => x.Booking_Id)  
                      .OnDelete(DeleteBehavior.Cascade); 
            });
        }
    }
}
