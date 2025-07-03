using Microsoft.EntityFrameworkCore;

namespace AzureBlobFileUpload.Data
{
    public class FileUploadDataContext: DbContext
    {
        public FileUploadDataContext(DbContextOptions<FileUploadDataContext> options)
            : base(options)
        {
        }
        public DbSet<AzureBlobFileUpload.Model.FileUpload>? FileUpload { get; set; }
    }
}
