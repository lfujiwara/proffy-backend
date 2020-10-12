using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCore.Authentication.ApiKey;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProffyBackend.Controllers.AdminController.Dto.AddApiKey;
using ProffyBackend.Models;

namespace ProffyBackend.Controllers.AdminController
{
    [ApiController]
    [Route("admin")]
    public class AdminController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public AdminController([FromServices] DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        [Route("api-keys")]
        [Authorize(AuthenticationSchemes = ApiKeyDefaults.AuthenticationScheme)]
        public async Task<IEnumerable<UserAPIKey>> GetApiKeys()
        {
            return await _dataContext.UserApiKeys.ToListAsync();
        }

        [HttpPost]
        [Route("api-keys")]
        public async Task<AddApiKeyResponseDto> AddApiKey([FromBody] AddApiKeyDto data)
        {
            var owner = await _dataContext.Users.FirstAsync(u => u.Email == data.OwnerEmail);
            var key = Guid.NewGuid().ToString();
            var apiKey = new UserAPIKey
                {Owner = owner, Key = BCrypt.Net.BCrypt.HashPassword(key), Description = data.Description};

            await _dataContext.UserApiKeys.AddAsync(apiKey);
            await _dataContext.SaveChangesAsync();

            return new AddApiKeyResponseDto {Key = key};
        }
    }
}