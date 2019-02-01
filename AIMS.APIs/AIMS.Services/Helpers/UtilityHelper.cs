using System;
using System.Collections.Generic;
using System.Text;

namespace AIMS.Services.Helpers
{
    public class UtilityHelper
    {
        public int GetMonthDifference(DateTime startDate, DateTime endDate)
        {
            int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            return Math.Abs(monthsApart);
        }
    }
}
