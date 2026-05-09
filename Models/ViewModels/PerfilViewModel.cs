using System.ComponentModel.DataAnnotations;

namespace SneackersStore.Models.ViewModels
{
    public class PerfilViewModel
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; } = string.Empty;

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [StringLength(200)]
        [Display(Name = "Dirección")]
        public string? Direccion { get; set; }

        [StringLength(100)]
        [Display(Name = "Ciudad")]
        public string? Ciudad { get; set; }

        [StringLength(10)]
        [Display(Name = "Código Postal")]
        public string? CodigoPostal { get; set; }

        // Pedidos recientes del usuario
        public List<Pedido> PedidosRecientes { get; set; } = new();
    }
}
