using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProcessServices.DataModels;
using ProcessServices.Services.Processor.Danger;
using ProcessServices.Services.Processor.Finding;

namespace ProcessServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FindingDataProcessController : ControllerBase
    {
        private IFindingDataProcess _IfindingDataProcess;
        public FindingDataProcessController(IFindingDataProcess IfindingDataProcess)
        {
            _IfindingDataProcess = IfindingDataProcess;
        }
        [HttpGet("{fileId}")]
        public IActionResult GetFindingStats(string fileId)
        {
            try
            {
                // Retrieve the file path using the given ID
                string filePath = _IfindingDataProcess.FindFilePathById(fileId);
                if (filePath == null)
                {
                    return NotFound(new { Message = "File not found" });
                }

                // Process the file data
                IEnumerable<FindingExcelData> allData = _IfindingDataProcess.GetDataFromFindingFilePath(filePath);
                Dictionary<string, int> FindingOwnerApparenceCount = _IfindingDataProcess.GetFindingOwnerApparenceCount(allData);
                var combinedData = new
                {
                    barData = FindingOwnerApparenceCount,
                    tableData = allData
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
