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
        ChangedMappingEffectedProject = 5
    }

    public enum DataTransactions
    {
        Inserted = 1,
        Updated = 2,
        Deleted = 3
    }

    public enum GranTypes
    {
        Grant = 1,
        Loan = 2,
        MutuallyExclusive = 3
    }

    public class EFProjectTypes
    {
        [Key]
        public int Id { get; set; }
        public string Type { get; set; }
    }

    public class EFOrganizationTypes
    {
        [Key]
        public int Id { get; set; }
        public string TypeName { get; set; }
    }

    public class EFOrganization
    {
        [Key]
        public int Id { get; set; }
        public string OrganizationName { get; set; }
        public EFOrganizationTypes OrganizationType { get; set; }
    }

    public class EFUser
    {
        [Key]
        public int Id { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public UserTypes UserType { get; set; }
        [ForeignKey("Organization")]
        public int OrganizationId { get; set; }
        public EFOrganization Organization { get; set; }
        [ForeignKey("ApprovedById")]
        public int? ApprovedById { get; set; }
        public EFUser ApprovedBy { get; set; }
        public bool IsApproved { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? DeActivatedOn { get; set; }
    }

    /*
     * The idea to adjust new sector is that for any open projects, if sector name
     * is updated, create a new sector for updated name and reference the open projects
     * with the name and update the old sector id reference
     */
    public class EFSectorTypes
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
    }

    public class EFSector
    {
        [Key]
        public int Id { get; set; }
        public string SectorName { get; set; }
        public EFSectorCategory Category { get; set; }
        public EFSectorSubCategory SubCategory { get; set; }
        public DateTime TimeStamp { get; set; }
    }

    public class EFSectorMappings
    {
        public int Id { get; set; }
        public int NativeSectorId { get; set; }
        public int MappedSectorId { get; set; }
        public int SectorTypeId { get; set; }
    }


    public class EFLocation
    {
        [Key]
        public int Id { get; set; }
        public string Location { get; set; }
        [Column(TypeName = "decimal(9, 2)")]
        public decimal Latitude { get; set; }
        [Column(TypeName = "decimal(9, 2)")]
        public decimal Longitude { get; set; }
    }

    /*
     * Need to work on Envelope data at the service level and need to incorporate in the Projects Table
     */
    public class EFProject
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(100)]
        public string Title { get; set; }
        public string Objective { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public EFProjectTypes ProjectType { get; set; } 
        public ICollection<EFLocation> Locations { get; set; }
        public ICollection<EFProjectDisbursements> Disbursements { get; set; }
        public ICollection<EFProjectFunders> Funders { get; set; }
        public ICollection<EFProjectImplementors> Implementors { get; set; }
        public ICollection<EFProjectDocuments> Documents { get; set; }
    }

    public class EFProjectDisbursements
    {
        public int Id { get; set; }
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public EFProject Project { get; set; }
        public int StartingYear { get; set; }
        public int StartingMonth { get; set; }
        public int EndingYear { get; set; }
        public int EndingMonth { get; set; }
        [Column(TypeName = "decimal(9, 2)")]
        public decimal Percentage { get; set; }
    }

    public class EFProjectSectors
    {
        public int ProjectId { get; set; }
        public EFProject Project { get; set; }
        public int SectorId { get; set; }
        public EFSector Sector { get; set; }
        [Column(TypeName = "decimal(9, 2)")]
        public decimal ContributedAmount { get; set; }
        [Column(TypeName = "decimal(9, 2)")]
        public decimal ExchangeRate { get; set; }
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
        public decimal Percentage { get; set; }
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

    public class EFProjectImplementors
    {
        [ForeignKey("Implementor")]
        public int ImplementorId { get; set; }
        public EFOrganization Implementor { get; set; }
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public EFProject Project { get; set; }
        
    }

    public class EFProjectFundings
    {
        public int Id { get; set; }
        [ForeignKey("Funder")]
        public int FunderId { get; set; }
        public EFOrganization Funder { get; set; }
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public EFProject Project { get; set; }
        public GranTypes GrantType { get; set; }
        [Column(TypeName = "decimal(9 ,2)")]
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        [Column(TypeName = "decimal(9, 2)")]
        public decimal ExchangeRate { get; set; }
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


}
