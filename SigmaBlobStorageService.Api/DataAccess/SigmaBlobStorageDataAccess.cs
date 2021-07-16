using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.IO.Compression;

namespace SigmaBlobStorageService.Api.DataAccess
{
    public class SigmaBlobStorageDataAccess : ISigmaBlobStorageDataAcess
    {
        public SigmaBlobStorageDataAccess(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public byte[] GetFileByPath(string path, string fileName)
        {
            using (var stream = GetFileStream(path))
            using (var zip = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                var entry = zip.GetEntry(fileName);
                using (var innerFile = entry.Open())
                {
                    using(var ms = new MemoryStream())
                    {
                        innerFile.CopyTo(ms);
                        return ms.ToArray();
                    }
                }
            }
        }

        private Stream GetFileStream(string path)
        {
            var container = GetCloudBlobContainer();
            var blob = container.GetBlockBlobReference(path);

            var task = blob.OpenReadAsync();
            task.Wait();

            return task.Result;
        }

        private CloudBlobContainer GetCloudBlobContainer()
        {
            var account = CloudStorageAccount.Parse(Configuration.GetConnectionString("BlobConnectionString"));
            var blobClient = account.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(Configuration.GetConnectionString("ContainerName"));

            return container;
        }
    }
}
