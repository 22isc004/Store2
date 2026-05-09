using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SneackersStore.Data;
using SneackersStore.Models;
using SneackersStore.Services;

namespace SneackersStore.Controllers
{
    // Controlador del carrito de compras con operaciones AJAX
    public class CarritoController : Controller
    {
        private readonly CarritoService _carritoService;
        private readonly ApplicationDbContext _context;

        public CarritoController(CarritoService carritoService, ApplicationDbContext context)
        {
            _carritoService = carritoService;
            _context = context;
        }

        // Vista principal del carrito
        public IActionResult Index()
        {
            var items = _carritoService.ObtenerItems();
            ViewBag.Total = _carritoService.ObtenerTotal();
            return View(items);
        }

        // Agregar producto al carrito (AJAX)
        [HttpPost]
        public async Task<IActionResult> Agregar(int productoId, int cantidad = 1, string? talla = null, string? color = null)
        {
            var producto = await _context.Productos.FindAsync(productoId);
            if (producto == null || !producto.Activo)
                return Json(new { success = false, message = "Producto no disponible" });

            if (producto.Stock < cantidad)
                return Json(new { success = false, message = "Stock insuficiente" });

            var item = new CarritoItem
            {
                ProductoId = producto.Id,
                Nombre = producto.Nombre,
                Precio = producto.PrecioOferta ?? producto.Precio,
                Cantidad = cantidad,
                Imagen = producto.Imagen,
                Talla = talla,
                Color = color,
                Marca = producto.Marca
            };

            _carritoService.AgregarItem(item);

            return Json(new
            {
                success = true,
                message = "Producto agregado al carrito",
                totalItems = _carritoService.ContarItems(),
                total = _carritoService.ObtenerTotal()
            });
        }

        // Actualizar cantidad (AJAX)
        [HttpPost]
        public IActionResult ActualizarCantidad(int productoId, string? talla, string? color, int cantidad)
        {
            _carritoService.ActualizarCantidad(productoId, talla, color, cantidad);

            var items = _carritoService.ObtenerItems();
            var item = items.FirstOrDefault(i =>
                i.ProductoId == productoId && i.Talla == talla && i.Color == color);

            return Json(new
            {
                success = true,
                subtotal = item?.Subtotal ?? 0,
                total = _carritoService.ObtenerTotal(),
                totalItems = _carritoService.ContarItems()
            });
        }

        // Eliminar ítem del carrito (AJAX)
        [HttpPost]
        public IActionResult Eliminar(int productoId, string? talla, string? color)
        {
            _carritoService.EliminarItem(productoId, talla, color);

            return Json(new
            {
                success = true,
                total = _carritoService.ObtenerTotal(),
                totalItems = _carritoService.ContarItems()
            });
        }

        // Vaciar carrito (AJAX)
        [HttpPost]
        public IActionResult Vaciar()
        {
            _carritoService.Limpiar();
            return Json(new { success = true });
        }

        // Mini carrito para el header (AJAX)
        [HttpGet]
        public IActionResult MiniCarrito()
        {
            var items = _carritoService.ObtenerItems();
            ViewBag.Total = _carritoService.ObtenerTotal();
            return PartialView("_MiniCarrito", items);
        }

        // Contador del carrito para el badge del header
        [HttpGet]
        public IActionResult Contador()
        {
            return Json(new { count = _carritoService.ContarItems() });
        }
    }
}
