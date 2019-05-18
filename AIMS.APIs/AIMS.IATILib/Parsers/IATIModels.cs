using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AIMS.IATILib.Parsers
{
    public enum OrganizationRole
    {
        Funding = 1,
        Accountable = 2,
        Extending = 3,
        Implementing = 4
    }

    public class IATITransactionTypes
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class IATICountry
    {
        public string Code { get; set; }
        public string ContributionPercentage { get; set; }
    }

    public class IATIRegion
    {
        public string Code { get; set; }
        public string ContributionPercentage { get; set; }
    }

    public class IATISector
    {
        public string Code { get; set; }
        public string SectorName { get; set; }
        public string FundsPercentage { get; set; }
    }

    public class IATISectorModel
    {
        public string SectorName { get; set; }
    }

    public class IATILocation
    {
        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }

    public class IATIOrganization
    {
        public int Id { get; set; }
        public string Project { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
    }

    public class IATIOrganizationModel
    {
        public string Name { get; set; }
    }

    public class IATITransaction
    {
        //public string AidType { get; set; }
        public int Id { get; set; }
        public string TransactionType { get; set; }
        public string Currency { get; set; }
        public string Amount { get; set; }
        public string Dated { get; set; }
        //public string Description { get; set; }
    }

    public class IATIDocument
    {
        public int Id { get; set; }
        public string DocumentTitle { get; set; }
        public string DocumentUrl { get; set; }
    }

    public class IATIActivity
    {
        public int Id { get; set; }
        public string Identifier { get; set; }
        public string DefaultCurrency { get; set; }
        public string Title { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string TrimmedTitle { get; set; }
        public string Description { get; set; }
        public ICollection<IATIBudget> Budgets { get; set; }
        public ICollection<IATIDisbursement> Disbursements { get; set; }
        public ICollection<IATILocation> Locations { get; set; }
        public ICollection<IATISector> Sectors { get; set; }
        public ICollection<IATIOrganization> Funders { get; set; }
        public ICollection<IATIOrganization> Implementers { get; set; }
        public ICollection<IATITransaction> Transactions { get; set; }
        public ICollection<IATIDocument> Documents { get; set; }
    }

    public class IATIView
    {
        public ICollection<IATIActivity> Activities { get; set; }
        public string Dated { get; set; }
    }

    public class IATIBudget
    {
        public int Id { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }

    public class IATIDisbursement
    {
        public int Id { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }

    public class IATIModel
    {
        [Required]
        public string Data { get; set; }
        public string Organizations { get; set; }
    }

    public class IATIProject
    {
        public int Id { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string IATIIdentifier { get; set; }
        public string Title { get; set; }
        public string TrimmedTitle { get; set; }
        public string DefaultCurrency { get; set; }
        public string Description { get; set; }
    }


}
