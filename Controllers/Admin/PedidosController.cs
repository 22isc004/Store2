using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SneackersStore.Data;
using SneackersStore.Models;

namespace SneackersStore.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PedidosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PedidosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lista de todos los pedidos
        public async Task<IActionResult> Index(EstadoPedido? estado, string? busqueda)
        {
            var query = _context.Pedidos
                .Include(p => p.Usuario)
                .Include(p => p.Items)
                .AsQueryable();

            if (estado.HasValue)
                query = query.Where(p => p.Estado == estado);

            if (!string.IsNullOrWhiteSpace(busqueda))
                query = query.Where(p =>
                    p.NumeroPedido.Contains(busqueda) ||
                    (p.Usuario != null && (p.Usuario.Nombre.Contains(busqueda) || p.Usuario.Email!.Contains(busqueda))));

            ViewBag.EstadoFiltro = estado;
            return View(await query.OrderByDescending(p => p.FechaPedido).ToListAsync());
        }

        // Detalle de un pedido
        public async Task<IActionResult> Detalles(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Usuario)
                .Include(p => p.Items)
                    .ThenInclude(i => i.Producto)
                .FirstOrDefaultAsync(p => p.Id == id);

            return pedido == null ? NotFound() : View(pedido);
        }

        // Actualizar estado del pedido (AJAX)
        [HttpPost]
        public async Task<IActionResult> ActualizarEstado(int id, EstadoPedido estado)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
                return Json(new { success = false, message = "Pedido no encontrado" });

            pedido.Estado = estado;
            pedido.FechaActualizacion = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Json(new { success = true, nuevoEstado = estado.ToString() });
        }
    }
}
