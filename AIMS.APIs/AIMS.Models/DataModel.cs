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

    public enum FieldTypes
    {
        DropDown = 1,
        CheckBox = 2,
        Text = 3,
    }

    public enum NotificationTypes
    {
        NewUser = 1,
        NewProjectToOrg = 2,
        NewIATIToProject = 3,
        UserInactive = 4,
        ChangedMappingEffectedProject = 5,
        NewIATISector = 6
    }

    public enum DataTransactions
    {
        Inserted = 1,
        Updated = 2,
        Deleted = 3
    }

    public enum GrantTypes
    {
        Grant = 1,
        Loan = 2,
        MutuallyExclusive = 3
    }

    public class EFOrganizationTypes
    {
        [Key]
        public int Id { get; set; }
        public string TypeName { get; set; }
    }

    public class EFSectorTypes
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
        public bool? IsDefault { get; set; }
        public bool? IsIATIType { get; set; }
        public IEnumerable<EFSector> Sectors { get; set; }
    }

    public class EFOrganization
    {
        [Key]
        public int Id { get; set; }
        public string OrganizationName { get; set; }
        public bool IsApproved { get; set; }
        public DateTime RegisteredOn { get; set; }
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
        [ForeignKey("ApprovedById")]
        public int? ApprovedById { get; set; }
        public EFUser ApprovedBy { get; set; }
        public bool IsApproved { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? LastLogin { get; set; }
    }

    /*
     * The idea to adjust new sector is that for any open projects, if sector name
     * is updated, create a new sector for updated name and reference the open projects
     * with the name and update the old sector id reference
     */
    /*public class EFSectorTypes
    {
        [Key]
        public int Id { get; set; }
        public string TypeName { get; set; }
        public ICollection<EFSectorCategory> SectorCategories { get; set; }
    }

    public class EFSectorCategory
    {
        [Key]
        public int Id { get; set; }
        public string Category { get; set; }
        [ForeignKey("SectorType")]
        public int SectorTypeId { get; set; }
        public EFSectorTypes SectorType { get; set; }
    }

    public class EFSectorSubCategory
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("SectoryCategory")]
        public int SectorCategoryId { get; set; }
        public EFSectorCategory SectorCategory { get; set; }
        public string SubCategory { get; set; }
    }*/

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
        public DateTime TimeStamp { get; set; }
    }

    public class EFSectorMappings
    {
        public int SectorId { get; set; }
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
        public ICollection<EFProjectSectors> Sectors { get; set; }
        public ICollection<EFProjectLocations> Locations { get; set; }
        public ICollection<EFProjectDisbursements> Disbursements { get; set; }
        public ICollection<EFProjectFunders> Funders { get; set; }
        public ICollection<EFProjectImplementers> Implementers { get; set; }
        public ICollection<EFProjectDocuments> Documents { get; set; }
        public DateTime DateUpdated { get; set; }
    }

    public class EFProjectDisbursements
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public EFProject Project { get; set; }
        public DateTime Dated { get; set; }
        [Column(TypeName = "decimal(9, 2)")]
        public decimal Amount { get; set; }
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

    public class EFProjectMarkers
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public EFProject Project { get; set; }
        public string Marker { get; set; }
        [Column(TypeName = "decimal(9, 2)")]
        public decimal Percentage { get; set; }
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
        [Column(TypeName = "decimal(9, 2)")]
        public decimal FundsPercentage { get; set; }
    }

    public class EFProjectFunders
    {
        [ForeignKey("Funder")]
        public int FunderId { get; set; }
        public EFOrganization Funder { get; set; }
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public EFProject Project { get; set; }
        [Column(TypeName = "decimal(9 ,2)")]
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        [Column(TypeName = "decimal(9, 2)")]
        public decimal ExchangeRate { get; set; }
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

    public class EFUserNotifications
    {
        [Key]
        public int Id { get; set; }
        public UserTypes UserType { get; set; }
        [ForeignKey("Organization")]
        public int OrganizationId { get; set; }
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
        public bool IsDefault { get; set; }
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

    public class EFCustomFields
    {
        [Key]
        public int Id { get; set; }
        public string FieldTitle { get; set; }
        public FieldTypes FieldType { get; set; }
        public DateTime ActiveFrom { get; set; }
        public DateTime ActiveUpto { get; set; }
        public ICollection<EFProjectCustomFields> ProjectFieldsList { get; set; }
    }

    public class EFProjectCustomFields
    {
        public int ProjectId { get; set; }
        public EFProject Project { get; set; }
        [ForeignKey("CustomField")]
        public int CustomFieldId { get; set; }
        public EFCustomFields CustomField { get; set; }
        public string Value { get; set; }
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
    }

    public class EFIATISettings
    {
        [Key]
        public int Id { get; set; }
        public string BaseUrl { get; set; }
    }

    public class EFExchangeRatesSettings
    {
        public int Id { get; set; }
        public bool IsAutomatic { get; set; }
        public string ManualExchangeRates { get; set; }
    }

    public class EFPasswordRecoveryRequests
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public DateTime Dated { get; set; }
    }

    public class EFFinancialYears
    {
        [Key]
        public int Id { get; set; }
        public int FinancialYear { get; set; }
    }
}
