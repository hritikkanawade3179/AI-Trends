using AITrendsNews.Models;
using Microsoft.AspNetCore.Identity;

namespace AITrendsNews.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = { "Admin", "Editor", "Viewer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Seed admin user
            var adminEmail = "admin@aitrendsnews.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Site Administrator",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(admin, "Admin@123!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }

            // Seed editor user
            var editorEmail = "editor@aitrendsnews.com";
            if (await userManager.FindByEmailAsync(editorEmail) == null)
            {
                var editor = new ApplicationUser
                {
                    UserName = editorEmail,
                    Email = editorEmail,
                    FullName = "News Editor",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(editor, "Editor@123!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(editor, "Editor");
            }
        }
    }
}
