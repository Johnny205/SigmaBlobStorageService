using SigmaBlobStorageService.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SigmaBlobStorageService.Api.DataAccess
{
    public interface ISigmaBlobStorageDataAcess
    {
        Task<FileModel> GetFileByPath(string path, string fileName);
        List<string> GetSensorTypesForDevice(string device);
    }
}
