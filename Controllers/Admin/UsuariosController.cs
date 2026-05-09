using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SneackersStore.Models;

namespace SneackersStore.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsuariosController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsuariosController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // Lista de todos los usuarios registrados
        public async Task<IActionResult> Index(string? busqueda)
        {
            var usuarios = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(busqueda))
                usuarios = usuarios.Where(u =>
                    u.Email!.Contains(busqueda) ||
                    u.Nombre.Contains(busqueda) ||
                    u.Apellido.Contains(busqueda));

            return View(await usuarios.OrderByDescending(u => u.FechaRegistro).ToListAsync());
        }

        // Activar/Desactivar usuario (AJAX)
        [HttpPost]
        public async Task<IActionResult> ToggleActivo(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
                return Json(new { success = false });

            usuario.Activo = !usuario.Activo;
            await _userManager.UpdateAsync(usuario);

            return Json(new { success = true, activo = usuario.Activo });
        }

        // Cambiar rol del usuario
        [HttpPost]
        public async Task<IActionResult> CambiarRol(string id, string rol)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
                return Json(new { success = false });

            var rolesActuales = await _userManager.GetRolesAsync(usuario);
            await _userManager.RemoveFromRolesAsync(usuario, rolesActuales);
            await _userManager.AddToRoleAsync(usuario, rol);

            return Json(new { success = true });
        }
    }
}
