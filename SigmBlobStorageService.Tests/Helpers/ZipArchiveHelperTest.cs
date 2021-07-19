using NUnit.Framework;
using SigmaBlobStorageService.Api.Helpers;
using SigmaBlobStorageService.Api.Models;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace SigmBlobStorageService.Tests.Helpers
{
    public class ZipArchiveHelperTest
    {
        private byte[] _fakeFileByteArray = new byte[1];

        [Test]
        public async Task ShouldReturnNullWhenGettingFileFromZipAndFileStreamIsNull() 
        {
            var zipArchiverHelper = new ZipArchiveHelper();
            var result = zipArchiverHelper.GetFileFromZip("aa", null);

            Assert.IsNull(result);
        }

        [Test]
        public async Task ShouldReturnNullWhenGettingFileFromZipAndEntryWithGivenFileNameNotFoundInZip() 
        {
            var zipArchiverHelper = new ZipArchiveHelper();
            using(var fileStream = new MemoryStream(GetFakeZippedFile("asd")))
            {
                var result = zipArchiverHelper.GetFileFromZip("aa", fileStream);

                Assert.IsNull(result);
            }
        }
        
        [Test]
        public async Task ShouldReturnFileModelOfEntryFoundInZipWhenGettingFileFromZip() 
        {
            var zipArchiverHelper = new ZipArchiveHelper();
            using (var fileStream = new MemoryStream(GetFakeZippedFile("asd")))
            {
                var result = zipArchiverHelper.GetFileFromZip("asd", fileStream);

                Assert.IsNotNull(result);
                Assert.AreEqual(result.FileContent, _fakeFileByteArray);
            }
        }

        [Test]
        public async Task ShouldReturnNullWhenZipFilesAndListOfFilesContainsNoElements() 
        {
            var zipArchiverHelper = new ZipArchiveHelper();
            var emptyFileList = new List<FileModel>();
            
            var result = zipArchiverHelper.ZipFiles(emptyFileList);
            
            Assert.IsNull(result);
        }

        private byte[] GetFakeZippedFile(string fakeZippedFileName)
        {
            using (var compressedFileStream = new MemoryStream())
            {
                using (var zippedArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Create, false))
                {
                    var zipEntry = zippedArchive.CreateEntry(fakeZippedFileName);

                    using (var originalFileStream = new MemoryStream(_fakeFileByteArray))
                    using (var zipEntryStream = zipEntry.Open())
                    {
                        originalFileStream.CopyTo(zipEntryStream);
                    }
                }
                return compressedFileStream.ToArray();
            }
        }
    }
}
