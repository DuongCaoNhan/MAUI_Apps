using System.Collections.Generic;
using System.Threading.Tasks;

namespace AutoInteractiveTool
{
    /// <summary>
    /// Interface for processing Excel files
    /// </summary>
    public interface IExcelProcessor
    {
        /// <summary>
        /// Reads text data from an Excel file
        /// </summary>
        /// <param name="filePath">Path to the Excel file</param>
        /// <returns>List of text data from each row</returns>
        Task<List<string>> ReadExcelFileAsync(string filePath);

        /// <summary>
        /// Validates if the file is a valid Excel file
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        /// <returns>True if valid Excel file, false otherwise</returns>
        bool IsValidExcelFile(string fileName);
    }
}