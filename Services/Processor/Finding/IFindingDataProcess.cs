using ProcessServices.DataModels;

namespace ProcessServices.Services.Processor.Finding
{
    public interface IFindingDataProcess
    {
        public string FindFilePathById(string fileid);
        public Dictionary<string, int> GetFindingOwnerApparenceCount(IEnumerable<FindingExcelData> data);
        public IEnumerable<FindingExcelData> GetDataFromFindingFilePath(string filePath);
    }
}
