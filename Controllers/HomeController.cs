using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SneackersStore.Data;
using SneackersStore.Models;
using System.Diagnostics;

namespace SneackersStore.Controllers
{
    // Controlador principal del sitio público
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Página de inicio: muestra destacados, nuevos y categorías
        public async Task<IActionResult> Index()
        {
            var categorias = await _context.Categorias
                .Where(c => c.Activa)
                .OrderBy(c => c.Orden)
                .ToListAsync();

            var productosDestacados = await _context.Productos
                .Include(p => p.Categoria)
                .Where(p => p.Activo && p.Destacado)
                .OrderByDescending(p => p.FechaCreacion)
                .Take(8)
                .ToListAsync();

            var productosNuevos = await _context.Productos
                .Include(p => p.Categoria)
                .Where(p => p.Activo && p.EsNuevo)
                .OrderByDescending(p => p.FechaCreacion)
                .Take(4)
                .ToListAsync();

            ViewBag.Categorias = categorias;
            ViewBag.ProductosDestacados = productosDestacados;
            ViewBag.ProductosNuevos = productosNuevos;

            return View();
        }

        // Página sobre nosotros
        public IActionResult Nosotros() => View();

        // Página de contacto
        public IActionResult Contacto() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }

    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
