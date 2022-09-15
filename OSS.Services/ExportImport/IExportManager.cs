using OSS.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OSS.Services.ExportImport
{
    /// <summary>
    /// Export manager interface
    /// </summary>
    public partial interface IExportManager
    {
        /// <summary>
        /// Export TestTables list to XLSX
        /// </summary>
        /// <param name="testTables">TestTableModel</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task<byte[]> ExportTestTablesToXlsxAsync(IList<TestTableModel> testTables);

        /// <summary>
        /// Export customer list to XML
        /// </summary>
        /// <param name="testTables">testTables</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result in XML format
        /// </returns>
        Task<string> ExportTestTablesToXmlAsync(IList<TestTableModel> testTables);

    }
}
