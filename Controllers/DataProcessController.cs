using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProcessServices.DataModels;
using ProcessServices.Services.Processor;

namespace ProcessServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataProcessController : ControllerBase
    {
        private IWebHostEnvironment _env;
        private IDataProcess _IdataProcess;
        public DataProcessController(IDataProcess IdataProcess, IWebHostEnvironment env)
        {
            _IdataProcess = IdataProcess;
            _env = env;

        }


        [HttpGet]
        public IActionResult GetStats(string fileId)
        {
            // retriev the file path using the given id
            string filePath = _IdataProcess.FindFilePathById(fileId);
            // if the file path variable is null means file path is not found
            if(filePath == null)
            {
                return NotFound();
            }
            IEnumerable<ExcelData> Alldata = _IdataProcess.GetDataFromFilePath(filePath);
            var BarChartData = _IdataProcess.CountVPCsByReporter(Alldata);
            var PieChartData = _IdataProcess.GetVpcTypeCount(Alldata);
            var CombinedData = new
            {
                BarChartData,
                PieChartData
            };
            // envoie des données groupé avec un status 200
            return Ok(CombinedData);

        }
        
        

        

    }
    
}
