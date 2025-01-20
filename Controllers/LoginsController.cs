using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using System.Linq;
using WebAPI_Rest_ArleliotRoom.Data;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using WebAPI_Rest_ArleliotRoom.Models.DTOs;

namespace WebAPI_Rest_ArleliotRoom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginsController : ControllerBase
    {
        private readonly WebAPI_Rest_ArleliotRoomContext _context;
        private readonly string _jwtSecret;

        public LoginsController(WebAPI_Rest_ArleliotRoomContext context, IConfiguration configuration)
        {
            _context = context;
            _jwtSecret = configuration["JwtSettings:Secret"] ?? throw new InvalidOperationException("JWT Secret not found.");
        }

        // Endpoint pour la connexion
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.MotDePasse))
            {
                return BadRequest("Email et mot de passe sont obligatoires.");
            }

            // Recherche de l'utilisateur dans la base de données
            var user = await _context.Utilisateurs
                .Where(u => u.Email == model.Email)
                .SingleOrDefaultAsync();

            if (user == null || user.MotDePasse != HashPassword(model.MotDePasse))
            {
                return Unauthorized("Identifiants invalides.");
            }

            // Génération du token JWT
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role ?? "User")
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret)),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Retour de la réponse avec des informations supplémentaires
            return Ok(new
            {
                Token = tokenHandler.WriteToken(token),
                ExpiresIn = tokenDescriptor.Expires?.Subtract(DateTime.UtcNow).TotalSeconds,
                User = new
                {
                    user.Id,
                    user.Email,
                    user.Role
                }
            });
        }

        // Méthode pour hacher les mots de passe
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}


