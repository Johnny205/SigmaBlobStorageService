using System;
using System.Threading.Tasks;

namespace SigmaBlobStorageService.Api.Services
{
    public interface ISigmaDevicesService
    {
        Task<byte[]> GetSensorDataForDeviceByDateAsync(string deviceId, DateTime date, string sensorType);
        Task<byte[]> GetDataForDeviceByDateAsync(string deviceId, DateTime date);
    }
}
