using AzureBlobFileUpload.Data;
using AzureBlobFileUpload.Enums;
using AzureBlobFileUpload.HelperClasses.Interfaces;
using AzureBlobFileUpload.Model;
using System.Net.Http;
using System.Reflection.Metadata;

namespace AzureBlobFileUpload.HelperClasses.Classes
{
    public class MultipleFileUpload : IMultipleFileUpload
    {
        private readonly IBlobService _blobService;
        public MultipleFileUpload(IBlobService blobService)
        {
            _blobService = blobService;
        }

        public List<string> GetFilePathForFileDownload(FileUploadDataContext context, FileUpload fileUpload)
        {
            List<string> path = new List<string>();
            Constants.FileUploadOperation fileUploadOperations;
            if (fileUpload == null || string.IsNullOrEmpty(fileUpload.FileUploadOperation.ToString())) return path;
            if (Enum.TryParse(fileUpload.FileUploadOperation.ToString().ToLower(), out fileUploadOperations))
            {
                switch (fileUploadOperations)
                {
                    //Add cases as per your business requirement
                    case Constants.FileUploadOperation.UploadInvoice:
                        var multiPathResult = context.FileUpload?.Where(f => f.MultiFileUploadID == fileUpload.MultiFileUploadID && f.FileUploadOperation == fileUpload.FileUploadOperation);
                        if (multiPathResult == null || multiPathResult.Count() == 0) return path;
                        foreach (var item in multiPathResult)
                        {
                            if (item != null && !string.IsNullOrEmpty(item.Path)) path.Add(item.Path);
                        }
                        break;
                    default:
                        var singlePathResult = context.FileUpload?.Where(f => f.MultiFileUploadID == fileUpload.MultiFileUploadID && f.FileUploadOperation == fileUpload.FileUploadOperation).OrderByDescending(f => f.FileUploadTime).First().Path;
                        if(!string.IsNullOrEmpty(singlePathResult)) path.Add(singlePathResult);
                        break;
                }
            }
            return path;
        }

        public async Task<int> UploadSingleFile(IFormFile file, FileUploadDataDto fileUploadDataDto, FileUploadDataContext context, IConfiguration _configuration)
        {
            #region fileupload
            var fileStream = file.OpenReadStream();
            if (fileStream == null)
                return 0;

            try
            {
                //Generate path
                var path = _blobService.CreatePath(fileUploadDataDto);
                //Upload file
                string[] fileInfo = file.FileName.Split(".");
                var fileName = Guid.NewGuid().ToString() + "." + fileInfo[fileInfo.Length - 1];
                await _blobService.UploadFileBlobAsync(fileStream, fileName, path, _configuration);
                //Store path in database
                await _blobService.StoreFilePath(context, path + fileName, fileUploadDataDto);
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
            #endregion
        }
    }
}
