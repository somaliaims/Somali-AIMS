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
        public string FundPercentage { get; set; }
    }

    public class IATIOrganization
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

    public class IATIActivity
    {
        public int Id { get; set; }
        public string Identifier { get; set; }
        public string DefaultCurrency { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ICollection<IATISector> Sectors { get; set; }
        public ICollection<IATICountry> Countries { get; set; }
        public ICollection<IATIRegion> Regions { get; set; }
        public ICollection<IATIOrganization> ParticipatingOrganizations { get; set; }
        public ICollection<IATITransaction> Transactions { get; set; }
    }

    public class IATIView
    {
        public ICollection<IATIActivity> Activities { get; set; }
        public string Dated { get; set; }
    }

    public class IATIModel
    {
        [Required]
        public string Data { get; set; }
        public string Organizations { get; set; }
    }

}