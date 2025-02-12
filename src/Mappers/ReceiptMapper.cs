using Taller_1_IDWM.src.DTOs.Receipts;
using Taller_1_IDWM.src.Models;

namespace Taller_1_IDWM.src.Mappers
{
    public static class ReceiptMapper
    {
        public static ReceiptProductUserDTO ReceiptProductToReceiptProductUserDTO(this ReceiptProduct receiptProduct) 
        {
            return new ReceiptProductUserDTO
            {
                Price = receiptProduct.UnitPrice,
                Quantity = receiptProduct.Quantity,
                TotalPrice = receiptProduct.UnitPrice * receiptProduct.Quantity,
            };

        }
        public static ReceiptUserDTO ReceiptToReceiptUserDTO(this Receipt receipt)
        {
            return new ReceiptUserDTO
            {
                BoughtAt = receipt.BoughtAt,
                TotalPrice = receipt.TotalPrice,
            };
        }
        public static BoughtReceiptDTO ReceiptToBoughtReceiptDTO(this Receipt receipt)
        {
            return new BoughtReceiptDTO
            {
                BoughtAt = receipt.BoughtAt,
                TotalPrice = receipt.TotalPrice,
                Country = receipt.Country,
                City = receipt.City,
                County = receipt.County,
                Address = receipt.Address,
            };
        }
    }
}