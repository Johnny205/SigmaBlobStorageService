namespace SigmaBlobStorageService.Api.DataAccess
{
    public interface ISigmaBlobStorageDataAcess
    {
        byte[] GetFileByPath(string path, string fileName);
    }
}
