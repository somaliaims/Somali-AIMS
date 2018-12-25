using System;
using System.Collections.Generic;
using System.Text;

namespace IATILib.Parsers
{
    /// <summary>
    /// IATI Models
    /// </summary>
    public enum OrganizationRole
    {
        Funding = 1,
        Accountable = 2,
        Extending = 3,
        Implementing = 4
    }

    public class AidTypes
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class TransactionTypes
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class Country
    {
        public string Code { get; set; }
        public string ContributionPercentage { get; set; }
    }

    public class Region
    {
        public string Code { get; set; }
        public string ContributionPercentage { get; set; }
    }

    public class Sector
    {
        public string Code { get; set; }
        public string FundPercentage { get; set; }
    }

    public class IATIActivity
    {
        public int Id { get; set; }
        public string Identifier { get; set; }
        public string DefaultCurrency { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ICollection<Sector> Sectors { get; set; }
        public ICollection<Country> Countries { get; set; }
        public ICollection<Region> Regions { get; set; }
        public ICollection<Organization> ParticipatingOrganizations { get; set; }
        public ICollection<IATITransaction> Transactions { get; set; }
    }

    public class Budget
    {
        public string PeriodStart { get; set; }
        public string PeriodEnd { get; set; }
        public string PlannedAmount { get; set; }
    }

    public class Organization
    {
        public string Project { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
    }

    public class IATITransaction
    {
        public string AidType { get; set; }
        public string TransactionType { get; set; }
        public string Currency { get; set; }
        public string Amount { get; set; }
        public string Dated { get; set; }
        public string Description { get; set; }
    }

    public class IATIModel
    {
        [Required]
        public string Data { get; set; }
        public string Organizations { get; set; }
    }

    public class IATIView
    {
        public ICollection<IATIActivity> Activities { get; set; }
        public string Dated { get; set; }
    }
}
