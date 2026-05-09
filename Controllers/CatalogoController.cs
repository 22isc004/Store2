using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SneackersStore.Data;
using SneackersStore.Models;
using SneackersStore.Models.ViewModels;

namespace SneackersStore.Controllers
{
    // Catálogo de productos con filtros y búsqueda
    public class CatalogoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int ProductosPorPagina = 12;

        public CatalogoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lista de productos con filtros (soporta AJAX para filtrado sin recarga)
        public async Task<IActionResult> Index(
            int? categoriaId,
            string? busqueda,
            decimal? precioMin,
            decimal? precioMax,
            string? marca,
            string? ordenar,
            bool soloOferta = false,
            bool soloDestacados = false,
            int pagina = 1)
        {
            var query = _context.Productos
                .Include(p => p.Categoria)
                .Where(p => p.Activo)
                .AsQueryable();

            // Aplicar filtros
            if (categoriaId.HasValue)
                query = query.Where(p => p.CategoriaId == categoriaId);

            if (!string.IsNullOrWhiteSpace(busqueda))
                query = query.Where(p =>
                    p.Nombre.Contains(busqueda) ||
                    (p.Descripcion != null && p.Descripcion.Contains(busqueda)) ||
                    (p.Marca != null && p.Marca.Contains(busqueda)));

            if (precioMin.HasValue)
                query = query.Where(p => p.Precio >= precioMin);

            if (precioMax.HasValue)
                query = query.Where(p => p.Precio <= precioMax);

            if (!string.IsNullOrWhiteSpace(marca))
                query = query.Where(p => p.Marca == marca);

            if (soloOferta)
                query = query.Where(p => p.PrecioOferta.HasValue && p.PrecioOferta < p.Precio);

            if (soloDestacados)
                query = query.Where(p => p.Destacado);

            // Ordenamiento
            query = ordenar switch
            {
                "precio_asc" => query.OrderBy(p => p.PrecioOferta ?? p.Precio),
                "precio_desc" => query.OrderByDescending(p => p.PrecioOferta ?? p.Precio),
                "nombre" => query.OrderBy(p => p.Nombre),
                "nuevos" => query.OrderByDescending(p => p.FechaCreacion),
                _ => query.OrderByDescending(p => p.Destacado).ThenByDescending(p => p.FechaCreacion)
            };

            var total = await query.CountAsync();
            var productos = await query
                .Skip((pagina - 1) * ProductosPorPagina)
                .Take(ProductosPorPagina)
                .ToListAsync();

            var marcasDisponibles = await _context.Productos
                .Where(p => p.Activo && p.Marca != null)
                .Select(p => p.Marca!)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();

            var viewModel = new ProductoFiltroViewModel
            {
                Productos = productos,
                Categorias = await _context.Categorias.Where(c => c.Activa).OrderBy(c => c.Orden).ToListAsync(),
                CategoriaId = categoriaId,
                Busqueda = busqueda,
                PrecioMin = precioMin,
                PrecioMax = precioMax,
                Marca = marca,
                Ordenar = ordenar,
                SoloOferta = soloOferta,
                SoloDestacados = soloDestacados,
                PaginaActual = pagina,
                TotalPaginas = (int)Math.Ceiling((double)total / ProductosPorPagina),
                TotalProductos = total,
                Marcas = marcasDisponibles
            };

            // Si es petición AJAX, devolver solo el parcial de productos
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_ListaProductos", viewModel);

            return View(viewModel);
        }

        // Detalle de un producto
        public async Task<IActionResult> Detalle(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id && p.Activo);

            if (producto == null)
                return NotFound();

            // Productos relacionados de la misma categoría
            var relacionados = await _context.Productos
                .Include(p => p.Categoria)
                .Where(p => p.CategoriaId == producto.CategoriaId && p.Id != id && p.Activo)
                .Take(4)
                .ToListAsync();

            ViewBag.Relacionados = relacionados;

            return View(producto);
        }

        // Búsqueda rápida por AJAX
        [HttpGet]
        public async Task<IActionResult> BusquedaRapida(string q)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
                return Json(new List<object>());

            var resultados = await _context.Productos
                .Where(p => p.Activo && (
                    p.Nombre.Contains(q) ||
                    (p.Marca != null && p.Marca.Contains(q))))
                .Take(6)
                .Select(p => new
                {
                    p.Id,
                    p.Nombre,
                    p.Marca,
                    Precio = p.PrecioOferta ?? p.Precio,
                    Imagen = p.Imagen ?? "/img/no-image.jpg"
                })
                .ToListAsync();

            return Json(resultados);
        }
    }
}
