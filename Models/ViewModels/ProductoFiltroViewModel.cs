namespace SneackersStore.Models.ViewModels
{
    // ViewModel para filtros del catálogo de productos
    public class ProductoFiltroViewModel
    {
        public List<Producto> Productos { get; set; } = new();
        public List<Categoria> Categorias { get; set; } = new();
        public int? CategoriaId { get; set; }
        public string? Busqueda { get; set; }
        public decimal? PrecioMin { get; set; }
        public decimal? PrecioMax { get; set; }
        public string? Marca { get; set; }
        public string? Ordenar { get; set; }
        public bool SoloOferta { get; set; }
        public bool SoloDestacados { get; set; }
        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; }
        public int TotalProductos { get; set; }
        public List<string> Marcas { get; set; } = new();
    }
}
