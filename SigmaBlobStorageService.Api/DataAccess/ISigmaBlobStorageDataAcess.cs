using SigmaBlobStorageService.Api.Models;
using System.Collections.Generic;

namespace SigmaBlobStorageService.Api.DataAccess
{
    public interface ISigmaBlobStorageDataAcess
    {
        FileModel GetFileByPath(string path, string fileName);
        List<string> GetSensorTypesForDevice(string device);
    }
}
