using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        public string UserId { get; set; }
        public List<OrderItem> Items { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
    }

    public class OrderItem
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
