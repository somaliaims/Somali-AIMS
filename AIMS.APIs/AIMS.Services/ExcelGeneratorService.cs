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

        /// <summary>
        /// Generates excel for location projects report
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        ActionResponse GenerateLocationProjectsReport(ProjectProfileReportByLocation report);
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
                    NPOI.SS.UserModel.IFont fontTitle = workbook.CreateFont();
                    fontTitle.Color = IndexedColors.DarkBlue.Index;
                    fontTitle.IsBold = true;
                    fontTitle.Boldweight = 18;
                    fontTitle.FontHeight = 18;
                    fontTitle.FontHeightInPoints = 18;
                    style.SetFont(fontTitle);
                    style.FillBackgroundColor = IndexedColors.LightYellow.Index;
                    style.Alignment = HorizontalAlignment.Center;
       

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

                    //Setting styles for different cell types
                    ICellStyle titleStyle = workbook.CreateCellStyle();
                    NPOI.SS.UserModel.IFont fontTitle = workbook.CreateFont();
                    fontTitle.Color = IndexedColors.DarkGreen.Index;
                    fontTitle.IsBold = true;
                    fontTitle.FontHeightInPoints = 16;
                    titleStyle.SetFont(fontTitle);
                    titleStyle.Alignment = HorizontalAlignment.CenterSelection;
                    titleStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle footerStyle = workbook.CreateCellStyle();
                    NPOI.SS.UserModel.IFont footerFontTitle = workbook.CreateFont();
                    footerFontTitle.Color = IndexedColors.DarkGreen.Index;
                    footerFontTitle.IsBold = true;
                    fontTitle.FontHeightInPoints = 16;
                    footerStyle.SetFont(fontTitle);
                    footerStyle.Alignment = HorizontalAlignment.Right;
                    footerStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle headerStyle = workbook.CreateCellStyle();
                    NPOI.SS.UserModel.IFont fontHeader = workbook.CreateFont();
                    fontHeader.Color = IndexedColors.Black.Index;
                    fontHeader.IsBold = true;
                    headerStyle.SetFont(fontTitle);
                    fontTitle.Boldweight = 11;
                    fontTitle.FontHeight = 11;
                    fontTitle.FontHeightInPoints = 11;
                    headerStyle.Alignment = HorizontalAlignment.Center;
                    headerStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle groupHeaderStyle = workbook.CreateCellStyle();
                    NPOI.SS.UserModel.IFont groupFontHeader = workbook.CreateFont();
                    groupFontHeader.Color = IndexedColors.DarkYellow.Index;
                    groupFontHeader.IsBold = true;
                    groupHeaderStyle.SetFont(fontTitle);
                    groupHeaderStyle.Alignment = HorizontalAlignment.Center;
                    groupHeaderStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle dataCellStyle = workbook.CreateCellStyle();
                    dataCellStyle.WrapText = true;
                    dataCellStyle.Alignment = HorizontalAlignment.Center;
                    dataCellStyle.VerticalAlignment = VerticalAlignment.Center;

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
                    titleCell.CellStyle = titleStyle;

                    //Header columns row
                    row = excelSheet.CreateRow(++rowCounter);
                    var projectCol = row.CreateCell(0);
                    projectCol.SetCellValue("Projects");
                    projectCol.CellStyle = headerStyle;

                    var funderCol = row.CreateCell(1);
                    funderCol.SetCellValue("Funders");
                    funderCol.CellStyle = headerStyle;

                    var implementerCol = row.CreateCell(2);
                    implementerCol.SetCellValue("Implementers");
                    implementerCol.CellStyle = headerStyle;

                    var projectCostCol = row.CreateCell(3);
                    projectCostCol.SetCellValue("Project Cost");
                    projectCostCol.CellStyle = headerStyle;

                    var actualDisbursementsCol = row.CreateCell(4);
                    actualDisbursementsCol.SetCellValue("Actual Disbursements");
                    actualDisbursementsCol.CellStyle = headerStyle;

                    var plannedDisbursementsCol = row.CreateCell(5);
                    plannedDisbursementsCol.SetCellValue("Planned Disbursements");
                    plannedDisbursementsCol.CellStyle = headerStyle;

                    foreach (var sector in report.SectorProjectsList)
                    {
                        string sectorName = sector.SectorName + " Funding (" + sector.TotalFunding + ") - Disbursements (" + sector.TotalDisbursements + ")";
                        row = excelSheet.CreateRow(++rowCounter);
                        grandTotalFunding += sector.TotalFunding;
                        grandTotalDisbursement += sector.TotalDisbursements;

                        var groupTitleCell = row.CreateCell(0, CellType.String);
                        excelSheet.AddMergedRegion(new CellRangeAddress(
                            rowCounter, rowCounter, 0, totalColumns));
                        groupTitleCell.SetCellValue(sectorName);
                        groupTitleCell.CellStyle = groupHeaderStyle;

                        foreach (var project in sector.Projects)
                        {
                            row = excelSheet.CreateRow(++rowCounter);
                            var titleDataCell = row.CreateCell(0);
                            titleDataCell.SetCellValue(project.Title);
                            titleDataCell.CellStyle = dataCellStyle;

                            var funderDataCell = row.CreateCell(1);
                            funderDataCell.SetCellValue(project.Funders);
                            funderDataCell.CellStyle = dataCellStyle;

                            var implementerDataCell = row.CreateCell(2);
                            implementerDataCell.SetCellValue(project.Implementers);
                            implementerDataCell.CellStyle = dataCellStyle;

                            var projectCostDataCell = row.CreateCell(3);
                            projectCostDataCell.SetCellValue(project.ProjectCost.ToString());
                            projectCostDataCell.CellStyle = dataCellStyle;

                            var actualDisbursementDataCell = row.CreateCell(4);
                            actualDisbursementDataCell.SetCellValue(project.ActualDisbursements.ToString());
                            actualDisbursementDataCell.CellStyle = dataCellStyle;

                            var plannedDisbursementDataCell = row.CreateCell(5);
                            plannedDisbursementDataCell.SetCellValue(project.PlannedDisbursements.ToString());
                            plannedDisbursementDataCell.CellStyle = dataCellStyle;
                        }
                    }

                    row = excelSheet.CreateRow(++rowCounter);
                    string grandTotalString = "Total funding: " + grandTotalFunding + " - Total disbursements: " + grandTotalDisbursement;
                    var footerCell = row.CreateCell(0, CellType.String);
                    footerCell.SetCellValue(grandTotalString);
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        rowCounter, rowCounter, 0, totalColumns));
                    footerCell.CellStyle = footerStyle;
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

        public ActionResponse GenerateLocationProjectsReport(ProjectProfileReportByLocation report)
        {
            ActionResponse response = new ActionResponse();
            try
            {
                string sWebRootFolder = hostingEnvironment.WebRootPath + "/ExcelFiles/";
                string sFileName = @"LocationProjects-" + DateTime.UtcNow.Ticks.ToString() + ".xlsx";
                FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
                var memory = new MemoryStream();
                using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
                {
                    int rowCounter = 0, totalColumns = 6;
                    decimal grandTotalFunding = 0, grandTotalDisbursement = 0;

                    
                    IWorkbook workbook;
                    workbook = new XSSFWorkbook();
                    ISheet excelSheet = workbook.CreateSheet("Report");

                    //Setting styles for different cell types
                    ICellStyle titleStyle = workbook.CreateCellStyle();
                    NPOI.SS.UserModel.IFont fontTitle = workbook.CreateFont();
                    fontTitle.Color = IndexedColors.DarkGreen.Index;
                    fontTitle.IsBold = true;
                    titleStyle.SetFont(fontTitle);
                    fontTitle.FontHeightInPoints = 16;
                    titleStyle.Alignment = HorizontalAlignment.CenterSelection;
                    titleStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle footerStyle = workbook.CreateCellStyle();
                    NPOI.SS.UserModel.IFont footerFontTitle = workbook.CreateFont();
                    footerFontTitle.Color = IndexedColors.DarkGreen.Index;
                    footerFontTitle.IsBold = true;
                    fontTitle.FontHeightInPoints = 16;
                    footerStyle.SetFont(fontTitle);
                    footerStyle.Alignment = HorizontalAlignment.Right;
                    footerStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle headerStyle = workbook.CreateCellStyle();
                    NPOI.SS.UserModel.IFont fontHeader = workbook.CreateFont();
                    fontHeader.Color = IndexedColors.Black.Index;
                    fontHeader.IsBold = true;
                    headerStyle.SetFont(fontTitle);
                    fontTitle.Boldweight = 11;
                    fontTitle.FontHeight = 11;
                    fontTitle.FontHeightInPoints = 11;
                    headerStyle.Alignment = HorizontalAlignment.Center;
                    headerStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle groupHeaderStyle = workbook.CreateCellStyle();
                    NPOI.SS.UserModel.IFont groupFontHeader = workbook.CreateFont();
                    groupFontHeader.Color = IndexedColors.DarkYellow.Index;
                    groupFontHeader.IsBold = true;
                    groupHeaderStyle.SetFont(fontTitle);
                    groupHeaderStyle.Alignment = HorizontalAlignment.Center;
                    groupHeaderStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle dataCellStyle = workbook.CreateCellStyle();
                    dataCellStyle.WrapText = true;
                    dataCellStyle.Alignment = HorizontalAlignment.Center;
                    dataCellStyle.VerticalAlignment = VerticalAlignment.Center;

                    IRow row = excelSheet.CreateRow(rowCounter);
                    row.CreateCell(0, CellType.Blank);
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        1, 1, 0, 3
                        ));

                    row = excelSheet.CreateRow(++rowCounter);
                    var titleCell = row.CreateCell(0, CellType.String);
                    titleCell.SetCellValue("Location Projects Report");
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        rowCounter, rowCounter, 0, totalColumns));
                    titleCell.CellStyle = titleStyle;

                    //Header columns row
                    row = excelSheet.CreateRow(++rowCounter);
                    var projectCol = row.CreateCell(0);
                    projectCol.SetCellValue("Projects");
                    projectCol.CellStyle = headerStyle;

                    var funderCol = row.CreateCell(1);
                    funderCol.SetCellValue("Funders");
                    funderCol.CellStyle = headerStyle;

                    var implementerCol = row.CreateCell(2);
                    implementerCol.SetCellValue("Implementers");
                    implementerCol.CellStyle = headerStyle;

                    var projectCostCol = row.CreateCell(3);
                    projectCostCol.SetCellValue("Project Cost");
                    projectCostCol.CellStyle = headerStyle;

                    var actualDisbursementsCol = row.CreateCell(4);
                    actualDisbursementsCol.SetCellValue("Actual Disbursements");
                    actualDisbursementsCol.CellStyle = headerStyle;

                    var plannedDisbursementsCol = row.CreateCell(5);
                    plannedDisbursementsCol.SetCellValue("Planned Disbursements");
                    plannedDisbursementsCol.CellStyle = headerStyle;

                    foreach (var location in report.LocationProjectsList)
                    {
                        string locationName = location.LocationName + " Funding (" + location.TotalFunding + ") - Disbursements (" + location.TotalDisbursements + ")";
                        row = excelSheet.CreateRow(++rowCounter);
                        grandTotalFunding += location.TotalFunding;
                        grandTotalDisbursement += location.TotalDisbursements;

                        var groupTitleCell = row.CreateCell(0, CellType.String);
                        excelSheet.AddMergedRegion(new CellRangeAddress(
                            rowCounter, rowCounter, 0, totalColumns));
                        groupTitleCell.SetCellValue(locationName);
                        groupTitleCell.CellStyle = groupHeaderStyle;

                        foreach (var project in location.Projects)
                        {
                            row = excelSheet.CreateRow(++rowCounter);
                            var titleDataCell = row.CreateCell(0);
                            titleDataCell.SetCellValue(project.Title);
                            titleDataCell.CellStyle = dataCellStyle;

                            var funderDataCell = row.CreateCell(1);
                            funderDataCell.SetCellValue(project.Funders);
                            funderDataCell.CellStyle = dataCellStyle;

                            var implementerDataCell = row.CreateCell(2);
                            implementerDataCell.SetCellValue(project.Implementers);
                            implementerDataCell.CellStyle = dataCellStyle;

                            var projectCostDataCell = row.CreateCell(3);
                            projectCostDataCell.SetCellValue(project.ProjectCost.ToString());
                            projectCostDataCell.CellStyle = dataCellStyle;

                            var actualDisbursementDataCell = row.CreateCell(4);
                            actualDisbursementDataCell.SetCellValue(project.ActualDisbursements.ToString());
                            actualDisbursementDataCell.CellStyle = dataCellStyle;

                            var plannedDisbursementDataCell = row.CreateCell(5);
                            plannedDisbursementDataCell.SetCellValue(project.PlannedDisbursements.ToString());
                            plannedDisbursementDataCell.CellStyle = dataCellStyle;
                        }
                    }

                    row = excelSheet.CreateRow(++rowCounter);
                    string grandTotalString = "Total funding: " + grandTotalFunding + " - Total disbursements: " + grandTotalDisbursement;
                    var footerCell = row.CreateCell(0, CellType.String);
                    footerCell.SetCellValue(grandTotalString);
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        rowCounter, rowCounter, 0, totalColumns));
                    footerCell.CellStyle = footerStyle;
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
