using SigmaBlobStorageService.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SigmaBlobStorageService.Api.DataAccess
{
    public interface ISigmaBlobStorageDataAcess
    {
        /// <summary>
        /// Returns FileModel of File got from Blob Storage
        /// </summary>
        /// <param name="path">blob Prefix</param>
        /// <param name="fileName">blob Name</param>
        /// <returns><paramref/>FileModel of blob</returns>
        Task<FileModel> GetFileByPath(string path, string fileName);

        /// <summary>
        /// Returns list of prefixes(sensorTypes) under deviceId blob 
        /// </summary>
        /// <param name="deviceId">main blob Prefix</param>
        /// <returns><paramref/>List of sensor type names</returns>
        List<string> GetSensorTypesForDevice(string deviceId);
    }
}
