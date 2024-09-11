using ProcessServices.DataModels;

namespace ProcessServices.Services.Processor.OverDue
{
    public interface IOverDueDataProcess
    {
        public string FindFilePathById(string fileid);
        public IEnumerable<OverDueExcelData> GetDataFromOverDueFilePath(string filePath);
        public Dictionary<string, int> GetAssignedApparenceCount(IEnumerable<OverDueExcelData> data);
    }
}
