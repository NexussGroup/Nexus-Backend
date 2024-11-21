using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class ProductDto
    {
        public string? Name { get; set; }  
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string? Category { get; set; }
        public string? ImageUrl { get; set; } 
    }

    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]  
        public string Id { get; set; } = Guid.NewGuid().ToString(); 

        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string? Category { get; set; }
        public string? ImageUrl { get; set; }
    }
}
