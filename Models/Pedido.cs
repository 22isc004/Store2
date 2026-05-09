using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SneackersStore.Models
{
    // Estados posibles de un pedido
    public enum EstadoPedido
    {
        Pendiente = 0,
        Confirmado = 1,
        EnProceso = 2,
        Enviado = 3,
        Entregado = 4,
        Cancelado = 5
    }

    // Pedido realizado por un cliente
    public class Pedido
    {
        public int Id { get; set; }

        [StringLength(20)]
        [Display(Name = "Número de Pedido")]
        public string NumeroPedido { get; set; } = string.Empty;

        // FK del usuario que realizó el pedido
        [Required]
        public string UserId { get; set; } = string.Empty;

        public ApplicationUser? Usuario { get; set; }

        [Display(Name = "Estado")]
        public EstadoPedido Estado { get; set; } = EstadoPedido.Pendiente;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Subtotal")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Envío")]
        public decimal CostoEnvio { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total")]
        public decimal Total { get; set; }

        // Datos de envío del pedido
        [Required(ErrorMessage = "La dirección es obligatoria")]
        [StringLength(300)]
        [Display(Name = "Dirección de Envío")]
        public string DireccionEnvio { get; set; } = string.Empty;

        [Required(ErrorMessage = "La ciudad es obligatoria")]
        [StringLength(100)]
        [Display(Name = "Ciudad")]
        public string Ciudad { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Estado/Provincia")]
        public string? EstadoProvincia { get; set; }

        [StringLength(20)]
        [Display(Name = "Código Postal")]
        public string? CodigoPostal { get; set; }

        [StringLength(100)]
        [Display(Name = "País")]
        public string Pais { get; set; } = "México";

        [StringLength(20)]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [StringLength(500)]
        [Display(Name = "Notas")]
        public string? Notas { get; set; }

        [Display(Name = "Fecha del Pedido")]
        public DateTime FechaPedido { get; set; } = DateTime.UtcNow;

        [Display(Name = "Última Actualización")]
        public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;

        // Productos del pedido
        public ICollection<PedidoItem> Items { get; set; } = new List<PedidoItem>();
    }
}
