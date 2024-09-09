using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProcessServices.DataModels;
using ProcessServices.Services.Processor.Danger;

namespace ProcessServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DangerDataProcessController : ControllerBase
    {
        private IDangerDataProcess _IdangerDataProcess;
        public DangerDataProcessController( IDangerDataProcess IdataDangerProcess)
        {
            _IdangerDataProcess = IdataDangerProcess;
        }
        [HttpGet("{fileId}")]
        public IActionResult GetDangerStats(string fileId)
        {
            try
            {
                // Retrieve the file path using the given ID
                string filePath = _IdangerDataProcess.FindFilePathById(fileId);
                if (filePath == null)
                {
                    return NotFound(new { Message = "File not found" });
                }

                // Process the file data
                IEnumerable<DangerExcelData> allData = _IdangerDataProcess.GetDataFromDangerFilePath(filePath);
                var VpcRepoterCount = _IdangerDataProcess.GetVpcRepoterCount(allData);

                // Return data with status 200
                return Ok(VpcRepoterCount);
            }
            catch (Exception ex)
            {
                // Handle any unexpected errors
                return StatusCode(500, new { Message = "An error occurred while processing the data.", Details = ex.Message });
            }
        }
    }
}
