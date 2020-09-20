using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ProffyBackend.Models;

namespace ProffyBackend.Services.Auth
{
    public static class AuthService
    {
        private static readonly string secret = "CHANGE_THIS_SECRET";

        private static string GenerateToken(IEnumerable<Claim> claims, DateTime expires)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
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

        public static string GenerateAccessToken(User user)
        {
            return AuthService.GenerateToken(
                new[]
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                },
                DateTime.UtcNow.AddMinutes(15)
            );
        }

        public static string GenerateRefreshToken(User user)
        {
            return AuthService.GenerateToken(
                new[]
                {
                    new Claim(ClaimTypes.Email, user.Email)
                },
                DateTime.UtcNow.AddHours(12)
            );
        }
    }
}