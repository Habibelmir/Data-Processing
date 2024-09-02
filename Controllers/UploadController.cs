using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProcessServices.Services.Uploader;

namespace ProcessServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private IFileUpload _IfileUpload;
        public UploadController(IFileUpload IFileUpload)
        {
            _IfileUpload = IFileUpload;
        }
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (await _IfileUpload.UploadExcelFile(file))
            {
                string fileId = _IfileUpload.GetFileId();
                return Ok(new { FileId = fileId });
            }
            return BadRequest();
        }
    }
}
