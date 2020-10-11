using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ProffyBackend.Models;

namespace ProffyBackend.Services.Auth
{
    public class AuthService
    {
        private readonly string JWT_SECRET;

        public AuthService(string secret)
        {
            JWT_SECRET = secret;
        }

        private string GenerateToken(IEnumerable<Claim> claims, DateTime expires)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(JWT_SECRET);
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            var token = handler.CreateToken(descriptor);

            return handler.WriteToken(token);
        }

        public string GenerateAccessToken(User user)
        {
            return GenerateToken(
                new[]
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                },
                DateTime.UtcNow.AddMinutes(15)
            );
        }

        public  string GenerateRefreshToken(User user)
        {
            return GenerateToken(
                new[]
                {
                    new Claim(ClaimTypes.Email, user.Email)
                },
                DateTime.UtcNow.AddHours(12)
            );
        }
    }
}