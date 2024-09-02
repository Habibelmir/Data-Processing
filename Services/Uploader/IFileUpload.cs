using Microsoft.AspNetCore.Mvc;

namespace ProcessServices.Services.Uploader
{
    public interface IFileUpload
    {
        public Task<bool> UploadExcelFile(IFormFile file);
        public Task<bool> IsFile(IFormFile file);
        public string GetFileId();
        public string CreateNewUniqueName(string fileName);
        public Task CreateDirectoryIfNotExist(string directoryPath);
        public Task SaveFile(IFormFile file, string filePath);


    }
}
