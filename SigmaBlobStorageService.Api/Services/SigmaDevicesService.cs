using SigmaBlobStorageService.Api.DataAccess;
using SigmaBlobStorageService.Api.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace SigmaBlobStorageService.Api.Services
{
    public class SigmaDevicesService : ISigmaDevicesService
    {
        public SigmaDevicesService(ISigmaBlobStorageDataAcess sigmaBlobStorageDataAcess)
        {
            _sigmaBlobStorageDataAcess = sigmaBlobStorageDataAcess;
        }

        public ISigmaBlobStorageDataAcess _sigmaBlobStorageDataAcess { get; }

        public byte[] GetSensorDataForDeviceByDate(string device, string sensorType, DateTime date)
        {
            _sigmaBlobStorageDataAcess.GetSensorTypesForDevice("");

            string path = $"{device}/{sensorType}";
            string fileName = $"{date.Date.ToString("yyyy-MM-dd")}.csv";

            try
            {
                var file = _sigmaBlobStorageDataAcess.GetFileByPath(path, fileName);
                byte[] zippedFile = GetZippedFile(new List<FileModel> { file });

                return zippedFile;
            }
            catch(AggregateException)
            {
                return null;
            }
        }

        public byte[] GetDataForDeviceByDate(string device, DateTime date)
        {
            var sensorTypes = _sigmaBlobStorageDataAcess.GetSensorTypesForDevice(device);
            List<FileModel> files = new List<FileModel>();

            try
            {
                foreach (var sensorType in sensorTypes)
                {
                    string path = $"{device}/{sensorType}";
                    string fileName = $"{date.Date.ToString("yyyy-MM-dd")}.csv";

                    FileModel file = _sigmaBlobStorageDataAcess.GetFileByPath(path, fileName);
                    file.Name = $"{sensorType}_{file.Name}";
                    files.Add(file);
                }

                var zippedFile = GetZippedFile(files);
                return zippedFile;
            }
            catch(AggregateException)
            {
                return null;
            }
        }

        private byte[] GetZippedFile(List<FileModel> files)
        {
            using (var compressedFileStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Create, false))
                {
                    foreach (var file in files)
                    {
                        var zipEntry = zipArchive.CreateEntry(file.Name);

                        using (var originalFileStream = new MemoryStream(file.FileContent))
                        using (var zipEntryStream = zipEntry.Open())
                        {
                            originalFileStream.CopyTo(zipEntryStream);
                        }
                    }
                }

                return compressedFileStream.ToArray();
            }
        }
    }
}
