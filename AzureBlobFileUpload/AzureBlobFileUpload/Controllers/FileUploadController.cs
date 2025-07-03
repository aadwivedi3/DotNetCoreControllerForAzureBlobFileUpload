using AzureBlobFileUpload.Data;
using AzureBlobFileUpload.HelperClasses.Interfaces;
using AzureBlobFileUpload.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json;
using System.IO.Compression;

namespace AzureBlobFileUpload.Controllers
{
    /// <summary>
    /// This is FileUpload controller class
    /// </summary>
    [Route("[controller]/[action]")]
    [ApiController]
    public class FileUploadsController : ControllerBase
    {
        private readonly FileUploadDataContext _context;
        private readonly IBlobService _blobService;
        private readonly IMultipleFileUpload _multipleFileUpload;
        private readonly IConfiguration _configuration;
        /// <summary>
        /// This is File Upload constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="blobService"></param>
        /// <param name="multipleFileUpload"></param>
        /// <param name="configuration"></param>
        public FileUploadsController(FileUploadDataContext context, IBlobService blobService, IMultipleFileUpload multipleFileUpload, IConfiguration configuration)
        {
            _context = context;
            _blobService = blobService;
            _multipleFileUpload = multipleFileUpload;
            _configuration = configuration;
        }
        /// <summary>
        /// Downloads single and multiple files based on path count
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        [HttpPost, Authorize]
        public async Task<IActionResult> DownloadFile([FromBody] dynamic jsonData)
        {
            FileUpload fileUpload = JsonConvert.DeserializeObject<FileUpload>(jsonData.ToString(), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            if (!ModelState.IsValid)
            {
                return BadRequest("Input data is not valid.");
            }
            List<string> path;
            try
            {
                path = _multipleFileUpload.GetFilePathForFileDownload(_context, fileUpload);
                if (path == null || path.Count == 0 || string.IsNullOrEmpty(path[0]))
                {
                    return BadRequest("Input data is not valid.");
                }
                #region Single File Download
                if (path.Count == 1)
                {
                    //Single File Download
                    var result = await _blobService.GetBlobAsync(path[0], _configuration);
                    if (result == null)
                    {
                        return BadRequest("Error occured while downloading the file");
                    }
                    new FileExtensionContentTypeProvider().TryGetContentType(result, out var mimeType);
                    if (mimeType != null)
                    {
                        var basename = Path.GetFileName(result);

                        Response.Headers.Append("Content-Disposition", $"attachment; filename={basename};");
                        FileStreamResult file = new FileStreamResult(System.IO.File.OpenRead(result), mimeType);
                        return file;
                    }
                    return BadRequest("Unable to determine the file type to download");
                    #endregion
                }
                else
                {
                    //Multiple File Download
                    var result = await _blobService.GetBlobListAsync(path, _configuration);
                    if (result == null || result.Count == 0)
                    {
                        return BadRequest("Error occured while downloading the file");
                    }
                    var tempFile = Path.GetRandomFileName();

                    using (var zipFile = System.IO.File.Create(tempFile))

                    using (var zipArchive = new ZipArchive(zipFile, ZipArchiveMode.Create, true))
                    {
                        foreach (var file in result)
                        {
                            zipArchive.CreateEntryFromFile(file, Path.GetFileName(file));
                        }
                    }
                    var stream = new FileStream(tempFile, FileMode.Open);
                    Response.Headers.Append("Content-Disposition", $"attachment; filename=images.zip;");
                    FileStreamResult file1 = new FileStreamResult(stream, "application/zip");
                    return file1;
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to download {ex.Message}");
            }
        }

        /// <summary>
        /// Uploads single or multiple file based on file object count
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Authorize]
        public async Task<IActionResult> UploadFile([FromForm] FileUploadDto dto)
        {
            if (dto.FormFile == null || dto.FormFile.Count == 0)
                return BadRequest("Incorrect input for the file upload.");

            //Multiple file upload
            foreach (var file in dto.FormFile)
            {
                if (dto.FileUpload == null) continue;
                var result = await _multipleFileUpload.UploadSingleFile(file, dto.FileUpload, _context, _configuration);
                if (result <= 0) return BadRequest("File upload Failed");
            }
            return Ok("Files uploaded successfully");

        }
    }
}
