using ClosedXML.Excel;
using ProcessServices.DataModels;

namespace ProcessServices.Services.Processor.Bog.Impl
{
    public class BogExcelDataProcess:IBogDataProcess
    {
        private IHostEnvironment _env;
        public BogExcelDataProcess(IHostEnvironment env)
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
        public IEnumerable<BogExcelData> GetDataFromBogFilePath(string filePath)
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
                    { "User", GetColumnIndex(headerRow, "User") },
                    { "TotalTime", GetColumnIndex(headerRow, "TotalTime") },
                    { "TourValidStatus", GetColumnIndex(headerRow, "TourValidStatus") }
                };

                // Lire les données à partir de la deuxième ligne
                foreach (var row in worksheet.RowsUsed().Skip(1))
                {
                    // Extract and clean data from cells and process
                    string user = row.Cell(columnIndexes["User"]).GetValue<string>();
                    string timePart = row.Cell(columnIndexes["TotalTime"]).GetValue<string>().Split(' ')[1];
                    // Return each ExcelData object
                    yield return new BogExcelData
                    {
                        User = string.IsNullOrWhiteSpace(user) ? null : user,
                        TotalTime = TimeSpan.Parse(timePart),
                        TourValidStatus = Enum.Parse<status>(row.Cell(columnIndexes["TourValidStatus"]).GetValue<string>())
                    };
                }
            }
        }
        private int GetColumnIndex(IXLRow headerRow, string columnName)
        {
            // verify if the column name exist in excel file column's
            var column = headerRow.Cells().FirstOrDefault(c => c.GetValue<string>() == columnName);
            return column?.Address.ColumnNumber ?? -1; // Return -1 if column not found
        }
        public Dictionary<string, double> GetTotalTimeByUserCount(IEnumerable<BogExcelData> data)
        {
            return data.Where(row => row.TourValidStatus == status.Valid)
                .GroupBy(row => row.User)
                .ToDictionary(
                    user => user.Key,
                    usersum => usersum.Sum(user => user.TotalTime.TotalMinutes)
                );
        }
    
    }
}
