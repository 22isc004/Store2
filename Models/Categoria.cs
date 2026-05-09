using System.ComponentModel.DataAnnotations;

namespace SneackersStore.Models
{
    // Categorías de productos: Tenis, Ropa, Perfumes, Gorras, Bolsos, Lentes, Accesorios
    public class Categoria
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Máximo 500 caracteres")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [StringLength(200)]
        [Display(Name = "Imagen")]
        public string? Imagen { get; set; }

        // Slug amigable para URL (ej: "tenis-sneakers")
        [StringLength(150)]
        public string? Slug { get; set; }

        [Display(Name = "Activa")]
        public bool Activa { get; set; } = true;

        public int Orden { get; set; } = 0;

        // Productos de esta categoría
        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
