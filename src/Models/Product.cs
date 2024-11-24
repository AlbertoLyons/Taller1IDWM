namespace Taller_1_IDWM.src.Models
{
    public class Product
    {
        // El id del producto
        public int ID { get; set; }
        // El nombre del producto
        public string Name { get; set; } = string.Empty;
        // El tipo de producto
        public string Type { get; set; } = string.Empty;
        // El precio del producto
        public int Price { get; set; }
        // La cantidad de stock del producto
        public int Stock { get; set; }
        // La url de la imagen del producto
        public string ImageUrl { get; set; } = string.Empty;
    }
}