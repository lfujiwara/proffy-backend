using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.Authentication.ApiKey;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProffyBackend.Controllers.UserController.Dto.Create;
using ProffyBackend.Controllers.UserController.Dto.ValidateEmail;
using ProffyBackend.Controllers.UserController.Dto.ValidatePhoneNumber;
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

        private async Task<User> GetUser()
        {
            var userEmail = HttpContext.User.FindFirst(c => c.Type == ClaimTypes.Email).Value;
            User user;
            try
            {
                user = await _dataContext.Users.FirstAsync(u => u.Email == userEmail);
            }
            catch (InvalidOperationException)
            {
                return null;
            }

            return user;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = ApiKeyDefaults.AuthenticationScheme)]
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
        [Authorize(AuthenticationSchemes = ApiKeyDefaults.AuthenticationScheme)]
        public async Task<IEnumerable<User>> ListUsers()
        {
            return await _dataContext.Users.Include(u => u.AvailableTimeWindows).ToListAsync();
        }

        [Route("{id}")]
        [HttpPut]
        public async Task<ActionResult<User>> Update([FromRoute] Guid id, [FromBody] Dto.Update.Request requestData)
        {
            try
            {
                var user = await _dataContext.Users
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

        [HttpPut]
        [Authorize(Role.User)]
        public async Task<ActionResult<User>> SelfUpdate([FromBody] Dto.Update.Request requestData)
        {
            try
            {
                var email = User.Claims.First(claim => claim.Type == ClaimTypes.Email).Value;

                var user = await _dataContext.Users
                    .FirstAsync(u => u.Email == email);

                if (requestData.SubjectId != null &&
                    await _dataContext.Subjects.CountAsync(u => u.Id == requestData.SubjectId) == 0)
                    return new BadRequestResult();

                user.FirstName = requestData.FirstName;
                user.LastName = requestData.LastName;
                user.Biography = requestData.Biography;
                user.HourlyRate = requestData.HourlyRate;
                user.Currency = requestData.Currency;
                user.SubjectId = requestData.SubjectId;
                user.IsActive = requestData.IsActive;

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
        [AllowAnonymous]
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

        [Route("me")]
        [Authorize(Role.User)]
        [HttpGet]
        public async Task<ActionResult<User>> Me()
        {
            var email = User.Claims.First(u => u.Type == ClaimTypes.Email).Value;
            var user = await _dataContext.Users.FirstAsync(u => u.Email == email);

            return user;
        }

        [Route("validate-email")]
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> ValidateEmail([FromQuery] ValidateEmailRequestDto data)
        {
            var n = await _dataContext.Users.CountAsync(u => u.Email == data.Email);
            if (n > 0) return new ConflictResult();
            return new OkResult();
        }

        [AllowAnonymous]
        [Route("validate-phone-number")]
        [HttpGet]
        public async Task<ActionResult> ValidatePhoneNumber([FromQuery] ValidatePhoneNumberRequestDto data)
        {
            var n = await _dataContext.Users
                .CountAsync(u =>
                    u.PhoneNumber.Replace("+", "").Replace(" ", "") ==
                    data.PhoneNumber.Replace("+", "").Replace(" ", ""));
            if (n > 0) return new ConflictResult();
            return new OkResult();
        }

        [Route("proffys")]
        [HttpGet]
        [Authorize(Role.User)]
        public async Task<ActionResult<IEnumerable<User>>> ListProffys()
        {
            var user = await GetUser();
            if (user is null) return new NotFoundResult();

            var proffys = await _dataContext.Users.Include(u => u.AvailableTimeWindows).Where(u =>
                u.IsActive && u.SubjectId != null && u.AvailableTimeWindows.Count > 0 && u.Id != user.Id).ToListAsync();

            return proffys;
        }

        [Route("proffys/count")]
        [HttpGet]
        [Authorize(Role.User)]
        public async Task<ActionResult<int>> CountProffys()
        {
            var user = await GetUser();
            if (user is null) return new NotFoundResult();

            var proffyCount = await _dataContext.Users.Include(u => u.AvailableTimeWindows).Where(u =>
                u.IsActive && u.SubjectId != null && u.AvailableTimeWindows.Count > 0 && u.Id != user.Id).CountAsync();

            return proffyCount;
        }
    }
}