using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SigmaBlobStorageService.Api.Controllers;
using SigmaBlobStorageService.Api.Services;
using System;
using System.Net;
using System.Threading.Tasks;

namespace SigmBlobStorageService.Tests.Controllers
{
    public class DeviceControllerTest
    {
        private const string _deviceId = "deviceId";
        private const string _sensorType = "sensor";
        private readonly DateTime _date = DateTime.Now;

        [Test]
        public async Task TestGetDataForDeviceShouldCallServiceWithGetParameters()
        {
            var mockService = new Mock<ISigmaDevicesService>();
            mockService.Setup(x => x.GetDataForDeviceByDateAsync(_deviceId, _date)).Returns(Task.FromResult(null as byte[]));

            var controller = new DevicesController(mockService.Object);
            var result = controller.GetDataForDevice(_deviceId, _date);

            mockService.Verify(x => x.GetDataForDeviceByDateAsync(_deviceId, _date), Times.Once());
        }

        [Test]
        public async Task TestGetDataForDeviceShouldReturnNotFoundWhenNoFilesFound()
        {
            var mockService = new Mock<ISigmaDevicesService>();
            mockService.Setup(x => x.GetDataForDeviceByDateAsync(_deviceId, _date)).Returns(Task.FromResult(null as byte[]));

            var controller = new DevicesController(mockService.Object);
            var result = controller.GetDataForDevice(_deviceId, _date);

            Assert.AreEqual((int)HttpStatusCode.NotFound, ((NotFoundResult)result.Result).StatusCode);
        }

        [Test]
        public async Task TestGetDataForDeviceShouldReturnFileContentForGivenData()
        {
            var resultObject = new byte[1];
            var mockService = new Mock<ISigmaDevicesService>();
            mockService.Setup(x => x.GetDataForDeviceByDateAsync(_deviceId, _date)).Returns(Task.FromResult(resultObject));

            var controller = new DevicesController(mockService.Object);
            var result = controller.GetDataForDevice(_deviceId, _date);
            
            Assert.AreEqual(resultObject, ((FileContentResult)result.Result).FileContents);
        }

        [Test]
        public async Task TestGetSensorDataForDeviceShouldCallServiceWithGetParameters()
        {
            var mockService = new Mock<ISigmaDevicesService>();
            mockService.Setup(x => x.GetSensorDataForDeviceByDateAsync(_deviceId, _date, _sensorType)).Returns(Task.FromResult(null as byte[]));

            var controller = new DevicesController(mockService.Object);
            var result = controller.GetData(_deviceId, _date, _sensorType);

            mockService.Verify(x => x.GetSensorDataForDeviceByDateAsync(_deviceId, _date, _sensorType), Times.Once());
        }

        [Test]
        public async Task TestGetSensorDataForDeviceShouldReturnNotFoundWhenNoFilesFound()
        {
            var mockService = new Mock<ISigmaDevicesService>();
            mockService.Setup(x => x.GetSensorDataForDeviceByDateAsync(_deviceId, _date, _sensorType)).Returns(Task.FromResult(null as byte[]));

            var controller = new DevicesController(mockService.Object);
            var result = controller.GetData(_deviceId, _date, _sensorType);

            Assert.AreEqual((int)HttpStatusCode.NotFound, ((NotFoundResult)result.Result).StatusCode);
        }

        [Test]
        public async Task TestGetSensorDataForDeviceShouldReturnFileContentForGivenData()
        {
            var resultObject = new byte[1];
            var mockService = new Mock<ISigmaDevicesService>();
            mockService.Setup(x => x.GetSensorDataForDeviceByDateAsync(_deviceId, _date, _sensorType)).Returns(Task.FromResult(resultObject));

            var controller = new DevicesController(mockService.Object);
            var result = controller.GetData(_deviceId, _date, _sensorType);

            Assert.AreEqual(resultObject, ((FileContentResult)result.Result).FileContents);
        }
    }
}
