using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SneackersStore.Data;
using SneackersStore.Models;
using SneackersStore.Models.ViewModels;

namespace SneackersStore.Controllers
{
    public class CuentaController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public CuentaController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // Página de login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // Procesar login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            var resultado = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RecordarMe, lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                // Redirigir a admin si es administrador
                var usuario = await _userManager.FindByEmailAsync(model.Email);
                if (usuario != null && await _userManager.IsInRoleAsync(usuario, "Admin"))
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos");
            return View(model);
        }

        // Página de registro
        [HttpGet]
        public IActionResult Registro()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View();
        }

        // Procesar registro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(RegistroViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuario = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                PhoneNumber = model.Telefono,
                FechaRegistro = DateTime.UtcNow,
                Activo = true
            };

            var resultado = await _userManager.CreateAsync(usuario, model.Password);
            if (resultado.Succeeded)
            {
                await _userManager.AddToRoleAsync(usuario, "Cliente");
                await _signInManager.SignInAsync(usuario, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in resultado.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        // Cerrar sesión
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // Perfil del usuario
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Perfil()
        {
            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null)
                return RedirectToAction("Login");

            var pedidosRecientes = await _context.Pedidos
                .Include(p => p.Items)
                .Where(p => p.UserId == usuario.Id)
                .OrderByDescending(p => p.FechaPedido)
                .Take(5)
                .ToListAsync();

            var viewModel = new PerfilViewModel
            {
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email ?? string.Empty,
                Telefono = usuario.PhoneNumber,
                Direccion = usuario.Direccion,
                Ciudad = usuario.Ciudad,
                CodigoPostal = usuario.CodigoPostal,
                PedidosRecientes = pedidosRecientes
            };

            return View(viewModel);
        }

        // Actualizar perfil
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Perfil(PerfilViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null)
                return RedirectToAction("Login");

            usuario.Nombre = model.Nombre;
            usuario.Apellido = model.Apellido;
            usuario.PhoneNumber = model.Telefono;
            usuario.Direccion = model.Direccion;
            usuario.Ciudad = model.Ciudad;
            usuario.CodigoPostal = model.CodigoPostal;

            await _userManager.UpdateAsync(usuario);

            TempData["Exito"] = "Perfil actualizado correctamente";
            return RedirectToAction("Perfil");
        }

        public IActionResult AccesoDenegado() => View();
    }
}
