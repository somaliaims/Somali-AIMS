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

        /// <summary>
        /// Generates excel for yearly projects report
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        ActionResponse GenerateYearlyProjectsReport(TimeSeriesReportByYear report);
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
                    IFont fontTitle = workbook.CreateFont();
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
                    int rowCounter = 0, totalColumns = 5, groupHeaderColumns = 2;
                    decimal grandTotalFunding = 0, grandTotalDisbursement = 0;
                    
                    IWorkbook workbook;
                    workbook = new XSSFWorkbook();
                    ISheet excelSheet = workbook.CreateSheet("Report");

                    //Setting styles for different cell types
                    ICellStyle titleStyle = workbook.CreateCellStyle();
                    IFont fontTitle = workbook.CreateFont();
                    fontTitle.Color = IndexedColors.DarkBlue.Index;
                    fontTitle.IsBold = true;
                    fontTitle.FontHeightInPoints = 16;
                    titleStyle.SetFont(fontTitle);
                    titleStyle.Alignment = HorizontalAlignment.CenterSelection;
                    titleStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle linkStyle = workbook.CreateCellStyle();
                    IFont linkFont = workbook.CreateFont();
                    linkFont.Color = IndexedColors.DarkBlue.Index;
                    linkFont.IsBold = true;
                    linkFont.FontHeightInPoints = 14;
                    linkStyle.SetFont(linkFont);
                    linkStyle.Alignment = HorizontalAlignment.CenterSelection;
                    linkStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle headerStyle = workbook.CreateCellStyle();
                    IFont fontHeader = workbook.CreateFont();
                    fontHeader.Color = IndexedColors.Black.Index;
                    fontHeader.IsBold = true;
                    fontHeader.FontHeightInPoints = 12;
                    headerStyle.SetFont(fontHeader);
                    headerStyle.Alignment = HorizontalAlignment.Center;
                    headerStyle.VerticalAlignment = VerticalAlignment.Center;


                    ICellStyle groupHeaderStyle = workbook.CreateCellStyle();
                    IFont groupFontHeader = workbook.CreateFont();
                    groupFontHeader.Color = IndexedColors.DarkYellow.Index;
                    groupFontHeader.FontHeightInPoints = 12;
                    groupFontHeader.IsBold = true;
                    groupHeaderStyle.SetFont(groupFontHeader);
                    groupHeaderStyle.Alignment = HorizontalAlignment.Center;
                    groupHeaderStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle dataCellStyle = workbook.CreateCellStyle();
                    dataCellStyle.WrapText = true;
                    dataCellStyle.Alignment = HorizontalAlignment.Center;
                    dataCellStyle.VerticalAlignment = VerticalAlignment.Center;

                    IRow row = excelSheet.CreateRow(rowCounter);
                    row.CreateCell(0, CellType.Blank);
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        1, 1, 0, totalColumns
                        ));

                    row = excelSheet.CreateRow(++rowCounter);
                    var titleCell = row.CreateCell(0, CellType.String);
                    titleCell.SetCellValue("SomaliAIMS sector report - generated on " + DateTime.Now.ToLongDateString());
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        rowCounter, rowCounter, 0, totalColumns));
                    titleCell.CellStyle = titleStyle;

                    //Adding extra line
                    row = excelSheet.CreateRow(++rowCounter);
                    row.CreateCell(0, CellType.Blank);
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        rowCounter, rowCounter, 0, totalColumns
                        ));

                    //Header columns row
                    row = excelSheet.CreateRow(++rowCounter);
                    var projectCol = row.CreateCell(0);
                    projectCol.SetCellValue("Project");
                    projectCol.CellStyle = headerStyle;

                    var funderCol = row.CreateCell(1);
                    funderCol.SetCellValue("Funders");
                    funderCol.CellStyle = headerStyle;

                    var implementerCol = row.CreateCell(2);
                    implementerCol.SetCellValue("Implementers");
                    implementerCol.CellStyle = headerStyle;

                    var projectCostCol = row.CreateCell(3);
                    projectCostCol.SetCellValue("Project value");
                    projectCostCol.CellStyle = headerStyle;

                    var actualDisbursementsCol = row.CreateCell(4);
                    actualDisbursementsCol.SetCellValue("Actual disbursements");
                    actualDisbursementsCol.CellStyle = headerStyle;

                    var plannedDisbursementsCol = row.CreateCell(5);
                    plannedDisbursementsCol.SetCellValue("Planned disbursements");
                    plannedDisbursementsCol.CellStyle = headerStyle;

                    foreach (var sector in report.SectorProjectsList)
                    {
                        decimal totalFunding = 0, totalDisbursements = 0;
                        totalFunding = Math.Round(sector.TotalFunding, MidpointRounding.AwayFromZero);
                        totalDisbursements = Math.Round(sector.TotalDisbursements, MidpointRounding.AwayFromZero);
                        row = excelSheet.CreateRow(++rowCounter);
                        grandTotalFunding += Math.Round(sector.TotalFunding, MidpointRounding.AwayFromZero);
                        grandTotalDisbursement += Math.Round(sector.TotalDisbursements, MidpointRounding.AwayFromZero);

                        var groupTitleCell = row.CreateCell(0, CellType.String);
                        excelSheet.AddMergedRegion(new CellRangeAddress(
                            rowCounter, rowCounter, 0, groupHeaderColumns));
                        groupTitleCell.SetCellValue(sector.SectorName);
                        groupTitleCell.CellStyle = groupHeaderStyle;

                        var groupFundTotalCell = row.CreateCell(3, CellType.Numeric);
                        groupFundTotalCell.SetCellValue(ApplyThousandFormat(sector.TotalFunding));
                        groupFundTotalCell.CellStyle = groupHeaderStyle;

                        var groupDisbursementTotalCell = row.CreateCell(4, CellType.Numeric);
                        groupDisbursementTotalCell.SetCellValue(ApplyThousandFormat(sector.TotalDisbursements));
                        groupDisbursementTotalCell.CellStyle = groupHeaderStyle;

                        var groupPlannedDisbursementTotalCell = row.CreateCell(5, CellType.Numeric);
                        groupPlannedDisbursementTotalCell.SetCellValue("");

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
                            projectCostDataCell.SetCellValue(ApplyThousandFormat(project.ProjectCost));
                            projectCostDataCell.CellStyle = dataCellStyle;

                            var actualDisbursementDataCell = row.CreateCell(4);
                            actualDisbursementDataCell.SetCellValue(ApplyThousandFormat(project.ActualDisbursements));
                            actualDisbursementDataCell.CellStyle = dataCellStyle;

                            var plannedDisbursementDataCell = row.CreateCell(5);
                            plannedDisbursementDataCell.SetCellValue(ApplyThousandFormat(project.PlannedDisbursements));
                            plannedDisbursementDataCell.CellStyle = dataCellStyle;
                        }
                    }

                    row = excelSheet.CreateRow(++rowCounter);
                    var footerCell = row.CreateCell(0, CellType.String);
                    footerCell.SetCellValue("Grand totals: ");
                    footerCell.CellStyle = headerStyle;
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        rowCounter, rowCounter, 0, groupHeaderColumns));
                    

                    var grandFundTotalCell = row.CreateCell(3, CellType.Numeric);
                    grandFundTotalCell.SetCellValue(ApplyThousandFormat(grandTotalFunding));
                    grandFundTotalCell.CellStyle = headerStyle;

                    var grandDisbursementTotalCell = row.CreateCell(4, CellType.Numeric);
                    grandDisbursementTotalCell.SetCellValue(ApplyThousandFormat(grandTotalDisbursement));
                    grandDisbursementTotalCell.CellStyle = headerStyle;

                    var grandPlannedDisbursementTotalCell = row.CreateCell(5, CellType.Blank);
                    grandPlannedDisbursementTotalCell.CellStyle = headerStyle;

                    row = excelSheet.CreateRow(++rowCounter);
                    var emptyCell = row.CreateCell(0, CellType.Blank);
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        rowCounter, rowCounter, 0, totalColumns));

                    row = excelSheet.CreateRow(++rowCounter);
                    var linkCell = row.CreateCell(0, CellType.String);
                    XSSFHyperlink urlLink = new XSSFHyperlink(HyperlinkType.Url)
                    {
                        Address = report.ReportSettings.ReportUrl
                    };
                    linkCell.SetCellValue("Click to view latest report");
                    linkCell.Hyperlink = (urlLink);
                    linkCell.CellStyle = linkStyle;
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        rowCounter, rowCounter, 0, totalColumns));

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
                    int rowCounter = 0, totalColumns = 5, groupHeaderColumns = 2;
                    decimal grandTotalFunding = 0, grandTotalDisbursement = 0;

                    
                    IWorkbook workbook;
                    workbook = new XSSFWorkbook();
                    ISheet excelSheet = workbook.CreateSheet("Report");

                    //Setting styles for different cell types
                    ICellStyle titleStyle = workbook.CreateCellStyle();
                    IFont fontTitle = workbook.CreateFont();
                    fontTitle.Color = IndexedColors.DarkBlue.Index;
                    fontTitle.IsBold = true;
                    fontTitle.FontHeightInPoints = 16;
                    titleStyle.SetFont(fontTitle);
                    titleStyle.Alignment = HorizontalAlignment.CenterSelection;
                    titleStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle linkStyle = workbook.CreateCellStyle();
                    IFont linkFont = workbook.CreateFont();
                    linkFont.Color = IndexedColors.DarkBlue.Index;
                    linkFont.IsBold = true;
                    linkFont.FontHeightInPoints = 14;
                    linkStyle.SetFont(linkFont);
                    linkStyle.Alignment = HorizontalAlignment.CenterSelection;
                    linkStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle headerStyle = workbook.CreateCellStyle();
                    IFont fontHeader = workbook.CreateFont();
                    fontHeader.Color = IndexedColors.Black.Index;
                    fontHeader.IsBold = true;
                    fontHeader.FontHeightInPoints = 12;
                    headerStyle.SetFont(fontHeader);
                    headerStyle.Alignment = HorizontalAlignment.Center;
                    headerStyle.VerticalAlignment = VerticalAlignment.Center;


                    ICellStyle groupHeaderStyle = workbook.CreateCellStyle();
                    IFont groupFontHeader = workbook.CreateFont();
                    groupFontHeader.Color = IndexedColors.DarkYellow.Index;
                    groupFontHeader.FontHeightInPoints = 12;
                    groupFontHeader.IsBold = true;
                    groupHeaderStyle.SetFont(groupFontHeader);
                    groupHeaderStyle.Alignment = HorizontalAlignment.Center;
                    groupHeaderStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle dataCellStyle = workbook.CreateCellStyle();
                    dataCellStyle.WrapText = true;
                    dataCellStyle.Alignment = HorizontalAlignment.Center;
                    dataCellStyle.VerticalAlignment = VerticalAlignment.Center;

                    IRow row = excelSheet.CreateRow(rowCounter);
                    row.CreateCell(0, CellType.Blank);
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        rowCounter, rowCounter, 0, totalColumns
                        ));

                    row = excelSheet.CreateRow(++rowCounter);
                    var titleCell = row.CreateCell(0, CellType.String);
                    titleCell.SetCellValue("SomaliAIMS location report - generated on " + DateTime.Now.ToLongDateString());
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        rowCounter, rowCounter, 0, totalColumns));
                    titleCell.CellStyle = titleStyle;

                    //Inserting extra row
                    row = excelSheet.CreateRow(++rowCounter);
                    row.CreateCell(0, CellType.Blank);
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        rowCounter, rowCounter, 0, totalColumns
                        ));

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
                    projectCostCol.SetCellValue("Project value");
                    projectCostCol.CellStyle = headerStyle;

                    var actualDisbursementsCol = row.CreateCell(4);
                    actualDisbursementsCol.SetCellValue("Actual disbursements");
                    actualDisbursementsCol.CellStyle = headerStyle;

                    var plannedDisbursementsCol = row.CreateCell(5);
                    plannedDisbursementsCol.SetCellValue("Planned disbursements");
                    plannedDisbursementsCol.CellStyle = headerStyle;

                    foreach (var location in report.LocationProjectsList)
                    {
                        row = excelSheet.CreateRow(++rowCounter);
                        grandTotalFunding += Math.Round(location.TotalFunding, MidpointRounding.AwayFromZero);
                        grandTotalDisbursement += Math.Round(location.TotalDisbursements, MidpointRounding.AwayFromZero);

                        var groupTitleCell = row.CreateCell(0, CellType.String);
                        excelSheet.AddMergedRegion(new CellRangeAddress(
                            rowCounter, rowCounter, 0, groupHeaderColumns));
                        groupTitleCell.SetCellValue(location.LocationName);
                        groupTitleCell.CellStyle = groupHeaderStyle;

                        var groupFundTotalCell = row.CreateCell(3, CellType.Numeric);
                        groupFundTotalCell.SetCellValue(ApplyThousandFormat(location.TotalFunding));
                        groupFundTotalCell.CellStyle = groupHeaderStyle;

                        var groupDisbursementTotalCell = row.CreateCell(4, CellType.Numeric);
                        groupDisbursementTotalCell.SetCellValue(ApplyThousandFormat(location.TotalDisbursements));
                        groupDisbursementTotalCell.CellStyle = groupHeaderStyle;

                        var groupPlannedDisbursementTotalCell = row.CreateCell(5, CellType.Numeric);
                        groupPlannedDisbursementTotalCell.SetCellValue("");

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
                            projectCostDataCell.SetCellValue(ApplyThousandFormat(project.ProjectCost));
                            projectCostDataCell.CellStyle = dataCellStyle;

                            var actualDisbursementDataCell = row.CreateCell(4);
                            actualDisbursementDataCell.SetCellValue(ApplyThousandFormat(project.ActualDisbursements));
                            actualDisbursementDataCell.CellStyle = dataCellStyle;

                            var plannedDisbursementDataCell = row.CreateCell(5);
                            plannedDisbursementDataCell.SetCellValue(ApplyThousandFormat(project.PlannedDisbursements));
                            plannedDisbursementDataCell.CellStyle = dataCellStyle;
                        }
                    }

                    row = excelSheet.CreateRow(++rowCounter);
                    var footerCell = row.CreateCell(0, CellType.String);
                    footerCell.SetCellValue("Grand totals: ");
                    footerCell.CellStyle = headerStyle;
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        rowCounter, rowCounter, 0, groupHeaderColumns));
                    

                    var grandFundTotalCell = row.CreateCell(3, CellType.Numeric);
                    grandFundTotalCell.SetCellValue(ApplyThousandFormat(grandTotalFunding));
                    grandFundTotalCell.CellStyle = headerStyle;

                    var grandDisbursementTotalCell = row.CreateCell(4, CellType.Numeric);
                    grandDisbursementTotalCell.SetCellValue(ApplyThousandFormat(grandTotalDisbursement));
                    grandDisbursementTotalCell.CellStyle = headerStyle;

                    var grandPlannedDisbursementTotalCell = row.CreateCell(5, CellType.Blank);
                    grandPlannedDisbursementTotalCell.CellStyle = headerStyle;

                    row = excelSheet.CreateRow(++rowCounter);
                    var emptyCell = row.CreateCell(0, CellType.Blank);
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        rowCounter, rowCounter, 0, totalColumns));

                    row = excelSheet.CreateRow(++rowCounter);
                    var linkCell = row.CreateCell(0, CellType.String);
                    XSSFHyperlink urlLink = new XSSFHyperlink(HyperlinkType.Url)
                    {
                        Address = report.ReportSettings.ReportUrl
                    };
                    linkCell.SetCellValue("Click to view latest report");
                    linkCell.Hyperlink = (urlLink);
                    linkCell.CellStyle = linkStyle;
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        rowCounter, rowCounter, 0, totalColumns));

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

        public ActionResponse GenerateYearlyProjectsReport(TimeSeriesReportByYear report)
        {
            ActionResponse response = new ActionResponse();
            return response;
        }

        private string ApplyThousandFormat(decimal number)
        {
            return (Math.Round(number).ToString("#,##0.00"));
        }
    }
}
