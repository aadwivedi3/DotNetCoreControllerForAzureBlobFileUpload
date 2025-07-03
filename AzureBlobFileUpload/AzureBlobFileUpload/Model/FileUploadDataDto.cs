using static AzureBlobFileUpload.Enums.Constants;

namespace AzureBlobFileUpload.Model
{
    /// <summary>
    /// This class defines the file upload dto and file upload operation
    /// </summary>
    public class FileUploadDataDto
    {
        public int FileUploadDataDtoID { get; set; }
        public int? MultiFileUploadID { get; set; }
        public FileUploadOperation FileUploadOperation { get; set; }
    }
}
