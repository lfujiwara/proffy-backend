using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProffyBackend.Controllers.UserController.Dto.Create;
using ProffyBackend.Models;

namespace ProffyBackend.Controllers.SubjectController
{
    [ApiController]
    [Route("subjects")]
    public class SubjectController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public SubjectController([FromServices] DataContext dataContext)
        {
            _dataContext = dataContext;
        }


        [HttpGet]
        [Authorize(Role.User)]
        public async Task<IEnumerable<string>> ListUsers()
        {
            return (await _dataContext.Subjects.ToListAsync()).Select(s => s.Id);
        }
    }
}