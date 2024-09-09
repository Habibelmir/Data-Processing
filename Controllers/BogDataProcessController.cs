using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProcessServices.DataModels;
using ProcessServices.Services.Processor.Bog;
using ProcessServices.Services.Processor.Danger;

namespace ProcessServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BogDataProcessController : ControllerBase
    {
        private IBogDataProcess _IbogDataProcess;
        public BogDataProcessController(IBogDataProcess IbogDataProcess)
        {
            _IbogDataProcess = IbogDataProcess;
        }
        [HttpGet("{fileId}")]
        public IActionResult GetBogStats(string fileId)
        {
            try
            {
                // Retrieve the file path using the given ID
                string filePath = _IbogDataProcess.FindFilePathById(fileId);
                if (filePath == null)
                {
                    return NotFound(new { Message = "File not found" });
                }

                // Process the file data
                IEnumerable<BogExcelData> allData = _IbogDataProcess.GetDataFromBogFilePath(filePath);
                var TotalTimeByUserCount = _IbogDataProcess.GetTotalTimeByUserCount(allData);

                // Return data with status 200
                return Ok(TotalTimeByUserCount);
            }
            catch (Exception ex)
            {
                // Handle any unexpected errors
                return StatusCode(500, new { Message = "An error occurred while processing the data.", Details = ex.Message });
            }
        }
    }
}
