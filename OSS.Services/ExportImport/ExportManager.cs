using OSS.Services.ExportImport.Help;
using OSS.Services.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace OSS.Services.ExportImport
{
    /// <summary>
    /// Export manager
    /// </summary>
    public partial class ExportManager : IExportManager
    {
        #region Methods

        /// <summary>
        /// Export category list to XML
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result in XML format
        /// </returns>
        public virtual async Task<string> ExportCategoriesToXmlAsync()
        {
            var settings = new XmlWriterSettings
            {
                Async = true,
                ConformanceLevel = ConformanceLevel.Auto
            };

            await using var stringWriter = new StringWriter();
            await using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            await xmlWriter.WriteStartDocumentAsync();
            await xmlWriter.WriteStartElementAsync("Categories");
            await xmlWriter.WriteAttributeStringAsync("Version", "1.0");
            await xmlWriter.WriteEndElementAsync();
            await xmlWriter.WriteEndDocumentAsync();
            await xmlWriter.FlushAsync();

            return stringWriter.ToString();
        }

        public virtual async Task<byte[]> ExportTestTablesToXlsxAsync(IList<TestTableModel> lst)
        {
            //property manager 
            var manager = new PropertyManager<TestTableModel>(new[]
            {
                new PropertyByName<TestTableModel>("Id", p => p.EncrypedId),
                new PropertyByName<TestTableModel>("Text1", p => p.Text1),
                new PropertyByName<TestTableModel>("Text2", p => p.Text2)
            });

            return await manager.ExportToXlsxAsync(lst);
        }

        public virtual async Task<string> ExportTestTablesToXmlAsync(IList<TestTableModel> lst)
        {
            var settings = new XmlWriterSettings
            {
                Async = true,
                ConformanceLevel = ConformanceLevel.Auto
            };

            await using var stringWriter = new StringWriter();
            await using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            await xmlWriter.WriteStartDocumentAsync();
            await xmlWriter.WriteStartElementAsync("Customers");
            await xmlWriter.WriteAttributeStringAsync("Version", "1.0");

            foreach (var entity in lst)
            {
                await xmlWriter.WriteStartElementAsync("TestTable");
                await xmlWriter.WriteElementStringAsync("Id", null, entity.EncrypedId);
                await xmlWriter.WriteElementStringAsync("Text1", null, entity.Text1);
                await xmlWriter.WriteElementStringAsync("Text2", null, entity.Text2);

                await xmlWriter.WriteEndElementAsync();
            }

            await xmlWriter.WriteEndElementAsync();
            await xmlWriter.WriteEndDocumentAsync();
            await xmlWriter.FlushAsync();

            return stringWriter.ToString();
        }

        #endregion
    }
}
