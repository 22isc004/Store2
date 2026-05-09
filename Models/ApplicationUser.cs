using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SneackersStore.Models
{
    // Usuario extendido con campos adicionales de perfil
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Direccion { get; set; }

        [StringLength(100)]
        public string? Ciudad { get; set; }

        [StringLength(10)]
        public string? CodigoPostal { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        public bool Activo { get; set; } = true;

        // Relación con pedidos del usuario
        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}
