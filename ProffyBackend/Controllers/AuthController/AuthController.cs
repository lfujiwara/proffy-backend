using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProffyBackend.Controllers.AuthController.Dto.Login;
using ProffyBackend.Models;
using ProffyBackend.Services.Auth;

namespace ProffyBackend.Controllers.AuthController
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public AuthController([FromServices] DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult<Response>> Login([FromBody] Request requestData)
        {
            try
            {
                var user = await _dataContext.Users.FirstAsync(u => u.Email == requestData.Email);
                if (BCrypt.Net.BCrypt.Verify(requestData.Password, user.Password))
                    return new Response
                    {
                        Refresh = AuthService.GenerateRefreshToken(user), Access = AuthService.GenerateAccessToken(user)
                    };
                return new BadRequestResult();
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

        [HttpPost]
        [Route("refresh")]
        [AllowAnonymous]
        public async Task<ActionResult<Dto.Refresh.Response>> Refresh([FromBody] Dto.Refresh.Request requestData)
        {
            try
            {
                var user = await _dataContext.Users.FirstAsync(u => u.Email == requestData.Email);
                if (BCrypt.Net.BCrypt.Verify(requestData.Password, user.Password))
                    return new Dto.Refresh.Response {Token = AuthService.GenerateRefreshToken(user)};
                return new BadRequestResult();
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

        [HttpPost]
        [Route("access")]
        [Authorize(AuthorizationPolicies.RefreshToken)]
        public async Task<ActionResult<Dto.Refresh.Response>> Access()
        {
            try
            {
                var email = User.Claims.First(u => u.Type == ClaimTypes.Email).Value;
                var user = await _dataContext.Users.FirstAsync(u => u.Email == email);
                return new Dto.Refresh.Response {Token = AuthService.GenerateAccessToken(user)};
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

        [HttpGet]
        [Route("check")]
        [AuthorizeRoles(AuthorizationPolicies.SuperAdmin, AuthorizationPolicies.Admin, AuthorizationPolicies.User)]
        public ActionResult<string> Check()
        {
            var claim = User.Claims.First(c => c.Type == ClaimTypes.Role);
            return claim.Value;
        }
    }
}