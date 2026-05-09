using System.Text.Json;
using SneackersStore.Models;

namespace SneackersStore.Services
{
    // Servicio para gestionar el carrito de compras en sesión
    public class CarritoService
    {
        private const string CarritoKey = "carrito_sneackers";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CarritoService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession Session => _httpContextAccessor.HttpContext!.Session;

        // Obtiene todos los ítems del carrito
        public List<CarritoItem> ObtenerItems()
        {
            var json = Session.GetString(CarritoKey);
            if (string.IsNullOrEmpty(json))
                return new List<CarritoItem>();

            return JsonSerializer.Deserialize<List<CarritoItem>>(json) ?? new List<CarritoItem>();
        }

        // Agrega o actualiza un producto en el carrito
        public void AgregarItem(CarritoItem item)
        {
            var items = ObtenerItems();
            // Buscar si ya existe el mismo producto con misma talla y color
            var existente = items.FirstOrDefault(i =>
                i.ProductoId == item.ProductoId &&
                i.Talla == item.Talla &&
                i.Color == item.Color);

            if (existente != null)
                existente.Cantidad += item.Cantidad;
            else
                items.Add(item);

            GuardarItems(items);
        }

        // Actualiza la cantidad de un ítem
        public void ActualizarCantidad(int productoId, string? talla, string? color, int cantidad)
        {
            var items = ObtenerItems();
            var item = items.FirstOrDefault(i =>
                i.ProductoId == productoId &&
                i.Talla == talla &&
                i.Color == color);

            if (item != null)
            {
                if (cantidad <= 0)
                    items.Remove(item);
                else
                    item.Cantidad = cantidad;
            }

            GuardarItems(items);
        }

        // Elimina un ítem del carrito
        public void EliminarItem(int productoId, string? talla, string? color)
        {
            var items = ObtenerItems();
            var item = items.FirstOrDefault(i =>
                i.ProductoId == productoId &&
                i.Talla == talla &&
                i.Color == color);

            if (item != null)
                items.Remove(item);

            GuardarItems(items);
        }

        // Vacía el carrito completo
        public void Limpiar()
        {
            Session.Remove(CarritoKey);
        }

        // Número total de ítems en el carrito
        public int ContarItems()
        {
            return ObtenerItems().Sum(i => i.Cantidad);
        }

        // Total del carrito
        public decimal ObtenerTotal()
        {
            return ObtenerItems().Sum(i => i.Subtotal);
        }

        private void GuardarItems(List<CarritoItem> items)
        {
            var json = JsonSerializer.Serialize(items);
            Session.SetString(CarritoKey, json);
        }
    }
}
