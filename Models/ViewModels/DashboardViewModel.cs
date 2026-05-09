namespace SneackersStore.Models.ViewModels
{
    // ViewModel para el dashboard administrativo
    public class DashboardViewModel
    {
        public int TotalProductos { get; set; }
        public int ProductosActivos { get; set; }
        public int TotalCategorias { get; set; }
        public int TotalPedidos { get; set; }
        public int PedidosPendientes { get; set; }
        public int TotalUsuarios { get; set; }
        public decimal VentasTotales { get; set; }
        public decimal VentasHoy { get; set; }
        public decimal VentasSemana { get; set; }
        public decimal VentasMes { get; set; }
        public List<Pedido> UltimosPedidos { get; set; } = new();
        public List<Producto> ProductosMasVendidos { get; set; } = new();
        public List<Producto> ProductosSinStock { get; set; } = new();
    }
}
