using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SigmaBlobStorageService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DevicesController : ControllerBase
    {

        private readonly ILogger<DevicesController> _logger;

        public DevicesController(ILogger<DevicesController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            return "get value";
        }
    }
}
