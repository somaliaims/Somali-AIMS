using AIMS.Models;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;

namespace AIMS.Services
{
    public interface IDataImportService
    {
        ICollection<ImportedDataEighteen> ImportAidDataEighteen(string filePath);
    }

    public class DataImportService
    {
        List<string> locationsList;
        public DataImportService()
        {
            locationsList = new List<string>()
            {
                "FGS",
                "BRA",
                "Galmudug",
                "Hiirshabelle",
                "Jubaland",
                "Puntland",
                "South West",
                "Somaliland",
                "Unattributed"
            };
        }

        public List<ImportedDataEighteen> ImportAidDataEighteen(string filePath)
        {
            List<ImportedDataEighteen> projectsList = new List<ImportedDataEighteen>();
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                int projectTitleIndex = 3, reportingOrgIndex = 2, startDateIndex = 0, endDateIndex = 0,
                    fundersIndex = 0, implementersIndex = 0, yearOneIndex = 0, yearTwoIndex = 0,
                    yearThreeIndex = 0, primarySectorIndex = 0, rrfMarkerIndex = 0;

                stream.Position = 0;
                XSSFWorkbook hssfwb = new XSSFWorkbook(stream);
                ISheet sheet = hssfwb.GetSheetAt(4);
                IRow headerRow = sheet.GetRow(0);
                int cellCount = headerRow.LastCellNum;

                
                for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
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

                    decimal disbursementValueOne = 0, disbursementValueTwo = 0, disbursementValueThree = 0;
                    decimal.TryParse(row.GetCell(yearOneIndex).ToString(), out disbursementValueOne);
                    decimal.TryParse(row.GetCell(yearTwoIndex).ToString(), out disbursementValueTwo);
                    decimal.TryParse(row.GetCell(yearThreeIndex).ToString(), out disbursementValueThree);

                    projectsList.Add(new ImportedDataEighteen()
                    {
                        ProjectTitle = row.GetCell(projectTitleIndex).ToString(),
                        ReportingOrganization = row.GetCell(reportingOrgIndex).ToString(),
                        StartDate = row.GetCell(startDateIndex).ToString(),
                        EndDate = row.GetCell(endDateIndex).ToString(),
                        Funders = row.GetCell(fundersIndex).ToString(),
                        Implementers = row.GetCell(implementersIndex).ToString(),
                        PreviousYearDisbursements =  disbursementValueOne,
                        CurrentYearDisbursements = disbursementValueTwo,
                        FutureYearDisbursements = disbursementValueThree,
                        PrimarySector = row.GetCell(primarySectorIndex).ToString(),
                        RRFMarker = row.GetCell(rrfMarkerIndex).ToString()
                    });
                }
            }
            return projectsList;
        }
    }
}
