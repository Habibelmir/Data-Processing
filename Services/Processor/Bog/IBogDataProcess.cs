using ProcessServices.DataModels;

namespace ProcessServices.Services.Processor.Bog
{
    public interface IBogDataProcess
    {
        public string FindFilePathById(string fileid);
        public IEnumerable<BogExcelData> GetDataFromBogFilePath(string filePath);
        public Dictionary<string, double> GetTotalTimeByUserCount(IEnumerable<BogExcelData> data);
    }
}
