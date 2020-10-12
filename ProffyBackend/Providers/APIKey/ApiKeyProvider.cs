using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.Authentication.ApiKey;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProffyBackend.Models;

namespace ProffyBackend.Providers.APIKey
{
    public class ApiKeyProvider : IApiKeyProvider
    {
        private readonly DataContext _dataContext;

        public ApiKeyProvider([FromServices] DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<IApiKey> ProvideAsync(string data)
        {
            // Todo - Optimize this query
            var userApiKeys = (await _dataContext.UserApiKeys.Include(k => k.Owner).ToListAsync())
                .Where(k => VerifyToken(data, k.Key)).ToList();
            if (userApiKeys.Count == 0) return null;
            var userApiKey = userApiKeys[0];

            var key = new ApiKey(userApiKey.Key,
                new[]
                {
                    new Claim(ClaimTypes.Email, userApiKey.Owner.Email),
                    new Claim(ClaimTypes.Role, userApiKey.Owner.Role)
                });
            return key;
        }

        private static bool VerifyToken(string text, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(text, hash);
        }
    }
}