using Microsoft.AspNetCore.Identity;

namespace ClinicManager.Data;

/// <summary>
/// Inicjalizuje bazę danych – tworzy role i konta testowe.
/// Uruchamiana automatycznie przy starcie aplikacji.
/// </summary>
public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        // 1. Tworzenie ról
        string[] roles = ["Admin", "Lekarz", "Rejestratorka"];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // 2. Tworzenie kont testowych (tylko jeśli nie istnieją)

        // Admin
        await CreateUserIfNotExists(userManager, "admin@clinic.pl", "Admin123!", "Admin");

        // Lekarz
        await CreateUserIfNotExists(userManager, "lekarz@clinic.pl", "Lekarz123!", "Lekarz");

        // Rejestratorka
        await CreateUserIfNotExists(userManager, "rejestratorka@clinic.pl", "Rejestr123!", "Rejestratorka");
    }

    private static async Task CreateUserIfNotExists(
        UserManager<IdentityUser> userManager,
        string email,
        string password,
        string role)
    {
        if (await userManager.FindByEmailAsync(email) == null)
        {
            var user = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true // nie wymagamy potwierdzenia w dev
            };

            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Nie można utworzyć użytkownika {email}: {errors}");
            }
        }
    }
}
