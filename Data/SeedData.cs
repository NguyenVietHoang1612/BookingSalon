using BookingSalon.Models.Entities;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Identity;

namespace BookingSalon.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<UsersModel>>();
            var rankService = serviceProvider.GetRequiredService<IRankService>();

            string[] roleNames = { "Admin", "Stylist", "Customer", "Reception", "Skinner" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);

                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            string adminEmail = "AdminSalonToc@StayHere.com";
            string password = "Admin@12345";
            var roleAdmin = await roleManager.FindByNameAsync("Admin");

            var user = await userManager.FindByEmailAsync(adminEmail);
            if (user == null)
            {
                var newUser = new UsersModel
                {
                    FullName = "Admin",
                    PhoneNumber = "0369049923",
                    RoleId = roleAdmin.Id,
                    UserName = adminEmail,
                    Email = adminEmail,
                    Status = true,
                    EmailConfirmed = true,
                    Create_At = DateTime.Now,
                    Update_At = DateTime.Now
                };

                var result = await userManager.CreateAsync(newUser, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newUser, "Admin");
                }
            }

            var existingRanks = await rankService.GetAllAsync();
            if (!existingRanks.Data.Any())
            {
                var defaultRanks = new List<RankModel>
                {
                    new RankModel {
                        RankName = "Chưa có hạng thành viên",
                        MinPoint = 0,
                        MaxBookingDays = 3,
                        DiscountPercent = 0,
                        Description = "Hạng mặc định cho khách hàng mới đăng ký"
                    },
                    new RankModel {
                        RankName = "Bạc",
                        MinPoint = 500,
                        MaxBookingDays = 5,
                        DiscountPercent = 5,
                        Description = "Ưu đãi giảm 5% cho mỗi hóa đơn"
                    },
                    new RankModel {
                        RankName = "Vàng",
                        MinPoint = 2500,
                        MaxBookingDays = 6,
                        DiscountPercent = 7,
                        Description = "Ưu đãi giảm 7% cho mỗi hóa đơn"
                    },
                    new RankModel {
                        RankName = "Kim cương",
                        MinPoint = 7000,
                        MaxBookingDays = 8,
                        DiscountPercent = 10,
                        Description = "Hạng cao cấp nhất, giảm 10% và nhiều đặc quyền"
                    }
                };

                foreach (var r in defaultRanks)
                {
                    await rankService.CreateAsync(r);
                }
            }

        }
    }
}
