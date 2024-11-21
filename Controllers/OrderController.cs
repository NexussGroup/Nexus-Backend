using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Order>>> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrderById(string id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound("Pedido não encontrado.");
            return Ok(order);
        }

        [HttpGet("user/{userId}")]
        public IActionResult GetOrdersByUserId(string userId)
        {
            try
            {
                var orders = _orderService.GetOrdersByUserId(userId);

                if (orders == null || !orders.Any())
                {
                    return NotFound(new { message = "Nenhum pedido encontrado para este usuário." });
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao buscar pedidos: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] Order order)
        {
            if (order == null || order.Items == null || !order.Items.Any())
                return BadRequest("Pedido inválido.");

            var createdOrder = await _orderService.CreateOrderAsync(order);
            return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.Id }, createdOrder);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(string id, [FromBody] Order updatedOrder)
        {
            if (updatedOrder == null || updatedOrder.Id != id)
                return BadRequest("Dados inválidos para atualização.");

            var isUpdated = await _orderService.UpdateOrderAsync(id, updatedOrder);
            if (!isUpdated)
                return NotFound("Pedido não encontrado.");
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(string id)
        {
            var isDeleted = await _orderService.DeleteOrderAsync(id);
            if (!isDeleted)
                return NotFound("Pedido não encontrado.");
            return NoContent();
        }
    }
}
