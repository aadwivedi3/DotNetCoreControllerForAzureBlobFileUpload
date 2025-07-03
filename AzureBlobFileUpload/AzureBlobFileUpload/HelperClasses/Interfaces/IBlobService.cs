using AzureBlobFileUpload.Data;
using AzureBlobFileUpload.Model;

namespace AzureBlobFileUpload.HelperClasses.Interfaces
{
    /// <summary>
    /// This is Blob Service interface for file upload and download
    /// </summary>
    public interface IBlobService
    {
        /// <summary>
        /// Downloads single file
        /// </summary>
        /// <param name="name"></param>
        /// <param name="_configuration"></param>
        /// <returns></returns>
        public Task<string> GetBlobAsync(string name, IConfiguration _configuration);
        /// <summary>
        /// Downloads multiple files
        /// </summary>
        /// <param name="name"></param>
        /// <param name="_configuration"></param>
        /// <returns></returns>
        public Task<List<string>> GetBlobListAsync(List<string> name, IConfiguration _configuration);
        /// <summary>
        /// Uploads file to blob storage
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <param name="_configuration"></param>
        /// <returns></returns>
        public Task UploadFileBlobAsync(Stream fileStream, string name, string path, IConfiguration _configuration);
        /// <summary>
        /// Deletes the file from blob storage
        /// </summary>
        /// <param name="blobName"></param>
        /// <returns></returns>
        public void DeleteBlobAsync(string blobName);
        /// <summary>
        /// Creates the blob file path based on file upload operation
        /// </summary>
        /// <param name="fileUploadDataDto"></param>
        /// <returns></returns>
        public string CreatePath(FileUploadDataDto fileUploadDataDto);
        /// <summary>
        /// Stores the file path to SQL storage
        /// </summary>
        /// <param name="context"></param>
        /// <param name="path"></param>
        /// <param name="fileUploadDataDto"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Task StoreFilePath(FileUploadDataContext context, string path, FileUploadDataDto fileUploadDataDto);
    }
}
