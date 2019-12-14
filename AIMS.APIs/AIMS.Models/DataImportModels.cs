using System;
using System.Collections.Generic;
using System.Text;

namespace AIMS.Models
{
    public class ImportedAidData
    {
        public string ProjectTitle { get; set; }
        public string ProjectDescription { get; set; }
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

    public class ImportedEnvelopeData
    {
        public string Organization { get; set; }
        public string Currency { get; set; }
        public decimal DevelopmentEighteen { get; set; }
        public decimal DevelopmentNineteen { get; set; }
        public decimal DevelopmentTwenty { get; set; }
        public decimal HumanitarianEighteen { get; set; }
        public decimal HumanitarianNineteen { get; set; }
        public decimal HumanitarianTwenty { get; set; }
        public decimal ExchangeRate { get; set; }
    }

    public class NewImportedAidData
    {
        public string ProjectTitle { get; set; }
        public string ProjectDescription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal ProjectValue { get; set; }
        public string Currency { get; set; }
        public decimal ExchangeRate { get; set; }
        public string StartYear { get; set; }
        public string EndYear { get; set; }
        public string Funders { get; set; }
        public string Implementers { get; set; }
        public decimal TwentySixteenDisbursements { get; set; }
        public decimal TwentySeventeenDisbursements { get; set; }
        public decimal TwentyEighteenDisbursements { get; set; }
        public decimal TwentyNineteenDisbursements { get; set; }
        public decimal TwentyTwentyDisbursements { get; set; }
        public decimal TwentyOneDisbursements { get; set; }
        public decimal TwentyTwoDisbursements { get; set; }
        public decimal TwentyThreeDisbursements { get; set; }
        public decimal TwentyFourDisbursements { get; set; }
        public string Sector { get; set; }
        public List<ImportedDocumentLinks> DocumentLinks { get; set; }
        public List<ImportedLocation> Locations { get; set; }
        public List<ImportedCustomFields> CustomFields { get; set; }
    }

    public class ImportedCustomFields
    {
        public string CustomField { get; set; }
        public string Value { get; set; }
    }

    public class ImportedOrganizationTypes
    {
        public int OrganizationTypeId { get; set; }
        public string OrganizationType { get; set; }
    }

    public class ImportedOrganizations
    {
        public string OrganizationType { get; set; }
        public string Organization { get; set; }
    }

    public class ImportedDocumentLinks
    {
        public string DocumentTitle { get; set; }
        public string DocumentUrl { get; set; }
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
