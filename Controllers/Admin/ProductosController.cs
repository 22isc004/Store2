using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SneackersStore.Data;
using SneackersStore.Models;

namespace SneackersStore.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProductosController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // Lista de productos con búsqueda
        public async Task<IActionResult> Index(string? busqueda, int? categoriaId, bool? activo)
        {
            var query = _context.Productos.Include(p => p.Categoria).AsQueryable();

            if (!string.IsNullOrWhiteSpace(busqueda))
                query = query.Where(p => p.Nombre.Contains(busqueda) || (p.Marca != null && p.Marca.Contains(busqueda)));

            if (categoriaId.HasValue)
                query = query.Where(p => p.CategoriaId == categoriaId);

            if (activo.HasValue)
                query = query.Where(p => p.Activo == activo);

            ViewBag.Categorias = new SelectList(
                await _context.Categorias.OrderBy(c => c.Nombre).ToListAsync(), "Id", "Nombre");
            ViewBag.Busqueda = busqueda;
            ViewBag.CategoriaId = categoriaId;

            return View(await query.OrderByDescending(p => p.FechaCreacion).ToListAsync());
        }

        // Formulario de creación
        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            ViewBag.Categorias = new SelectList(
                await _context.Categorias.Where(c => c.Activa).OrderBy(c => c.Nombre).ToListAsync(),
                "Id", "Nombre");
            return View(new Producto());
        }

        // Procesar creación
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Producto producto, IFormFile? imagenFile)
        {
            if (ModelState.IsValid)
            {
                // Subir imagen si se proporcionó
                if (imagenFile != null && imagenFile.Length > 0)
                    producto.Imagen = await GuardarImagen(imagenFile);

                producto.FechaCreacion = DateTime.UtcNow;
                producto.FechaActualizacion = DateTime.UtcNow;

                _context.Productos.Add(producto);
                await _context.SaveChangesAsync();

                TempData["Exito"] = $"Producto '{producto.Nombre}' creado correctamente";
                return RedirectToAction("Index");
            }

            ViewBag.Categorias = new SelectList(
                await _context.Categorias.Where(c => c.Activa).OrderBy(c => c.Nombre).ToListAsync(),
                "Id", "Nombre", producto.CategoriaId);
            return View(producto);
        }

        // Formulario de edición
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            ViewBag.Categorias = new SelectList(
                await _context.Categorias.Where(c => c.Activa).OrderBy(c => c.Nombre).ToListAsync(),
                "Id", "Nombre", producto.CategoriaId);
            return View(producto);
        }

        // Procesar edición
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Producto producto, IFormFile? imagenFile)
        {
            if (id != producto.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var productoExistente = await _context.Productos.FindAsync(id);
                if (productoExistente == null) return NotFound();

                // Actualizar imagen si se subió una nueva
                if (imagenFile != null && imagenFile.Length > 0)
                    productoExistente.Imagen = await GuardarImagen(imagenFile);

                productoExistente.Nombre = producto.Nombre;
                productoExistente.Descripcion = producto.Descripcion;
                productoExistente.Precio = producto.Precio;
                productoExistente.PrecioOferta = producto.PrecioOferta;
                productoExistente.Stock = producto.Stock;
                productoExistente.Marca = producto.Marca;
                productoExistente.Modelo = producto.Modelo;
                productoExistente.Tallas = producto.Tallas;
                productoExistente.Colores = producto.Colores;
                productoExistente.CategoriaId = producto.CategoriaId;
                productoExistente.Activo = producto.Activo;
                productoExistente.Destacado = producto.Destacado;
                productoExistente.EsNuevo = producto.EsNuevo;
                productoExistente.FechaActualizacion = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                TempData["Exito"] = $"Producto '{producto.Nombre}' actualizado correctamente";
                return RedirectToAction("Index");
            }

            ViewBag.Categorias = new SelectList(
                await _context.Categorias.Where(c => c.Activa).OrderBy(c => c.Nombre).ToListAsync(),
                "Id", "Nombre", producto.CategoriaId);
            return View(producto);
        }

        // Detalle del producto
        public async Task<IActionResult> Detalles(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id);

            return producto == null ? NotFound() : View(producto);
        }

        // Eliminar producto (AJAX)
        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
                return Json(new { success = false, message = "Producto no encontrado" });

            // Desactivar en lugar de eliminar para preservar historial de pedidos
            producto.Activo = false;
            producto.FechaActualizacion = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Producto desactivado" });
        }

        // Activar/Desactivar producto (AJAX)
        [HttpPost]
        public async Task<IActionResult> ToggleActivo(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
                return Json(new { success = false });

            producto.Activo = !producto.Activo;
            producto.FechaActualizacion = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Json(new { success = true, activo = producto.Activo });
        }

        // Guardar imagen subida en wwwroot/img/productos/
        private async Task<string> GuardarImagen(IFormFile archivo)
        {
            var carpeta = Path.Combine(_environment.WebRootPath, "img", "productos");
            Directory.CreateDirectory(carpeta);

            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            var nombreArchivo = $"{Guid.NewGuid()}{extension}";
            var ruta = Path.Combine(carpeta, nombreArchivo);

            using var stream = new FileStream(ruta, FileMode.Create);
            await archivo.CopyToAsync(stream);

            return $"/img/productos/{nombreArchivo}";
        }
    }
}
