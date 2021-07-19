using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace SigmaBlobStorageService.Api.Helpers
{
    internal class BlobContainerProvider : IBlobContainerProvider
    {
        private IConfiguration Configuration { get; }
        private string BlobConnectionString => Configuration.GetConnectionString("BlobConnectionString");
        private string ContainerName => Configuration.GetConnectionString("ContainerName");

        public BlobContainerProvider(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public CloudBlockBlob GetCloudBlockBlobReferenceByName(string blobName)
        {
            var cloudBlobContainer = GetCloudBlobContainer();
            return cloudBlobContainer.GetBlockBlobReference(blobName);
        }

        public Pageable<BlobHierarchyItem> GetBlobsByHierarchy(string deviceId)
        {
            var blobContainerClient = new BlobContainerClient(BlobConnectionString, ContainerName);

            return blobContainerClient.GetBlobsByHierarchy(prefix: $"{deviceId}/", delimiter: "/");
        }

        private CloudBlobContainer GetCloudBlobContainer()
        {
            var account = CloudStorageAccount.Parse(BlobConnectionString);
            var blobClient = account.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(ContainerName);

            return container;
        }
    }
}
