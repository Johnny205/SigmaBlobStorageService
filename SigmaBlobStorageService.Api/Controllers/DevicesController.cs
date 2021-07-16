using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using SigmaBlobStorageService.Api.Services;
using System.Threading.Tasks;

namespace SigmaBlobStorageService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DevicesController : ControllerBase
    {
        private readonly ILogger<DevicesController> _logger;
        private readonly ISigmaDevicesService _sigmaDevicesService;

        public DevicesController(ILogger<DevicesController> logger, ISigmaDevicesService sigmaDevicesService)
        {
            _logger = logger;
            _sigmaDevicesService = sigmaDevicesService;
        }

        [HttpGet]
        public async Task<FileContentResult> Get()
        {
            return _sigmaDevicesService.GetFileByPath("dockan/humidity/historical.zip", "2010-11-01.csv");
        }
    }
}
