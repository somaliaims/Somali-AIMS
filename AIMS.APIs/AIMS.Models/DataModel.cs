using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIMS.Models
{
    public enum UserTypes
    {
        Manager = 1,
        Standard = 2
    }

    public enum FieldTypes
    {
        DropDown = 1,
        CheckBox = 2,
        Text = 3,
    }

    public class EFProjectTypes
    {
        [Key]
        public int Id { get; set; }
        public string ProjectType { get; set; }
    }

    public class EFTimeIntervals
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public int DurationInMonths { get; set; }
    }

    public class EFOrganization
    {
        [Key]
        public int Id { get; set; }
        public string OrganizationName { get; set; }
    }

    public class EFUser
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }
        [EmailAddress]
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
        public UserTypes UserType { get; set; }
        [ForeignKey("Organization")]
        public int OrganizationId { get; set; }
        public EFOrganization Organization { get; set; }
        public bool IsApproved { get; set; }
        public DateTime RegistrationDate { get; set; }
    }

    /*
     * The idea to adjust new sector is that for any open projects, if sector name
     * is updated, create a new sector for updated name and reference the open projects
     * with the name and update the old sector id reference
     */
    public class EFSector
    {
        [Key]
        public int Id { get; set; }
        public string SectorName { get; set; }
    }

    

    public class EFLocation
    {
        [Key]
        public int Id { get; set; }
        public string Location { get; set; }
        public int Latitude { get; set; }
        public int Longitude { get; set; }
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
        public DateTime DateStarted { get; set; }
        public DateTime DateEnded { get; set; }
        public EFProjectTypes ProjectType { get; set; } 
        public ICollection<EFLocation> Locations { get; set; }
    }

    public class EFProjectSectors
    {
        public int ProjectId { get; set; }
        public EFProject Project { get; set; }
        public int SectorId { get; set; }
        public EFSector Sector { get; set; }
        [Column(TypeName = "decimal(9, 2)")]
        public decimal ContributedAmount { get; set; }
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

    public class EFProjectFundings
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public EFProject Project { get; set; }
        [Column(TypeName = "decimal(9 ,2)")]
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        [Column(TypeName = "decimal(9, 2)")]
        public decimal ExchangeRate { get; set; }
    }

    public class EFUserSubscriptions
    {
        [ForeignKey("User")]
        public int UserId { get; set; }
        public EFUser User { get; set; }
        [ForeignKey("Report")]
        public int ReportId { get; set; }
        public EFStaticReports Report { get; set; }
        [ForeignKey("TimeInterval")]
        public int IntervalId { get; set; }
        public EFTimeIntervals TimeInterval { get; set; }
        public DateTime DateSubscribed { get; set; }
        public DateTime ReportSentOn { get; set; }
        public bool IsActive { get; set; }
    }    
    
    /*public class EFOrganizationUsers
    {
        [ForeignKey("Organization")]
        public int OrganizationId { get; set; }
        public EFOrganization Organization { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public EFUser User { get; set; }
        public DateTime RegistrationDate { get; set; }
    }*/

    public class EFUserNotifications
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Organization")]
        public int OrganizationId { get; set; }
        public EFOrganization Organization { get; set; }
        public string Message { get; set; }
        public DateTime Dated { get; set; }
        public bool IsSeen { get; set; }
    }

    public class EFProjectLogs
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public EFProject Project { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public EFUser UpdatedBy { get; set; }
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
    }

    public class EFProjectCustomFields
    {
        [Key]
        public int Id { get; set; }
        public int ProjectId { get; set; }
    }

    /*public class EFEntity
    {

    }*/

}
