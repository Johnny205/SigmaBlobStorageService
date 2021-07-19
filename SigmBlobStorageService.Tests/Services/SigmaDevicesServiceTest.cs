using Moq;
using NUnit.Framework;
using SigmaBlobStorageService.Api.DataAccess;
using SigmaBlobStorageService.Api.Helpers;
using SigmaBlobStorageService.Api.Models;
using SigmaBlobStorageService.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SigmBlobStorageService.Tests.Services
{
    public class SigmaDevicesServiceTest
    {
        private const string _deviceId = "device";
        private const string _sensorType = "sensorType";
        private readonly DateTime _date = DateTime.Now;
        private readonly string _path = $"{_deviceId}/{_sensorType}";
        private string FileName => $"{_date.Date:yyyy-MM-dd}.csv";
        private Func<string, string> CustomPath = (string sensorType) => $"{_deviceId}/{sensorType}";

        [Test]
        public async Task ShouldCallSigmaBlobStorageDataAccessToGetFileByPathWithProperPathAndFileNameWhenGettingSensorDataForDeviceByDateAsync() 
        {
            var sigmaBlobStorageDataAccessMock = new Mock<ISigmaBlobStorageDataAcess>();
            var zipArchiveHelperMock = new Mock<IZipArchiveHelper>();
            sigmaBlobStorageDataAccessMock.Setup(x => x.GetFileByPath(_path, FileName)).Returns(Task.FromResult(null as FileModel));
            zipArchiveHelperMock.Setup(x => x.ZipFile(null)).Returns(null as byte[]);

            var sigmaDevicesService = new SigmaDevicesService(sigmaBlobStorageDataAccessMock.Object, zipArchiveHelperMock.Object);
            await sigmaDevicesService.GetSensorDataForDeviceByDateAsync(_deviceId, _date, _sensorType);

            sigmaBlobStorageDataAccessMock.Verify(x => x.GetFileByPath(_path, FileName), Times.Once());
        }

        [Test]
        public async Task ShouldCallZipArchiverHelperWhenGettingSensorDataForDeviceByDateAsyncWithFoundFileModel() 
        {
            var sigmaBlobStorageDataAccessMock = new Mock<ISigmaBlobStorageDataAcess>();
            var zipArchiveHelperMock = new Mock<IZipArchiveHelper>();
            sigmaBlobStorageDataAccessMock.Setup(x => x.GetFileByPath(_path, FileName)).Returns(Task.FromResult(null as FileModel));
            zipArchiveHelperMock.Setup(x => x.ZipFile(null)).Returns(null as byte[]);

            var sigmaDevicesService = new SigmaDevicesService(sigmaBlobStorageDataAccessMock.Object, zipArchiveHelperMock.Object);
            await sigmaDevicesService.GetSensorDataForDeviceByDateAsync(_deviceId, _date, _sensorType);

            zipArchiveHelperMock.Verify(x => x.ZipFile(null), Times.Once());
        }

        [Test]
        public async Task ShouldCallSigmaBlobStorageDataAccessToGetSensorWithGivenDevideIdWhenGettingDataForDeviceByDateAsync() 
        {
            var sigmaBlobStorageDataAccessMock = new Mock<ISigmaBlobStorageDataAcess>();
            var zipArchiveHelperMock = new Mock<IZipArchiveHelper>();
            sigmaBlobStorageDataAccessMock.Setup(x => x.GetFileByPath(_path, FileName)).Returns(Task.FromResult(null as FileModel));
            sigmaBlobStorageDataAccessMock.Setup(x => x.GetSensorTypesForDevice(_deviceId)).Returns(new List<string>() { "sensor1"});
            zipArchiveHelperMock.Setup(x => x.ZipFile(null)).Returns(null as byte[]);

            var sigmaDevicesService = new SigmaDevicesService(sigmaBlobStorageDataAccessMock.Object, zipArchiveHelperMock.Object);
            await sigmaDevicesService.GetDataForDeviceByDateAsync(_deviceId, _date);

            sigmaBlobStorageDataAccessMock.Verify(x => x.GetSensorTypesForDevice(_deviceId), Times.Once());
        }

        [Test]
        public async Task ShouldCallSigmaBlobStorageDataAccessToGetFileByPathWithProperPathAndFileNameForEachSensorTypeWhenGettingDataForDeviceByDateAsync() 
        {
            var listOfSensorTypes = new List<string>() { "sensor1", "sensor2" };
            var sigmaBlobStorageDataAccessMock = new Mock<ISigmaBlobStorageDataAcess>();
            var zipArchiveHelperMock = new Mock<IZipArchiveHelper>();
            sigmaBlobStorageDataAccessMock.Setup(x => x.GetFileByPath(_path, FileName)).Returns(Task.FromResult(null as FileModel));
            sigmaBlobStorageDataAccessMock.Setup(x => x.GetSensorTypesForDevice(_deviceId)).Returns(listOfSensorTypes);
            zipArchiveHelperMock.Setup(x => x.ZipFile(null)).Returns(null as byte[]);

            var sigmaDevicesService = new SigmaDevicesService(sigmaBlobStorageDataAccessMock.Object, zipArchiveHelperMock.Object);
            await sigmaDevicesService.GetDataForDeviceByDateAsync(_deviceId, _date);

            foreach(var sensorType in listOfSensorTypes)
            {
                sigmaBlobStorageDataAccessMock.Verify(x => x.GetFileByPath(CustomPath(sensorType), FileName), Times.Once());
            }
        }

        [Test]
        public async Task ShouldConcatEachFileNameWithSensorTypeWhenGettingDataForDeviceByDateAsync() 
        {
            var sensorTypes = new List<string>() { "sensor1", "sensor2" };
            var fileModel1 = new FileModel() { Name = FileName };
            var fileModel2 = new FileModel() { Name = FileName };
            var expectedFileName1 = $"{sensorTypes[0]}_{_date.Date:yyyy-MM-dd}.csv";
            var expectedFileName2 = $"{sensorTypes[1]}_{_date.Date:yyyy-MM-dd}.csv";
            List<FileModel> listOfFilesUsedToZip = null;
            var sigmaBlobStorageDataAccessMock = new Mock<ISigmaBlobStorageDataAcess>();
            var zipArchiveHelperMock = new Mock<IZipArchiveHelper>();
            sigmaBlobStorageDataAccessMock.Setup(x => x.GetFileByPath(CustomPath(sensorTypes[0]), FileName)).Returns(Task.FromResult(fileModel1));
            sigmaBlobStorageDataAccessMock.Setup(x => x.GetFileByPath(CustomPath(sensorTypes[1]), FileName)).Returns(Task.FromResult(fileModel2));
            sigmaBlobStorageDataAccessMock.Setup(x => x.GetSensorTypesForDevice(_deviceId)).Returns(sensorTypes);
            zipArchiveHelperMock.Setup(x => x.ZipFiles(It.IsAny<IEnumerable<FileModel>>())).Callback<IEnumerable<FileModel>>(x => listOfFilesUsedToZip = (List<FileModel>)x);

            var sigmaDevicesService = new SigmaDevicesService(sigmaBlobStorageDataAccessMock.Object, zipArchiveHelperMock.Object);
            await sigmaDevicesService.GetDataForDeviceByDateAsync(_deviceId, _date);

            Assert.IsNotNull(listOfFilesUsedToZip);
            Assert.AreEqual(2, listOfFilesUsedToZip.Count);
            List<string> concatenatedFileNames = listOfFilesUsedToZip.Select(e => e.Name).ToList();
            Assert.IsTrue(concatenatedFileNames.Contains(expectedFileName1));
            Assert.IsTrue(concatenatedFileNames.Contains(expectedFileName2));
        }

        [Test]
        public async Task ShouldCallZipArchiverHelperWhenGettingDataForDeviceByDateAsyncWithFoundFileModels() 
        {
            var sigmaBlobStorageDataAccessMock = new Mock<ISigmaBlobStorageDataAcess>();
            var zipArchiveHelperMock = new Mock<IZipArchiveHelper>();
            sigmaBlobStorageDataAccessMock.Setup(x => x.GetFileByPath(_path, FileName)).Returns(Task.FromResult(null as FileModel));
            sigmaBlobStorageDataAccessMock.Setup(x => x.GetSensorTypesForDevice(_deviceId)).Returns(new List<string>() { "sensor1" });
            zipArchiveHelperMock.Setup(x => x.ZipFiles(null)).Returns(null as byte[]);

            var sigmaDevicesService = new SigmaDevicesService(sigmaBlobStorageDataAccessMock.Object, zipArchiveHelperMock.Object);
            await sigmaDevicesService.GetDataForDeviceByDateAsync(_deviceId, _date);

            zipArchiveHelperMock.Verify(x => x.ZipFiles(new List<FileModel>()), Times.Once());
        }
    }
}
