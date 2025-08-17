using OfficeOpenXml;
using System.Text;

namespace AutoInteractiveTool
{
    /// <summary>
    /// Excel file processor implementation using EPPlus
    /// </summary>
    public class ExcelProcessor : IExcelProcessor
    {
        public ExcelProcessor()
        {
            // Set EPPlus license context for non-commercial use
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        /// <summary>
        /// Reads text data from an Excel file
        /// </summary>
        /// <param name="filePath">Path to the Excel file</param>
        /// <returns>List of text data from each row</returns>
        public async Task<List<string>> ReadExcelFileAsync(string filePath)
        {
            var textData = new List<string>();

            try
            {
                using var package = new ExcelPackage(new FileInfo(filePath));
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();

                if (worksheet == null)
                {
                    throw new InvalidOperationException("No worksheet found in the Excel file.");
                }

                var rowCount = worksheet.Dimension?.Rows ?? 0;
                var colCount = worksheet.Dimension?.Columns ?? 0;

                for (int row = 1; row <= rowCount; row++)
                {
                    var rowText = new StringBuilder();
                    var hasData = false;

                    for (int col = 1; col <= colCount; col++)
                    {
                        var cellValue = worksheet.Cells[row, col].Value?.ToString();
                        if (!string.IsNullOrWhiteSpace(cellValue))
                        {
                            if (hasData)
                                rowText.Append(" ");
                            rowText.Append(cellValue);
                            hasData = true;
                        }
                    }

                    if (hasData)
                    {
                        textData.Add(rowText.ToString());
                    }
                }

                await Task.CompletedTask; // For async consistency
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error reading Excel file: {ex.Message}", ex);
            }

            return textData;
        }

        /// <summary>
        /// Validates if the file is a valid Excel file
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        /// <returns>True if valid Excel file, false otherwise</returns>
        public bool IsValidExcelFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension == ".xlsx" || extension == ".xls";
        }
    }
}