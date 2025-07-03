using Azure.Storage.Blobs;
using AzureBlobFileUpload.Data;
using AzureBlobFileUpload.Enums;
using AzureBlobFileUpload.HelperClasses.Interfaces;
using AzureBlobFileUpload.Model;
using System.Net.Http;
using System.Text;

namespace AzureBlobFileUpload.HelperClasses.Classes
{
    /// <summary>
    /// This is Blob Service class for file upload and download
    /// </summary>
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;

        /// <summary>
        /// Blob Sevice Constructor
        /// </summary>
        /// <param name="blobServiceClient"></param>
        public BlobService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }
        /// <summary>
        /// Creates the blob file path based on file upload operation
        /// </summary>
        /// <param name="fileUploadDataDto"></param>
        /// <returns></returns>
        public string CreatePath(FileUploadDataDto fileUploadDataDto)
        {
            if (fileUploadDataDto == null) return string.Empty;
            StringBuilder path = new StringBuilder();
            path.Append(fileUploadDataDto.MultiFileUploadID);
            Constants.FileUploadOperation fileUploadOperations;
            if (Enum.TryParse(fileUploadDataDto.FileUploadOperation.ToString().ToLower(), out fileUploadOperations))
            {
                switch (fileUploadOperations)
                {
                    case Constants.FileUploadOperation.UploadInvoice:
                        path.Append("//"+Constants.FileUploadOperation.UploadInvoice+"//");
                        break;
                    case Constants.FileUploadOperation.UploadQuotation:
                        path.Append("//" + Constants.FileUploadOperation.UploadQuotation + "//");
                        break;
                }
            }
            return path.ToString();
        }
        /// <summary>
        /// Deletes the file from blob storage
        /// </summary>
        /// <param name="blobName"></param>
        /// <returns></returns>
        public void DeleteBlobAsync(string blobName)
        {
            if (string.IsNullOrEmpty(blobName)) return;
            try
            {
                System.IO.File.Delete(blobName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());//Put your logging here
            }
        }
        /// <summary>
        /// Downloads single file
        /// </summary>
        /// <param name="name"></param>
        /// <param name="_configuration"></param>
        /// <returns></returns>
        public async Task<string> GetBlobAsync(string name, IConfiguration _configuration)
        {
            string[] fileInfo = name.Split(".");
            var path = @"temp/" + Guid.NewGuid().ToString() + "." + fileInfo[fileInfo.Length - 1];
            var containrBlobClient = _blobServiceClient.GetBlobContainerClient(_configuration.GetValue<string>(Constants.Configurations.BlobContainerName.ToString()));
            var blobClient = containrBlobClient.GetBlobClient(@name);
            using (var fileStream = System.IO.File.OpenWrite(path))
            {
                await blobClient.DownloadToAsync(fileStream);
                return path;
            }
        }
        /// <summary>
        /// Downloads multiple files
        /// </summary>
        /// <param name="name"></param>
        /// <param name="_configuration"></param>
        /// <returns></returns>
        public async Task<List<string>> GetBlobListAsync(List<string> name, IConfiguration _configuration)
        {
            List<string> list = new List<string>();
            foreach (string file in name)
            {
                string[] fileInfo = file.Split(".");
                var path = @"temp/" + Guid.NewGuid().ToString() + "." + fileInfo[1];
                var containrBlobClient = _blobServiceClient.GetBlobContainerClient(_configuration.GetValue<string>(Constants.Configurations.BlobContainerName.ToString()));
                var blobClient = containrBlobClient.GetBlobClient(@file);
                using (var fileStream = System.IO.File.OpenWrite(path))
                {
                    await blobClient.DownloadToAsync(fileStream);
                    list.Add(path);
                }
            }
            return list;
        }
        /// <summary>
        /// Stores the file path to SQL storage
        /// </summary>
        /// <param name="context"></param>
        /// <param name="path"></param>
        /// <param name="fileUploadDataDto"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task StoreFilePath(FileUploadDataContext context, string path, FileUploadDataDto fileUploadDataDto)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(path);
            FileUpload fileUpload = new FileUpload()
            {
                Path = path,
                FileUploadOperation = fileUploadDataDto.FileUploadOperation,
                FileUploadTime = DateTime.UtcNow,
                MultiFileUploadID = fileUploadDataDto.MultiFileUploadID
            };
            context.Add(fileUpload);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Uploads file to blob storage
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <param name="_configuration"></param>
        /// <returns></returns>
        public async Task UploadFileBlobAsync(Stream fileStream, string name, string path, IConfiguration _configuration)
        {
            var containrBlobClient = _blobServiceClient.GetBlobContainerClient(_configuration.GetValue<string>(Constants.Configurations.BlobContainerName.ToString()));
            var blob = containrBlobClient.GetBlobClient(path + name);
            await blob.UploadAsync(fileStream, overwrite: true);
        }

    }
}
