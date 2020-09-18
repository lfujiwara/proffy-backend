using Microsoft.AspNetCore.Mvc;
using ProffyBackend.Controllers.HealthCheck.Dto;

namespace ProffyBackend.Controllers.HealthCheck
{
    [ApiController]
    [Route("[controller]")]
    public class HealthCheckController : ControllerBase
    {
        [HttpGet]
        public GetIndexDto Get()
        {
            return new GetIndexDto {Message = "System is running"};
        }
    }
}