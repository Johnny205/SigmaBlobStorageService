using System;
using System.Threading.Tasks;

namespace SigmaBlobStorageService.Api.Services
{
    public interface ISigmaDevicesService
    {
        Task<byte[]> GetSensorDataForDeviceByDateAsync(string device, string sensorType, DateTime date);
        Task<byte[]> GetDataForDeviceByDateAsync(string device, DateTime date);
    }
}
