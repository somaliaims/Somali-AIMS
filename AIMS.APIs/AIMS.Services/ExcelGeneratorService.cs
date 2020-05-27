using AIMS.Models;
using Microsoft.AspNetCore.Hosting;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Security.Permissions;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Globalization;

namespace AIMS.Services
{
    public interface IExcelGeneratorService
    {
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

        /// <summary>
        /// Generates excel for projects budget
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        ActionResponse GenerateProjectBudgetReportSummary(ProjectsBudgetReportSummary report);

        /// <summary>
        /// Generates excel for project budget report
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        ActionResponse GenerateBudgetReport(BudgetReport report);

        /// <summary>
        /// Generates excel report for Envelope
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        ActionResponse GenerateEnvelopeReport (EnvelopeReport report);

        /// <summary>
        /// Generates excel report for all the projects
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        ActionResponse GenerateAllProjectsReport(ProjectReportView projectsReport);
    }

    public class ExcelGeneratorService : IExcelGeneratorService
    {
        private IHostingEnvironment hostingEnvironment;
        string sWebRootFolder = "";
        public ExcelGeneratorService(IHostingEnvironment _hostingEnvironment)
        {
            hostingEnvironment = _hostingEnvironment;
            sWebRootFolder = hostingEnvironment.WebRootPath + "/ExcelFiles/";
            Directory.CreateDirectory(sWebRootFolder);
            FileIOPermission fp = new FileIOPermission(FileIOPermissionAccess.Write, sWebRootFolder);
            try
            {
                fp.Demand();
            }
            catch (Exception)
            {
            }
        }

