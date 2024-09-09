using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProcessServices.DataModels;
using ProcessServices.Services.Processor.Danger;
using ProcessServices.Services.Processor.Vpcs;

namespace ProcessServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VpcsDataProcessController : ControllerBase
    {
        private IWebHostEnvironment _env;
        private IDataProcess _IdataProcess;
        public VpcsDataProcessController(IDataProcess IdataProcess, IWebHostEnvironment env)
        {
            _IdataProcess = IdataProcess;
            _env = env;

        }


        [HttpGet("{fileId}")]
        public IActionResult GetVpcsStats(string fileId)
        {
            try
            {
                // Retrieve the file path using the given ID
                string filePath = _IdataProcess.FindFilePathById(fileId);
                if (filePath == null)
                {
                    return NotFound(new { Message = "File not found" });
                }

                // Process the file data
                IEnumerable<VpcsExcelData> allData = _IdataProcess.GetDataFromVpcsFilePath(filePath);
                var VPCsCountByReporter = _IdataProcess.CountVPCsByReporter(allData);
                var VpcTypeCount = _IdataProcess.GetVpcTypeCount(allData);

                // Combine data for the response
                var combinedData = new
                {
                    BarChartData = VPCsCountByReporter,
                    PieChartData = VpcTypeCount
                };

                // Return combined data with status 200
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
