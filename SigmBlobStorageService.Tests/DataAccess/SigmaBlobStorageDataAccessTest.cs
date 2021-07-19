using Azure;
using Azure.Storage.Blobs.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using Moq;
using NUnit.Framework;
using SigmaBlobStorageService.Api.DataAccess;
using SigmaBlobStorageService.Api.Helpers;
using SigmaBlobStorageService.Api.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SigmBlobStorageService.Tests.DataAccess
{
    public class SigmaBlobStorageDataAccessTest
    {
        private const string _path = "path";
        private const string _zipName = "historical.zip";
        private const string _fileName = "file123";
        private const string _deviceId = "device";
        private readonly string _blobName = $"{_path}/{_zipName}";
        private readonly List<string> _blobPrefixes = new List<string> { $"{_deviceId}/\\blob1", $"{_deviceId}/\\blob2" };

        [Test]
        public async Task ShouldCallBlobContainerProviderToGetCloudBlockBlobReferenceByNameWhenGetFileByPath() 
        {
            var zipArchiveHelperMock = new Mock<IZipArchiveHelper>();
            var cloudBlockBlobMock = new Mock<CloudBlockBlob>(new Uri("http://tempuri.org/blob"));
            var blobContainerProviderMock = new Mock<IBlobContainerProvider>();
            cloudBlockBlobMock.Setup(m => m.ExistsAsync()).ReturnsAsync(false);
            blobContainerProviderMock.Setup(x => x.GetCloudBlockBlobReferenceByName(_blobName)).Returns(cloudBlockBlobMock.Object);

            var sigmaBlobStorageDataAccess = new SigmaBlobStorageDataAccess(zipArchiveHelperMock.Object, blobContainerProviderMock.Object);
            sigmaBlobStorageDataAccess.GetFileByPath(_path, _fileName);

            blobContainerProviderMock.Verify(x => x.GetCloudBlockBlobReferenceByName(_blobName), Times.Once());
        }

        [Test]
        public async Task ShouldCallZipArchiverWhenGetFileByPath() 
        {
            var stream = new MemoryStream();
            var zipArchiveHelperMock = new Mock<IZipArchiveHelper>();
            var cloudBlockBlobMock = new Mock<CloudBlockBlob>(new Uri("http://tempuri.org/blob"));
            var blobContainerProviderMock = new Mock<IBlobContainerProvider>();
            zipArchiveHelperMock.Setup(x => x.GetFileFromZip(_fileName, stream)).Returns(new FileModel());
            cloudBlockBlobMock.Setup(m => m.ExistsAsync()).ReturnsAsync(false);
            blobContainerProviderMock.Setup(x => x.GetCloudBlockBlobReferenceByName(_blobName)).Returns(cloudBlockBlobMock.Object);

            var sigmaBlobStorageDataAccess = new SigmaBlobStorageDataAccess(zipArchiveHelperMock.Object, blobContainerProviderMock.Object);
            sigmaBlobStorageDataAccess.GetFileByPath(_path, _fileName);

            zipArchiveHelperMock.Verify(x => x.GetFileFromZip(_fileName, null), Times.Once());
        }

        [Test]
        public async Task ShouldReturnNullWhenGetFileByPathAndFileNotFound() 
        {
            var stream = new MemoryStream();
            var zipArchiveHelperMock = new Mock<IZipArchiveHelper>();
            var cloudBlockBlobMock = new Mock<CloudBlockBlob>(new Uri("http://tempuri.org/blob"));
            var blobContainerProviderMock = new Mock<IBlobContainerProvider>();
            zipArchiveHelperMock.Setup(x => x.GetFileFromZip(_fileName, stream)).Returns(null as FileModel);
            cloudBlockBlobMock.Setup(m => m.ExistsAsync()).ReturnsAsync(false);
            blobContainerProviderMock.Setup(x => x.GetCloudBlockBlobReferenceByName(_blobName)).Returns(cloudBlockBlobMock.Object);

            var sigmaBlobStorageDataAccess = new SigmaBlobStorageDataAccess(zipArchiveHelperMock.Object, blobContainerProviderMock.Object);
            var result = sigmaBlobStorageDataAccess.GetFileByPath(_path, _fileName);

            Assert.IsNull(result.Result);
        }

        [Test]
        public async Task ShouldCallBlobContainerProviderToGetBlobsByHierarchyWhenGetSensorTypesForDevice() 
        {
            var zipArchiveHelperMock = new Mock<IZipArchiveHelper>();
            var blobContainerProviderMock = new Mock<IBlobContainerProvider>();
            var cloudBlockBlobMock = new Mock<CloudBlockBlob>(new Uri("http://tempuri.org/blob"));
            cloudBlockBlobMock.Setup(m => m.ExistsAsync()).ReturnsAsync(false);
            blobContainerProviderMock.Setup(x => x.GetBlobsByHierarchy(_deviceId)).Returns(GetFakeHierarchyItems());

            var sigmaBlobStorageDataAccess = new SigmaBlobStorageDataAccess(zipArchiveHelperMock.Object, blobContainerProviderMock.Object);
            sigmaBlobStorageDataAccess.GetSensorTypesForDevice(_deviceId);

            blobContainerProviderMock.Verify(x => x.GetBlobsByHierarchy(_deviceId), Times.Once());
        }

        [Test]
        public async Task ShouldReturnListOfBlobPrefixesUnderGivenDeviceIdWhenGetSensorTypesForDevice()
        {
            var zipArchiveHelperMock = new Mock<IZipArchiveHelper>();
            var cloudBlockBlobMock = new Mock<CloudBlockBlob>(new Uri("http://tempuri.org/blob"));
            var blobContainerProviderMock = new Mock<IBlobContainerProvider>();
            cloudBlockBlobMock.Setup(m => m.ExistsAsync()).ReturnsAsync(false);
            blobContainerProviderMock.Setup(x => x.GetBlobsByHierarchy(_deviceId)).Returns(GetFakeHierarchyItems());

            var sigmaBlobStorageDataAccess = new SigmaBlobStorageDataAccess(zipArchiveHelperMock.Object, blobContainerProviderMock.Object);
            var returnList = sigmaBlobStorageDataAccess.GetSensorTypesForDevice(_deviceId);

            Assert.AreEqual(_blobPrefixes.Count, returnList.Count);
        }

        [Test]
        public async Task ShouldEscapedeviceAndBackSlashesAndForwardSlashesOfBlobPrefixesWhenGetSensorTypesForDevice() 
        {
            var zipArchiveHelperMock = new Mock<IZipArchiveHelper>();
            var cloudBlockBlobMock = new Mock<CloudBlockBlob>(new Uri("http://tempuri.org/blob"));
            var blobContainerProviderMock = new Mock<IBlobContainerProvider>();
            cloudBlockBlobMock.Setup(m => m.ExistsAsync()).ReturnsAsync(false);
            blobContainerProviderMock.Setup(x => x.GetBlobsByHierarchy(_deviceId)).Returns(GetFakeHierarchyItems());

            var sigmaBlobStorageDataAccess = new SigmaBlobStorageDataAccess(zipArchiveHelperMock.Object, blobContainerProviderMock.Object);
            var returnList = sigmaBlobStorageDataAccess.GetSensorTypesForDevice(_deviceId);

            Assert.IsFalse(returnList.Any(e => e.Contains("\\") || e.Contains("/") || e.Contains(_deviceId)));
        }

        private Pageable<BlobHierarchyItem> GetFakeHierarchyItems()
        {
            var blobHierarchyItems = new List<BlobHierarchyItem>();
            foreach(var blobPrefix in _blobPrefixes)
            {
                BlobHierarchyItem blobHierarchyItem = BlobsModelFactory.BlobHierarchyItem(blobPrefix, BlobsModelFactory.BlobItem(@"/\blobName", false));
                blobHierarchyItems.Add(blobHierarchyItem);
            }
            
            var page = Page<BlobHierarchyItem>.FromValues(blobHierarchyItems.AsReadOnly(), "", null);
            var pagesList = new List<Page<BlobHierarchyItem>>() { page };

            return Pageable<BlobHierarchyItem>.FromPages(pagesList);
        }
    }
}
