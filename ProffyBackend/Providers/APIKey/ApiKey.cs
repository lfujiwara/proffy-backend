using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AspNetCore.Authentication.ApiKey;

namespace ProffyBackend.Models
{
    public class ApiKey : IApiKey
    {
        public ApiKey(string key, IReadOnlyCollection<Claim> claims = null)
        {
            Key = key;
            Claims = claims;
            OwnerName = claims!.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            OwnerName ??= "N/A";
        }

        public string Key { get; }
        public IReadOnlyCollection<Claim> Claims { get; }
        public string OwnerName { get; }
    }
}