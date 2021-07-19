using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SigmaBlobStorageService.Api.Services;
using System;
using System.Threading.Tasks;

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


        /// <summary>
        /// Returns zipped file for given parameters or 404 File not found when file not found in blob storage
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="date"></param>
        /// <param name="sensorType"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileContentResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
        [Route("GetData/{deviceId}/{date}/{sensorType}")]
        public async Task<IActionResult> GetData(string deviceId, DateTime date, string sensorType)
        {
            var data =  await _sigmaDevicesService.GetSensorDataForDeviceByDateAsync(deviceId, date, sensorType);
            if (data == null)
                return NotFound();

            return File(data, "application/zip", "data.zip");
        }

        /// <summary>
        /// Returns zipped file of sensor data for given parameters or 404 File not found when no file found in blob storage
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="date"></param>
        /// <param name="sensorType"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileContentResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
        [Route("GetData/{deviceId}/{date}")]
        public async Task<IActionResult> GetDataForDevice(string deviceId, DateTime date)
        {
            var data = await _sigmaDevicesService.GetDataForDeviceByDateAsync(deviceId, date);
            if (data == null)
                return NotFound();

            return File(data, "application/zip", "data.zip");
        }
    }
}
