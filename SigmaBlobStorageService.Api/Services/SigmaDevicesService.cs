using SigmaBlobStorageService.Api.DataAccess;
using SigmaBlobStorageService.Api.Helpers;
using SigmaBlobStorageService.Api.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SigmaBlobStorageService.Api.Services
{
    public class SigmaDevicesService : ISigmaDevicesService
    {
        private ISigmaBlobStorageDataAcess SigmaBlobStorageDataAccess { get; }
        private IZipArchiveHelper ZipArchiveHelper { get; }

        public SigmaDevicesService(ISigmaBlobStorageDataAcess sigmaBlobStorageDataAcess, IZipArchiveHelper zipArchiveHelper)
        {
            SigmaBlobStorageDataAccess = sigmaBlobStorageDataAcess;
            ZipArchiveHelper = zipArchiveHelper;
        }

        public async Task<byte[]> GetSensorDataForDeviceByDateAsync(string deviceId, DateTime date, string sensorType)
        {
            var path = $"{deviceId}/{sensorType}";
            var fileName = $"{date.Date:yyyy-MM-dd}.csv";
            var fileModel = await SigmaBlobStorageDataAccess.GetFileByPath(path, fileName);

            return ZipArchiveHelper.ZipFile(fileModel);
        }

        public async Task<byte[]> GetDataForDeviceByDateAsync(string deviceId, DateTime date)
        {
            var fileModels = new List<FileModel>();
            var fileName = $"{date.Date:yyyy-MM-dd}.csv";
            var sensorTypes = SigmaBlobStorageDataAccess.GetSensorTypesForDevice(deviceId);

            foreach (var sensorType in sensorTypes)
            {
                var path = $"{deviceId}/{sensorType}";
                var fileModel = await SigmaBlobStorageDataAccess.GetFileByPath(path, fileName);

                if (fileModel == null) 
                    continue;
                    
                fileModel.Name = $"{sensorType}_{fileModel.Name}";
                fileModels.Add(fileModel);
            }

            return ZipArchiveHelper.ZipFiles(fileModels);            
        }
    }
}
