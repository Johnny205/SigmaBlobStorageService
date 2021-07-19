namespace SigmaBlobStorageService.Api.Models
{
    public class FileModel
    {
        /// <summary>
        /// Name of file
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// FileStream as byte array
        /// </summary>
        public byte[] FileContent { get; set; }
    }
}