        public ActionResponse GenerateAllProjectsReport(ProjectReportView projectsReport)
        {
            ActionResponse response = new ActionResponse();
            try
            {
                string sFileName = @"AIMSProjects-" + DateTime.UtcNow.Ticks.ToString() + ".xlsx";
                FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
                var memory = new MemoryStream();
                using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
                {
                    int rowCounter = 0;
                    int currentYear = DateTime.Now.Year;
                    int startingYear = (projectsReport.StartingFinancialYear < 2000) ? (currentYear - 3) : projectsReport.StartingFinancialYear,
                        endingYear = projectsReport.EndingFinancialYear;
                    var locations = projectsReport.Locations;
                    var markers = projectsReport.Markers;
                    var sectors = projectsReport.Sectors;

                    IWorkbook workbook;
                    workbook = new XSSFWorkbook();
                    ISheet excelSheet = workbook.CreateSheet("Report");

                    ICellStyle titleStyle = workbook.CreateCellStyle();
                    IFont fontTitle = workbook.CreateFont();
                    fontTitle.Color = IndexedColors.DarkBlue.Index;
                    fontTitle.IsBold = true;
                    fontTitle.FontHeightInPoints = 16;
                    titleStyle.SetFont(fontTitle);
                    titleStyle.Alignment = HorizontalAlignment.CenterSelection;
                    titleStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle headerStyle = workbook.CreateCellStyle();
                    IFont fontHeader = workbook.CreateFont();
                    fontHeader.Color = IndexedColors.Black.Index;
                    fontHeader.IsBold = true;
                    fontHeader.FontHeightInPoints = 12;
                    headerStyle.SetFont(fontHeader);
                    headerStyle.Alignment = HorizontalAlignment.Left;
                    headerStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle numericHeaderStyle = workbook.CreateCellStyle();
                    fontHeader.Color = IndexedColors.Black.Index;
                    fontHeader.IsBold = true;
                    fontHeader.FontHeightInPoints = 12;
                    numericHeaderStyle.SetFont(fontHeader);
                    numericHeaderStyle.Alignment = HorizontalAlignment.Right;
                    numericHeaderStyle.VerticalAlignment = VerticalAlignment.Center;
                    numericHeaderStyle.DataFormat = (workbook.GetCreationHelper().CreateDataFormat().GetFormat("#,##0.00"));

                    ICellStyle dataCellStyle = workbook.CreateCellStyle();
                    dataCellStyle.WrapText = true;
                    dataCellStyle.Alignment = HorizontalAlignment.Left;
                    dataCellStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle numericCellStyle = workbook.CreateCellStyle();
                    numericCellStyle.WrapText = true;
                    numericCellStyle.Alignment = HorizontalAlignment.Right;
                    numericCellStyle.VerticalAlignment = VerticalAlignment.Center;
                    numericCellStyle.DataFormat = (workbook.GetCreationHelper().CreateDataFormat().GetFormat("#,##0.00"));

                    int colIndex = 0;
                    var row = excelSheet.CreateRow(rowCounter);
                    var projectTitle = row.CreateCell(colIndex);
                    projectTitle.SetCellValue("Project title");
                    projectTitle.CellStyle = headerStyle;

                    var projectDesc = row.CreateCell(++colIndex);
                    projectDesc.SetCellValue("Description");
                    projectDesc.CellStyle = headerStyle;

                    var startYear = row.CreateCell(++colIndex);
                    startYear.SetCellValue("Start year");
                    startYear.CellStyle = headerStyle;

                    var endYear = row.CreateCell(++colIndex);
                    endYear.SetCellValue("End year");
                    endYear.CellStyle = headerStyle;

                    var funders = row.CreateCell(++colIndex);
                    funders.SetCellValue("Funder(s)");
                    funders.CellStyle = headerStyle;

                    var implementers = row.CreateCell(++colIndex);
                    implementers.SetCellValue("Implementer(s)");
                    implementers.CellStyle = headerStyle;

                    var currency = row.CreateCell(++colIndex);
                    currency.SetCellValue("Currency");
                    currency.CellStyle = headerStyle;

                    var totalValue = row.CreateCell(++colIndex);
                    totalValue.SetCellValue("Total value");
                    totalValue.CellStyle = headerStyle;

                    var exchangeRate = row.CreateCell(++colIndex);
                    exchangeRate.SetCellValue("Exchange rate");
                    exchangeRate.CellStyle = headerStyle;

                    /*for (int yr = startingYear; yr <= endingYear; yr++)
                    {
                        var yearCell = row.CreateCell(++colIndex);
                        yearCell.SetCellValue("Disbursements " + yr.ToString());
                        yearCell.CellStyle = headerStyle;
                    }*/
                    foreach(var year in projectsReport.FinancialYears)
                    {
                        var yearCell = row.CreateCell(++colIndex);
                        if (year.FinancialYear < projectsReport.CurrentFinancialYear)
                        {
                            yearCell.SetCellValue("Actual disbursements " + year.Label);
                        }
                        else if (year.FinancialYear == projectsReport.CurrentFinancialYear)
                        {
                            yearCell.SetCellValue("Actual disbursements " + year.Label);
                            yearCell.CellStyle = headerStyle;
                            yearCell = row.CreateCell(++colIndex);
                            yearCell.SetCellValue("Planned disbursements " + year.Label);
                        }
                        else if(year.FinancialYear > projectsReport.CurrentFinancialYear)
                        {
                            yearCell.SetCellValue("Planned disbursements " + year.Label);
                        }
                        yearCell.CellStyle = headerStyle;
                    }

                    ICell sectorCell = null;
                    foreach (var sector in sectors)
                    {
                        sectorCell = row.CreateCell(++colIndex);
                        sectorCell.SetCellValue(sector.ParentSector + " - " + sector.Sector);
                        sectorCell.CellStyle = headerStyle;
                    }

                    ICell locationCell = null;
                    foreach(var location in locations)
                    {
                        locationCell = row.CreateCell(++colIndex);
                        locationCell.SetCellValue(location.Location);
                        locationCell.CellStyle = headerStyle;
                    }

                    ICell markerHeadingCell = null;
                    foreach (var marker in markers)
                    {
                        markerHeadingCell = row.CreateCell(++colIndex);
                        markerHeadingCell.SetCellValue(marker.Marker);
                        markerHeadingCell.CellStyle = headerStyle;
                    }

                    var documentTitleHeadingCell = row.CreateCell(++colIndex);
                    documentTitleHeadingCell.SetCellValue("Document title");
                    documentTitleHeadingCell.CellStyle = headerStyle;

                    var documentUrlHeadingCell = row.CreateCell(++colIndex);
                    documentUrlHeadingCell.SetCellValue("Document url");
                    documentUrlHeadingCell.CellStyle = headerStyle;

                    var projects = projectsReport.Projects;
                    foreach (var project in projects)
                    {
                        row = excelSheet.CreateRow(++rowCounter);

                        int col = 0;
                        var titleCell = row.CreateCell(col);
                        titleCell.SetCellValue(project.Title);
                        titleCell.CellStyle = dataCellStyle;

                        var descriptionCell = row.CreateCell(++col);
                        descriptionCell.SetCellValue(project.Description);
                        descriptionCell.CellStyle = dataCellStyle;

                        var startYearCell = row.CreateCell(++col);
                        startYearCell.SetCellValue(project.StartingFinancialYear.ToString());
                        startYearCell.CellStyle = dataCellStyle;

                        var endYearCell = row.CreateCell(++col);
                        endYearCell.SetCellValue(project.EndingFinancialYear.ToString());
                        endYearCell.CellStyle = dataCellStyle;

                        var funderNames = string.Join(", ", (from f in project.Funders
                                                       select f.Name));
                        var implementerNames = string.Join(", ", (from i in project.Implementers
                                                                  select i.Name));
                        var fundersCell = row.CreateCell(++col);
                        fundersCell.SetCellValue(funderNames);
                        fundersCell.CellStyle = dataCellStyle;

                        var implementersCell = row.CreateCell(++col);
                        implementersCell.SetCellValue(implementerNames);
                        implementersCell.CellStyle = dataCellStyle;

                        var currencyCell = row.CreateCell(++col);
                        currencyCell.SetCellValue(project.ProjectCurrency);
                        currencyCell.CellStyle = dataCellStyle;

                        var projectValueCell = row.CreateCell(++col, CellType.Numeric);
                        projectValueCell.SetCellValue(Convert.ToDouble(project.ProjectValue));
                        projectValueCell.CellStyle = numericCellStyle;

                        var exchangeRateCell = row.CreateCell(++col, CellType.Numeric);
                        exchangeRateCell.SetCellValue(project.ExchangeRate.ToString());
                        exchangeRateCell.CellStyle = numericCellStyle;

                        var disbursements = project.Disbursements;
                        DisbursementAbstractView actualDisbursement = null, plannedDisbursement = null;
                        for(int yr = projectsReport.StartingFinancialYear; yr <= projectsReport.EndingFinancialYear; yr++)
                        {
                            actualDisbursement = (from disb in disbursements
                                        where disb.Year == yr && disb.DisbursementType == "Actual"
                                        select disb).FirstOrDefault();

                            var disbursementCell = row.CreateCell(++col, CellType.Numeric);
                            if (actualDisbursement == null)
                            {
                                disbursementCell.SetCellValue("0");
                            }
                            else
                            {
                                disbursementCell.SetCellValue(Convert.ToDouble(actualDisbursement.Disbursement));
                            }
                            disbursementCell.CellStyle = numericCellStyle;

                            if (yr == projectsReport.CurrentFinancialYear)
                            {
                                plannedDisbursement = (from disb in disbursements
                                                      where disb.Year == yr && disb.DisbursementType == "Planned"
                                                      select disb).FirstOrDefault();

                                var plannedDisbursementCell = row.CreateCell(++col, CellType.Numeric);
                                if (plannedDisbursement == null)
                                {
                                    plannedDisbursementCell.SetCellValue("0");
                                }
                                else
                                {
                                    plannedDisbursementCell.SetCellValue(Convert.ToDouble(plannedDisbursement.Disbursement));
                                }
                                plannedDisbursementCell.CellStyle = numericCellStyle;
                            }

                        }

                        var projectSectors = project.Sectors;
                        foreach (var sector in sectors)
                        {
                            var projectSectorCell = row.CreateCell(++col, CellType.Numeric);
                            var retrieveSector = (from s in projectSectors
                                                  where s.Name.Equals(sector.Sector, StringComparison.OrdinalIgnoreCase)
                                                  select s).FirstOrDefault();

                            if (retrieveSector != null)
                            {
                                projectSectorCell.SetCellValue(retrieveSector.FundsPercentage.ToString());
                            }
                            else
                            {
                                projectSectorCell.SetCellValue("0");
                            }
                            projectSectorCell.CellStyle = numericCellStyle;
                        }

                        var projectLocations = project.Locations;
                        foreach(var location in locations)
                        {
                            var projectLocationCell = row.CreateCell(++col, CellType.Numeric);
                            var retrieveLocation = (from l in projectLocations
                                                    where l.Name.Equals(location.Location, StringComparison.OrdinalIgnoreCase)
                                                    select l).FirstOrDefault();

                            if (retrieveLocation != null)
                            {
                                projectLocationCell.SetCellValue(retrieveLocation.FundsPercentage.ToString());
                            }
                            else
                            {
                                projectLocationCell.SetCellValue("0");
                            }
                            projectLocationCell.CellStyle = numericCellStyle;
                        }

                        var projectMarkers = project.Markers;
                        foreach(var marker in markers)
                        {
                            var markerCell = row.CreateCell(++col, CellType.String);
                            var projectMarker = (from m in projectMarkers
                                                 where m.Marker.Equals(marker.Marker, StringComparison.OrdinalIgnoreCase)
                                                 select m).FirstOrDefault();
                            
                            if (projectMarker == null)
                            {
                                markerCell.SetCellValue("");
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(projectMarker.Values))
                                {
                                    if (projectMarker.MarkerType == FieldTypes.Text)
                                    {
                                        markerCell.SetCellValue(projectMarker.Values);
                                    }
                                    else
                                    {
                                        string values = projectMarker.Values;
                                        if (CheckIfStringIsJson(projectMarker.Values))
                                        {
                                            List<MarkerValues> parsedValues = JsonConvert.DeserializeObject<List<MarkerValues>>(projectMarker.Values);
                                            values = string.Join(",", (from pv in parsedValues
                                                                              select pv.Value).ToList());
                                        }
                                        markerCell.SetCellValue(values);
                                    }
                                }
                                else
                                {
                                    markerCell.SetCellValue("");
                                }
                                
                            }
                            markerCell.CellStyle = dataCellStyle;
                        }

                        string documents = "";
                        string documentsUrl = "";
                        if (project.Documents.Any())
                        {
                            var docList = (from doc in project.Documents
                                                     select doc.DocumentTitle);
                            if (docList.Count() > 0)
                            {
                                documents = string.Join(",", docList);
                            }

                            var urlsList = (from doc in project.Documents
                                            select doc.DocumentUrl);
                            if (urlsList.Count() > 0)
                            {
                                documentsUrl = string.Join(",", urlsList);
                            }
                        }
                        var documentCell = row.CreateCell(++col);
                        documentCell.SetCellValue(documents);
                        documentCell.CellStyle = dataCellStyle;

                        var documentUrlCell = row.CreateCell(++col);
                        documentUrlCell.SetCellValue(documentsUrl);
                        documentUrlCell.CellStyle = dataCellStyle;
                    }
                    workbook.Write(fs);
                    response.Message = sFileName;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ActionResponse GenerateSectorProjectsReport(ProjectProfileReportBySector report)
        {
            ActionResponse response = new ActionResponse();
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-Us");
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
                    headerStyle.Alignment = HorizontalAlignment.Left;
                    headerStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle numericHeaderStyle = workbook.CreateCellStyle();
                    numericHeaderStyle.SetFont(fontHeader);
                    numericHeaderStyle.Alignment = HorizontalAlignment.Left;
                    numericHeaderStyle.VerticalAlignment = VerticalAlignment.Center;
                    numericHeaderStyle.DataFormat = (workbook.GetCreationHelper().CreateDataFormat().GetFormat("#,##0.00"));

                    ICellStyle groupHeaderStyle = workbook.CreateCellStyle();
                    IFont groupFontHeader = workbook.CreateFont();
                    groupFontHeader.Color = IndexedColors.DarkYellow.Index;
                    groupFontHeader.FontHeightInPoints = 12;
                    groupFontHeader.IsBold = true;
                    groupHeaderStyle.SetFont(groupFontHeader);
                    groupHeaderStyle.Alignment = HorizontalAlignment.Left;
                    groupHeaderStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle numericGroupHeaderStyle = workbook.CreateCellStyle();
                    numericGroupHeaderStyle.SetFont(groupFontHeader);
                    numericGroupHeaderStyle.Alignment = HorizontalAlignment.Right;
                    numericGroupHeaderStyle.VerticalAlignment = VerticalAlignment.Center;
                    numericGroupHeaderStyle.DataFormat = (workbook.GetCreationHelper().CreateDataFormat().GetFormat("#,##0.00"));

                    ICellStyle dataCellStyle = workbook.CreateCellStyle();
                    dataCellStyle.WrapText = true;
                    dataCellStyle.Alignment = HorizontalAlignment.Left;
                    dataCellStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle numericCellStyle = workbook.CreateCellStyle();
                    numericCellStyle.WrapText = true;
                    numericCellStyle.Alignment = HorizontalAlignment.Right;
                    numericCellStyle.VerticalAlignment = VerticalAlignment.Center;
                    numericCellStyle.DataFormat = (workbook.GetCreationHelper().CreateDataFormat().GetFormat("#,##0.00"));

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

                    row = excelSheet.CreateRow(++rowCounter);
                    row.CreateCell(0, CellType.Blank);
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        rowCounter, rowCounter, 0, totalColumns
                        ));

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

                    var actualDisbursementsCol = row.CreateCell(3);
                    actualDisbursementsCol.SetCellValue("Actual disbursements (" + report.ReportSettings.DefaultCurrency + ")");
                    actualDisbursementsCol.CellStyle = headerStyle;

                    var plannedDisbursementsCol = row.CreateCell(4);
                    plannedDisbursementsCol.SetCellValue("Planned disbursements (" + report.ReportSettings.DefaultCurrency + ")");
                    plannedDisbursementsCol.CellStyle = headerStyle;

                    foreach (var sector in report.SectorProjectsList)
                    {
                        decimal totalFunding = 0, totalDisbursements = 0;
                        totalFunding = Math.Round(sector.ActualDisbursements, MidpointRounding.AwayFromZero);
                        totalDisbursements = Math.Round(sector.TotalDisbursements, MidpointRounding.AwayFromZero);
                        row = excelSheet.CreateRow(++rowCounter);
                        grandTotalFunding += Math.Round(sector.ActualDisbursements, MidpointRounding.AwayFromZero);
                        grandTotalDisbursement += Math.Round(sector.TotalDisbursements, MidpointRounding.AwayFromZero);

                        var groupTitleCell = row.CreateCell(0, CellType.String);
                        excelSheet.AddMergedRegion(new CellRangeAddress(
                            rowCounter, rowCounter, 0, groupHeaderColumns));
                        groupTitleCell.SetCellValue(sector.SectorName);
                        groupTitleCell.CellStyle = groupHeaderStyle;

                        var groupDisbursementTotalCell = row.CreateCell(3, CellType.Numeric);
                        groupDisbursementTotalCell.SetCellValue(Convert.ToDouble(sector.TotalDisbursements));
                        groupDisbursementTotalCell.CellStyle = numericGroupHeaderStyle;

                        var groupPlannedDisbursementTotalCell = row.CreateCell(4);
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

                            var actualDisbursementDataCell = row.CreateCell(3, CellType.Numeric);
                            actualDisbursementDataCell.SetCellValue(Convert.ToDouble(project.ActualDisbursements));
                            actualDisbursementDataCell.CellStyle = numericCellStyle;

                            var plannedDisbursementDataCell = row.CreateCell(4, CellType.Numeric);
                            plannedDisbursementDataCell.SetCellValue(Convert.ToDouble(project.PlannedDisbursements));
                            plannedDisbursementDataCell.CellStyle = numericCellStyle;
                        }
                    }

