using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProffyBackend.Controllers.AvailableTimeWindowController.Dto;
using ProffyBackend.Models;

namespace ProffyBackend.Controllers.AvailableTimeWindowController
{
    [ApiController]
    [Route("available-time-windows")]
    public class AvailableTimeWindowController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public AvailableTimeWindowController([FromServices] DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<ActionResult<AvailableTimeWindow>> Add(AvailableTimeWindow availableTimeWindow)
        {
            if (availableTimeWindow.StartHour > availableTimeWindow.EndHour) return new BadRequestResult();
            if (await _dataContext.AvailableTimeWindows.CountAsync(a =>
                a.OwnerId == availableTimeWindow.OwnerId && a.WeekDay == availableTimeWindow.WeekDay) > 0)
                return new ConflictResult();
            await _dataContext.AvailableTimeWindows.AddAsync(availableTimeWindow);
            await _dataContext.SaveChangesAsync();
            return new CreatedResult("", availableTimeWindow);
        }

        [HttpPost]
        [Authorize(Role.User)]
        public async Task<ActionResult<AvailableTimeWindow>> Add([FromBody] AddDto data)
        {
            User owner;
            try
            {
                owner = await _dataContext.Users.FirstAsync(u => u.Id == data.OwnerId);
            }
            catch (InvalidOperationException)
            {
                return new NotFoundResult();
            }

            var availableTimeWindow = new AvailableTimeWindow
            {
                Owner = owner, OwnerId = owner.Id, StartHour = data.StartHour, EndHour = data.EndHour,
                WeekDay = data.WeekDay
            };

            return new CreatedResult("Created", await Add(availableTimeWindow));
        }

        [Authorize(Role.User)]
        public async Task<ActionResult<AvailableTimeWindow>> AddSelf([FromBody] AddDto data)
        {
            var ownerEmail = HttpContext.User.FindFirst(c => c.Type == ClaimTypes.Email).Value;
            User owner;
            try
            {
                owner = await _dataContext.Users.FirstAsync(u => u.Email == ownerEmail);
            }
            catch (InvalidOperationException)
            {
                return new NotFoundResult();
            }

            var availableTimeWindow = new AvailableTimeWindow
            {
                Owner = owner, OwnerId = owner.Id, StartHour = data.StartHour, EndHour = data.EndHour,
                WeekDay = data.WeekDay
            };

            return new CreatedResult("Created", await Add(availableTimeWindow));
        }

        [HttpDelete]
        [Authorize(Role.User)]
        public async Task<ActionResult> Delete([FromQuery] DeleteDto data)
        {
            var atw =
                await _dataContext.AvailableTimeWindows.FirstOrDefaultAsync(a => a.Id == data.AvailableTimeWindowId);
            if (atw is null)
                return new NotFoundResult();
            if (!HttpContext.User.IsInRole(Role.Admin))
            {
                var email = HttpContext.User.FindFirst(c => c.Type == ClaimTypes.Email).Value;
                var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null || atw.OwnerId != user.Id) return new ForbidResult();
            }

            _dataContext.AvailableTimeWindows.Remove(atw);
            await _dataContext.SaveChangesAsync();

            return new AcceptedResult();
        }
    }
}