﻿using System;
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

    public class IATIFinanceTypes
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class IATISectorsVocabulary
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class IATIOrganizationTypeVocabulary
    {
        public int Code { get; set; }
        public string Name { get; set; }
    }

    public class IATISectorView
    {
        public string Name { get; set; }
    }

    public class IATICountryCode
    {
        public string Code { get; set; }
        public string Country { get; set; }
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
        public int SectorTypeCode { get; set; }
        public string Code { get; set; }
        public string SectorName { get; set; }
        public string FundsPercentage { get; set; }
    }

    public class IATISectorModel
    {
        public string SectorTypeCode { get; set; }
        public string SectorCode { get; set; } = null;
        public string SectorName { get; set; }
    }

    public class SourceSectorsView
    {
        public string SectorTypeName { get; set; }
        public List<SourceSectorModel> SectorsList { get; set; }
    }

    public class SourceSectorModel
    {
        public string SectorCode { get; set; }
        public string SectorName { get; set; }
    }

    public class IATILocation
    {
        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }

    public class IATILocationView
    {
        public string Name { get; set; }
    }

    public class IATIOrganization
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string Project { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
    }

    public class IATIOrganizationView
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }
    }

    public class IATIOrganizationModel
    {
        public int Code { get; set; }
        public string Name { get; set; }
    }

    public enum FinanceDisplayType
    {
        Funding = 1,
        Disbursement = 2
    }

    public class IATITransaction
    {
        //public string AidType { get; set; }
        public int Id { get; set; }
        public string TransactionType { get; set; }
        public string Currency { get; set; }
        public string Amount { get; set; }
        public string Dated { get; set; }
        public FinanceDisplayType FinanceType { get; set; }
    }

    public class IATIFundingTransaction
    {
        public int Id { get; set; }
        public string TransactionType { get; set; }
        public string Currency { get; set; }
        public string Amount { get; set; }
        public string Dated { get; set; }
    }

    public class IATIDisbursementTransaction
    {
        public int Id { get; set; }
        public string TransactionType { get; set; }
        public string Currency { get; set; }
        public string Amount { get; set; }
        public string Dated { get; set; }
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
        public string DefaultFinanceType { get; set; }
        public string Title { get; set; }
        public string ProjectValue { get; set; }
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
        public ICollection<IATIFundingTransaction> FundingTransactions { get; set; }
        public ICollection<IATIDisbursementTransaction> DisbursementTransactions { get; set; }
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
        public string budgetType { get; set; }
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
        public decimal ProjectValue { get; set; }
        public string DefaultFinanceType { get; set; }
        public string DefaultCurrency { get; set; }
        public string Description { get; set; }
        public ICollection<IATIOrganizationView> Organizations { get; set; }
        public ICollection<IATILocationView> Locations { get; set; }
        public ICollection<IATISectorView> Sectors { get; set; }
    }


}
