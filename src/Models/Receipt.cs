namespace Taller_1_IDWM.src.Models
{
    public class Receipt
    {
        // El id del recibo
        public int Id { get; set; }
        // El id del usuario que compro
        public int UserId { get; set; }
        // La fecha en la que se realizo la compra
        public DateTime BoughtAt { get; set; }
        // El total de precio de la compra
        public int TotalPrice { get; set; }
        // El país en el que se realizo la compra
        public string Country { get; set; } = string.Empty;
        // La ciudad en la que se realizo la compra
        public string City { get; set; } = string.Empty;
        // La comuna en el que se realizo la compra
        public string County { get; set; } = string.Empty;
        // La dirección en la que se realizo la compra
        public string Address { get; set; } = string.Empty;
    }
}