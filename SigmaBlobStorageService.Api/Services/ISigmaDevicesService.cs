using System;
using System.Threading.Tasks;

namespace SigmaBlobStorageService.Api.Services
{
    public interface ISigmaDevicesService
    {
        /// <summary>
        /// Returns byte array of archived file(zip) with data for given device, sensor type and date
        /// </summary>
        /// <param name="deviceId">device name</param>
        /// <param name="date">date of stored data file</param>
        /// <param name="sensorType">name of sensor type</param>
        /// <returns></returns>
        Task<byte[]> GetSensorDataForDeviceByDateAsync(string deviceId, DateTime date, string sensorType);

        /// <summary>
        /// Returns byte array of archived file(zip) with data for all sensors of given device by given date
        /// </summary>
        /// <param name="deviceId">device name</param>
        /// <param name="date">date of stored data files</param>
        /// <returns></returns>
        Task<byte[]> GetDataForDeviceByDateAsync(string deviceId, DateTime date);
    }
}
