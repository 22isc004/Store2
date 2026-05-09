using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SneackersStore.Data;
using SneackersStore.Models;
using SneackersStore.Models.ViewModels;

namespace SneackersStore.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Panel principal con métricas del negocio
        public async Task<IActionResult> Index()
        {
            var hoy = DateTime.UtcNow.Date;
            var inicioSemana = hoy.AddDays(-(int)hoy.DayOfWeek);
            var inicioMes = new DateTime(hoy.Year, hoy.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            var viewModel = new DashboardViewModel
            {
                TotalProductos = await _context.Productos.CountAsync(),
                ProductosActivos = await _context.Productos.CountAsync(p => p.Activo),
                TotalCategorias = await _context.Categorias.CountAsync(),
                TotalPedidos = await _context.Pedidos.CountAsync(),
                PedidosPendientes = await _context.Pedidos.CountAsync(p => p.Estado == EstadoPedido.Pendiente),
                TotalUsuarios = await _context.Users.CountAsync(),

                VentasTotales = await _context.Pedidos
                    .Where(p => p.Estado != EstadoPedido.Cancelado)
                    .SumAsync(p => (decimal?)p.Total) ?? 0,

                VentasHoy = await _context.Pedidos
                    .Where(p => p.Estado != EstadoPedido.Cancelado && p.FechaPedido.Date == hoy)
                    .SumAsync(p => (decimal?)p.Total) ?? 0,

                VentasSemana = await _context.Pedidos
                    .Where(p => p.Estado != EstadoPedido.Cancelado && p.FechaPedido >= inicioSemana)
                    .SumAsync(p => (decimal?)p.Total) ?? 0,

                VentasMes = await _context.Pedidos
                    .Where(p => p.Estado != EstadoPedido.Cancelado && p.FechaPedido >= inicioMes)
                    .SumAsync(p => (decimal?)p.Total) ?? 0,

                UltimosPedidos = await _context.Pedidos
                    .Include(p => p.Usuario)
                    .Include(p => p.Items)
                    .OrderByDescending(p => p.FechaPedido)
                    .Take(10)
                    .ToListAsync(),

                ProductosSinStock = await _context.Productos
                    .Include(p => p.Categoria)
                    .Where(p => p.Stock == 0 && p.Activo)
                    .Take(5)
                    .ToListAsync()
            };

            return View(viewModel);
        }
    }
}
