using ProcessServices.DataModels;

namespace ProcessServices.Services.Processor.Danger
{
    public interface IDangerDataProcess
    {
        public string FindFilePathById(string id);
        public IEnumerable<DangerExcelData> GetDataFromDangerFilePath(string filePath);
        public Dictionary<string, int> GetVpcRepoterCount(IEnumerable<DangerExcelData> data);
    }
}
