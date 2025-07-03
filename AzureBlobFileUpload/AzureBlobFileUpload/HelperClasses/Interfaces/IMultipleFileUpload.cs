using AzureBlobFileUpload.Data;
using AzureBlobFileUpload.Model;

namespace AzureBlobFileUpload.HelperClasses.Interfaces
{
    public interface IMultipleFileUpload
    {
        public Task<int> UploadSingleFile(IFormFile file, FileUploadDataDto fileUploadDataDto, FileUploadDataContext context, IConfiguration _configuration);
        public List<string> GetFilePathForFileDownload(FileUploadDataContext context, FileUpload fileUpload);
    }
}
