using BookingSalon.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace BookingSalon.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<Users>>();

            string[] roleNames = { "Admin", "Stylist", "Customer", "Passerby" };

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
                var newUser = new Users
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

           

        }
    }
}
