using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SneackersStore.Data;
using SneackersStore.Models;

namespace SneackersStore.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoriasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriasController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categorias = await _context.Categorias
                .OrderBy(c => c.Orden)
                .ToListAsync();
            return View(categorias);
        }

        [HttpGet]
        public IActionResult Crear() => View(new Categoria());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                // Generar slug automático desde el nombre
                if (string.IsNullOrWhiteSpace(categoria.Slug))
                    categoria.Slug = GenerarSlug(categoria.Nombre);

                _context.Categorias.Add(categoria);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Categoría creada correctamente";
                return RedirectToAction("Index");
            }
            return View(categoria);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            return categoria == null ? NotFound() : View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Categoria categoria)
        {
            if (id != categoria.Id) return NotFound();

            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(categoria.Slug))
                    categoria.Slug = GenerarSlug(categoria.Nombre);

                _context.Update(categoria);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Categoría actualizada correctamente";
                return RedirectToAction("Index");
            }
            return View(categoria);
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
                return Json(new { success = false });

            var tieneProductos = await _context.Productos.AnyAsync(p => p.CategoriaId == id);
            if (tieneProductos)
                return Json(new { success = false, message = "No se puede eliminar: tiene productos asociados" });

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        private static string GenerarSlug(string texto)
        {
            return texto.ToLower()
                .Replace(" ", "-")
                .Replace("á", "a").Replace("é", "e").Replace("í", "i")
                .Replace("ó", "o").Replace("ú", "u").Replace("ñ", "n")
                .Replace("&", "y").Replace("/", "-")
                .Trim('-');
        }
    }
}