                    row = excelSheet.CreateRow(++rowCounter);
                    var footerCell = row.CreateCell(0, CellType.String);
                    footerCell.SetCellValue("Grand totals: ");
                    footerCell.CellStyle = headerStyle;
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        rowCounter, rowCounter, 0, groupHeaderColumns));

                    var grandDisbursementTotalCell = row.CreateCell(3, CellType.Numeric);
                    grandDisbursementTotalCell.SetCellValue(Convert.ToDouble(grandTotalDisbursement));
                    grandDisbursementTotalCell.CellStyle = numericHeaderStyle;

                    var grandPlannedDisbursementTotalCell = row.CreateCell(4, CellType.Blank);
                    grandPlannedDisbursementTotalCell.CellStyle = numericHeaderStyle;

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
                    headerStyle.Alignment = HorizontalAlignment.Left;
                    headerStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle numericHeaderStyle = workbook.CreateCellStyle();
                    numericHeaderStyle.SetFont(fontHeader);
                    numericHeaderStyle.Alignment = HorizontalAlignment.Right;
                    numericHeaderStyle.VerticalAlignment = VerticalAlignment.Center;
                    numericHeaderStyle.DataFormat = (workbook.GetCreationHelper().CreateDataFormat().GetFormat("#,##0.00"));

                    ICellStyle groupHeaderStyle = workbook.CreateCellStyle();
                    IFont groupFontHeader = workbook.CreateFont();
                    groupFontHeader.Color = IndexedColors.DarkYellow.Index;
                    groupFontHeader.FontHeightInPoints = 12;
                    groupFontHeader.IsBold = true;
                    groupHeaderStyle.SetFont(groupFontHeader);
                    groupHeaderStyle.Alignment = HorizontalAlignment.Left;
                    groupHeaderStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle numericGroupHeaderStyle = workbook.CreateCellStyle();
                    numericGroupHeaderStyle.SetFont(groupFontHeader);
                    numericGroupHeaderStyle.Alignment = HorizontalAlignment.Right;
                    numericGroupHeaderStyle.VerticalAlignment = VerticalAlignment.Center;
                    numericGroupHeaderStyle.DataFormat = (workbook.GetCreationHelper().CreateDataFormat().GetFormat("#,##0.00"));

                    ICellStyle dataCellStyle = workbook.CreateCellStyle();
                    dataCellStyle.WrapText = true;
                    dataCellStyle.Alignment = HorizontalAlignment.Center;
                    dataCellStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle numericCellStyle = workbook.CreateCellStyle();
                    numericCellStyle.WrapText = true;
                    numericCellStyle.Alignment = HorizontalAlignment.Right;
                    numericCellStyle.VerticalAlignment = VerticalAlignment.Center;
                    numericCellStyle.DataFormat = (workbook.GetCreationHelper().CreateDataFormat().GetFormat("#,##0.00"));

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

                    row = excelSheet.CreateRow(++rowCounter);
                    row.CreateCell(0, CellType.Blank);
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        rowCounter, rowCounter, 0, totalColumns
                        ));

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

                    var actualDisbursementsCol = row.CreateCell(3);
                    actualDisbursementsCol.SetCellValue("Actual disbursements (" + report.ReportSettings.DefaultCurrency + ")");
                    actualDisbursementsCol.CellStyle = headerStyle;

                    var plannedDisbursementsCol = row.CreateCell(4);
                    plannedDisbursementsCol.SetCellValue("Planned disbursements (" + report.ReportSettings.DefaultCurrency + ")");
                    plannedDisbursementsCol.CellStyle = headerStyle;

                    foreach (var location in report.LocationProjectsList)
                    {
                        row = excelSheet.CreateRow(++rowCounter);
                        grandTotalFunding += Math.Round(location.TotalFunding, MidpointRounding.AwayFromZero);
                        grandTotalDisbursement += Math.Round(location.ActualDisbursements, MidpointRounding.AwayFromZero);

                        var groupTitleCell = row.CreateCell(0, CellType.String);
                        excelSheet.AddMergedRegion(new CellRangeAddress(
                            rowCounter, rowCounter, 0, groupHeaderColumns));
                        groupTitleCell.SetCellValue(location.LocationName);
                        groupTitleCell.CellStyle = groupHeaderStyle;

                        var groupDisbursementTotalCell = row.CreateCell(3, CellType.Numeric);
                        groupDisbursementTotalCell.SetCellValue(Convert.ToDouble(location.ActualDisbursements));
                        groupDisbursementTotalCell.CellStyle = numericGroupHeaderStyle;

                        var groupPlannedDisbursementTotalCell = row.CreateCell(4);
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

                            var actualDisbursementDataCell = row.CreateCell(3, CellType.Numeric);
                            actualDisbursementDataCell.SetCellValue(Convert.ToDouble(project.ActualDisbursements));
                            actualDisbursementDataCell.CellStyle = numericCellStyle;

                            var plannedDisbursementDataCell = row.CreateCell(4, CellType.Numeric);
                            plannedDisbursementDataCell.SetCellValue(Convert.ToDouble(project.PlannedDisbursements));
                            plannedDisbursementDataCell.CellStyle = numericCellStyle;
                        }
                    }

                    row = excelSheet.CreateRow(++rowCounter);
                    var footerCell = row.CreateCell(0, CellType.String);
                    footerCell.SetCellValue("Grand totals: ");
                    footerCell.CellStyle = headerStyle;
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        rowCounter, rowCounter, 0, groupHeaderColumns));

                    var grandDisbursementTotalCell = row.CreateCell(3, CellType.Numeric);
                    grandDisbursementTotalCell.SetCellValue(Convert.ToDouble(grandTotalDisbursement));
                    grandDisbursementTotalCell.CellStyle = numericHeaderStyle;

                    var grandPlannedDisbursementTotalCell = row.CreateCell(4, CellType.Blank);
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
            try
            {
                string sFileName = @"YearProjects-" + DateTime.UtcNow.Ticks.ToString() + ".xlsx";
                FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
                var memory = new MemoryStream();
                using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
                {
                    int rowCounter = 0, totalColumns = 5, groupHeaderColumns = 2;
                    decimal grandTotalFunding = 0, grandTotalDisbursement = 0;

                    IWorkbook workbook;
                    workbook = new XSSFWorkbook();
                    ISheet excelSheet = workbook.CreateSheet("Report");

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
                    headerStyle.Alignment = HorizontalAlignment.Left;
                    headerStyle.VerticalAlignment = VerticalAlignment.Center;
                    
                    ICellStyle numericHeaderStyle = workbook.CreateCellStyle();
                    numericHeaderStyle.SetFont(fontHeader);
                    numericHeaderStyle.Alignment = HorizontalAlignment.Right;
                    numericHeaderStyle.VerticalAlignment = VerticalAlignment.Center;
                    numericHeaderStyle.DataFormat = (workbook.GetCreationHelper().CreateDataFormat().GetFormat("#,##0.00"));

                    ICellStyle groupHeaderStyle = workbook.CreateCellStyle();
                    IFont groupFontHeader = workbook.CreateFont();
                    groupFontHeader.Color = IndexedColors.DarkYellow.Index;
                    groupFontHeader.FontHeightInPoints = 12;
                    groupFontHeader.IsBold = true;
                    groupHeaderStyle.SetFont(groupFontHeader);
                    groupHeaderStyle.Alignment = HorizontalAlignment.Left;
                    groupHeaderStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle numericGroupHeaderStyle = workbook.CreateCellStyle();
                    numericGroupHeaderStyle.SetFont(groupFontHeader);
                    numericGroupHeaderStyle.Alignment = HorizontalAlignment.Right;
                    numericGroupHeaderStyle.VerticalAlignment = VerticalAlignment.Center;
                    numericGroupHeaderStyle.DataFormat = (workbook.GetCreationHelper().CreateDataFormat().GetFormat("#,##0.00"));

                    ICellStyle dataCellStyle = workbook.CreateCellStyle();
                    dataCellStyle.WrapText = true;
                    dataCellStyle.Alignment = HorizontalAlignment.Left;
                    dataCellStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle numericCellStyle = workbook.CreateCellStyle();
                    numericCellStyle.WrapText = true;
                    numericCellStyle.Alignment = HorizontalAlignment.Right;
                    numericCellStyle.VerticalAlignment = VerticalAlignment.Center;
                    numericCellStyle.DataFormat = (workbook.GetCreationHelper().CreateDataFormat().GetFormat("#,##0.00"));

                    IRow row = excelSheet.CreateRow(rowCounter);
                    row.CreateCell(0, CellType.Blank);
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        rowCounter, rowCounter, 0, totalColumns
                        ));

                    row = excelSheet.CreateRow(++rowCounter);
                    var titleCell = row.CreateCell(0, CellType.String);
                    titleCell.SetCellValue("SomaliAIMS year-wise projects report - generated on " + DateTime.Now.ToLongDateString());
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        rowCounter, rowCounter, 0, totalColumns));
                    titleCell.CellStyle = titleStyle;

                    row = excelSheet.CreateRow(++rowCounter);
                    row.CreateCell(0, CellType.Blank);
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        rowCounter, rowCounter, 0, totalColumns
                        ));

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

                    var actualDisbursementsCol = row.CreateCell(3);
                    actualDisbursementsCol.SetCellValue("Actual disbursements (" + report.ReportSettings.DefaultCurrency + ")");
                    actualDisbursementsCol.CellStyle = headerStyle;

                    var plannedDisbursementsCol = row.CreateCell(4);
                    plannedDisbursementsCol.SetCellValue("Planned disbursements (" + report.ReportSettings.DefaultCurrency + ")");
                    plannedDisbursementsCol.CellStyle = headerStyle;

                    foreach (var year in report.YearlyProjectsList)
                    {
                        row = excelSheet.CreateRow(++rowCounter);
                        grandTotalFunding += Math.Round(year.TotalFunding, MidpointRounding.AwayFromZero);
                        grandTotalDisbursement += Math.Round(year.TotalActualDisbursements, MidpointRounding.AwayFromZero);

                        var groupTitleCell = row.CreateCell(0, CellType.Numeric);
                        excelSheet.AddMergedRegion(new CellRangeAddress(
                            rowCounter, rowCounter, 0, groupHeaderColumns));
                        groupTitleCell.SetCellValue(year.Year);
                        groupTitleCell.CellStyle = groupHeaderStyle;
                        
                        var groupDisbursementTotalCell = row.CreateCell(3, CellType.Numeric);
                        groupDisbursementTotalCell.SetCellValue(Convert.ToDouble(year.TotalActualDisbursements));
                        groupDisbursementTotalCell.CellStyle = numericGroupHeaderStyle;

                        var groupPlannedDisbursementTotalCell = row.CreateCell(4);
                        groupPlannedDisbursementTotalCell.SetCellValue("");

                        foreach (var project in year.Projects)
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
                            
                            var actualDisbursementDataCell = row.CreateCell(3, CellType.Numeric);
                            actualDisbursementDataCell.SetCellValue(Convert.ToDouble(project.ActualDisbursements));
                            actualDisbursementDataCell.CellStyle = numericCellStyle;

                            var plannedDisbursementDataCell = row.CreateCell(4, CellType.Numeric);
                            plannedDisbursementDataCell.SetCellValue(Convert.ToDouble(project.PlannedDisbursements));
                            plannedDisbursementDataCell.CellStyle = numericCellStyle;
                        }
                    }

                    row = excelSheet.CreateRow(++rowCounter);
                    var footerCell = row.CreateCell(0, CellType.String);
                    footerCell.SetCellValue("Grand totals: ");
                    footerCell.CellStyle = headerStyle;
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        rowCounter, rowCounter, 0, groupHeaderColumns));

                    var grandDisbursementTotalCell = row.CreateCell(3, CellType.Numeric);
                    grandDisbursementTotalCell.SetCellValue(Convert.ToDouble(grandTotalDisbursement));
                    grandDisbursementTotalCell.CellStyle = numericHeaderStyle;

                    var grandPlannedDisbursementTotalCell = row.CreateCell(4, CellType.Blank);
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
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Success = false;
            }
            return response;
        }

        public ActionResponse GenerateBudgetReport(BudgetReport report)
        {
            ActionResponse response = new ActionResponse();
            try
            {
                var yearsList = (from y in report.Years
                                       orderby y.Year ascending
                                       select y).ToList();

                string sFileName = @"ProjectBudget-" + DateTime.UtcNow.Ticks.ToString() + ".xlsx";
                FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
                var memory = new MemoryStream();
                using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
                {
                    int rowCounter = 0, totalColumns = 7;
                    IWorkbook workbook;
                    workbook = new XSSFWorkbook();
                    ISheet excelSheet = workbook.CreateSheet("Report");

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
                    headerStyle.Alignment = HorizontalAlignment.Left;
                    headerStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle numericHeaderStyle = workbook.CreateCellStyle();
                    numericHeaderStyle.SetFont(fontHeader);
                    numericHeaderStyle.Alignment = HorizontalAlignment.Right;
                    numericHeaderStyle.VerticalAlignment = VerticalAlignment.Center;
                    numericHeaderStyle.DataFormat = (workbook.GetCreationHelper().CreateDataFormat().GetFormat("#,##0.00"));

                    ICellStyle highlightStyle = workbook.CreateCellStyle();
                    IFont fontHighlight = workbook.CreateFont();
                    fontHighlight.Color = IndexedColors.DarkBlue.Index;
                    fontHighlight.FontHeightInPoints = 12;
                    highlightStyle.SetFont(fontHighlight);
                    highlightStyle.Alignment = HorizontalAlignment.Left;
                    highlightStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle groupHeaderStyle = workbook.CreateCellStyle();
                    IFont groupFontHeader = workbook.CreateFont();
                    groupFontHeader.Color = IndexedColors.DarkYellow.Index;
                    groupFontHeader.FontHeightInPoints = 12;
                    groupFontHeader.IsBold = true;
                    groupHeaderStyle.SetFont(groupFontHeader);
                    groupHeaderStyle.Alignment = HorizontalAlignment.Left;
                    groupHeaderStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle numericGroupHeaderStyle = workbook.CreateCellStyle();
                    numericGroupHeaderStyle.SetFont(groupFontHeader);
                    numericGroupHeaderStyle.Alignment = HorizontalAlignment.Right;
                    numericGroupHeaderStyle.VerticalAlignment = VerticalAlignment.Center;
                    numericGroupHeaderStyle.DataFormat = (workbook.GetCreationHelper().CreateDataFormat().GetFormat("#,##0.00"));

                    ICellStyle dataCellStyle = workbook.CreateCellStyle();
                    dataCellStyle.WrapText = true;
                    dataCellStyle.Alignment = HorizontalAlignment.Left;
                    dataCellStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle numericCellStyle = workbook.CreateCellStyle();
                    numericCellStyle.WrapText = true;
                    numericCellStyle.Alignment = HorizontalAlignment.Right;
                    numericCellStyle.VerticalAlignment = VerticalAlignment.Center;
                    numericCellStyle.DataFormat = (workbook.GetCreationHelper().CreateDataFormat().GetFormat("#,##0.00"));

                    IRow row = excelSheet.CreateRow(rowCounter);
                    row.CreateCell(0, CellType.Blank);
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        rowCounter, rowCounter, 0, totalColumns
                        ));

                    row = excelSheet.CreateRow(++rowCounter);
                    var titleCell = row.CreateCell(0, CellType.String);
                    titleCell.SetCellValue("SomaliAIMS budget report - generated on " + DateTime.Now.ToLongDateString() + " (Currency: " + report.ReportSettings.DefaultCurrency + ")");
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        rowCounter, rowCounter, 0, totalColumns));
                    titleCell.CellStyle = titleStyle;

                    row = excelSheet.CreateRow(++rowCounter);
                    row.CreateCell(0, CellType.Blank);
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        rowCounter, rowCounter, 0, totalColumns
                        ));

                    row = excelSheet.CreateRow(++rowCounter);
                    var sectorHeaderCell = row.CreateCell(0, CellType.String);
                    sectorHeaderCell.SetCellValue("Sectors");
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        rowCounter, rowCounter, 0, totalColumns));
                    sectorHeaderCell.CellStyle = titleStyle;

                    row = excelSheet.CreateRow(++rowCounter);
                    row.CreateCell(0, CellType.Blank);
                    int index = 0;
                    foreach (var year in yearsList)
                    {
                        var yearCol = row.CreateCell(++index);
                        yearCol.SetCellValue(year.Label);
                        yearCol.CellStyle = headerStyle;
                    }

                    var sectorDisbursements = report.ParentSectorDisbursements;
                    foreach(var disbursement in sectorDisbursements)
                    {
                        row = excelSheet.CreateRow(++rowCounter);
                        var sectorCell = row.CreateCell(0, CellType.String);
                        sectorCell.SetCellValue(disbursement.SectorName);
                        sectorCell.CellStyle = dataCellStyle;

                        index = 0;
                        foreach(var yearDisbursement in disbursement.Disbursements)
                        {
                            var disbursementCol = row.CreateCell(++index, CellType.Numeric);
                            disbursementCol.SetCellValue(Convert.ToDouble(yearDisbursement.TotalValue));
                            disbursementCol.CellStyle = numericCellStyle;
                        }
                    }

                    row = excelSheet.CreateRow(++rowCounter);
                    var locationHeaderCell = row.CreateCell(0, CellType.String);
                    locationHeaderCell.SetCellValue("Locations");
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        rowCounter, rowCounter, 0, totalColumns));
                    locationHeaderCell.CellStyle = titleStyle;

                    var locationDisbursements = report.LocationDisbursements;
                    foreach (var disbursement in locationDisbursements)
                    {
                        row = excelSheet.CreateRow(++rowCounter);
                        var sectorCell = row.CreateCell(0, CellType.String);
                        sectorCell.SetCellValue(disbursement.LocationName);
                        sectorCell.CellStyle = dataCellStyle;

                        index = 0;
                        foreach (var yearDisbursement in disbursement.Disbursements)
                        {
                            var disbursementCol = row.CreateCell(++index, CellType.Numeric);
                            disbursementCol.SetCellValue(Convert.ToDouble(yearDisbursement.TotalValue));
                            disbursementCol.CellStyle = numericCellStyle;
                        }
                    }

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
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        public ActionResponse GenerateProjectBudgetReportSummary(ProjectsBudgetReportSummary report)
        {
            ActionResponse response = new ActionResponse();
            try
            {
                List<int> yearsList = (from y in report.TotalYearlyDisbursements
                                       orderby y.Year ascending
                                       select y.Year).ToList();

                string sFileName = @"ProjectBudget-" + DateTime.UtcNow.Ticks.ToString() + ".xlsx";
                FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
                var memory = new MemoryStream();
                using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
                {
                    int rowCounter = 0, totalColumns = 7;
                    IWorkbook workbook;
                    workbook = new XSSFWorkbook();
                    ISheet excelSheet = workbook.CreateSheet("Report");

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
                    headerStyle.Alignment = HorizontalAlignment.Left;
                    headerStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle highlightStyle = workbook.CreateCellStyle();
                    IFont fontHighlight = workbook.CreateFont();
                    fontHighlight.Color = IndexedColors.DarkBlue.Index;
                    fontHighlight.FontHeightInPoints = 12;
                    highlightStyle.SetFont(fontHighlight);
                    highlightStyle.Alignment = HorizontalAlignment.Left;
                    highlightStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle groupHeaderStyle = workbook.CreateCellStyle();
                    IFont groupFontHeader = workbook.CreateFont();
                    groupFontHeader.Color = IndexedColors.DarkYellow.Index;
                    groupFontHeader.FontHeightInPoints = 12;
                    groupFontHeader.IsBold = true;
                    groupHeaderStyle.SetFont(groupFontHeader);
                    groupHeaderStyle.Alignment = HorizontalAlignment.Left;
                    groupHeaderStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle numericGroupHeaderStyle = workbook.CreateCellStyle();
                    numericGroupHeaderStyle.SetFont(groupFontHeader);
                    numericGroupHeaderStyle.Alignment = HorizontalAlignment.Right;
                    numericGroupHeaderStyle.VerticalAlignment = VerticalAlignment.Center;
                    numericGroupHeaderStyle.DataFormat = (workbook.GetCreationHelper().CreateDataFormat().GetFormat("#,##0.00"));

                    ICellStyle dataCellStyle = workbook.CreateCellStyle();
                    dataCellStyle.WrapText = true;
                    dataCellStyle.Alignment = HorizontalAlignment.Left;
                    dataCellStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle numericCellStyle = workbook.CreateCellStyle();
                    numericCellStyle.WrapText = true;
                    numericCellStyle.Alignment = HorizontalAlignment.Left;
                    numericCellStyle.VerticalAlignment = VerticalAlignment.Center;
                    numericCellStyle.DataFormat = (workbook.GetCreationHelper().CreateDataFormat().GetFormat("#,##0.00"));

                    IRow row = excelSheet.CreateRow(rowCounter);
                    row.CreateCell(0, CellType.Blank);
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        rowCounter, rowCounter, 0, totalColumns
                        ));

                    row = excelSheet.CreateRow(++rowCounter);
                    var titleCell = row.CreateCell(0, CellType.String);
                    titleCell.SetCellValue("SomaliAIMS budget report - generated on " + DateTime.Now.ToLongDateString() + " (Currency: " + report.ReportSettings.DefaultCurrency + ")");
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        rowCounter, rowCounter, 0, totalColumns));
                    titleCell.CellStyle = titleStyle;

                    row = excelSheet.CreateRow(++rowCounter);
                    row.CreateCell(0, CellType.Blank);
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        rowCounter, rowCounter, 0, totalColumns
                        ));

                    row = excelSheet.CreateRow(++rowCounter);
                    row.CreateCell(0, CellType.Blank);
                    row.CreateCell(1, CellType.Blank);
                    int index = 1;
                    foreach(var year in yearsList)
                    {
                        var funderCol = row.CreateCell(++index);
                        funderCol.SetCellValue(year.ToString());
                        funderCol.CellStyle = headerStyle;
                    }
                    
                    foreach (var project in report.Projects)
                    {
                        index = 1;
                        row = excelSheet.CreateRow(++rowCounter);
                        var projectTitleCell = row.CreateCell(0, CellType.String);
                        projectTitleCell.SetCellValue(project.Title);
                        projectTitleCell.CellStyle = groupHeaderStyle;
                        var tDisbusementCell = row.CreateCell(1, CellType.String);
                        tDisbusementCell.SetCellValue("Total disbursements");
                        tDisbusementCell.CellStyle = highlightStyle;
                        foreach (var disbursement in project.YearlyDisbursements)
                        {
                            var disbursementCol = row.CreateCell(++index, CellType.Numeric);
                            disbursementCol.SetCellValue(Convert.ToDouble(disbursement.Disbursements));
                            disbursementCol.CellStyle = numericCellStyle;
                        }

                        index = 1;
                        row = excelSheet.CreateRow(++rowCounter);
                        var rowOneBlankCell = row.CreateCell(0, CellType.Blank);
                        var aDisbusementCell = row.CreateCell(1, CellType.String);
                        aDisbusementCell.SetCellValue("Actual");
                        aDisbusementCell.CellStyle = highlightStyle;
                        foreach (var disbursement in project.DisbursementsBreakup)
                        {
                            var disbursementCol = row.CreateCell(++index, CellType.Numeric);
                            disbursementCol.SetCellValue(Convert.ToDouble(disbursement.ActualDisbursements));
                            disbursementCol.CellStyle = numericCellStyle;
                        }

                        index = 1;
                        row = excelSheet.CreateRow(++rowCounter);
                        var rowTwoBlankCell = row.CreateCell(0, CellType.Blank);
                        var eDisbusementCell = row.CreateCell(1, CellType.Numeric);
                        eDisbusementCell.SetCellValue("Planned");
                        eDisbusementCell.CellStyle = highlightStyle;
                        foreach (var disbursement in project.DisbursementsBreakup)
                        {
                            var disbursementCol = row.CreateCell(++index, CellType.Numeric);
                            disbursementCol.SetCellValue(disbursement.ExpectedDisbursements.ToString());
                            disbursementCol.CellStyle = numericCellStyle;
                        }
                    }

                    row = excelSheet.CreateRow(++rowCounter);
                    row.CreateCell(0, CellType.Blank);
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        rowCounter, rowCounter, 0, totalColumns));

                    row = excelSheet.CreateRow(++rowCounter);
                    var footerYearsBlankOne = row.CreateCell(0, CellType.Blank);
                    var footerYearsBlankTwo = row.CreateCell(1, CellType.Blank);
                    index = 1;
                    foreach (var year in yearsList)
                    {
                        var yearCell = row.CreateCell(++index, CellType.Numeric);
                        yearCell.SetCellValue(year);
                        yearCell.CellStyle = numericGroupHeaderStyle;
                    }

                    row = excelSheet.CreateRow(++rowCounter);
                    var totalHeading = row.CreateCell(0, CellType.String);
                    totalHeading.SetCellValue("Total");
                    totalHeading.CellStyle = groupHeaderStyle;
                    var actualDisbursementCell = row.CreateCell(1, CellType.Blank);
                    actualDisbursementCell.SetCellValue("Actual");
                    actualDisbursementCell.CellStyle = highlightStyle;
                    index = 1;
                    foreach(var d in report.TotalYearlyDisbursements)
                    {
                        var disbursementCell = row.CreateCell(++index, CellType.Numeric);
                        disbursementCell.SetCellValue(Convert.ToDouble(d.TotalDisbursements));
                        disbursementCell.CellStyle = numericCellStyle;
                    }

                    row = excelSheet.CreateRow(++rowCounter);
                    row.CreateCell(0, CellType.Blank);
                    var expectedDisbursementCell = row.CreateCell(1, CellType.Blank);
                    expectedDisbursementCell.SetCellValue("Planned");
                    expectedDisbursementCell.CellStyle = highlightStyle;
                    index = 1;
                    foreach (var d in report.TotalYearlyDisbursements)
                    {
                        var eDisbursementsCell = row.CreateCell(++index, CellType.Numeric);
                        eDisbursementsCell.SetCellValue(Convert.ToDouble(d.TotalExpectedDisbursements));
                        eDisbursementsCell.CellStyle = numericCellStyle;
                    }

                    row = excelSheet.CreateRow(++rowCounter);
                    row.CreateCell(0, CellType.Blank);
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
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Success = false;
            }
            return response;
        }

        public ActionResponse GenerateEnvelopeReport(EnvelopeReport report)
        {
            ActionResponse response = new ActionResponse();
            try
            {
                List<int> yearsList = (from y in report.EnvelopeYears
                                       orderby y ascending
                                       select y).ToList<int>();

                string sFileName = @"EnvelopeReport-" + DateTime.UtcNow.Ticks.ToString() + ".xlsx";
                FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
                var memory = new MemoryStream();
                using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
                {
                    int rowCounter = 0, totalColumns = yearsList.Count + 2;
                    IWorkbook workbook;
                    workbook = new XSSFWorkbook();
                    ISheet excelSheet = workbook.CreateSheet("Report");

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
                    headerStyle.Alignment = HorizontalAlignment.Left;
                    headerStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle numericHeaderStyle = workbook.CreateCellStyle();
                    numericHeaderStyle.SetFont(fontHeader);
                    numericHeaderStyle.Alignment = HorizontalAlignment.Right;
                    numericHeaderStyle.VerticalAlignment = VerticalAlignment.Center;
                    numericHeaderStyle.DataFormat = (workbook.GetCreationHelper().CreateDataFormat().GetFormat("#,##0.00"));

                    ICellStyle highlightStyle = workbook.CreateCellStyle();
                    IFont fontHighlight = workbook.CreateFont();
                    fontHighlight.Color = IndexedColors.DarkBlue.Index;
                    fontHighlight.FontHeightInPoints = 12;
                    highlightStyle.SetFont(fontHighlight);
                    highlightStyle.Alignment = HorizontalAlignment.Left;
                    highlightStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle groupHeaderStyle = workbook.CreateCellStyle();
                    IFont groupFontHeader = workbook.CreateFont();
                    groupFontHeader.Color = IndexedColors.DarkYellow.Index;
                    groupFontHeader.FontHeightInPoints = 12;
                    groupFontHeader.IsBold = true;
                    groupHeaderStyle.SetFont(groupFontHeader);
                    groupHeaderStyle.Alignment = HorizontalAlignment.Left;
                    groupHeaderStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle numericGroupHeaderStyle = workbook.CreateCellStyle();
                    numericGroupHeaderStyle.SetFont(groupFontHeader);
                    numericGroupHeaderStyle.Alignment = HorizontalAlignment.Right;
                    numericGroupHeaderStyle.VerticalAlignment = VerticalAlignment.Center;
                    numericGroupHeaderStyle.DataFormat = (workbook.GetCreationHelper().CreateDataFormat().GetFormat("#,##0.00"));

                    ICellStyle dataCellStyle = workbook.CreateCellStyle();
                    dataCellStyle.WrapText = true;
                    dataCellStyle.Alignment = HorizontalAlignment.Left;
                    dataCellStyle.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle numericCellStyle = workbook.CreateCellStyle();
                    numericCellStyle.WrapText = true;
                    numericCellStyle.Alignment = HorizontalAlignment.Right;
                    numericCellStyle.VerticalAlignment = VerticalAlignment.Center;
                    numericCellStyle.DataFormat = (workbook.GetCreationHelper().CreateDataFormat().GetFormat("#,##0.00"));

                    IRow row = excelSheet.CreateRow(rowCounter);
                    row.CreateCell(0, CellType.Blank);
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        rowCounter, rowCounter, 0, totalColumns
                        ));

                    row = excelSheet.CreateRow(++rowCounter);
                    var titleCell = row.CreateCell(0, CellType.String);
                    titleCell.SetCellValue("SomaliAIMS envelope report - generated on " + DateTime.Now.ToLongDateString() + " (Currency: " + report.ReportSettings.DefaultCurrency + ")");
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        rowCounter, rowCounter, 0, totalColumns));
                    titleCell.CellStyle = titleStyle;

                    row = excelSheet.CreateRow(++rowCounter);
                    row.CreateCell(0, CellType.Blank);
                    excelSheet.AddMergedRegion(new CellRangeAddress(
                        rowCounter, rowCounter, 0, totalColumns
                        ));

                    row = excelSheet.CreateRow(++rowCounter);
                    row.CreateCell(0, CellType.Blank);
                    row.CreateCell(1, CellType.Blank);
                    int index = 0;
                    foreach (var year in yearsList)
                    {
                        var funderCol = row.CreateCell(++index);
                        funderCol.SetCellValue(year.ToString());
                        funderCol.CellStyle = headerStyle;
                    }
                    var totalCell = row.CreateCell(++index);
                    totalCell.SetCellValue("Total");
                    totalCell.CellStyle = headerStyle;

                    foreach (var envelope in report.Envelope)
                    {
                        index = 1;
                        row = excelSheet.CreateRow(++rowCounter);
                        var funderTitleCell = row.CreateCell(0, CellType.String);
                        funderTitleCell.SetCellValue(envelope.Funder);
                        funderTitleCell.CellStyle = groupHeaderStyle;
                        excelSheet.AddMergedRegion(new CellRangeAddress(
                        rowCounter, rowCounter, 0, totalColumns
                        ));
                        funderTitleCell.SetCellValue(envelope.Funder);
                        index = 0;
                        foreach(var breakupByType in envelope.EnvelopeBreakupsByType)
                        {
                            row = excelSheet.CreateRow(++rowCounter);
                            var envelopeTypeCell = row.CreateCell(0, CellType.String);
                            envelopeTypeCell.SetCellValue(breakupByType.EnvelopeType);

                            double envelopeTypeTotalAmount = 0;
                            int yearlyIndex = 0;
                            foreach(var yearly in breakupByType.YearlyBreakup)
                            {
                                var yearlyCell = row.CreateCell(++yearlyIndex, CellType.Numeric);
                                yearlyCell.CellStyle = numericCellStyle;
                                double yearlyAmount = Convert.ToDouble(yearly.Amount);
                                yearlyCell.SetCellValue(yearlyAmount);
                                envelopeTypeTotalAmount += yearlyAmount;
                            }
                            var yearlyTotalCell = row.CreateCell(++yearlyIndex, CellType.Numeric);
                            yearlyTotalCell.CellStyle = numericHeaderStyle;
                            yearlyTotalCell.SetCellValue(envelopeTypeTotalAmount);
                        }

                        index = 0;
                        row = excelSheet.CreateRow(++rowCounter);
                        var blankCell = row.CreateCell(index, CellType.Blank);

                        foreach(var year in report.EnvelopeYears)
                        {
                            var yearlyBreakUps = (from e in envelope.EnvelopeBreakupsByType
                                               select e.YearlyBreakup);
                            double yearlyTotalAmount = 0;
                            foreach(var yearly in yearlyBreakUps)
                            {
                                yearlyTotalAmount += (double)(from y in yearly
                                                             where y.Year == year
                                                             select y.Amount).FirstOrDefault();
                            }
                            var yearlyTotalCell = row.CreateCell(++index, CellType.Numeric);
                            yearlyTotalCell.CellStyle = numericHeaderStyle;
                            yearlyTotalCell.SetCellValue(yearlyTotalAmount);
                        }
                        
                    }

                    row = excelSheet.CreateRow(++rowCounter);
                    row.CreateCell(0, CellType.Blank);
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
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Success = false;
            }
            return response;
        }

        private string ApplyThousandFormat(decimal number)
        {
            //return (Math.Round(number).ToString("#,##0.00"));
            return number.ToString("N2");
        }

        private bool CheckIfStringIsJson(string isJosn)
        {
            try
            {
                var result = JArray.Parse(isJosn);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }
    }
}
