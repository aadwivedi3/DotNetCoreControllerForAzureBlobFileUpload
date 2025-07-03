using static AzureBlobFileUpload.Enums.Constants;

namespace AzureBlobFileUpload.Model
{
    /// <summary>
    /// This is the file upload model to be used for reusable file upload logic
    /// </summary>
    public class FileUpload
    {
        public int FileUploadID { get; set; }
        public string? Path { get; set; }
        public int? MultiFileUploadID { get; set; }
        public FileUploadOperation FileUploadOperation { get; set; }
        public DateTime? FileUploadTime { get; set; }
    }
}
