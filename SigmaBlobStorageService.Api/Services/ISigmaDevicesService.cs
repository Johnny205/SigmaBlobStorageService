namespace SigmaBlobStorageService.Api.Services
{
    public interface ISigmaDevicesService
    {
        Microsoft.AspNetCore.Mvc.FileContentResult GetFileByPath(string path, string fileName);
    }
}
