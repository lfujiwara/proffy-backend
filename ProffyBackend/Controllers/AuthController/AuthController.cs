using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProffyBackend.Controllers.AuthController.Dto.Login;
using ProffyBackend.Controllers.AuthController.Dto.Refresh;
using ProffyBackend.Models;
using ProffyBackend.Services.Auth;

namespace ProffyBackend.Controllers.AuthController
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly AuthService _authService;

        public AuthController([FromServices] DataContext dataContext, [FromServices] AuthService authService)
        {
            _dataContext = dataContext;
            _authService = authService;
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto requestData)
        {
            try
            {
                var user = await _dataContext.Users.FirstAsync(u => u.Email == requestData.Email);
                if (BCrypt.Net.BCrypt.Verify(requestData.Password, user.Password))
                {
                    var refreshToken = _authService.GenerateRefreshToken(user);

                    if (!requestData.DontSetCookie)
                    {
                        var cookieOptions = requestData.RememberMe
                            ? new CookieOptions {HttpOnly = true, Expires = DateTime.UtcNow.AddHours(12)}
                            : new CookieOptions {HttpOnly = true};
                        HttpContext.Response.Cookies.Append("proffy-refresh", refreshToken, cookieOptions);
                    }

                    return new LoginResponseDto
                    {
                        Refresh = refreshToken, Access = _authService.GenerateAccessToken(user)
                    };
                }

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
        [Route("logout")]
        [AllowAnonymous]
        public void Logout()
        {
            HttpContext.Response.Cookies.Delete("proffy-refresh");
        }

        [HttpPost]
        [Route("refresh")]
        [AllowAnonymous]
        public async Task<ActionResult<RefreshResponseDto>> Refresh([FromBody] RefreshRequestDto requestData)
        {
            try
            {
                var user = await _dataContext.Users.FirstAsync(u => u.Email == requestData.Email);
                if (BCrypt.Net.BCrypt.Verify(requestData.Password, user.Password))
                    return new RefreshResponseDto {Token = _authService.GenerateRefreshToken(user)};
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
        public async Task<ActionResult<RefreshResponseDto>> Access()
        {
            try
            {
                var email = User.Claims.First(u => u.Type == ClaimTypes.Email).Value;
                var user = await _dataContext.Users.FirstAsync(u => u.Email == email);
                return new RefreshResponseDto {Token = _authService.GenerateAccessToken(user)};
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
        [Authorize(AuthorizationPolicies.User)]
        public ActionResult<string> Check()
        {
            var claim = User.Claims.First(c => c.Type == ClaimTypes.Role);
            return claim.Value;
        }
    }
}