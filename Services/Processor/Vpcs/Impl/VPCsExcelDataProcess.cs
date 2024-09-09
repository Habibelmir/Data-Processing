

using ClosedXML.Excel;
using ProcessServices.DataModels;
using System.Text.RegularExpressions;

namespace ProcessServices.Services.Processor.Vpcs.Impl
{
    public class VPCsExcelDataProcess : IDataProcess
    {
        private IHostEnvironment _env;
        public VPCsExcelDataProcess(IHostEnvironment env)
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
        private static string CleanVpcFocus(string focus)
        {
            // Define the list of valid categories
            var validCategories = new List<string>
        {
            "Travail en hauteur",
            "S - travail à chaud / soudage / meulage",
            "S - Sécurité électrique",
            "Isolation d'énergie",
            "Émissions atmosphériques - Poussières fugitives",
            "Émissions atmosphériques - Cheminée",
            "H - Bien-être et Housekeeping",
            "E - Consommation d'eau",
            "Protection machine",
            "Levage et support des charges",
            "H - Dangers biologiques",
            "H - Dangers chimiques",
            "S - Matière chaude"
        };

            // Remove text within parentheses and any extra spaces
            var cleanedFocus = Regex.Replace(focus, @"\s*\(.*?\)", string.Empty).Trim();

            // Check if the cleaned focus matches one of the valid categories
            return validCategories.FirstOrDefault(cat => cleanedFocus.Contains(cat)) ?? cleanedFocus;
        }
        public IEnumerable<VpcsExcelData> GetDataFromVpcsFilePath(string filePath)
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
                    { "ReportedBy", GetColumnIndex(headerRow, "VPC rapporté par") },
                    { "CPVFocus", GetColumnIndex(headerRow, "Ce CPV porte sur") },
                    { "VPCType", GetColumnIndex(headerRow, "Type de VPC effectué") }
                };

                // Lire les données à partir de la deuxième ligne
                foreach (var row in worksheet.RowsUsed().Skip(1))
                {
                    // Extract and clean data from cells and process
                    string reportedBy = row.Cell(columnIndexes["ReportedBy"]).GetValue<string>();
                    string cpvFocus = row.Cell(columnIndexes["CPVFocus"]).IsEmpty() ? null : row.Cell(columnIndexes["CPVFocus"]).GetValue<string>();
                    string cleanedCPVFocus = cpvFocus != null ? CleanVpcFocus(cpvFocus) : null;

                    // Return each ExcelData object
                    yield return new VpcsExcelData
                    {
                        ReportedBy = string.IsNullOrWhiteSpace(reportedBy) ? null : GetFirstAndLastName(reportedBy),
                        CPVFocus = cleanedCPVFocus,
                        VPCType = row.Cell(columnIndexes["VPCType"]).IsEmpty() ? null : row.Cell(columnIndexes["VPCType"]).GetValue<string>(),
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

        public Dictionary<string, Dictionary<string, int>> CountVPCsByReporter(IEnumerable<VpcsExcelData> dataList)
        {
            return dataList
                .GroupBy(data => data.ReportedBy)  // Group by Reporter
                .ToDictionary(
                    reporterGroup => reporterGroup.Key,  // Reporter name
                    reporterGroup => new Dictionary<string, int>
                    {
                // Count "Rôle" or "Axé sur le sujet" and store as "VPC"
                {
                    "VPC", reporterGroup.Count(data => data.VPCType == "Rôle" || data.VPCType == "Axé sur le sujet")
                },
                // Count "Critical VPC" and store as "Critical VPC"
                {
                    "Critical VPC", reporterGroup.Count(data => data.VPCType == "Critical VPC")
                }
                    }
                );
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

        public Dictionary<string, int> GetVpcTypeCount(IEnumerable<VpcsExcelData> data)
        {
            return data.GroupBy(data => data.VPCType)
                .ToDictionary(
                Type => Type.Key,
                Type => Type.Count()
                );
        }








    }
}
