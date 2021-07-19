using SigmaBlobStorageService.Api.Models;
using System.Collections.Generic;
using System.IO;

namespace SigmaBlobStorageService.Api.Helpers
{
    public interface IZipArchiveHelper
    {
        /// <summary>
        /// Returns FileModel of file from given Zip Archive
        /// </summary>
        /// <param name="fileName">Name of file to get from archive</param>
        /// <param name="fileStream">Stream to Zip archive</param>
        FileModel GetFileFromZip(string fileName, Stream fileStream);

        /// <summary>
        /// Returns byte array of archived file 
        /// </summary>
        /// <param name="file">stream to file to archive</param>
        /// <returns></returns>
        byte[] ZipFile(FileModel file) => file != null ? ZipFiles(new List<FileModel> { file }) : null;

        /// <summary>
        /// Returns byt array of archived files
        /// </summary>
        /// <param name="files">collection of files to archive</param>
        /// <returns></returns>
        byte[] ZipFiles(IEnumerable<FileModel> files);
    }
}
