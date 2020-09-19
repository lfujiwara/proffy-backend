using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProffyBackend.Controllers.UserController.Dto.Create;
using ProffyBackend.Models;

namespace ProffyBackend.Controllers.UserController
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public UserController([FromServices] DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Create([FromBody] Request requestData)
        {
            // Wrap logic into service layer later
            var newUser = requestData.ToUser();

            // Data validation (db-aware)
            if (await _dataContext.Users.CountAsync(u =>
                u.Email == requestData.Email || u.PhoneNumber == requestData.PhoneNumber) > 0)
                return new ConflictResult();

            // Data processing
            newUser.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password);

            // Feeding DB
            await _dataContext.Users.AddAsync(newUser);
            await _dataContext.SaveChangesAsync();

            return await _dataContext.Users.FirstAsync(u => u.Email == newUser.Email);
        }

        [HttpGet]
        public async Task<IEnumerable<User>> ListUsers()
        {
            return await _dataContext.Users.ToListAsync();
        }

        [Route("{id}")]
        [HttpPut]
        public async Task<ActionResult<User>> Update([FromRoute] Guid id, [FromBody] Dto.Update.Request requestData)
        {
            try
            {
                var user = await _dataContext.Users
                    .Include(u => u.Subject)
                    .FirstAsync(u => u.Id == id);

                if (requestData.SubjectId != null &&
                    await _dataContext.Subjects.CountAsync(u => u.Id == requestData.SubjectId) == 0)
                    return new BadRequestResult();

                user.FirstName = requestData.FirstName;
                user.LastName = requestData.LastName;
                user.Biography = requestData.Biography;
                user.HourlyRate = requestData.HourlyRate;
                user.Currency = requestData.Currency;
                user.SubjectId = requestData.SubjectId;

                await _dataContext.SaveChangesAsync();

                return user;
            }
            catch (InvalidOperationException)
            {
                return new NotFoundResult();
            }
            catch (ArgumentNullException)
            {
                return new NotFoundResult();
            }
        }

        [Route("{id}/change-password")]
        [HttpPut]
        public async Task<ActionResult> ChangePassword([FromRoute] Guid id,
            [FromBody] Dto.ChangePassword.Request requestData)
        {
            try
            {
                var user = await _dataContext.Users.FirstAsync(u => u.Id == id);
                if (!BCrypt.Net.BCrypt.Verify(requestData.Password, user.Password)) return new NotFoundResult();
                user.Password = BCrypt.Net.BCrypt.HashPassword(requestData.NewPassword);
                await _dataContext.SaveChangesAsync();
                return new AcceptedResult();
            }
            catch (InvalidOperationException)
            {
                return new NotFoundResult();
            }
            catch (ArgumentNullException)
            {
                return new NotFoundResult();
            }
        }

        [Route("{id}")]
        [HttpDelete]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            try
            {
                var user = await _dataContext.Users.FirstAsync(u => u.Id == id);
                _dataContext.Users.Remove(user);
                await _dataContext.SaveChangesAsync();
                return new AcceptedResult();
            }
            catch (InvalidOperationException)
            {
                return new NotFoundResult();
            }
            catch (ArgumentNullException)
            {
                return new NotFoundResult();
            }
        }
    }
}