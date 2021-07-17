using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SigmaBlobStorageService.Api.Models;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace SigmaBlobStorageService.Api.DataAccess
{
    public class SigmaBlobStorageDataAccess : ISigmaBlobStorageDataAcess
    {
        private const string _zipName = "historical.zip";

        public SigmaBlobStorageDataAccess(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public FileModel GetFileByPath(string path, string fileName)
        {
            try
            {
                using (var stream = GetFileStreamAsync(path))
                using (var zip = new ZipArchive(stream, ZipArchiveMode.Read))
                {
                    var entry = zip.GetEntry(fileName);
                    if (entry == null) return null;

                    using (var innerFile = entry.Open())
                    {
                        using (var ms = new MemoryStream())
                        {
                            innerFile.CopyTo(ms);

                            return new FileModel { Name = fileName, FileContent = ms.ToArray() };
                        }
                    }
                }
            }
            catch(System.AggregateException)
            {
                throw;
            }
            
        }

        public List<string> GetSensorTypesForDevice(string device)
        {
            List<string> listOfDirectories = new List<string>();
            var container = GetBlobContainerClient();
            string prefix = $"{device}/";

            foreach (Page<BlobHierarchyItem> page in container.GetBlobsByHierarchy(prefix: prefix, delimiter: "/").AsPages())
            {
                foreach (var blobPrefix in page.Values.Where(item => item.IsPrefix).Select(item => item.Prefix))
                {
                    string dir = blobPrefix.Replace(prefix, "").Replace("/", "").Replace("\\", "");
                    listOfDirectories.Add(dir);
                }
            }

            return listOfDirectories;
        }

        private Stream GetFileStreamAsync(string path)
        {
            var container = GetCloudBlobContainer();
            var blob = container.GetBlockBlobReference($"{path}/{_zipName}");

            try
            {
                return blob.OpenReadAsync().Result;
            }
            catch(System.AggregateException)
            {
                throw;
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
    }
}

