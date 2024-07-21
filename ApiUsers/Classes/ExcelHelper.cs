using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using SpreadsheetGear;
using System.Data;

namespace ApiUsers.Classes
{
    public class ExcelHelper
    {
        public static bool IsValidFile()
        {
            return true;
        }
        public static async Task<IEnumerable<T>> SerializeToObject<T>(IFormFile _excelfile)
        {
            DataTable dataTable = await GetDataTableFromExcel(_excelfile) ?? new DataTable();

            //VERIFY NOT NULL
            IEnumerable<T> list = JArray.FromObject(dataTable).ToObject<List<T>>() ?? new List<T>();

            return list;
        }

        public static async Task<DataTable?> GetDataTableFromExcel(IFormFile _excelfile)
        {
            string tempFile = await FileHelper.SaveTempFile(_excelfile);

            DataSet ds = GetDataSetFromExcel(tempFile);

            FileHelper.DeleteTempFile(tempFile);

            return (ds.Tables.Count > 0) ? ds.Tables[0] : null;

        }
        public static DataTable? GetDataTableFromExcel(string _filepath)
        {
            try
            {
                DataSet ds = GetDataSetFromExcel(_filepath);

                if (ds.Tables.Count > 0)
                {
                    return ds.Tables[0];
                }

                return null;
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public static DataSet GetDataSetFromExcel(string _filepath) 
        {
            try
            {
                DataSet ds = new DataSet();

                IWorkbook wb = Factory.GetWorkbook(_filepath);

                if (wb != null)
                {
                    IWorksheets sheets = wb.Worksheets;

                    foreach (IWorksheet sheet in sheets)
                    {
                        string name = sheet.Name;
                        DataTable dt = new DataTable(name);
                        IRange usedRange = sheet.UsedRange;
                        //Para obtener el header manualmente solo hay que tomar la fila 0 por defecto
                        dt = usedRange.GetDataTable(SpreadsheetGear.Data.GetDataFlags.None);
                        ds.Tables.Add(dt);
                    }
                }

                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception($"GetDataSetFromExcel: {ex.Message}");
            }
            
        }

        public static void ExportToExcel(string _filepath, DataTable dt)
        { 

        }

        public static string GetBase64FromFile(string _filepath)
        {
            return string.Empty;
        }
    }
}
