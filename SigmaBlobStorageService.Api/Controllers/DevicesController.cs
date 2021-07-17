using Microsoft.AspNetCore.Mvc;
using SigmaBlobStorageService.Api.Services;
using System;

namespace SigmaBlobStorageService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DevicesController : ControllerBase
    {
        private readonly ISigmaDevicesService _sigmaDevicesService;

        public DevicesController(ISigmaDevicesService sigmaDevicesService)
        {
            _sigmaDevicesService = sigmaDevicesService;
        }

        [HttpGet]
        [Route("GetData/{deviceName}/{date}/{sensorType}")]
        public IActionResult GetData(string deviceName, DateTime date, string sensorType)
        {
            var data =  _sigmaDevicesService.GetSensorDataForDeviceByDate(deviceName, sensorType, date);
            if (data != null)
            {
                return File(data, "application/zip", "data.zip");
            }

            return NotFound();
        }

        [HttpGet]
        [Route("GetData/{deviceName}/{date}")]
        public IActionResult GetDataForDevice(string deviceName, DateTime date)
        {
            var data = _sigmaDevicesService.GetDataForDeviceByDate(deviceName, date);
            if (data != null)
            {
                return File(data, "application/zip", "data.zip");
            }

            return NotFound();
        }
    }
}
