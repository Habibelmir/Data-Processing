using ClosedXML.Excel;
using DotNetEnv;
using ProcessServices.DataModels;

namespace ProcessServices.Services.Processor.Finding.Impl
{
    public class FindingExcelDataProcess : IFindingDataProcess
    {
        private IHostEnvironment _env;
        public FindingExcelDataProcess(IHostEnvironment env)
        {

            _env = env;

        }
        public string FindFilePathById(string fileid)
        {
            // Define the directory where the files are stored
            string uploadDirectory = Path.Combine(_env.ContentRootPath, "Uploads");

            // Get the file with the unique id in the directory
            var file = Directory.GetFiles(uploadDirectory, $"{fileid}_*");

            // If a matching file is found, return the full path
            if (file.Length > 0)
            {
                return file[0]; // Since IDs are unique, there should only be one match
            }

            // If no file is found, return null or throw an exception based on your needs
            return null;
        }

        public Dictionary<string, int> GetFindingOwnerApparenceCount(IEnumerable<FindingExcelData> data)
        {
            return data
                .GroupBy(row => row.FindingOwner)
                .ToDictionary(
                    owner => owner.Key,
                    owner => owner.Count()
                );
        }

        public IEnumerable<FindingExcelData> GetDataFromFindingFilePath(string filePath)
        {
            // Reading from Excel using the library ClosedXML
            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1); // Utilise la première feuille

                // Read the header row (first row ) 
                var headerRow = worksheet.Row(1);
                // Dictionary to set each column and its index in the Excel file
                var columnIndexes = new Dictionary<string, int>
                {
                    { "FindingOwner", GetColumnIndex(headerRow, "Finding Owner") },
                    { "FindingNumber", GetColumnIndex(headerRow, "Finding Number") },
                    { "FindingSummary", GetColumnIndex(headerRow, "Finding Summary") },
                    { "FindingType", GetColumnIndex(headerRow, "Finding Type") }
                };

                // Lire les données à partir de la deuxième ligne
                foreach (var row in worksheet.RowsUsed().Skip(1))
                {
                    // Extract and clean data from cells and process
                    string FindingOwner = row.Cell(columnIndexes["FindingOwner"]).GetValue<string>();
                    // Return each ExcelData object
                    yield return new FindingExcelData
                    {
                        FindingOwner = string.IsNullOrWhiteSpace(FindingOwner) ? null : GetFirstAndLastName(FindingOwner),
                        FindingNumber = row.Cell(columnIndexes["FindingNumber"]).GetValue<string>(),
                        FindingSummary = row.Cell(columnIndexes["FindingSummary"]).GetValue<string>(),
                        FindingType = ExtractTextBetweenBrackets(row.Cell(columnIndexes["FindingType"]).GetValue<string>())
                    };
                }
            }
        }
        private string GetFirstAndLastName(string reporterInfo)
        {
            string[] parts = reporterInfo.Split(',');
            // Extract the first and last name
            string firstName = parts[0].Trim();
            string lastName = parts[1];
            return firstName + lastName;
            //fa6f2e32-efaf-40ee-9043-3bbc18977b29
        }
        private int GetColumnIndex(IXLRow headerRow, string columnName)
        {
            // verify if the column name exist in excel file column's
            var column = headerRow.Cells().FirstOrDefault(c => c.GetValue<string>() == columnName);
            return column?.Address.ColumnNumber ?? -1; // Return -1 if column not found
        }

        public string ExtractTextBetweenBrackets(string findingType)
        {
            int startIndex = findingType.IndexOf('(') + 1;
            int endIndex = findingType.IndexOf(')');

            if (startIndex > 0 && endIndex > startIndex)
            {
                return findingType.Substring(startIndex, endIndex - startIndex);
            }
            return null;
        }
    }
}
