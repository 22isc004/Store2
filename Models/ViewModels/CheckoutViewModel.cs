using System.ComponentModel.DataAnnotations;

namespace SneackersStore.Models.ViewModels
{
    // ViewModel para el proceso de checkout
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(100)]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Email no válido")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [Phone(ErrorMessage = "Teléfono no válido")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [StringLength(300)]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La ciudad es obligatoria")]
        [StringLength(100)]
        [Display(Name = "Ciudad")]
        public string Ciudad { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Estado/Provincia")]
        public string? Estado { get; set; }

        [StringLength(20)]
        [Display(Name = "Código Postal")]
        public string? CodigoPostal { get; set; }

        [StringLength(500)]
        [Display(Name = "Notas del pedido")]
        public string? Notas { get; set; }

        // Items del carrito para mostrar resumen
        public List<CarritoItem> Items { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal CostoEnvio { get; set; }
        public decimal Total { get; set; }
    }
}
