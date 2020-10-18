using System;
using System.Collections.Generic;
using System.Linq;
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

        [HttpPut]
        [Authorize(Role.User)]
        public async Task<ActionResult<AvailableTimeWindow>> SetAvailableTimeWindow(
            [FromBody] SetAvailableTimeWindowDto data)
        {
            var user = await GetUser();
            if (user == null) return new NotFoundResult();
            AvailableTimeWindow atw;

            if (data.StartHour >= data.EndHour) return new BadRequestResult();

            try
            {
                atw = await _dataContext.AvailableTimeWindows.FirstAsync(a =>
                    a.OwnerId == user.Id && a.WeekDay == data.WeekDay);
                atw.StartHour = data.StartHour;
                atw.EndHour = data.EndHour;
                await _dataContext.SaveChangesAsync();
            }
            catch (InvalidOperationException)
            {
                atw = new AvailableTimeWindow
                    {OwnerId = user.Id, StartHour = data.StartHour, EndHour = data.EndHour, WeekDay = data.WeekDay};
                await _dataContext.AvailableTimeWindows.AddAsync(atw);
                await _dataContext.SaveChangesAsync();
            }

            return new ActionResult<AvailableTimeWindow>(atw);
        }

        [HttpDelete]
        [Authorize(Role.User)]
        public async Task<ActionResult> DeleteAvailableTimeWindow([FromQuery] DeleteAvailableTimeWindowDto data)
        {
            var user = await GetUser();
            if (user == null) return new NotFoundResult();

            var matchingTimeWindows = await
                _dataContext.AvailableTimeWindows.Where(a => a.OwnerId == user.Id && a.WeekDay == data.WeekDay)
                    .ToListAsync();

            _dataContext.AvailableTimeWindows.RemoveRange(matchingTimeWindows);
            await _dataContext.SaveChangesAsync();
            return new AcceptedResult();
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AvailableTimeWindow>>> List()
        {
            var user = await GetUser();
            if (user == null) return new NotFoundResult();

            return await _dataContext.AvailableTimeWindows.Where(atw => atw.OwnerId == user.Id).ToListAsync();
        }


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