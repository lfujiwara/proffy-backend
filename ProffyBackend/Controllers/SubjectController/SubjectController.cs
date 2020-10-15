using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProffyBackend.Controllers.SubjectController.Dto;
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

        [HttpPost]
        public async Task<ActionResult<Subject>> AddSubject([FromBody] AddSubjectDto data)
        {
            if (await _dataContext.Subjects.CountAsync(s => s.Id == data.Id) > 0) return new ConflictResult();
            var subject = new Subject {Id = data.Id};
            await _dataContext.Subjects.AddAsync(subject);
            await _dataContext.SaveChangesAsync();
            return subject;
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteSubject([FromBody] DeleteSubjectDto data)
        {
            try
            {
                var subject = await _dataContext.Subjects.FirstAsync(s => s.Id == data.Id);
                _dataContext.Remove(subject);
                await _dataContext.SaveChangesAsync();
            }
            catch (InvalidOperationException)
            {
                return new NotFoundResult();
            }

            return new NoContentResult();
        }
    }
}