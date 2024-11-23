using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Taller_1_IDWM.src.Interfaces;

namespace Taller_1_IDWM.src.Controllers
{
    [Route("api/receipts")]
    [ApiController]
    public class ReceiptController : ControllerBase
    {
        private readonly IReceiptRepository _receiptRepository;
        private readonly IReceiptProductRepository _receiptProductRepository;


        public ReceiptController(IReceiptRepository receiptRepository, IReceiptProductRepository receiptProductRepository)
        {
            _receiptRepository = receiptRepository;
            _receiptProductRepository = receiptProductRepository;
        }

        [HttpGet("GetAllReceipts")]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                // Obtén todos los registros
                var receipts = await _receiptRepository.GetAllAsync();

                // Filtrar por rango de fechas si se proporcionan los parámetros
                if (startDate.HasValue)
                {
                    receipts = receipts.Where(r => r.BoughtAt >= startDate.Value).ToList();
                }

                if (endDate.HasValue)
                {
                    receipts = receipts.Where(r => r.BoughtAt <= endDate.Value).ToList();
                }

                // Ordenar los recibos desde los más recientes a los más antiguos
                receipts = receipts.OrderByDescending(r => r.BoughtAt).ToList();

                // Calcula la cantidad total de registros y páginas después del filtrado
                var totalRecords = receipts.Count();
                var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

                // Asegúrate de que el número de página esté dentro de un rango válido
                pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages));

                // Obtén los registros para la página solicitada
                var paginatedReceipts = receipts
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Devuelve la respuesta con la información de paginación
                var response = new
                {
                    Message = "Registros obtenidos exitosamente.",
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    Receipts = paginatedReceipts
                };

                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("GetAllReceiptsProducts")]
        public async Task<IActionResult> GetAll(int id)
        {
            try{
                var receiptsProducts = await _receiptProductRepository.GetByReceiptId(id);
                return Ok(receiptsProducts);
            }
            catch (Exception e)
            {
                return BadRequest(new {message = e.Message});
            }
        }

        [HttpGet("GetOrderHistory")]
                public async Task<IActionResult> GetOrderHistory(int id)
        {
            try{
                var receipts = await _receiptRepository.GetOrderHistory(id);
                return Ok(receipts);
            }
            catch (Exception e)
            {
                return BadRequest(new {message = e.Message});
            }
        }
    }
}