using Azure;
using Azure.Storage.Blobs.Models;
using SigmaBlobStorageService.Api.Helpers;
using SigmaBlobStorageService.Api.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SigmaBlobStorageService.Api.DataAccess
{
    public class SigmaBlobStorageDataAccess : ISigmaBlobStorageDataAcess
    {
        private const string _zipName = "historical.zip";
        private IZipArchiveHelper ZipArchiveHelper { get; }
        private IBlobContainerProvider BlobContainerProvider { get; }

        public SigmaBlobStorageDataAccess(IZipArchiveHelper zipArchiveHelper, IBlobContainerProvider blobContainerProvider)
        {
            ZipArchiveHelper = zipArchiveHelper;
            BlobContainerProvider = blobContainerProvider;
        }        

        public async Task<FileModel> GetFileByPath(string path, string fileName)
        {
            using (var stream = await GetFileStreamAsync(path))
                return ZipArchiveHelper.GetFileFromZip(fileName, stream);
        }

        public List<string> GetSensorTypesForDevice(string deviceId)
        {
            var listOfDirectories = new List<string>();
            var blobs = BlobContainerProvider.GetBlobsByHierarchy(deviceId);

            foreach (Page<BlobHierarchyItem> page in blobs.AsPages())
                foreach (var blobPrefix in page.Values.Where(item => item.IsPrefix).Select(item => item.Prefix))
                {
                    var dir = GetBlobDirectoryFromBlobPrefix(blobPrefix, deviceId);
                    listOfDirectories.Add(dir);
                }

            return listOfDirectories;
        }

        private async Task<Stream> GetFileStreamAsync(string path)
        {
            var blob = BlobContainerProvider.GetCloudBlockBlobReferenceByName($"{path}/{_zipName}");

            if(!await blob.ExistsAsync())
                return null;
                
            return await blob.OpenReadAsync();
        }

        private string GetBlobDirectoryFromBlobPrefix(string blobPrefix, string deviceName)
        {
            return blobPrefix.Replace(deviceName, "").Replace("/", "").Replace("\\", "");
        }
    }
}