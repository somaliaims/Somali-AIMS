using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIMS.Models
{
    public enum UserTypes
    {
        SuperAdmin = 1,
        Manager = 2,
        Standard = 3
    }

    public enum DisbursementTypes
    {
        Actual = 1,
        Planned = 2
    }


    public enum ProjectDeletionStatus
    {
        Requested = 1,
        Approved = 2,
        Cancelled = 3
    }

    public enum OrganizationSourceType
    {
        IATI = 1,
        User = 2
    }

    public enum FieldTypes
    {
        DropDown = 1,
        CheckBox = 2,
        Text = 3,
        Radio = 4
    }

    public enum CurrencySource
    {
        OpenExchange = 1,
        Manual = 2
    }

    public enum MembershipTypes
    {
        Funder = 1,
        Implementer = 2
    }

    public enum NotificationTypes
    {
        NewUser = 1,
        NewProjectToOrg = 2,
        NewIATIToProject = 3,
        UserInactive = 4,
        ChangedMappingEffectedProject = 5,
        NewIATISector = 6,
        OrganizationMerged = 7,
        ProjectDeletion = 8
    }

    public enum EmailMessageType
    {
        NewUser = 1,
        NewProjectToOrg = 2,
        UserInactive = 3,
        ChangedMappingEffectedProject = 4,
        NewIATISector = 5,
        OrganizationMerged = 6,
        NewOrgToProject = 7,
        ProjectPermissionGranted = 8,
        ProjectPermissionDenied = 9,
        UserApproved = 10,
        OrganizationRenamed = 11,
        ProjectDeletionRequest = 12,
        ProjectDeleted = 13,
        ProjectDeletionApproved = 14,
        ProjectDeletionCancelled = 15,
        ResetPassword = 16,
        NewIATIOrganization = 17,
        MergeOrganizationRequest = 18,
        MergeOrganizationRejected = 19,
        UserApprovedUnAffiliated = 20,
        OrganizationDeletedAndMapped = 21
    }

    public enum HelpForEntity
    {
        Project = 1,
        ProjectFunders = 2,
        ProjectImplementers = 3,
        ProjectDocuments = 4,
        ProjectDisbursements = 5,
        ProjectExpectedDisbursements = 6,
        ProjectSectors = 7,
        ProjectLocations = 8
    }

    public enum DataTransactions
    {
        Inserted = 1,
        Updated = 2,
        Deleted = 3
    }

    public enum ExchangeRateSources
    {
        OpenExchange = 1,
        CentralBank = 2,
        Manual = 3
    }

    public enum ExchangeRateUsageSection
    {
        DataEntry = 1,
        Reporting = 2
    }

    public class EFFundingTypes
    {
        public int Id { get; set; }
        public string FundingType { get; set; }
    }

    public class EFOrganizationTypes
    {
        [Key]
        public int Id { get; set; }
        public string TypeName { get; set; }
        public bool IsUnAffiliated { get; set; } = false;
    }

    public class EFSectorTypes
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
        public bool? IsPrimary { get; set; }
        public bool IsSourceType { get; set; } = true;
        public string IATICode { get; set; }
        public string SourceUrl { get; set; }
        public IEnumerable<EFSector> Sectors { get; set; }
    }

    public class EFOrganization
    {
        [Key]
        public int Id { get; set; }
        public string OrganizationName { get; set; }
        public bool IsApproved { get; set; } = true;
        [ForeignKey("OrganizationType")]
        public int? OrganizationTypeId { get; set; }
        public bool IsUnAffiliated { get; set; } = false;
        public virtual EFOrganizationTypes OrganizationType { get; set; }
        public DateTime DateUpdated { get; set; } = new DateTime(2020, 03, 20);
    }

    public class EFIATIOrganization
    {
        [Key]
        public int Id { get; set; }
        public string OrganizationName { get; set; }
    }

    public class EFHelp
    {
        public int Id { get; set; }
        public HelpForEntity Entity { get; set; }
        public string HelpInfoJson { get; set; }
    }

    public class EFUser
    {
        [Key]
        public int Id { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public UserTypes UserType { get; set; }
        [ForeignKey("Organization")]
        public int OrganizationId { get; set; }
        public EFOrganization Organization { get; set; }
        public bool IsApproved { get; set; }
        public bool IsUnAffiliated { get; set; } = false;
        public DateTime? ApprovedOn { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? LastLogin { get; set; }
    }

    public class EFSector
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("SectorType")]
        public int SectorTypeId { get; set; }
        public EFSectorTypes SectorType { get; set; }
        public string SectorName { get; set; }
        [ForeignKey("ParentSector")]
        public int? ParentSectorId { get; set; }
        public EFSector ParentSector { get; set; }
        [MaxLength(10)]
        public string IATICode { get; set; } = null;
        public bool IsUnAttributed { get; set; } = false;
        public DateTime TimeStamp { get; set; }
    }

    public class EFSectorMappings
    {
        //This is about other sector types
        public int SectorId { get; set; }
         //This is primary sector
        public int MappedSectorId { get; set; }
        public int SectorTypeId { get; set; }
    }


    public class EFLocation
    {
        [Key]
        public int Id { get; set; }
        public string Location { get; set; }
        [Column(TypeName = "decimal(9, 5)")]
        public decimal? Latitude { get; set; }
        [Column(TypeName = "decimal(9, 5)")]
        public decimal? Longitude { get; set; }
        public bool IsUnAttributed { get; set; } = false;
    }

    public class EFSubLocation
    {
        [Key]
        public int Id { get; set; }
        public string SubLocation { get; set; }
        [ForeignKey("Location")]
        public int LocationId { get; set; }
        public EFLocation Location { get; set; }
    }

    /*
     * Need to work on Envelope data at the service level and need to incorporate in the Projects Table
     */
    public class EFProject
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(1000)]
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public EFFinancialYears StartingFinancialYear { get; set; }
        public EFFinancialYears EndingFinancialYear { get; set; }
        [ForeignKey("FundingTypeId")]
        public int FundingTypeId { get; set; }
        public EFFundingTypes FundingType { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal ProjectValue { get; set; }
        [Column(TypeName = "decimal(9, 2)")]
        public decimal ExchangeRate { get; set; }
        public string ProjectCurrency { get; set; }
        public ICollection<EFProjectSectors> Sectors { get; set; }
        public ICollection<EFProjectLocations> Locations { get; set; }
        public ICollection<EFProjectDisbursements> Disbursements { get; set; }
        public ICollection<EFProjectFunders> Funders { get; set; }
        public ICollection<EFProjectImplementers> Implementers { get; set; }
        public ICollection<EFProjectDocuments> Documents { get; set; }
        public ICollection<EFProjectMarkers> Markers { get; set; }
        public DateTime DateUpdated { get; set; }
        public int? CreatedById { get; set; }
        public EFUser CreatedBy { get; set; } = null;
    }

    public class EFEnvelopeTypes
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string TypeName { get; set; }
    }

    public class EFEnvelope
    {
        public int Id { get; set; }
        [ForeignKey("Funder")]
        public int FunderId { get; set; }
        public EFOrganization Funder { get; set; }
        public string Currency { get; set; }
        [Column(TypeName = "decimal(9, 2)")]
        public decimal ExchangeRate { get; set; } = 1;
    }

    public class EFEnvelopeYearlyBreakup
    {
        [ForeignKey("EnvelopeType")]
        public int EnvelopeTypeId { get; set; }
        public EFEnvelopeTypes EnvelopeType { get; set; }
        [ForeignKey("Envelope")]
        public int EnvelopeId { get; set; }
        public EFEnvelope Envelope { get; set; }
        [ForeignKey("Year")]
        public int YearId { get; set; }
        public EFFinancialYears Year { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }
    }

    public class EFProjectDisbursements
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public EFProject Project { get; set; }
        [ForeignKey("Year")]
        public int YearId { get; set; }
        public EFFinancialYears Year { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public DisbursementTypes DisbursementType { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        [Column(TypeName = "decimal(9, 2)")]
        public decimal ExchangeRate { get; set; }
    }

    public class EFProjectSectors
    {
        public int ProjectId { get; set; }
        public EFProject Project { get; set; }
        [ForeignKey("Sector")]
        public int SectorId { get; set; }
        public EFSector Sector { get; set; }
        [Column(TypeName = "decimal(9, 2)")]
        public decimal FundsPercentage { get; set; }
    }

    public class EFProjectDocuments
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public EFProject Project { get; set; }
        public string DocumentTitle { get; set; }
        public string DocumentUrl { get; set; }
    }

    public class EFProjectLocations
    {
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public EFProject Project { get; set; }
        [ForeignKey("Location")]
        public int LocationId { get; set; }
        public EFLocation Location { get; set; }
        public string SubLocations { get; set; } = null;
        public string SubLocationIds { get; set; } = null;
        [Column(TypeName = "decimal(9, 2)")]
        public decimal FundsPercentage { get; set; }
    }

   /*public class EFProjectSubLocations
    {
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public EFProject Project { get; set; }
        [ForeignKey("Location")]
        public int LocationId { get; set; }
        public EFLocation Location { get; set; } 
        [ForeignKey("SubLocation")]
        public int SubLocationId { get; set; }
        public EFSubLocation SubLocation { get; set; }
    }*/

    public class EFProjectFunders
    {
        [ForeignKey("Funder")]
        public int FunderId { get; set; }
        public EFOrganization Funder { get; set; }
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public EFProject Project { get; set; }
    }

    public class EFProjectMembershipRequests
    {
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public EFProject Project { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public EFUser User { get; set; }
        [ForeignKey("Organization")]
        public int? OrganizationId { get; set; } = 0;
        public EFOrganization Organization { get; set; }
        public MembershipTypes MembershipType { get; set; }
        public DateTime Dated { get; set; }
        public bool IsApproved { get; set; }
    }

    public class EFOrganizationMergeRequests
    {
        [Key]
        public int Id { get; set; }
        public bool IsApproved { get; set; } = false;
        [Required]
        public DateTime Dated { get; set; }
        [Required]
        public int OrganizationTypeId { get; set; }
        [Required]
        public string NewName { get; set; }
        public string OrganizationIdsJson { get; set; }
        [ForeignKey("RequestedBy")]
        public int? RequestedById { get; set; }
        public EFUser RequestedBy { get; set; }
        [ForeignKey("EnvelopeOrganization")]
        public int? EnvelopeOrganizationId { get; set; }
        public EFOrganization EnvelopeOrganization { get; set; }
        public ICollection<EFOrganizationsToMerge> Organizations { get; set; }
    }

    public class EFUserRoleSettlementRequests
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        [Required]
        public EFUser User { get; set; }
        [Required]
        public UserTypes RequestedUserType { get; set; }
        [Required]
        public DateTime Dated { get; set; }
        [Required]
        public bool Status { get; set; } = false;
    }

    public class EFOrganizationsToMerge
    {
        [ForeignKey("Request")]
        public int RequestId { get; set; }
        public EFOrganizationMergeRequests Request { get; set; }
        [ForeignKey("Organization")]
        public int OrganizationId { get; set; }
        public EFOrganization Organization { get; set; }
    }

    public class EFProjectImplementers
    {
        [ForeignKey("Implementer")]
        public int ImplementerId { get; set; }
        public EFOrganization Implementer { get; set; }
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public EFProject Project { get; set; }
    }

    public class EFReportSubscriptions
    {
        [ForeignKey("User")]
        public int UserId { get; set; }
        public EFUser User { get; set; }
        [ForeignKey("Report")]
        public int ReportId { get; set; }
        public EFStaticReports Report { get; set; }
    }    

    public class EFExchangeRatesUsageSettings
    {
        public int Id { get; set; }
        public ExchangeRateSources Source { get; set; }
        public ExchangeRateUsageSection UsageSection { get; set; }
        public int Order { get; set; }
    }

    public class EFUserNotifications
    {
        [Key]
        public int Id { get; set; }
        public UserTypes UserType { get; set; }
        [ForeignKey("Organization")]
        public int? OrganizationId { get; set; }
        public string Email { get; set; } = null;
        public int TreatmentId { get; set; }
        public EFOrganization Organization { get; set; }
        public string Message { get; set; }
        public DateTime Dated { get; set; }
        public bool IsSeen { get; set; }
        public NotificationTypes NotificationType { get; set; }
    }

    public class EFLogs
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public EFProject Project { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public EFUser UpdatedBy { get; set; }
        public DataTransactions ActionPerformed { get; set; }
        public DateTime Dated { get; set; }
    }

    public class EFStaticReports
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
    }

    public class EFCurrency
    {
        [Key]
        public int Id { get; set; }
        public string Currency { get; set; }
        public string CurrencyName { get; set; } = null;
        public bool IsDefault { get; set; }
        public bool IsNational { get; set; } = false;
        public CurrencySource? Source { get; set; } = CurrencySource.OpenExchange;

    }

    public class EFExchangeRates
    {
        [Key]
        public int Id { get; set; }
        public string ExchangeRatesJson { get; set; }
        public DateTime Dated { get; set; }
    }

    public class EFExchangeRatesAPIsCount
    {
        public int Id { get; set; }
        public DateTime Dated { get; set; }
        public int Count { get; set; }
    }

    public class EFMarkers
    {
        [Key]
        public int Id { get; set; }
        public string FieldTitle { get; set; }
        public FieldTypes FieldType { get; set; }
        public string Values { get; set; }
        public string Help { get; set; }
    }

    public class EFProjectMarkers
    {
        public int ProjectId { get; set; }
        public EFProject Project { get; set; }
        [ForeignKey("Marker")]
        public int MarkerId { get; set; }
        public EFMarkers Marker { get; set; }
        public FieldTypes FieldType { get; set; }
        public string Values { get; set; }
    }

    public class EFIATIData
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public string Organizations { get; set; }
        public DateTime Dated { get; set; }
    }

    public class EFSMTPSettings
    {
        [Key]
        public int Id { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string AdminEmail { get; set; }
        public string SenderName { get; set; } = "AIMS";
    }

    public class EFIATISettings
    {
        [Key]
        public int Id { get; set; }
        public string BaseUrl { get; set; }
        public string TransactionTypesJson { get; set; }
    }

    public class EFDropboxSettings
    {
        [Key]
        public string Token { get; set; }
    }

    public class EFManualExchangeRates
    {
        [Key]
        public int Id { get; set; }
        public string Currency { get; set; }
        public int Year { get; set; }
        [Column(TypeName = "decimal(9, 2)")]
        public decimal ExchangeRate { get; set; }
        public string DefaultCurrency { get; set; }
    }

    public class EFExchangeRatesSettings
    {
        public int Id { get; set; }
        public string APIKeyOpenExchangeRates { get; set; }
        public string ManualExchangeRates { get; set; }
        public string ManualExchangeRateSource { get; set; } = null;
        public int FinancialYearStartingMonth { get; set; }
        public int FinancialYearEndingMonth { get; set; }
    }

    public class EFEmailMessages
    {
        [Key]
        public int Id { get; set; }
        public EmailMessageType MessageType { get; set; }
        [MaxLength(100)]
        public string TypeDefinition { get; set; }
        [MaxLength(200)]
        public string Subject { get; set; } = "No subject";
        [MaxLength(1000)]
        public string Message { get; set; } = null;
        public string FooterMessage { get; set; } = null;
    }

    public class EFPasswordRecoveryRequests
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public DateTime Dated { get; set; }
    }

    public class EFFinancialYearSettings
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int Month { get; set; }
        [Required]
        public int Day { get; set; }
    }

    public class EFFinancialYears
    {
        [Key]
        public int Id { get; set; }
        public int FinancialYear { get; set; }
        public string Label { get; set; }
    }

    public class EFProjectDeletionRequests
    {
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public EFProject Project { get; set; }
        [ForeignKey("RequestedBy")]
        public int RequestedById { get; set; }
        public EFUser RequestedBy { get; set; }
        public DateTime RequestedOn { get; set; }
        public DateTime StatusUpdatedOn { get; set; }
        public ProjectDeletionStatus Status { get; set; }
    }

    public class EFIATICountryCodes
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Country { get; set; }
        public bool IsActive { get; set; }
    }

    public class EFHomePageSettings
    {
        public int Id { get; set; }
        public string AIMSTitleBarText { get; set; }
        public string FaviconPath { get; set; }
        public string AIMSTitle { get; set; }
        public string IntroductionHeading { get; set; }
        public string IntroductionText { get; set; }
    }

    public class EFFinancialYearTransition
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public bool IsAutomated { get; set; } = false;
        [ForeignKey("AppliedBy")]
        public int? AppliedById { get; set; }
        public EFUser AppliedBy { get; set; }
        public DateTime AppliedOn { get; set; }
    }

    public class EFDocumentLinks
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }
        [Required]
        public string URL { get; set; }
        public DateTime DatePosted { get; set; }
    }

    public class EFContactMessages
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string SenderName { get; set; }
        [Required]
        [EmailAddress]
        public string SenderEmail { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Message { get; set; }
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public EFProject Project { get; set; }
        public DateTime Dated { get; set; }
        public bool IsNotified { get; set; }
        public ContactEmailType EmailType { get; set; }
    }
}
