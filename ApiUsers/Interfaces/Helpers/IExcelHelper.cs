using MiniExcelLibs;
using MiniExcelLibs.OpenXml;
using System.Data;

namespace ApiUsers.Interfaces.Helpers
{
    public interface IExcelHelper
    {
        Task<string> CreateWorkBookAsync<TRow>(IEnumerable<TRow> rows, 
            string sheetName = "Export", 
            OpenXmlConfiguration? exportConfiguration = null, 
            string[] ignoreColumns = null, 
            bool takeDefaultValues = true, 
            CancellationToken cancellationToken = default) where TRow : class;

        Task<IEnumerable<TRow>> ReadWorkSheetAsync<TRow>(string filePath, ExcelType excelType,
            MiniExcelLibs.IConfiguration configuration = default,
            string sheetName = "Import_Layout",
            CancellationToken cancellationToken = default) where TRow : class, new();

        Task<IEnumerable<TRow>> ReadWorkSheetAsync<TRow>(string filePath, ExcelType excelType,
            Dictionary<string, string> columns = null,
            MiniExcelLibs.IConfiguration configuration = default,
            string sheetName = "Import_Layout",
            CancellationToken cancellationToken = default) where TRow : class;
    }
}
