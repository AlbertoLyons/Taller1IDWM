using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Taller_1_IDWM.src.Interfaces;

namespace Taller_1_IDWM.src.Controllers
{
    [Route("api/receipts")]
    [ApiController]
    [Authorize]
    public class ReceiptController : ControllerBase
    {
        private readonly IReceiptRepository _receiptRepository;
        private readonly IReceiptProductRepository _receiptProductRepository;
        private readonly IUserRepository _userRepository;


        public ReceiptController(IReceiptRepository receiptRepository, IReceiptProductRepository receiptProductRepository, IUserRepository userRepository)
        {
            _receiptRepository = receiptRepository;
            _receiptProductRepository = receiptProductRepository;
            _userRepository = userRepository;
        }
        /// <summary>
        /// Obtiene los recibos de la base de datos.
        /// </summary>
        /// <param name="pageNumber">Numero de paginas</param>
        /// <param name="pageSize">Tamaño de la pagina</param>
        /// <param name="startDate">Desde donde empieza las fechas</param>
        /// <param name="endDate">Hasta donde termina</param>
        /// <returns>Recibos de la base de datos</returns>
        /// <status code="200">Si los recibos se obtienen exitosamente.</status>
        /// <status code="400">Si ocurre un error al obtener los recibos.</status>
        [HttpGet("GetAllReceipts")]
        [Authorize(Roles = "Admin")]
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
        /// <summary>
        /// Obtiene la boleta por Id.
        /// </summary>
        /// <param name="id">Id de la boleta</param>
        /// <returns>Boleta</returns>
        /// <status code="200">Si la boleta se obtiene exitosamente.</status>
        /// <status code="400">Si ocurre un error al obtener la boleta.</status>
        [HttpGet("GetAllReceiptsProducts")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll(int id)
        {
            try{
                var receiptsProducts = await _receiptProductRepository.GetByReceiptId(id);
                var response = new
                {
                    Message = "Registros obtenidos exitosamente.",
                    Receipts = receiptsProducts
                };
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(new {message = e.Message});
            }
        }
        /// <summary>
        /// Obtiene el historial de compras del usuario.
        /// </summary>
        /// <returns>Las boletas del usuario</returns>
        /// <status code="200">Si las boletas se obtienen exitosamente.</status>
        /// <status code="400">Si ocurre un error al obtener las boletas.</status>
        [HttpGet("GetOrderHistory")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetOrderHistory()
        {
            try{
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var receipts = await _receiptRepository.GetOrderHistory(userId);
                receipts = receipts.OrderByDescending(r => r.BoughtAt).ToList();
                var response = new
                {
                    Message = "Registros obtenidos exitosamente.",
                    Receipts = receipts
                };
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(new {message = e.Message});
            }
        }
        /// <summary>
        /// Obitne las boletas por el nombre de usuario.
        /// </summary>
        /// <param name="userName">Nombre del usuario</param>
        /// <returns>Boletas del usuario buscado</returns>
        /// <status code="200">Si las boletas se obtienen exitosamente.</status>
        /// <status code="400">Si ocurre un error al obtener las boletas.</status>
        [HttpGet("GetByUserName")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByUserName(string userName)
        {
            try{
                var users = await _userRepository.GetByUserName(userName);
                var receipts = await _receiptRepository.GetByUserName(users.ToList());
                var response = new
                {
                    Message = "Registros obtenidos exitosamente.",
                    Receipts = receipts
                };
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(new {message = e.Message});
            }
        }
    }
}