## Description
Little service to provide IoT data stored in archives on Blob Storage by REST Api

## Prerequisites

- .NET 5.0 SDK
- Configured `ConnectionString` and `ContainerName` in `appsettings.json`
```json
{
  "ConnectionStrings": {
    "BlobConnectionString": "<ConnectionString>",
    "ContainerName" :  "<ContainerName>"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}
```

## Usage


- ### api/devices/GetData/{deviceId}/{date}/{sensorType}
Returns zipped file for given parameters or 404 File not found when file not found in blob storage

- ### api/devices/GetData/{deviceId}/{date}
Returns zipped file of sensor data for given parameters or 404 File not found when no file found in blob storage
## License
[MIT](https://choosealicense.com/licenses/mit/)