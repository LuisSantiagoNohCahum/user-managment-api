using MiniExcelLibs.OpenXml;
using System.Data;

namespace ApiUsers.Interfaces.Helpers
{
    public interface IExcelHelper
    {
        Task<byte[]> CreateWorkBookAsync<TRow>(IEnumerable<TRow> rows, string sheetName = "Export", OpenXmlConfiguration? exportConfiguration = null, string[]? ignoreColumns = null, bool takeDefaultValues = true, CancellationToken ct = default) where TRow : class;
        
        //Task<byte[]> CreateMassiveWoorkBooks<TRow>(IEnumerable<TRow> rows, int partitionSize = 10000, bool multifile = true, CancellationToken ct = default) where TRow : class;
        Task<IEnumerable<TRow>> ReadWorkSheetAsync<TRow>(string filePath, string sheetName = "Import_Layout", bool isExpendedModelType = false, CancellationToken ct = default) where TRow : class, new();
    }
}
