using System;
using System.Collections.Generic;
using System.Text;

namespace AIMS.Models
{
    public class ImportedDataEighteen
    {
        public string ProjectTitle { get; set; }
        public string ReportingOrganization { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Funders { get; set; }
        public string Implementers { get; set; }
        public decimal PreviousYearDisbursements { get; set; }
        public decimal CurrentYearDisbursements { get; set; }
        public decimal FutureYearDisbursements { get; set; }
        public string PrimarySector { get; set; }
        public string RRFMarker { get; set; }
    }
}
