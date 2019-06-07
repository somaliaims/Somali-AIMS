using AIMS.Models;
using Microsoft.AspNetCore.Hosting;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Drawing;
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

        /// <summary>
        /// Generates excel for sector projects report
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        ActionResponse GenerateSectorProjectsReport(ProjectProfileReportBySector report);
    }

    public class ExcelGeneratorService : IExcelGeneratorService
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
                string sWebRootFolder = hostingEnvironment.WebRootPath + "/ExcelFiles/";
                string sFileName = @"demo.xlsx";
                FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
                var memory = new MemoryStream();
                using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
                {
                    IWorkbook workbook;
                    workbook = new XSSFWorkbook();
                    ISheet excelSheet = workbook.CreateSheet("Demo");
                    IRow row = excelSheet.CreateRow(0);

                    var style = workbook.CreateCellStyle();
                    NPOI.SS.UserModel.IFont font1 = workbook.CreateFont();
                    font1.Color = IndexedColors.DarkBlue.Index;
                    font1.IsBold = true;
                    font1.Boldweight = 16;
                    font1.FontHeight = 18;
                    font1.FontHeightInPoints = 18;
                    style.SetFont(font1);
                    style.FillBackgroundColor = IndexedColors.LightYellow.Index;
                    style.Alignment = HorizontalAlignment.Center;
                    row.RowStyle = style;

                    var titleCell = row.CreateCell(0, CellType.String);
                    titleCell.SetCellValue("Sector Projects Report");
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        1, 1, 0, 3));
                    titleCell.CellStyle = style;


                    row = excelSheet.CreateRow(2);
                    row.CreateCell(0, CellType.Blank);
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        2, 2, 0, 3
                        ));

                    row = excelSheet.CreateRow(3);
                    row.CreateCell(0).CellStyle = style;
                    row.CreateCell(0).SetCellValue("ID");
                    row.CreateCell(1).SetCellValue("Name");
                    row.CreateCell(2).SetCellValue("Age");

                    row = excelSheet.CreateRow(4);
                    row.CreateCell(0).SetCellValue(1);
                    row.CreateCell(1).SetCellValue("Kane Williamson");
                    row.CreateCell(2).SetCellValue(29);

                    row = excelSheet.CreateRow(5);
                    row.CreateCell(0).SetCellValue(2);
                    row.CreateCell(1).SetCellValue("Martin Guptil");
                    row.CreateCell(2).SetCellValue(33);

                    row = excelSheet.CreateRow(6);
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

        public ActionResponse GenerateSectorProjectsReport(ProjectProfileReportBySector report)
        {
            ActionResponse response = new ActionResponse();
            try
            {
                string sWebRootFolder = hostingEnvironment.WebRootPath + "/ExcelFiles/";
                string sFileName = @"SectorProjects-" + DateTime.UtcNow.Ticks.ToString() + ".xlsx";
                FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
                var memory = new MemoryStream();
                using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
                {
                    int rowCounter = 0, totalColumns = 6;
                    decimal grandTotalFunding = 0, grandTotalDisbursement = 0;
                    IWorkbook workbook;
                    workbook = new XSSFWorkbook();
                    ISheet excelSheet = workbook.CreateSheet("Report");
                    IRow row = excelSheet.CreateRow(rowCounter);
                    row.CreateCell(0, CellType.Blank);
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        1, 1, 0, 3
                        ));

                    row = excelSheet.CreateRow(++rowCounter);
                    var titleCell = row.CreateCell(0, CellType.String);
                    titleCell.SetCellValue("Sector Projects Report");
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        rowCounter, rowCounter, 0, totalColumns));

                    ICellStyle style = workbook.CreateCellStyle();
                    NPOI.SS.UserModel.IFont font1 = workbook.CreateFont();
                    font1.Color = IndexedColors.DarkBlue.Index;
                    font1.IsBold = true;
                    font1.Boldweight = 16;
                    font1.FontHeight = 18;
                    font1.FontHeightInPoints = 18;
                    style.SetFont(font1);
                    style.Alignment = HorizontalAlignment.Center;
                    style.FillBackgroundColor = IndexedColors.LightYellow.Index;
                    row.RowStyle = style;
                    titleCell.CellStyle = style;

                    //Header columns row
                    row = excelSheet.CreateRow(++rowCounter);
                    row.CreateCell(0).SetCellValue("Projects");
                    row.CreateCell(1).SetCellValue("Funders");
                    row.CreateCell(2).SetCellValue("Implementers");
                    row.CreateCell(3).SetCellValue("Project Cost");
                    row.CreateCell(4).SetCellValue("Actual Disbursements");
                    row.CreateCell(5).SetCellValue("Planned Disbursements");

                    foreach (var sector in report.SectorProjectsList)
                    {
                        string sectorName = sector.SectorName + " Funding (" + sector.TotalFunding + ") - Disbursements (" + sector.TotalDisbursements + ")";
                        row = excelSheet.CreateRow(++rowCounter);
                        grandTotalFunding += sector.TotalFunding;
                        grandTotalDisbursement += sector.TotalDisbursements;

                        row.CreateCell(0, CellType.String).SetCellValue(sectorName);
                        excelSheet.AddMergedRegion(new CellRangeAddress(
                            rowCounter, rowCounter, 0, totalColumns));

                        foreach(var project in sector.Projects)
                        {
                            row = excelSheet.CreateRow(++rowCounter);
                            row.CreateCell(0).SetCellValue(project.Title);
                            row.CreateCell(1).SetCellValue(project.Funders);
                            row.CreateCell(2).SetCellValue(project.Implementers);
                            row.CreateCell(3).SetCellValue(project.ProjectCost.ToString());
                            row.CreateCell(4).SetCellValue(project.ActualDisbursements.ToString());
                            row.CreateCell(5).SetCellValue(project.PlannedDisbursements.ToString());
                        }
                        row = excelSheet.CreateRow(++rowCounter);
                        string grandTotalString = "Total funding: " + grandTotalFunding + " - Total disbursements: " + grandTotalDisbursement;
                        row.CreateCell(0, CellType.String).SetCellValue(grandTotalString);
                        excelSheet.AddMergedRegion(new CellRangeAddress(
                            rowCounter, rowCounter, 0, totalColumns));
                    }

                    workbook.Write(fs);
                }
                response.Message = sFileName;
            }
            catch(Exception ex)
            {
                response.Message = ex.Message;
                response.Success = false;
            }
            return response;
        }
    }
}
