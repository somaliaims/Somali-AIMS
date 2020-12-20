using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIMS.APIs.Helpers
{
    public class UtilityHelper
    {
        public string FormatDateAsString(DateTime dated)
        {
            string monthStr = dated.Month < 10 ? "0" + dated.Month : dated.Month.ToString();
            string dateStr = dated.Day < 10 ? "0" + dated.Day : dated.Day.ToString();
            string datedStr = dated.Year + "-" + monthStr + "-" + dateStr;
            return datedStr;
        }
    }
}
