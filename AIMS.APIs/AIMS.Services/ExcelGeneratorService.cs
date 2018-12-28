using AIMS.Models;
using Microsoft.AspNetCore.Hosting;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Services
{
    public interface IExcelGeneratorService
    {
        /// <summary>
        /// Generates excel format for project report
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //Task<ActionResponse> GenerateProjectsReportAsync(ProjectReport model);
        Task<ActionResponse> GenerateProjectsReportAsync();
    }

    public class ExcelGeneratorService
    {
        private IHostingEnvironment hostingEnvironment;
        public ExcelGeneratorService(IHostingEnvironment _hostingEnvironment)
        {
            hostingEnvironment = _hostingEnvironment;
        }

        public async Task<ActionResponse> GenerateProjectsReportAsync()
        {
            ActionResponse response = new ActionResponse();
            try
            {
                string sWebRootFolder = hostingEnvironment.WebRootPath;
                string sFileName = @"demo.xlsx";
                FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
                var memory = new MemoryStream();
                using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
                {
                    IWorkbook workbook;
                    workbook = new XSSFWorkbook();
                    ISheet excelSheet = workbook.CreateSheet("Demo");
                    IRow row = excelSheet.CreateRow(0);

                    row.CreateCell(0).SetCellValue("ID");
                    row.CreateCell(1).SetCellValue("Name");
                    row.CreateCell(2).SetCellValue("Age");

                    row = excelSheet.CreateRow(1);
                    row.CreateCell(0).SetCellValue(1);
                    row.CreateCell(1).SetCellValue("Kane Williamson");
                    row.CreateCell(2).SetCellValue(29);

                    row = excelSheet.CreateRow(2);
                    row.CreateCell(0).SetCellValue(2);
                    row.CreateCell(1).SetCellValue("Martin Guptil");
                    row.CreateCell(2).SetCellValue(33);

                    row = excelSheet.CreateRow(3);
                    row.CreateCell(0).SetCellValue(3);
                    row.CreateCell(1).SetCellValue("Colin Munro");
                    row.CreateCell(2).SetCellValue(23);

                    workbook.Write(fs);
                }
                response.Message = sFileName;
            }
            catch(Exception ex)
            {
                response.Message = ex.Message;
                response.Success = false;
            }
            return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
        }
    }
}
