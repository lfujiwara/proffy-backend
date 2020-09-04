using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ProffyBackend.Controllers.Users.Dto;
using ProffyBackend.Repositories;
using ProffyBackend.Models;

namespace ProffyBackend.Controllers.Users
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _usersRepository;

        public UsersController([FromServices] IUserRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }
        
        [HttpPost]
        public User PostIndex([FromBody] PostIndexRequestDto user)
        {
            _usersRepository.Save(new User {Email = user.Email, Password = user.Password, Locale = user.Locale});
            return _usersRepository.FindByEmail(user.Email); 
        }
    }
}