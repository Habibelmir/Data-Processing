using ClosedXML.Excel;
using ProcessServices.DataModels;
using System.Globalization;

namespace ProcessServices.Services.Processor.OverDue.Impl
{
    public class OverDueExcelDataProcess:IOverDueDataProcess
    {
        private IHostEnvironment _env;
        public OverDueExcelDataProcess(IHostEnvironment env)
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
        public IEnumerable<OverDueExcelData> GetDataFromOverDueFilePath(string filePath)
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
                    { "DueDate", GetColumnIndex(headerRow, "Due Date") },
                    { "ActionNumber", GetColumnIndex(headerRow, "Action Number") },
                    { "AssignedTo", GetColumnIndex(headerRow, "Assigned To") },
                    { "summary", GetColumnIndex(headerRow, "Action Summary") }
                };

                // Lire les données à partir de la deuxième ligne
                foreach (var row in worksheet.RowsUsed().Skip(1))
                {
                    // Extract and clean data from cells and process
                    string AssignedTo = row.Cell(columnIndexes["AssignedTo"]).GetValue<string>();
                    //DateTime dateTime = DateTime.ParseExact(row.Cell(columnIndexes["DueDate"]).GetValue<string>(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    // Return each ExcelData object
                    yield return new OverDueExcelData
                    {
                        AssignedTo = string.IsNullOrWhiteSpace(AssignedTo) ? null : GetFirstAndLastName(AssignedTo),
                        DueDate = DateTime.Parse(row.Cell(columnIndexes["DueDate"]).GetValue<string>()),
                        ActionNumber = row.Cell(columnIndexes["ActionNumber"]).GetValue<string>(),
                        summary = row.Cell(columnIndexes["summary"]).GetValue<string>()
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
        public Dictionary<string, int> GetAssignedApparenceCount(IEnumerable<OverDueExcelData> data)
        {
            return data
                .GroupBy(row => row.AssignedTo)
                .ToDictionary(
                    row => row.Key,
                    row => row.Count());
        }
        
    }
}
