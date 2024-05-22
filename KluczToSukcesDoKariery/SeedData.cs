using Microsoft.AspNetCore.Identity;

public class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        Console.WriteLine("Initialize method called.");
        string roleName = "Administrator";
        IdentityResult roleResult;

        // Sprawdź, czy rola już istnieje
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            // Jeśli rola nie istnieje, utwórz ją
            roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
        }

        // Sprawdź, czy administrator już istnieje
        IdentityUser admin = await userManager.FindByEmailAsync("admin@admin.com");

        if (admin == null)
        {
            // Utwórz administratora tylko jeśli nie istnieje
            IdentityUser user = new IdentityUser()
            {
                UserName = "admin@admin.com",
                Email = "admin@admin.com",
            };

            // Jeśli administrator nie istnieje, utwórz go
            IdentityResult result = await userManager.CreateAsync(user, "Admin123$");

            if (result.Succeeded)
            {
                user.EmailConfirmed= true;
                await userManager.UpdateAsync(user);

                // Przypisz rolę "Administrator" do administratora
                await userManager.AddToRoleAsync(user, roleName);
            }
        }
    }
}
