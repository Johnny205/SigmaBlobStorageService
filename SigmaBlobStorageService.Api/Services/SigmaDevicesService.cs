using Microsoft.AspNetCore.Mvc;
using SigmaBlobStorageService.Api.DataAccess;
using System.IO;
using System.IO.Compression;

namespace SigmaBlobStorageService.Api.Services
{
    public class SigmaDevicesService : ISigmaDevicesService
    {
        public SigmaDevicesService(ISigmaBlobStorageDataAcess sigmaBlobStorageDataAcess)
        {
            _sigmaBlobStorageDataAcess = sigmaBlobStorageDataAcess;
        }

        public ISigmaBlobStorageDataAcess _sigmaBlobStorageDataAcess { get; }

        public FileContentResult GetFileByPath(string path, string fileName)
        {
            byte[] file = _sigmaBlobStorageDataAcess.GetFileByPath(path, fileName);
            using (var compressedFileStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Create, false))
                {
                        var zipEntry = zipArchive.CreateEntry(fileName);

                        using (var originalFileStream = new MemoryStream(file))
                        using (var zipEntryStream = zipEntry.Open())
                        {
                            originalFileStream.CopyTo(zipEntryStream);
                        }
                }

                return new FileContentResult(compressedFileStream.ToArray(), "application/zip") { FileDownloadName = "Filename.zip" };
            }

        }
    }
}
