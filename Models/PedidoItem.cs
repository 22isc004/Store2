using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SneackersStore.Models
{
    // Línea de producto dentro de un pedido
    public class PedidoItem
    {
        public int Id { get; set; }

        [Required]
        public int PedidoId { get; set; }

        public Pedido? Pedido { get; set; }

        [Required]
        public int ProductoId { get; set; }

        public Producto? Producto { get; set; }

        // Cantidad pedida
        [Required]
        [Range(1, 100, ErrorMessage = "La cantidad debe ser entre 1 y 100")]
        public int Cantidad { get; set; }

        // Precio al momento de compra (puede variar después)
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Precio Unitario")]
        public decimal PrecioUnitario { get; set; }

        // Talla seleccionada
        [StringLength(20)]
        [Display(Name = "Talla")]
        public string? Talla { get; set; }

        // Color seleccionado
        [StringLength(50)]
        [Display(Name = "Color")]
        public string? Color { get; set; }

        // Subtotal calculado
        [NotMapped]
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }
}
