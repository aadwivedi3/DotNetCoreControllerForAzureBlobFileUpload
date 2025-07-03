namespace AzureBlobFileUpload.Model
{
    /// <summary>
    /// This class contains list of files to be uploaded for their specific file upload operation
    /// </summary>
    public class FileUploadDto
    {
        public List<IFormFile>? FormFile { get; set; }
        public FileUploadDataDto? FileUpload { get; set; }
    }
}
