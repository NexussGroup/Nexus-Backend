using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{

    public class UserDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Street { get; set; }
        public string? Number { get; set; }
        public string? City { get; set; }
        public string? ImageUrl { get; set; }
        public string? Phone { get; set; }
        public bool IsAdmin { get; set; } 
    }

    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]  
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        [Required]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public required string Email { get; set; }

        public required string Password { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Street { get; set; }
        public string? Number { get; set; }
        public string? City { get; set; }
        public string? ImageUrl { get; set; }
        public string? Phone { get; set; }

        public bool IsAdmin { get; set; } = false;
    }
}