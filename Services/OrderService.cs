using backend.Models;
using MongoDB.Driver;

namespace backend.Services
{
    public class OrderService
    {
        private readonly IMongoCollection<Order> _orders;

        public OrderService(IMongoDatabase database)
        {
            _orders = database.GetCollection<Order>("Orders");
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _orders.Find(order => true).ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(string id)
        {
            return await _orders.Find(order => order.Id == id).FirstOrDefaultAsync();
        }

        public List<Order> GetOrdersByUserId(string userId)
        {
            return _orders.Find(order => order.UserId == userId).ToList();
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            order.CreatedAt = DateTime.UtcNow; 
            await _orders.InsertOneAsync(order);
            return order;
        }

        public async Task<bool> UpdateOrderAsync(string id, Order updatedOrder)
        {
            var result = await _orders.ReplaceOneAsync(order => order.Id == id, updatedOrder);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteOrderAsync(string id)
        {
            var result = await _orders.DeleteOneAsync(order => order.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }
    }
}
