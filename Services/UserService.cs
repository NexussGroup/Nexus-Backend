using MongoDB.Driver;
using MongoDB.Bson;
using backend.Models;

using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace backend.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IMongoDatabase database)
        {
            _users = database.GetCollection<User>("Users");
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _users.Find(user => true).ToListAsync();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _users.Find(user => user.Email == email).FirstOrDefaultAsync();
        }

       public string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),  
                new Claim(ClaimTypes.Email, user.Email),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("supersecretkeythatshouldbe128bitstotallyunique"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "backend",
                audience: "frontend",
                claims: claims,
                expires: DateTime.Now.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                return null;
            }

            return await _users.Find(user => user.Id == objectId).FirstOrDefaultAsync();
        }

        public async Task CreateUserAsync(User user) 
        {
            await _users.InsertOneAsync(user);
        }

        public async Task UpdateUserAsync(string id, User updateUser)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                return;
            }

            updateUser.Id = objectId;
            await _users.ReplaceOneAsync(user => user.Id == objectId, updateUser);
        }

        public async Task DeleteUserAsync(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                return;
            }

            await _users.DeleteOneAsync(user => user.Id == objectId);
        }
    }
}