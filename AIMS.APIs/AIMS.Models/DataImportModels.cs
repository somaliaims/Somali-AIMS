using System;
using System.Collections.Generic;
using System.Text;

namespace AIMS.Models
{
    public class ImportedAidData
    {
        public string ProjectTitle { get; set; }
        public string ReportingOrganization { get; set; }
        public decimal ProjectValue { get; set; }
        public string Currency { get; set; }
        public decimal ExchangeRate { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Funders { get; set; }
        public string Implementers { get; set; }
        public decimal PreviousYearDisbursements { get; set; }
        public decimal CurrentYearDisbursements { get; set; }
        public decimal FutureYearDisbursements { get; set; }
        public string PrimarySector { get; set; }
        public string Links { get; set; }
        public List<ImportedLocation> Locations { get; set; }
        public List<ImportedCustomFields> CustomFields { get; set; }
    }

    public class ImportedCustomFields
    {
        public string CustomField { get; set; }
        public string Value { get; set; }
    }

    public class ImportedLocation
    {
        public string Location { get; set; }
        public decimal Percentage { get; set; }
    }

    public class ImportedDataMatch
    {
        public int TotalProjectsNew { get; set; } = 0;
        public int TotalProjectsOld { get; set; } = 0;
        public int CurrentYearProjectsNew { get; set; } = 0;
        public int FutureYearProjectsNew { get; set; } = 0;
        public int TotalMatchedProjects { get; set; } = 0;
    }

    public class ActiveProject
    {
        public string ProjectTitle { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string IsMatched { get; set; }
    }
}
