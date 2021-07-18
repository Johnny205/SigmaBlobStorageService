﻿using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        [Route("GetData/{deviceId}/{date}/{sensorType}")]
        public async Task<IActionResult> GetData(string deviceId, DateTime date, string sensorType)
        {
            var data =  await _sigmaDevicesService.GetSensorDataForDeviceByDateAsync(deviceId, sensorType, date);
            if (data == null)
                return NotFound();

            return File(data, "application/zip", "data.zip");
        }

        [HttpGet]
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
