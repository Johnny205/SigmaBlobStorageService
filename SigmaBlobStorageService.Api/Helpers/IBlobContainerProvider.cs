using Azure;
using Azure.Storage.Blobs.Models;
using Microsoft.WindowsAzure.Storage.Blob;

namespace SigmaBlobStorageService.Api.Helpers
{
    public interface IBlobContainerProvider
    { 
        /// <summary>
        /// Provides CloudBlockBlob From Blob Storage that connection properties
        /// are set in appsettings.json file
        /// </summary>
        /// <param name="blobName">Name of Blob to get reference to</param>
        /// <returns></returns>
        CloudBlockBlob GetCloudBlockBlobReferenceByName(string blobName);

        /// <summary>
        /// Provides BlobHierarchy in PageableStructure under blob with given prefix
        /// </summary>
        /// <param name="deviceId">Main Blob Prefix Name</param>
        /// <returns></returns>
        Pageable<BlobHierarchyItem> GetBlobsByHierarchy(string deviceId);
    }
}
