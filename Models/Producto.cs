using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SneackersStore.Models
{
    // Modelo principal de producto para la tienda
    public class Producto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(200, ErrorMessage = "Máximo 200 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Máximo 2000 caracteres")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, 99999.99, ErrorMessage = "El precio debe ser mayor a 0")]
        [Display(Name = "Precio")]
        public decimal Precio { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Precio Oferta")]
        public decimal? PrecioOferta { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        [Display(Name = "Stock")]
        public int Stock { get; set; }

        [StringLength(300)]
        [Display(Name = "Imagen Principal")]
        public string? Imagen { get; set; }

        // Imágenes adicionales separadas por |
        [StringLength(1000)]
        [Display(Name = "Imágenes Adicionales")]
        public string? ImagenesAdicionales { get; set; }

        [StringLength(100)]
        [Display(Name = "Marca")]
        public string? Marca { get; set; }

        [StringLength(100)]
        [Display(Name = "Modelo")]
        public string? Modelo { get; set; }

        // Tallas disponibles separadas por , (ej: "38,39,40,41,42")
        [StringLength(200)]
        [Display(Name = "Tallas Disponibles")]
        public string? Tallas { get; set; }

        // Colores disponibles separados por ,
        [StringLength(200)]
        [Display(Name = "Colores")]
        public string? Colores { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        [Display(Name = "Destacado")]
        public bool Destacado { get; set; } = false;

        [Display(Name = "Nuevo")]
        public bool EsNuevo { get; set; } = true;

        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Display(Name = "Última Actualización")]
        public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;

        // Clave foránea de categoría
        [Required(ErrorMessage = "La categoría es obligatoria")]
        [Display(Name = "Categoría")]
        public int CategoriaId { get; set; }

        // Navegación a categoría
        public Categoria? Categoria { get; set; }

        // Items de pedido que incluyen este producto
        public ICollection<PedidoItem> PedidoItems { get; set; } = new List<PedidoItem>();

        // Precio efectivo (oferta si existe, sino precio normal)
        [NotMapped]
        public decimal PrecioEfectivo => PrecioOferta.HasValue && PrecioOferta.Value < Precio
            ? PrecioOferta.Value
            : Precio;

        // Porcentaje de descuento
        [NotMapped]
        public int PorcentajeDescuento => PrecioOferta.HasValue && PrecioOferta.Value < Precio
            ? (int)Math.Round((1 - PrecioOferta.Value / Precio) * 100)
            : 0;
    }
}
