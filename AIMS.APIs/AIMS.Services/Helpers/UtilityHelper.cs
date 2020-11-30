using AIMS.Models;
using Newtonsoft.Json;
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

        public IEnumerable<MarkerValues> ParseAndExtractIfJson(String json)
        {
            List<MarkerValues> markerValues = new List<MarkerValues>();
            try
            {
                markerValues = JsonConvert.DeserializeObject<List<MarkerValues>>(json);
            }
            catch (Exception)
            {
            }
            return markerValues;
        }
    }
}
