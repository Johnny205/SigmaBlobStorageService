using SigmaBlobStorageService.Api.DataAccess;
using SigmaBlobStorageService.Api.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace SigmaBlobStorageService.Api.Services
{
    public class SigmaDevicesService : ISigmaDevicesService
    {
        private ISigmaBlobStorageDataAcess SigmaBlobStorageDataAccess { get; }
        
        public SigmaDevicesService(ISigmaBlobStorageDataAcess sigmaBlobStorageDataAcess)
        {
            SigmaBlobStorageDataAccess = sigmaBlobStorageDataAcess;
        }

        public async Task<byte[]> GetSensorDataForDeviceByDateAsync(string deviceId, DateTime date, string sensorType)
        {
            var path = $"{deviceId}/{sensorType}";
            var fileName = $"{date.Date:yyyy-MM-dd}.csv";
            var fileModel = await SigmaBlobStorageDataAccess.GetFileByPath(path, fileName);

            if (fileModel == null) 
                return null;

            return GetZippedFile(fileModel);
        }

        public async Task<byte[]> GetDataForDeviceByDateAsync(string deviceId, DateTime date)
        {
            var fileModels = new List<FileModel>();
            var fileName = $"{date.Date:yyyy-MM-dd}.csv";
            var sensorTypes = SigmaBlobStorageDataAccess.GetSensorTypesForDevice(deviceId);

            foreach (var sensorType in sensorTypes)
            {
                var path = $"{deviceId}/{sensorType}";
                var file = await SigmaBlobStorageDataAccess.GetFileByPath(path, fileName);

                if (file == null) 
                    continue;
                    
                file.Name = $"{sensorType}_{file.Name}";
                fileModels.Add(file);
            }

            return fileModels.Any() ? GetZippedFile(fileModels) : null;            
        }

        private byte[] GetZippedFile(FileModel file) => GetZippedFile(new List<FileModel> { file });

        private byte[] GetZippedFile(IEnumerable<FileModel> files)
        {
            using (var compressedFileStream = new MemoryStream())
            {
                ArchiveFiles(compressedFileStream, files);
                return compressedFileStream.ToArray();
            }
        }

        private void ArchiveFiles(Stream fileStream, IEnumerable<FileModel> fileModels)
        {
            using (var zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Create, false))
                foreach (var fileModel in fileModels)
                    AddFileToArchive(zipArchive, fileModel); 
        }

        private void AddFileToArchive(ZipArchive zipArchive, FileModel fileModel)
        {
            var zipEntry = zipArchive.CreateEntry(fileModel.Name);

            using (var originalFileStream = new MemoryStream(fileModel.FileContent))
            using (var zipEntryStream = zipEntry.Open())
                originalFileStream.CopyTo(zipEntryStream);
        }
    }
}
