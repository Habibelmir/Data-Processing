using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProcessServices.DataModels;
using ProcessServices.Services.Processor.OverDue;

namespace ProcessServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OverDueDataProcessController : ControllerBase
    {
        private IOverDueDataProcess _IoverDueDataProcess;
        public OverDueDataProcessController(IOverDueDataProcess IoverDueDataProcess)
        {
            _IoverDueDataProcess = IoverDueDataProcess;
        }
        [HttpGet("{fileId}")]
        public IActionResult GetOverDueStats(string fileId)
        {
            try
            {
                // Retrieve the file path using the given ID
                string filePath = _IoverDueDataProcess.FindFilePathById(fileId);
                if (filePath == null)
                {
                    return NotFound(new { Message = "File not found" });
                }

                // Process the file data
                IEnumerable<OverDueExcelData> allData = _IoverDueDataProcess.GetDataFromOverDueFilePath(filePath);
                var AssignedApparenceCount = _IoverDueDataProcess.GetAssignedApparenceCount(allData);
                var combinedData = new
                {
                    BarData = AssignedApparenceCount,
                    TableData = allData
                };

                // Return data with status 200
                return Ok(combinedData);
            }
            catch (Exception ex)
            {
                // Handle any unexpected errors
                return StatusCode(500, new { Message = "An error occurred while processing the data.", Details = ex.Message });
            }
        }
    }
}
