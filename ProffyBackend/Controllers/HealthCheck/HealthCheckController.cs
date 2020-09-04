using Microsoft.AspNetCore.Mvc;
using ProffyBackend.Controllers.HealthCheck.Dto;

namespace ProffyBackend.Controllers.HealthCheck
{
    [ApiController]
    [Route("[controller]")]
    public class HealthCheckController : ControllerBase
    {
        [HttpGet]
        public GetIndexResponseDto Get()
        {
            return new GetIndexResponseDto {Message = "System is running"};
        }
    }
}