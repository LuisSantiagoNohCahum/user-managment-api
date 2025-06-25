using MiniExcelLibs;
using MiniExcelLibs.Attributes;
using MiniExcelLibs.OpenXml;
using System.Dynamic;
using System.Reflection;

namespace ApiUsers.Helpers
{
    public class ExcelHelper : IExcelHelper
    {
        private readonly string _tempPath;
        public ExcelHelper(IWebHostEnvironment webHostEnvironment)
        {
            _tempPath = Path.Combine(webHostEnvironment.WebRootPath, "Temp");
        }

        public async Task<string> CreateWorkBookAsync<TRow>(IEnumerable<TRow> rows, string sheetName = "Export", OpenXmlConfiguration? exportConfiguration = null, string[] ignoreColumns = null, bool takeDefaultValues = true, CancellationToken cancellationToken = default) where TRow : class
        {
            exportConfiguration ??= new OpenXmlConfiguration()
            {
                EnableWriteNullValueCell = takeDefaultValues,
                TableStyles = TableStyles.Default,
                DynamicColumnFirst = true,
                FastMode = true,
                EnableAutoWidth = true,
                DynamicColumns = GetDynamicColumnsSetup<TRow>(ignoreColumns)
            };

            if (!Directory.Exists(_tempPath))
            { 
                Directory.CreateDirectory(_tempPath); 
            }

            string fullTempFilePath = Path.Combine(_tempPath, $"{Guid.NewGuid()}.xlsx");

            await MiniExcel.SaveAsAsync(fullTempFilePath, rows, true, sheetName, configuration: exportConfiguration, cancellationToken: cancellationToken);

            return fullTempFilePath;
        }

        private DynamicExcelColumn[] GetDynamicColumnsSetup<TRow>(string[] ignoreColumns)
        {
            var columns = typeof(TRow).GetProperties().Select(p => new 
            { 
                Key = p.Name,
                Caption = p.Name.Replace("_", " ").ToUpper(),
            });

            List<DynamicExcelColumn> dynamicColumns = new List<DynamicExcelColumn>();

            int index = 0;

            foreach (var column in columns)
            {
                DynamicExcelColumn dynamicColumn;

                // TODO. Allow send custom display name in a dictionary as on read and delete the ignore columns
                // (if you need skip columns send only the need data on anonymus object or send the required columns)
                if (ignoreColumns is not null && ignoreColumns.Contains(column.Key, StringComparer.OrdinalIgnoreCase))
                {
                    dynamicColumn = new DynamicExcelColumn(column.Key)
                    {
                        Name = column.Caption,
                        Ignore = true,
                        Width = 0,
                    };
                }
                else
                {
                    dynamicColumn = new DynamicExcelColumn(column.Key)
                    {
                        Name = column.Caption,
                        Index = index,
                        Ignore = false
                    };

                    index++;
                }

                dynamicColumns.Add(dynamicColumn);
            }

            return dynamicColumns.ToArray();
        }

        public async Task<IEnumerable<TRow>> ReadWorkSheetAsync<TRow>(string filePath,  ExcelType excelType, 
            MiniExcelLibs.IConfiguration configuration = default, 
            string sheetName = "Import_Layout", 
            CancellationToken cancellationToken = default) where TRow : class, new()
        {
            filePath.Guard(nameof(filePath));

            using (var excelStream = File.OpenRead(filePath))
            {
                return await excelStream.QueryAsync<TRow>(
                    sheetName: sheetName, 
                    excelType: excelType, 
                    configuration: configuration, 
                    cancellationToken: cancellationToken);
            }
        }

        public async Task<IEnumerable<TRow>> ReadWorkSheetAsync<TRow>(string filePath, ExcelType excelType,
            Dictionary<string, string> columns = null,
            MiniExcelLibs.IConfiguration configuration = default,
            string sheetName = "ImportLayout",
            CancellationToken cancellationToken = default) where TRow : class
        {
            filePath.Guard(nameof(filePath));

            using (var excelStream = File.OpenRead(filePath))
            {
                var rows = await excelStream.QueryAsync(
                    useHeaderRow: true,
                    sheetName: sheetName,
                    excelType: excelType,
                    configuration: configuration,
                    cancellationToken: cancellationToken);

                if (rows is null || !rows.Any())
                {
                    return Enumerable.Empty<TRow>();
                }

                if (typeof(TRow) == typeof(object) || (typeof(IDynamicMetaObjectProvider)).IsAssignableFrom(typeof(TRow)))
                {
                    return rows.Select(row => (TRow)row);
                }

                var targetProperties = typeof(TRow).GetProperties().ToList();

                var parsedRows = new List<TRow>();

                foreach (var row in rows)
                {
                    var instance = Activator.CreateInstance<TRow>();

                    var expando = (row as IDictionary<string, object>);

                    foreach(var tp in targetProperties)
                    {
                        string fieldName = columns is not null && columns.Any() && columns.TryGetValue(tp.Name, out string field) ? field : tp.Name;

                        if (expando.TryGetValue(fieldName, out var value) && value is not null)
                        {
                            tp.SetValue(instance, ParseValue(value, tp.PropertyType));
                        }
                    }

                    parsedRows.Add(instance);
                }

                return parsedRows;
            }
        }

        private object ParseValue(object value, Type targetType)
            => Type.GetTypeCode(targetType) switch
            {
                TypeCode.Boolean => Convert.ToBoolean(int.TryParse(value.ToString(), out var result) ? result : value),
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
