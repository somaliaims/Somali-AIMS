using AIMS.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace AIMS.Services
{
    public interface IDataImportService
    {
        /// <summary>
        /// Imports past data from 2018 file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        List<ImportedAidData> ImportAidDataEighteen(string filePath, IFormFile file);

        /// <summary>
        /// Imports past data from 2017 file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        List<ImportedAidData> ImportAidDataSeventeen(string filePath);

        /// <summary>
        /// Imports data for testing
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        List<NewImportedAidData> ImportLatestAidData(string filePath, IFormFile file);

        /// <summary>
        /// Gets data matches for both old and new data
        /// </summary>
        /// <returns></returns>
        ImportedDataMatch GetMatchForOldNewData(string fileFolder);

        /// <summary>
        /// Generates Excel sheet for the provided file path
        /// </summary>
        /// <param name="dataFilePath"></param>
        /// <returns></returns>
        string GenerateExcelFileForActiveProjects(string fileFolder);
    }

    public class DataImportService : IDataImportService
    {
        private IHostingEnvironment hostingEnvironment;
        string sWebRootFolder = "";
        NameValueCollection newDataLocations;
        NameValueCollection oldDataLocations;
        NameValueCollection oldCustomFields;
        NameValueCollection newCustomFields;
        NameValueCollection latestDataLocations;
        NameValueCollection latestCustomFields;

        private DataFormatter dataFormatter;
        private IFormulaEvaluator formulaEvaluator;

        public DataImportService(IHostingEnvironment _hostingEnvironment)
        {
            hostingEnvironment = _hostingEnvironment;
            sWebRootFolder = hostingEnvironment.WebRootPath + "/ExcelFiles/";
            Directory.CreateDirectory(sWebRootFolder);

            oldDataLocations = new NameValueCollection()
            {
                { "39", "FGS" },
                { "40", "BRA" },
                { "41", "Galmudug" },
                { "42", "Hiirshabelle" },
                { "43", "Jubaland" },
                { "44", "Puntland" },
                { "45", "South West" },
                { "46", "Somaliland" },
                { "47", "Unattributed" }
            };

            newDataLocations = new NameValueCollection()
            {
                { "15", "FGS" },
                { "16", "BRA" },
                { "17", "Galmudug" },
                { "18", "Hiirshabelle" },
                { "19", "Jubaland" },
                { "20", "Puntland" },
                { "21", "South West" },
                { "22", "Somaliland" },
                { "23", "Unattributed" }
            };

            latestDataLocations = new NameValueCollection()
            {
                { "18", "FGS" },
                { "19", "BRA" },
                { "20", "Galmudug" },
                { "21", "Hiirshabelle" },
                { "22", "Jubaland" },
                { "23", "Puntland" },
                { "24", "South West" },
                { "25", "Somaliland" },
                { "26", "Unattributed" }
            };

            latestCustomFields = new NameValueCollection()
            {
                { "28", "GENDER MARKER" },
                { "20", "CAPACITY DEVELOPMENT MARKER" },
                { "30", "STABALIZATION/CRESTA" },
                { "31", "DURABLE SOLUTIONS" },
                { "32", "YOUTH MARKER" },
                { "33", "PCVE MARKER" },
                { "34", "RRF MARKER" },
            };

            oldCustomFields = new NameValueCollection()
            {
                { "32", "Gender" },
                { "33", "Capacity Building" },
                { "34", "Stabalization/Cresta" },
                { "35", "Durable Solutions" },
                { "36", "Youth" },
                { "37", "Conflict Sensitivity Analysis" },
            };

            newCustomFields = new NameValueCollection()
            {
                {"36", "Recovery & Resilience" },
                {"37", "Gender" },
                {"38", "Durable Solutions" },
                { "40", "Capacity Development" },
                {"41", "Stabilization" },
                {"42", "PCVE" },
                {"43", "Youth" }
            };
        }

        public List<NewImportedAidData> ImportLatestAidData(string filePath, IFormFile file)
        {
            List<NewImportedAidData> projectsList = new List<NewImportedAidData>();
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                int projectTitleIndex = 0, projectDescriptionIndex = 1, sectorIndex = 2, startYearIndex = 3,
                    endYearIndex = 4, funderIndex = 6, implementerIndex = 7, currencyIndex = 9, projectCostIndex = 10,
                    previousMinusYearIndex = 12, previousYearIndex = 13, currentYearIndex = 14, currentYearPlannedIndex = 15,
                    futureYearPlannedIndex = 15, locationLowerIndex = 18, locationUpperIndex = 26, markerLowerIndex = 28,
                    markerUpperIndex = 34, documentLinkIndex = 40, documentDescriptionIndex = 41;

                file.CopyTo(stream);
                stream.Position = 0;

                XSSFWorkbook hssfwb = new XSSFWorkbook(stream);
                this.dataFormatter = new DataFormatter(CultureInfo.InvariantCulture);
                this.formulaEvaluator = WorkbookFactory.CreateFormulaEvaluator(hssfwb);

                ISheet sheet = hssfwb.GetSheetAt(2);
                IRow headerRow = sheet.GetRow(0);
                int cellCount = headerRow.LastCellNum;

                for (int i = (sheet.FirstRowNum + 1); i < sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null)
                    {
                        continue;
                    }
                    if (row.Cells.All(d => d.CellType == CellType.Blank))
                    {
                        continue;
                    }

                    decimal disbursementsPreviousMinusYear = 0, disbursementsPreviousYear = 0, disbursementsCurrentYear = 0, 
                        exchangeRate = 0, disbursementsPlannedCurrentYear = 0, disbursementsPlannedFutureYear = 0;
                    decimal.TryParse(this.GetFormattedValue(row.GetCell(previousMinusYearIndex)), out disbursementsPreviousMinusYear);
                    decimal.TryParse(this.GetFormattedValue(row.GetCell(previousYearIndex)), out disbursementsPreviousYear);
                    decimal.TryParse(this.GetFormattedValue(row.GetCell(currentYearIndex)), out disbursementsCurrentYear);
                    decimal.TryParse(this.GetFormattedValue(row.GetCell(currentYearPlannedIndex)), out disbursementsPlannedCurrentYear);
                    decimal.TryParse(this.GetFormattedValue(row.GetCell(futureYearPlannedIndex)), out disbursementsPlannedFutureYear);

                    List<ImportedLocation> locationsList = new List<ImportedLocation>();
                    for (int l = locationLowerIndex; l <= locationUpperIndex; l++)
                    {
                        decimal percentage = 0;
                        decimal.TryParse(row.GetCell(l).NumericCellValue.ToString(), out percentage);
                        locationsList.Add(new ImportedLocation()
                        {
                            Location = newDataLocations[l.ToString()],
                            Percentage = (percentage * 100)
                        });

                        decimal totalPercentage = (from loc in locationsList
                                                   select loc.Percentage).Sum();
                        if (totalPercentage == 100)
                        {
                            break;
                        }
                    }

                    List<ImportedCustomFields> customFieldsList = new List<ImportedCustomFields>();
                    for (int c = markerLowerIndex; c <= markerUpperIndex; c++)
                    {
                        if (c == 39)
                            continue;

                        customFieldsList.Add(new ImportedCustomFields()
                        {
                            CustomField = latestCustomFields[c.ToString()],
                            Value = this.GetFormattedValue(row.GetCell(c))
                        });
                    }

                    List<ImportedDocumentLinks> documentsList = new List<ImportedDocumentLinks>();
                    var documentLink = this.GetFormattedValue(row.GetCell(documentLinkIndex));
                    var documentDescription = this.GetFormattedValue(row.GetCell(documentDescriptionIndex));
                    if (!string.IsNullOrEmpty(documentLink) || !string.IsNullOrEmpty(documentDescription))
                    {
                        documentsList.Add(new ImportedDocumentLinks()
                        {
                            DocumentUrl = documentLink,
                            DocumentTitle = documentDescription
                        });
                    }

                    int startingYear = 0, endingYear = 0;
                    decimal projectCost = 0, previousMinusYearDisbursements = 0, previousYearDisbursements = 0,
                        currentYearDisbursements = 0, currentYearPlannedDisbursements = 0, futureYearPlannedDisbursements = 0;

                    int.TryParse(this.GetFormattedValue(row.GetCell(startYearIndex)), out startingYear);
                    int.TryParse(this.GetFormattedValue(row.GetCell(endYearIndex)), out endingYear);
                    decimal.TryParse(this.GetFormattedValue(row.GetCell(previousMinusYearIndex)), out previousMinusYearDisbursements);
                    decimal.TryParse(this.GetFormattedValue(row.GetCell(previousYearIndex)), out previousYearDisbursements);
                    decimal.TryParse(this.GetFormattedValue(row.GetCell(currentYearIndex)), out currentYearDisbursements);
                    decimal.TryParse(this.GetFormattedValue(row.GetCell(currentYearPlannedIndex)), out currentYearPlannedDisbursements);
                    decimal.TryParse(this.GetFormattedValue(row.GetCell(futureYearPlannedIndex)), out futureYearPlannedDisbursements);
                    decimal.TryParse(this.GetFormattedValue(row.GetCell(projectCostIndex)), out projectCost);
                    projectCost = (previousMinusYearDisbursements + previousYearDisbursements + currentYearDisbursements +
                        currentYearPlannedDisbursements + futureYearPlannedDisbursements);

                    projectsList.Add(new NewImportedAidData()
                    {
                        ProjectTitle = this.GetFormattedValue(row.GetCell(projectTitleIndex)),
                        ProjectDescription = this.GetFormattedValue(row.GetCell(projectDescriptionIndex)),
                        ProjectValue = projectCost,
                        StartYear = startingYear.ToString(),
                        EndYear = endingYear.ToString(),
                        Funders = this.GetFormattedValue(row.GetCell(funderIndex)),
                        Currency = this.GetFormattedValue(row.GetCell(currencyIndex)),
                        ExchangeRate = exchangeRate,
                        Implementers = this.GetFormattedValue(row.GetCell(implementerIndex)),
                        PreviousMinusYearDisbursements = previousMinusYearDisbursements,
                        PreviousYearDisbursements = previousYearDisbursements,
                        CurrentYearDisbursements = currentYearDisbursements,
                        CurrentYearPlannedDisbursements = currentYearPlannedDisbursements,
                        FutureYearPlannedDisbursements = futureYearPlannedDisbursements,
                        Sector = this.GetFormattedValue(row.GetCell(sectorIndex)),
                        Locations = locationsList,
                        CustomFields = customFieldsList,
                        DocumentLinks = documentsList
                    });
                }
            }
            return projectsList;
        }

        public List<ImportedAidData> ImportAidDataEighteen(string filePath, IFormFile file)
        {
            List<ImportedAidData> projectsList = new List<ImportedAidData>();
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                int projectTitleIndex = 3, reportingOrgIndex = 2, startDateIndex = 5, endDateIndex = 6,
                    fundersIndex = 7, implementersIndex = 8, yearOneIndex = 11, yearTwoIndex = 12,
                    yearThreeIndex = 13, primarySectorIndex = 26, currencyIndex = 12, exRateIndex = 13,
                    locationLowerIndex = 15, locationUpperIndex = 23, customFieldsLowerIndex = 36,
                    customFieldsUpperIndex = 43, linksIndex = 44;

                file.CopyTo(stream);
                stream.Position = 0;
                
                XSSFWorkbook hssfwb = new XSSFWorkbook(stream);
                this.dataFormatter = new DataFormatter(CultureInfo.InvariantCulture);
                this.formulaEvaluator = WorkbookFactory.CreateFormulaEvaluator(hssfwb);

                ISheet sheet = hssfwb.GetSheetAt(5);
                IRow headerRow = sheet.GetRow(0);
                int cellCount = headerRow.LastCellNum;
                
                for (int i = (sheet.FirstRowNum + 1); i < sheet.LastRowNum; i++)
                    {
                    IRow row = sheet.GetRow(i);
                    if (row == null)
                    {
                        continue;
                    }
                    if (row.Cells.All(d => d.CellType == CellType.Blank))
                    {
                        continue;
                    }

                    decimal disbursementValueOne = 0, disbursementValueTwo = 0, disbursementValueThree = 0, exchangeRate = 0;
                    decimal.TryParse(this.GetFormattedValue(row.GetCell(yearOneIndex)), out disbursementValueOne);
                    decimal.TryParse(this.GetFormattedValue(row.GetCell(yearTwoIndex)), out disbursementValueTwo);
                    decimal.TryParse(this.GetFormattedValue(row.GetCell(yearThreeIndex)), out disbursementValueThree);
                    decimal.TryParse(this.GetFormattedValue(row.GetCell(exRateIndex)), out exchangeRate);

                    List<ImportedLocation> locationsList = new List<ImportedLocation>();
                    for (int l = locationLowerIndex; l <= locationUpperIndex; l++)
                    {
                        decimal percentage = 0;
                        decimal.TryParse(row.GetCell(l).NumericCellValue.ToString(), out percentage);
                        locationsList.Add(new ImportedLocation()
                        {
                            Location = newDataLocations[l.ToString()],
                            Percentage = (percentage * 100 )
                        });
                    }

                    List<ImportedCustomFields> customFieldsList = new List<ImportedCustomFields>();
                    for (int c = customFieldsLowerIndex; c <= customFieldsUpperIndex; c++)
                    {
                        if (c == 39)
                            continue;

                        customFieldsList.Add(new ImportedCustomFields()
                        {
                            CustomField = newCustomFields[c.ToString()],
                            Value = this.GetFormattedValue(row.GetCell(c))
                        });
                    }

                    projectsList.Add(new ImportedAidData()
                    {
                        ProjectTitle = this.GetFormattedValue(row.GetCell(projectTitleIndex)),
                        ReportingOrganization = this.GetFormattedValue(row.GetCell(reportingOrgIndex)),
                        StartDate = this.GetFormattedValue(row.GetCell(startDateIndex)),
                        EndDate = this.GetFormattedValue(row.GetCell(endDateIndex)),
                        Funders = this.GetFormattedValue(row.GetCell(fundersIndex)),
                        Currency = this.GetFormattedValue(row.GetCell(currencyIndex)),
                        ExchangeRate = exchangeRate,
                        Implementers = this.GetFormattedValue(row.GetCell(implementersIndex)),
                        PreviousYearDisbursements =  disbursementValueOne,
                        CurrentYearDisbursements = disbursementValueTwo,
                        FutureYearDisbursements = disbursementValueThree,
                        PrimarySector = this.GetFormattedValue(row.GetCell(primarySectorIndex)),
                        Links = this.GetFormattedValue(row.GetCell(linksIndex)),
                        Locations = locationsList,
                        CustomFields = customFieldsList,
                    });
                }
            }
            return projectsList;
        }

        public List<ImportedAidData> ImportAidDataSeventeen(string filePath)
        {
            int projectTitleIndex = 0, reportingOrgIndex = 9, startDateIndex = 2, endDateIndex = 3,
                    fundersIndex = 10, implementersIndex = 11, yearOneIndex = 19, yearTwoIndex = 20,
                    yearThreeIndex = 21, primarySectorIndex = 6, currencyIndex = 16, exRateIndex = 17,
                    projectValueIndex = 18, locationLowerIndex = 39, locationUpperIndex = 47, linksIndex = 28,
                    customFieldsLowerIndex = 32, customFieldsUpperIndex = 37;

            List<ImportedAidData> projectsList = new List<ImportedAidData>();
            XSSFWorkbook hssfwb = new XSSFWorkbook(filePath);
            this.dataFormatter = new DataFormatter(CultureInfo.InvariantCulture);
            this.formulaEvaluator = WorkbookFactory.CreateFormulaEvaluator(hssfwb);

            ISheet sheet = hssfwb.GetSheetAt(1);
            IRow headerRow = sheet.GetRow(1);
            int cellCount = headerRow.LastCellNum;


            for (int i = (sheet.FirstRowNum + 1); i < sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null)
                {
                    continue;
                }
                if (row.Cells.All(d => d.CellType == CellType.Blank))
                {
                    continue;
                }

                decimal projectValue = 0, exchangeRate = 0, disbursementValueOne = 0, disbursementValueTwo = 0, disbursementValueThree = 0;
                decimal.TryParse(this.GetFormattedValue(row.GetCell(projectValueIndex)), out projectValue);
                decimal.TryParse(this.GetFormattedValue(row.GetCell(exRateIndex)), out exchangeRate);
                decimal.TryParse(this.GetFormattedValue(row.GetCell(yearOneIndex)), out disbursementValueOne);
                decimal.TryParse(this.GetFormattedValue(row.GetCell(yearTwoIndex)), out disbursementValueTwo);
                decimal.TryParse(this.GetFormattedValue(row.GetCell(yearThreeIndex)), out disbursementValueThree);

                List<ImportedLocation> locationsList = new List<ImportedLocation>();
                for(int l = locationLowerIndex; l <= locationUpperIndex; l++)
                {
                    decimal percentage = 0;
                    decimal.TryParse(row.GetCell(l).NumericCellValue.ToString(), out percentage);
                    locationsList.Add(new ImportedLocation()
                    {
                        Location = oldDataLocations[l.ToString()],
                        Percentage = (percentage * 100)
                    });
                }

                List<ImportedCustomFields> customFieldsList = new List<ImportedCustomFields>();
                for (int c = customFieldsLowerIndex; c <= customFieldsUpperIndex; c++)
                {
                    customFieldsList.Add(new ImportedCustomFields()
                    {
                        CustomField = oldCustomFields[c.ToString()],
                        Value = this.GetFormattedValue(row.GetCell(c))
                    });
                }

                projectsList.Add(new ImportedAidData()
                {
                    ProjectTitle = this.GetFormattedValue(row.GetCell(projectTitleIndex)),
                    ReportingOrganization = this.GetFormattedValue(row.GetCell(reportingOrgIndex)),
                    StartDate = this.GetFormattedValue(row.GetCell(startDateIndex)),
                    EndDate = this.GetFormattedValue(row.GetCell(endDateIndex)),
                    ProjectValue = projectValue,
                    Currency = this.GetFormattedValue(row.GetCell(currencyIndex)),
                    ExchangeRate = exchangeRate,
                    Funders = this.GetFormattedValue(row.GetCell(fundersIndex)),
                    Implementers = this.GetFormattedValue(row.GetCell(implementersIndex)),
                    PreviousYearDisbursements = disbursementValueOne,
                    CurrentYearDisbursements = disbursementValueTwo,
                    FutureYearDisbursements = disbursementValueThree,
                    PrimarySector = this.GetFormattedValue(row.GetCell(primarySectorIndex)),
                    Links = this.GetFormattedValue(row.GetCell(linksIndex)),
                    Locations = locationsList,
                    CustomFields = customFieldsList
                });
            }
            return projectsList;
        }

        public ImportedDataMatch GetMatchForOldNewData(string fileFolder)
        {
            List<string> oldProjectsList = new List<string>();
            List<string> newProjectsList = new List<string>();

            string oldDataFile = fileFolder + "/" + "2017-Somalia-Aid-Mapping.xlsx";
            string newDataFile = fileFolder + "/" + "2018-Somalia-Aid-Mapping.xlsx";

            XSSFWorkbook oldWorkBook = new XSSFWorkbook(oldDataFile);
            this.dataFormatter = new DataFormatter(CultureInfo.InvariantCulture);
            this.formulaEvaluator = WorkbookFactory.CreateFormulaEvaluator(oldWorkBook);
            ISheet sheetOld = oldWorkBook.GetSheetAt(1);
            IRow headerRowOld = sheetOld.GetRow(1);
            int projectTitleIndexOld = 0;

            for (int i = (sheetOld.FirstRowNum + 1); i <= sheetOld.LastRowNum; i++)
            {
                IRow row = sheetOld.GetRow(i);
                if (row == null)
                {
                    continue;
                }
                if (row.Cells.All(d => d.CellType == CellType.Blank))
                {
                    continue;
                }
                oldProjectsList.Add(this.GetFormattedValue(row.GetCell(projectTitleIndexOld)));
            }

            XSSFWorkbook newWorkBook = new XSSFWorkbook(newDataFile);
            this.dataFormatter = new DataFormatter(CultureInfo.InvariantCulture);
            this.formulaEvaluator = WorkbookFactory.CreateFormulaEvaluator(newWorkBook);
            ISheet sheetNew = newWorkBook.GetSheetAt(5);
            IRow headerRowNew = sheetNew.GetRow(1);
            int projectTitleIndexNew = 3, endDateIndex = 6, currentYearProjects = 0, futureYearProjects = 0, currentYear = DateTime.Now.Year;
            DateTime endDate = DateTime.Now;

            for (int i = (sheetNew.FirstRowNum + 1); i <= sheetNew.LastRowNum; i++)
            {
                IRow row = sheetNew.GetRow(i);
                if (row == null)
                {
                    continue;
                }
                if (row.Cells.All(d => d.CellType == CellType.Blank))
                {
                    continue;
                }
                newProjectsList.Add(this.GetFormattedValue(row.GetCell(projectTitleIndexNew)));
                bool isValidDate = DateTime.TryParse(this.GetFormattedValue(row.GetCell(endDateIndex)), out endDate); 
                if (isValidDate)
                {
                    if (endDate.Year == currentYear)
                    {
                        ++currentYearProjects;
                    }
                    else if (endDate.Year > currentYear)
                    {
                        ++futureYearProjects;
                    }
                }
            }

            int matches = 0;
            foreach(string project in newProjectsList)
            {
                var isProjectMatch = (from p in oldProjectsList
                                      where p.Equals(project, StringComparison.OrdinalIgnoreCase)
                                      select p).FirstOrDefault();
                matches += (isProjectMatch != null) ? 1 : 0;
            }

            
            ImportedDataMatch dataMatch = new ImportedDataMatch()
            {
                TotalProjectsNew = newProjectsList.Count,
                TotalProjectsOld = oldProjectsList.Count,
                CurrentYearProjectsNew = currentYearProjects,
                FutureYearProjectsNew = futureYearProjects,
                TotalMatchedProjects = matches
            };
            return dataMatch;
        }

        public string GenerateExcelFileForActiveProjects(string fileFolder)
        {
            List<string> oldProjectsList = new List<string>();
            List<string> newProjectsList = new List<string>();
            List<ActiveProject> activeProjectsList = new List<ActiveProject>();

            string oldDataFile = fileFolder + "/" + "2017-Somalia-Aid-Mapping.xlsx";
            string newDataFile = fileFolder + "/" + "2018-Somalia-Aid-Mapping.xlsx";

            XSSFWorkbook oldWorkBook = new XSSFWorkbook(oldDataFile);
            this.dataFormatter = new DataFormatter(CultureInfo.InvariantCulture);
            this.formulaEvaluator = WorkbookFactory.CreateFormulaEvaluator(oldWorkBook);
            ISheet sheetOld = oldWorkBook.GetSheetAt(1);
            IRow headerRowOld = sheetOld.GetRow(1);
            int projectTitleIndexOld = 0;

            for (int i = (sheetOld.FirstRowNum + 1); i <= sheetOld.LastRowNum; i++)
            {
                IRow row = sheetOld.GetRow(i);
                if (row == null)
                {
                    continue;
                }
                if (row.Cells.All(d => d.CellType == CellType.Blank))
                {
                    continue;
                }
                oldProjectsList.Add(this.GetFormattedValue(row.GetCell(projectTitleIndexOld)));
            }

            XSSFWorkbook newWorkBook = new XSSFWorkbook(newDataFile);
            this.dataFormatter = new DataFormatter(CultureInfo.InvariantCulture);
            this.formulaEvaluator = WorkbookFactory.CreateFormulaEvaluator(newWorkBook);
            ISheet sheetNew = newWorkBook.GetSheetAt(5);
            IRow headerRowNew = sheetNew.GetRow(1);
            int projectTitleIndexNew = 3, endDateIndex = 6, currentYearProjects = 0, futureYearProjects = 0, currentYear = DateTime.Now.Year;
            string projectTitle = "";
            DateTime endDate = DateTime.Now;

            for (int i = (sheetNew.FirstRowNum + 1); i <= sheetNew.LastRowNum; i++)
            {
                IRow row = sheetNew.GetRow(i);
                if (row == null)
                {
                    continue;
                }
                if (row.Cells.All(d => d.CellType == CellType.Blank))
                {
                    continue;
                }

                projectTitle = this.GetFormattedValue(row.GetCell(projectTitleIndexNew));
                newProjectsList.Add(projectTitle);
                bool isValidDate = DateTime.TryParse(this.GetFormattedValue(row.GetCell(endDateIndex)), out endDate);
                if (isValidDate)
                {
                    if (endDate.Year >= currentYear)
                    {
                        activeProjectsList.Add(new ActiveProject()
                        {
                            ProjectTitle = projectTitle,
                            EndDate = endDate.ToShortDateString(),
                            IsMatched = "No"
                        });
                    }
                    
                    if (endDate.Year == currentYear)
                    {
                        ++currentYearProjects;
                    }
                    else if (endDate.Year > currentYear)
                    {
                        ++futureYearProjects;
                    }
                }
            }

            int matches = 0;
            foreach (string project in newProjectsList)
            {
                var isProjectMatch = (from p in oldProjectsList
                                      where p.Equals(project, StringComparison.OrdinalIgnoreCase)
                                      select p).FirstOrDefault();
                matches += (isProjectMatch != null) ? 1 : 0;
            }

            foreach (var project in activeProjectsList)
            {
                var matchedProjects = (from p in oldProjectsList
                                        where p.Equals(project.ProjectTitle, StringComparison.OrdinalIgnoreCase)
                                        select p);

                if (matchedProjects.Any())
                {
                    project.IsMatched = "Yes";
                }
            }

            string sFileName = @"MatchinProjects-" + DateTime.UtcNow.Ticks.ToString() + ".xlsx";
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
                headerStyle.Alignment = HorizontalAlignment.Center;
                headerStyle.VerticalAlignment = VerticalAlignment.Center;

                ICellStyle highlightStyle = workbook.CreateCellStyle();
                IFont fontHighlight = workbook.CreateFont();
                fontHighlight.Color = IndexedColors.DarkBlue.Index;
                fontHighlight.FontHeightInPoints = 12;
                highlightStyle.SetFont(fontHighlight);
                highlightStyle.Alignment = HorizontalAlignment.Center;
                highlightStyle.VerticalAlignment = VerticalAlignment.Center;

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
                titleCell.SetCellValue("SomaliAIMS projects report for import - generated on " + DateTime.Now.ToLongDateString());
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

                var cellSerial = row.CreateCell(1, CellType.String);
                cellSerial.SetCellValue("Serial");
                cellSerial.CellStyle = headerStyle;

                var projectCell = row.CreateCell(2, CellType.String);
                projectCell.SetCellValue("Project title");
                projectCell.CellStyle = headerStyle;

                var endDateCell = row.CreateCell(3, CellType.String);
                endDateCell.SetCellValue("End date");
                endDateCell.CellStyle = headerStyle;

                var isMatchedCell = row.CreateCell(4, CellType.String);
                isMatchedCell.SetCellValue("Matched with 2017");
                isMatchedCell.CellStyle = headerStyle;

                int rowIndex = 0;
                foreach (var project in activeProjectsList)
                {
                    int colIndex = 0;
                    row = excelSheet.CreateRow(++rowCounter);
                    row.CreateCell(colIndex, CellType.Blank);

                    var serialCell = row.CreateCell(++colIndex, CellType.Numeric);
                    serialCell.SetCellValue(++rowIndex);
                    serialCell.CellStyle = dataCellStyle;

                    var projectTitleCell = row.CreateCell(++colIndex, CellType.String);
                    projectTitleCell.SetCellValue(project.ProjectTitle);
                    projectTitleCell.CellStyle = dataCellStyle;

                    var projectEnddateCell = row.CreateCell(++colIndex, CellType.String);
                    projectEnddateCell.SetCellValue(project.EndDate);
                    projectEnddateCell.CellStyle = dataCellStyle;

                    var isMatchedDataCell = row.CreateCell(++colIndex, CellType.String);
                    isMatchedDataCell.SetCellValue(project.IsMatched);
                    isMatchedDataCell.CellStyle = dataCellStyle;
                }
                workbook.Write(fs);
                return sFileName;
            }
        }

        private string ApplyThousandFormat(decimal number)
        {
            return (Math.Round(number).ToString("#,##0.00"));
        }

        private string GetFormattedValue(ICell cell)
        {
            string returnValue = string.Empty;
            if (cell != null)
            {
                try
                {
                    // Get evaluated and formatted cell value
                    returnValue = this.dataFormatter.FormatCellValue(cell, this.formulaEvaluator);
                }
                catch
                {
                    // When failed in evaluating the formula, use stored values instead...
                    // and set cell value for reference from formulae in other cells...
                    if (cell.CellType == CellType.Formula)
                    {
                        switch (cell.CachedFormulaResultType)
                        {
                            case CellType.String:
                                returnValue = cell.StringCellValue;
                                cell.SetCellValue(cell.StringCellValue);
                                break;
                            case CellType.Numeric:
                                returnValue = dataFormatter.FormatRawCellContents
                                (cell.NumericCellValue, 0, cell.CellStyle.GetDataFormatString());
                                cell.SetCellValue(cell.NumericCellValue);
                                break;
                            case CellType.Boolean:
                                returnValue = cell.BooleanCellValue.ToString();
                                cell.SetCellValue(cell.BooleanCellValue);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            return (returnValue ?? string.Empty).Trim();
        }

        
    }
}
