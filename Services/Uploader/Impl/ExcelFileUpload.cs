using Microsoft.AspNetCore.Mvc;

namespace ProcessServices.Services.Uploader.Impl
{
    public class ExcelFileUpload : IFileUpload
    {
        private IWebHostEnvironment _env;
        private string _fileId;
        public ExcelFileUpload(IWebHostEnvironment env)
        {
            _env = env;
        }
        public async Task<bool> UploadExcelFile(IFormFile file )
        {
            // verify if it's a valid File
            if (await IsFile(file))
            {
                // ensure that is a excel file 
                if (await IsExcel(file.FileName))
                {
                    // Create the upload directory's path
                    string UploadDirectoryPath = Path.Combine(_env.ContentRootPath, "Uploads");
                    // Create Uploads Directory if it doesn't exist
                    CreateDirectoryIfNotExist(UploadDirectoryPath);
                    string NewFileName=CreateNewUniqueName(file.FileName);
                    SetFiLeId(NewFileName);
                    // Uploaded File path
                    string FilePath = Path.Combine(UploadDirectoryPath, NewFileName);
                    SaveFile(file, FilePath);
                    return true;
                }
                return false;
            }
            return false;

        }
        private void SetFiLeId(string FullfileName)
        {
            int underscoreIndex = FullfileName.IndexOf('_');
            _fileId = FullfileName.Substring(0, underscoreIndex);
        }
        public string GetFileId()
        {
            return _fileId;
        }

        private async Task<bool> IsExcel(string fileName)
        {
            // Allowed extensions to be uploaded
            string[] allowedExtensions = { ".xla", ".xlam", ".xll", ".xlm", ".xls", ".xlsm", ".xlsx", ".xlt", ".xltm", ".xltx" };

            // Get the file extension
            string fileExtension = Path.GetExtension(fileName);

            // Ensure that the uploaded file extension is an Excel file
            return allowedExtensions.Contains(fileExtension.ToLower());

        }

        public async Task<bool> IsFile(IFormFile file)
        {
            return file != null && file.Length > 0;
        }
        public string CreateNewUniqueName(string fileName)
        {
            // The new unique file name
            return Guid.NewGuid() + "_" + fileName;
        }
        public async Task CreateDirectoryIfNotExist(string directoryPath)
        {
            // Ensure the directory exists if not create it
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                
            }
            
        }

        public async Task SaveFile(IFormFile file, string filePath)
        {
            try
            {
                using (FileStream stream = File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

      
    }
}
