using ProcessServices.DataModels;

namespace ProcessServices.Services.Processor
{
    public interface IDataProcess
    {
        public string FindFilePathById(string id);
        public IEnumerable<ExcelData> GetDataFromFilePath(string filePath);
        public Dictionary<string, Dictionary<string, int>> CountVPCsByReporter(IEnumerable<ExcelData> dataList);
    }
}
