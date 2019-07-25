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

        public void ImportAidData(string filePath)
        {
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
                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        if (row.GetCell(j) != null)
                        {

                        }
                            //sb.Append("<td>" + row.GetCell(j).ToString() + "</td>");
                    }
                    //sb.AppendLine("</tr>");
                }
            }
                
        }
    }
}
