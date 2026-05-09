using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SneackersStore.Data;
using SneackersStore.Models;
using SneackersStore.Models.ViewModels;
using SneackersStore.Services;

namespace SneackersStore.Controllers
{
    [Authorize]
    public class PedidoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly CarritoService _carritoService;
        private readonly UserManager<ApplicationUser> _userManager;

        public PedidoController(
            ApplicationDbContext context,
            CarritoService carritoService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _carritoService = carritoService;
            _userManager = userManager;
        }

        // Formulario de checkout
        public async Task<IActionResult> Checkout()
        {
            var items = _carritoService.ObtenerItems();
            if (!items.Any())
                return RedirectToAction("Index", "Carrito");

            var usuario = await _userManager.GetUserAsync(User);
            var subtotal = _carritoService.ObtenerTotal();
            var envio = subtotal >= 1500 ? 0 : 150;

            var viewModel = new CheckoutViewModel
            {
                Nombre = usuario?.Nombre ?? string.Empty,
                Apellido = usuario?.Apellido ?? string.Empty,
                Email = usuario?.Email ?? string.Empty,
                Telefono = usuario?.PhoneNumber ?? string.Empty,
                Direccion = usuario?.Direccion ?? string.Empty,
                Ciudad = usuario?.Ciudad ?? string.Empty,
                CodigoPostal = usuario?.CodigoPostal ?? string.Empty,
                Items = items,
                Subtotal = subtotal,
                CostoEnvio = envio,
                Total = subtotal + envio
            };

            return View(viewModel);
        }

        // Procesar el pedido
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var items = _carritoService.ObtenerItems();
            if (!items.Any())
                return RedirectToAction("Index", "Carrito");

            var subtotal = _carritoService.ObtenerTotal();
            var envio = subtotal >= 1500 ? 0 : 150;
            model.Items = items;
            model.Subtotal = subtotal;
            model.CostoEnvio = envio;
            model.Total = subtotal + envio;

            if (!ModelState.IsValid)
                return View(model);

            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null)
                return RedirectToAction("Login", "Cuenta");

            // Crear el pedido
            var pedido = new Pedido
            {
                NumeroPedido = GenerarNumeroPedido(),
                UserId = usuario.Id,
                Estado = EstadoPedido.Pendiente,
                Subtotal = subtotal,
                CostoEnvio = envio,
                Total = model.Total,
                DireccionEnvio = model.Direccion,
                Ciudad = model.Ciudad,
                EstadoProvincia = model.Estado,
                CodigoPostal = model.CodigoPostal,
                Pais = "México",
                Telefono = model.Telefono,
                Notas = model.Notas,
                FechaPedido = DateTime.UtcNow,
                FechaActualizacion = DateTime.UtcNow
            };

            // Agregar items al pedido y reducir stock
            foreach (var item in items)
            {
                var producto = await _context.Productos.FindAsync(item.ProductoId);
                if (producto != null)
                {
                    pedido.Items.Add(new PedidoItem
                    {
                        ProductoId = item.ProductoId,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = item.Precio,
                        Talla = item.Talla,
                        Color = item.Color
                    });

                    producto.Stock = Math.Max(0, producto.Stock - item.Cantidad);
                }
            }

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            // Limpiar carrito después del pedido
            _carritoService.Limpiar();

            return RedirectToAction("Confirmacion", new { id = pedido.Id });
        }

        // Confirmación del pedido
        public async Task<IActionResult> Confirmacion(int id)
        {
            var usuario = await _userManager.GetUserAsync(User);
            var pedido = await _context.Pedidos
                .Include(p => p.Items)
                    .ThenInclude(i => i.Producto)
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == usuario!.Id);

            if (pedido == null)
                return NotFound();

            return View(pedido);
        }

        // Mis pedidos
        public async Task<IActionResult> MisPedidos()
        {
            var usuario = await _userManager.GetUserAsync(User);
            var pedidos = await _context.Pedidos
                .Include(p => p.Items)
                .Where(p => p.UserId == usuario!.Id)
                .OrderByDescending(p => p.FechaPedido)
                .ToListAsync();

            return View(pedidos);
        }

        // Detalle de un pedido propio
        public async Task<IActionResult> Detalle(int id)
        {
            var usuario = await _userManager.GetUserAsync(User);
            var pedido = await _context.Pedidos
                .Include(p => p.Items)
                    .ThenInclude(i => i.Producto)
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == usuario!.Id);

            if (pedido == null)
                return NotFound();

            return View(pedido);
        }

        private static string GenerarNumeroPedido()
        {
            return $"SS-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}";
        }
    }
}
