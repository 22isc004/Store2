using System.ComponentModel.DataAnnotations.Schema;

namespace SneackersStore.Models
{
    // Ítem del carrito de compras (almacenado en sesión)
    public class CarritoItem
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public string? Imagen { get; set; }
        public string? Talla { get; set; }
        public string? Color { get; set; }
        public string? Marca { get; set; }

        // Subtotal del ítem
        [NotMapped]
        public decimal Subtotal => Precio * Cantidad;
    }
}
