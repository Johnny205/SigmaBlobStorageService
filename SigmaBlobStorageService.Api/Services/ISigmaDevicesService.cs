using System;

namespace SigmaBlobStorageService.Api.Services
{
    public interface ISigmaDevicesService
    {
        byte[] GetSensorDataForDeviceByDate(string device, string sensorType, DateTime date);
        byte[] GetDataForDeviceByDate(string device, DateTime date);
    }
}
