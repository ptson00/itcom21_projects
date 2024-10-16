using Dashboard_ITCom21.Models;
using Ganss.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard_ITCom21.Tools
{
    class ExportExcel
    {
        public static void CreateExcelExample(string fileExport)
        {
            var excel = new ExcelMapper();
            List<Report> data = new List<Report>();
            data.Add(new Report { });
            excel.SaveAsync(fileExport, data, sheetName: "example");
        }
        public async static void ExportExcelSingleSheet<T>(string fileExport, IEnumerable<T> listData)
        {
            var excel = new ExcelMapper();
            if (listData.Any())
            {
                await excel.SaveAsync(fileExport, listData, sheetName: "DailyReport");
            }

        }
    }
}
