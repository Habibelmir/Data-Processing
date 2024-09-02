

using ClosedXML.Excel;
using ProcessServices.DataModels;

namespace ProcessServices.Services.Processor.Impl
{
    public class ExcelDataProcess : IDataProcess
    {
        private IHostEnvironment _env;
        public ExcelDataProcess(IHostEnvironment env)
        {

            _env = env;

        }
        public string FindFilePathById(string id)
        {
            // Define the directory where the files are stored
            string uploadDirectory = Path.Combine(_env.ContentRootPath, "Uploads");

            // Get the file with the unique id in the directory
            var file = Directory.GetFiles(uploadDirectory, $"{id}_*");

            // If a matching file is found, return the full path
            if (file.Length > 0)
            {
                return file[0]; // Since IDs are unique, there should only be one match
            }

            // If no file is found, return null or throw an exception based on your needs
            return null;
        }

        public IEnumerable<ExcelData> GetDataFromFilePath(string filePath)
        {
            // Reading from Excel using the library ClosedXML
            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1); // Utilise la première feuille

                // Read the header row (first row ) 
                var headerRow = worksheet.Row(1);
                // dictionary to set each column and it's index in the excel file
                var columnIndexes = new Dictionary<string, int>
                {
                    { "ReportedBy", GetColumnIndex(headerRow, "VPC rapporté par") },
                    { "CPVFocus", GetColumnIndex(headerRow, "Ce CPV porte sur") },
                    { "VPCType", GetColumnIndex(headerRow, "Type de VPC effectué") }
                };
                // Lire les données à partir de la deuxième ligne
                foreach (var row in worksheet.RowsUsed().Skip(1))
                {
                    // Extract data from cells and process
                    string reportedBy = row.Cell(columnIndexes["ReportedBy"]).GetValue<string>();
                    
                    //return chaque object ExcelData une fois
                    yield return new ExcelData
                    {
                        ReportedBy = GetFirstAndLastName(reportedBy),
                        CPVFocus = row.Cell(columnIndexes["CPVFocus"]).GetValue<string>(),
                        VPCType = row.Cell(columnIndexes["VPCType"]).GetValue<string>()
                    };
                }
                
            }
        }

        // to get the index from column name , ensuring dynamic changes of columns indexs
        private int GetColumnIndex(IXLRow headerRow, string columnName)
        {
            // verify if the column name exist in excel file column's
            var column = headerRow.Cells().FirstOrDefault(c => c.GetValue<string>() == columnName);
            return column?.Address.ColumnNumber ?? -1; // Return -1 if column not found
        }

        public Dictionary<string, Dictionary<string, int>> CountVPCsByReporter(IEnumerable<ExcelData> dataList)
        {
            return dataList
            .GroupBy(data => data.ReportedBy)  // Group by Reporter
            .ToDictionary(
            reporterGroup => reporterGroup.Key,  // Reporter name
            reporterGroup => reporterGroup
                .GroupBy(data => data.VPCType)  // Group by VPCType within each reporter
                .ToDictionary(
                    vpcGroup => vpcGroup.Key,  // VPCType
                    vpcGroup => vpcGroup.Count()  // Count occurrences of each VPCType
                )
            );
        }

        private string GetFirstAndLastName(string reporterInfo)
        {
            string[] parts = reporterInfo.Split(',');
            // Extract and trim the full name part
            string firstName = parts[0].Trim();
            string lastName = parts[1];
            return firstName + lastName;
            //fa6f2e32-efaf-40ee-9043-3bbc18977b29
        }
    }
}
