using SigmaBlobStorageService.Api.Models;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace SigmaBlobStorageService.Api.Helpers
{
    public class ZipArchiveHelper : IZipArchiveHelper
    {
        public FileModel GetFileFromZip(string fileName, Stream fileStream)
        {
            if (fileStream == null)
                return null;

            using (var zip = new ZipArchive(fileStream, ZipArchiveMode.Read))
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

        public byte[] ZipFiles(IEnumerable<FileModel> files)
        {
            if (!files.Any())
                return null;

            using (var compressedFileStream = new MemoryStream())
            {
                ArchiveFiles(compressedFileStream, files);
                return compressedFileStream.ToArray();
            }
        }

        private void ArchiveFiles(Stream fileStream, IEnumerable<FileModel> fileModels)
        {
            using (var zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Create, false))
                foreach (var fileModel in fileModels)
                    AddFileToArchive(zipArchive, fileModel);
        }

        private void AddFileToArchive(ZipArchive zipArchive, FileModel fileModel)
        {
            if (fileModel == null)
                return;

            var zipEntry = zipArchive.CreateEntry(fileModel.Name);

            using (var originalFileStream = new MemoryStream(fileModel.FileContent))
            using (var zipEntryStream = zipEntry.Open())
                originalFileStream.CopyTo(zipEntryStream);
        }
    }
}
