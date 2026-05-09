using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SneackersStore.Models;

namespace SneackersStore.Data
{
    // Contexto de base de datos principal con Identity integrado
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoItem> PedidoItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configuración de Categoria
            builder.Entity<Categoria>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Slug).IsUnique();
            });

            // Configuración de Producto
            builder.Entity<Producto>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Precio).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PrecioOferta).HasColumnType("decimal(18,2)");

                // Relación Producto -> Categoria
                entity.HasOne(e => e.Categoria)
                    .WithMany(c => c.Productos)
                    .HasForeignKey(e => e.CategoriaId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de Pedido
            builder.Entity<Pedido>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NumeroPedido).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Total).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Subtotal).HasColumnType("decimal(18,2)");
                entity.Property(e => e.CostoEnvio).HasColumnType("decimal(18,2)");

                // Relación Pedido -> Usuario
                entity.HasOne(e => e.Usuario)
                    .WithMany(u => u.Pedidos)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de PedidoItem
            builder.Entity<PedidoItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(18,2)");

                // Relación PedidoItem -> Pedido
                entity.HasOne(e => e.Pedido)
                    .WithMany(p => p.Items)
                    .HasForeignKey(e => e.PedidoId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación PedidoItem -> Producto (sin cascada para preservar historial)
                entity.HasOne(e => e.Producto)
                    .WithMany(p => p.PedidoItems)
                    .HasForeignKey(e => e.ProductoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Datos semilla: categorías iniciales
            builder.Entity<Categoria>().HasData(
                new Categoria { Id = 1, Nombre = "Tenis & Sneakers", Slug = "tenis-sneakers", Descripcion = "Los mejores tenis y sneakers de las mejores marcas", Activa = true, Orden = 1 },
                new Categoria { Id = 2, Nombre = "Ropa", Slug = "ropa", Descripcion = "Ropa urbana y de lifestyle", Activa = true, Orden = 2 },
                new Categoria { Id = 3, Nombre = "Perfumes", Slug = "perfumes", Descripcion = "Fragancias exclusivas", Activa = true, Orden = 3 },
                new Categoria { Id = 4, Nombre = "Gorras", Slug = "gorras", Descripcion = "Gorras y sombreros de moda", Activa = true, Orden = 4 },
                new Categoria { Id = 5, Nombre = "Bolsos", Slug = "bolsos", Descripcion = "Bolsos y mochilas de diseñador", Activa = true, Orden = 5 },
                new Categoria { Id = 6, Nombre = "Lentes", Slug = "lentes", Descripcion = "Lentes de sol y ópticos", Activa = true, Orden = 6 },
                new Categoria { Id = 7, Nombre = "Accesorios", Slug = "accesorios", Descripcion = "Accesorios de moda y lifestyle", Activa = true, Orden = 7 }
            );
        }
    }
}
