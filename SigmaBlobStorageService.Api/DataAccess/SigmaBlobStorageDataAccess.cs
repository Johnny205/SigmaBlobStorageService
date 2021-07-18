using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SigmaBlobStorageService.Api.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace SigmaBlobStorageService.Api.DataAccess
{
    public class SigmaBlobStorageDataAccess : ISigmaBlobStorageDataAcess
    {
        private const string ZipName = "historical.zip";
        private IConfiguration Configuration { get; }

        public SigmaBlobStorageDataAccess(IConfiguration configuration)
        {
            Configuration = configuration;
        }        

        public async Task<FileModel> GetFileByPath(string path, string fileName)
        {
            try
            {
                using (var stream = await GetFileStreamAsync(path))
                using (var zip = new ZipArchive(stream, ZipArchiveMode.Read))
                {
                    var entry = zip.GetEntry(fileName);
                    if (entry == null) 
                        return null;

                    using (var innerFile = entry.Open())
                    using (var ms = new MemoryStream())
                    {
                        innerFile.CopyTo(ms);
                        return new FileModel { Name = fileName, FileContent = ms.ToArray() };
                    }
                }
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        public List<string> GetSensorTypesForDevice(string deviceId)
        {
            var listOfDirectories = new List<string>();
            var container = GetBlobContainerClient();

            foreach (Page<BlobHierarchyItem> page in container.GetBlobsByHierarchy(prefix: $"{deviceId}/", delimiter: "/").AsPages())
                foreach (var blobPrefix in page.Values.Where(item => item.IsPrefix).Select(item => item.Prefix))
                {
                    var dir = GetBlobDirectoryFromBlobPrefix(blobPrefix, deviceId);
                    listOfDirectories.Add(dir);
                }

            return listOfDirectories;
        }

        private async Task<Stream> GetFileStreamAsync(string path)
        {
            var container = GetCloudBlobContainer();
            var blob = container.GetBlockBlobReference($"{path}/{ZipName}");

            try
            {
                return await blob.OpenReadAsync();
            }
            catch(Exception ex) when (ex is AggregateException || ex is StorageException)
            {
                throw new FileNotFoundException();
            }
        }

        private CloudBlobContainer GetCloudBlobContainer()
        {
            var account = CloudStorageAccount.Parse(Configuration.GetConnectionString("BlobConnectionString"));
            var blobClient = account.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(Configuration.GetConnectionString("ContainerName"));

            return container;
        }

        private BlobContainerClient GetBlobContainerClient()
        {
            return new BlobContainerClient(Configuration.GetConnectionString("BlobConnectionString"), Configuration.GetConnectionString("ContainerName"));
        }

        private string GetBlobDirectoryFromBlobPrefix(string blobPrefix, string deviceName)
        {
            return blobPrefix.Replace(deviceName, "").Replace("/", "").Replace("\\", "");
        }
    }
}