namespace Taller_1_IDWM.src.Models
{
    public class ReceiptProduct
    {
        // El id del recibo al que pertenece el producto (Clave foránea)
        public int ReceiptId { get; set; }
        // El id del producto que se compró (Clave foránea)
        public int ProductId { get; set; }
        // El precio unitario del producto
        public int UnitPrice { get; set; }
        // La cantidad de productos comprados
        public int Quantity { get; set; }
        // El precio total de la compra
        public int TotalPrice { get; set; }
    }
}