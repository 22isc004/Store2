using Microsoft.AspNetCore.Identity;
using SneackersStore.Models;

namespace SneackersStore.Data
{
    // Inicializa roles y usuario administrador por defecto
    public static class SeedData
    {
        public static async Task InicializarAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Crear roles si no existen
            string[] roles = { "Admin", "Cliente" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Crear usuario administrador por defecto
            const string adminEmail = "admin@sneackersstore.com";
            const string adminPassword = "Admin123!";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Nombre = "Administrador",
                    Apellido = "SneackersStore",
                    EmailConfirmed = true,
                    FechaRegistro = DateTime.UtcNow,
                    Activo = true
                };

                var resultado = await userManager.CreateAsync(admin, adminPassword);
                if (resultado.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
