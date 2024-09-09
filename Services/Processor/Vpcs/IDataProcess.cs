using ProcessServices.DataModels;

namespace ProcessServices.Services.Processor.Vpcs
{
    public interface IDataProcess
    {
        public string FindFilePathById(string id);
        public IEnumerable<VpcsExcelData> GetDataFromVpcsFilePath(string filePath);
        public Dictionary<string, Dictionary<string, int>> CountVPCsByReporter(IEnumerable<VpcsExcelData> dataList);
        public Dictionary<string, int> GetVpcTypeCount(IEnumerable<VpcsExcelData> data);

    }
}
