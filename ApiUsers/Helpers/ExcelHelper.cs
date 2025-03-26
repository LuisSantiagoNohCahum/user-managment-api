using MiniExcelLibs;
using MiniExcelLibs.Attributes;
using MiniExcelLibs.OpenXml;

namespace ApiUsers.Helpers
{
    //TODO. If Sheet name is null or empty take the first sheet in the workbook if exist, otherwise take the given sheet name.
    public class ExcelHelper : IExcelHelper
    {
        private readonly string _tempPath;
        public ExcelHelper(IWebHostEnvironment webHostEnvironment)
        {
            _tempPath = webHostEnvironment.WebRootPath + "/Temp";
        }

        public async Task<byte[]> CreateWorkBookAsync<TRow>(IEnumerable<TRow> rows, string sheetName = "Export", OpenXmlConfiguration? exportConfiguration = null, string[]? ignoreColumns = null, bool takeDefaultValues = true, CancellationToken ct = default) where TRow : class
        {
            exportConfiguration ??= new OpenXmlConfiguration()
            {
                EnableWriteNullValueCell = takeDefaultValues,
                TableStyles = MiniExcelLibs.OpenXml.TableStyles.None,
                DynamicColumnFirst = true,
                DynamicColumns = GetDynamicColumnsSetup<TRow>(ignoreColumns)
            };

            if (Directory.Exists(_tempPath)) Directory.CreateDirectory(_tempPath);

            string fullTempFilePath = Path.Combine(_tempPath, $"{Guid.NewGuid()}.xlsx");

            await MiniExcel.SaveAsAsync(fullTempFilePath, rows, true, sheetName, configuration: exportConfiguration, cancellationToken: ct);

            var fileData = await File.ReadAllBytesAsync(fullTempFilePath, ct);

            try { File.Delete(fullTempFilePath); } catch {}
            
            return fileData;
        }

        private DynamicExcelColumn[] GetDynamicColumnsSetup<TRow>(string[]? ignoreColumns)
        {
            var columns = typeof(TRow).GetProperties();

            List<DynamicExcelColumn> dynamicColumns = new List<DynamicExcelColumn>();

            int index = 0;
            foreach (var column in columns)
            {
                DynamicExcelColumn dynamicColumn;

                if (ignoreColumns is not null && ignoreColumns.Contains(column.Name, StringComparer.OrdinalIgnoreCase))
                {
                    dynamicColumn = new DynamicExcelColumn(column.Name.ToLower())
                    {
                        Name = column.Name,
                        Ignore = true,
                        Width = 0
                    };
                }
                else
                {
                    dynamicColumn = new DynamicExcelColumn(column.Name.ToLower())
                    {
                        Index = index,
                        Name = column.Name,
                        Ignore = false
                    };

                    index++;
                }

                dynamicColumns.Add(dynamicColumn);
            }

            return dynamicColumns.ToArray();
        }

        public async Task<IEnumerable<TRow>> ReadWorkSheetAsync<TRow>(string filePath, string sheetName = "Import_Layout", bool isExpendedModelType = false, CancellationToken ct = default) where TRow : class, new()
        {
            filePath.Guard(nameof(filePath));

            if (!IsValidFile(filePath))
                throw new Exception($"Invalid excel file.");

            var result = new List<TRow>();

            var targetProps = typeof(TRow).GetProperties();

            using (var excelStream = File.OpenRead(filePath))
            {
                if (!isExpendedModelType)
                    return await excelStream.QueryAsync<TRow>(sheetName: sheetName, cancellationToken: ct);

                var rows = await excelStream.QueryAsync(useHeaderRow: true, sheetName: sheetName, cancellationToken: ct);

                if (rows is null || !rows.Any()) 
                    return result;
                
                foreach (var row in rows)
                {
                    var instance = Activator.CreateInstance<TRow>();
                    object parsedRow = row as object;

                    foreach (var targetProp in targetProps)
                    {
                        var matchedProp = parsedRow
                            .GetType()
                            .GetProperties()
                            .Where(p => p.Name.Equals(targetProp.Name, StringComparison.OrdinalIgnoreCase))
                            .FirstOrDefault();

                        if (matchedProp is null) continue;

                        object? value = matchedProp.GetValue(parsedRow);
                        if (value is null) continue;

                        targetProp.SetValue(instance, ParseValue(value, targetProp.PropertyType));
                    }
                    result.Add(instance);
                }
            }
            return result;
        }

        private bool IsValidFile(string filePath)
            => File.Exists(filePath)
            && (filePath.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) 
            || filePath.EndsWith(".xls", StringComparison.OrdinalIgnoreCase));

        private object? ParseValue(object value, Type targetType)
        {
            TypeCode typeCode = Type.GetTypeCode(targetType);

            return typeCode switch
            {
                TypeCode.Boolean => Convert.ToBoolean(value),
                TypeCode.Char => Convert.ToChar(value),
                TypeCode.Int16 => Convert.ToInt16(value),
                TypeCode.UInt16 => Convert.ToUInt16(value),
                TypeCode.Int32 => Convert.ToInt32(value),
                TypeCode.UInt32 => Convert.ToUInt32(value),
                TypeCode.Int64 => Convert.ToInt64(value),
                TypeCode.UInt64 => Convert.ToUInt64(value),
                TypeCode.Single => Convert.ToSingle(value),
                TypeCode.Double => Convert.ToDouble(value),
                TypeCode.Decimal => Convert.ToDecimal(value),
                TypeCode.DateTime => Convert.ToDateTime(value, System.Globalization.CultureInfo.InvariantCulture),
                TypeCode.String => Convert.ToString(value),
                _ => throw new Exception("Not supported type.")
            };
        }
    }
}
