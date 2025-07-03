namespace AzureBlobFileUpload.Enums
{
    public static class Constants
    {
        public enum FileUploadOperation
        {
            UploadInvoice,
            UploadQuotation
        };
        public enum Configurations
        {
            SQLConnectionString,
            BlobConnectionString,
            BlobContainerName
        }
    }
}
