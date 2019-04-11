using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AIMS.Models
{
    public class ActionResponse
    {
        public int ReturnedId { get; set; } = 0;
        public string Message { get; set; } = "";
        public bool Success { get; set; } = true;
    }

    public class ViewModel
    {
        public string Title { get; set; }
    }

    /// <summary>
    /// Token models
    /// </summary>
    public class TokenModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string OrganizationId { get; set; }
        public int UserType { get; set; }
        public string JwtKey { get; set; }
        public string JwtAudience { get; set; }
        public string JwtIssuer { get; set; }
        public string TokenExpirationDays { get; set; }
    }

    public class PasswordTokenModel
    {
        public string Email { get; set; }
        public DateTime TokenDate { get; set; }
    }

    /// <summary>
    /// Currency models
    /// </summary>
    public class CurrencyView
    {
        public int Id { get; set; }
        public string Currency { get; set; }
        public bool IsDefault { get; set; }
    }

    public class ManualCurrencyRateModel
    {
        [Required]
        public List<CurrencyWithRates> Rates { get; set; }
    }

    public class LocationKeyPreview
    {
        public int LocationId { get; set; }
        public int ProjectId { get; set; }
    }

    public class MappingsKeyView
    {
        public int SectorId { get; set; }
        public int MappingId { get; set; }
    }

    public class FundersKeyView
    {
        public int FunderId { get; set; }
        public int ProjectId { get; set; }
    }

    public class ImplementersKeyView
    {
        public int ImplementerId { get; set; }
        public int ProjectId { get; set; }
    }

    public class SectorsKeyView
    {
        public int SectorId { get; set; }
        public int ProjectId { get; set; }
    }

    public class ExchangeRates
    {
        public string Base { get; set; }
        public string Rates { get; set; }
    }

    public class ExchangeRatesView
    {
        public string Base { get; set; }
        public string Dated { get; set; }
        public List<CurrencyWithRates> Rates { get; set; }
    }

    public class ExchangeRatesSettingsView
    {
        public bool IsAutomatic { get; set; }
        public bool IsOpenExchangeKeySet { get; set; }
    }

    public class ExRateAutoSetting
    {
        [Required]
        public bool IsAutomatic { get; set; }
    }

    public class CurrencyWithRates
    {
        public string Currency { get; set; }
        public decimal Rate { get; set; }
    }

    public class ExchangeRateAPIKeyModel
    {
        [Required]
        public string Key { get; set; }
    }

    public class CurrencyModel
    {
        [Required]
        [MaxLength(3)]
        public string Currency { get; set; }
        public bool IsDefault { get; set; } = false;
    }

    public class ExchangeRatesModel
    {
        public string RatesJson { get; set; }
        public DateTime Dated { get; set; }
    }


    /// <summary>
    /// Organization models
    /// </summary>
    public class OrganizationView
    {
        public int Id { get; set; }
        public string OrganizationName { get; set; }
    }

    public class OrganizationModel
    {
        [Required]
        public string Name { get; set; }
    }

    public class MergeOrganizationModel
    {
        [Required]
        public string NewName { get; set; }
        [Required]
        public List<int> Ids { get; set; }
    }

    public class OrganizationViewModel
    {
        public int Id { get; set; }
        public string OrganizationName { get; set; }
    }

    /// <summary>
    /// Sector models
    /// </summary>

    public class SectorCategoryModel
    {
        public int SectorTypeId { get; set; }
        public string Category { get; set; }
    }

    public class SectorCategoryView
    {
        public int Id { get; set; }
        public int SectorTypeId { get; set; }
        public string SectorType { get; set; }
        public string Category { get; set; }
    }

    public class SectorCategoryViewModel
    {
        public int Id { get; set; }
        public int SectorTypeId { get; set; }
        public string Category { get; set; }
    }

    public class SectorSubCategoryModel
    {
        public int CategoryId { get; set; }
        public string SubCategory { get; set; }
    }

    public class SectorSubCategoryViewModel
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string SubCategory { get; set; }
    }

    public class SectorSubCategoryView
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string SectorCategory { get; set; }
        public string SubCategory { get; set; }
    }

    public class SectorTypesView
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
        public bool? IsDefault { get; set; }
    }

    public class SectorTypesModel
    {
        [Required]
        public string TypeName { get; set; }
        public bool IsDefault { get; set; } = false;
        public bool IsIATIType { get; set; } = false;
    }

    public class SectorDetailedView
    {
        public int Id { get; set; }
        public int SectorTypeId { get; set; }
        public string SectorType { get; set; }
        public string ParentSector { get; set; }
        public string SectorName { get; set; }
        public bool IsDefault { get; set; } = false;
        public bool IsIATIType { get; set; } = false;
    }

    public class SectorView
    {
        public int Id { get; set; }
        public string ParentSector { get; set; }
        public string SectorName { get; set; }
    }

    public class SectorViewModel
    {
        public int Id { get; set; }
        public int SectorTypeId { get; set; }
        public int ParentId { get; set; }
        public string SectorName { get; set; }
    }

    public class SectorModel
    {
        [Required]
        public int SectorTypeId { get; set; }
        public int? ParentId { get; set; }
        [Required]
        public string SectorName { get; set; }
    }

    public class IATINewSectorModel
    {
        public int? ParentId { get; set; }
        [Required]
        public string SectorName { get; set; }
        [Required]
        public int MappingSectorId { get; set; }
    }

    public class SectorMappingsModel
    {
        [Required]
        public int SectorTypeId { get; set; }
        public int SectorId { get; set; }
        public List<int> MappingIds { get; set; }
    }

    public class MappingSectors
    {
        public int SectorTypeId { get; set; }
        public string SectorType { get; set; }
        public List<SectorSimpleView> Sectors { get; set; }
    }

    public class SectorSimpleView
    {
        public int SectorId { get; set; }
        public string Sector { get; set; }
    }

    public class SectorMappingsView
    {
        public string Sector { get; set; }
        public int SectorId { get; set; }
        public IEnumerable<MappingSectors> MappedSectors { get; set; }
    }

    /// <summary>
    /// Organization type models
    /// </summary>
    public class OrganizationTypeView
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
    }

    public class OrganizationTypeModel
    {
        [Required]
        public string TypeName { get; set; }
    }

    /// <summary>
    /// User models
    /// </summary>
    public class UserView
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public UserTypes UserType { get; set; }
        public int OrganizationId { get; set; }
        public string Organization { get; set; }
        public bool IsApproved { get; set; }
        public string RegistrationDate { get; set; }
    }

    public class UserAuthenticationView
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public int OrganizationId { get; set; }
        public UserTypes UserType { get; set; }
    }

    public class UserReturnView
    {
        public string Token { get; set; }
        public int OrganizationId { get; set; }
        public UserTypes UserType { get; set; }
    }

    public class EmailsModel
    {
        public string Email { get; set; }
        public UserTypes UserType { get; set; }
    }

    public class PasswordResetRequest
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
    }

    public class PasswordResetModel
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }

    public class PasswordResetEmailModel
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Token { get; set; }
        [Url]
        [Required]
        public string Url { get; set; }
    }

    public class AuthenticateModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class UserModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public UserTypes UserType { get; set; }
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        //public int OrganizationTypeId { get; set; }
        public bool IsNewOrganization { get; set; }
    }

    public class EditUserOrganization
    {
        public int UserId { get; set; }
        public int OrganizationId { get; set; }
    }

    public class EditUserPassword
    {
        public string Password { get; set; }
    }

    public class UserApprovalModel
    {
        public int ApprovedById { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int NotificationId { get; set; }
    }

    /// <summary>
    /// Location models
    /// </summary>
    public class LocationView
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }

    public class LocationModel
    {
        public string Location { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }

    /// <summary>
    /// Project type models
    /// </summary>
    public class ProjectTypesView
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
    }

    public class ProjectTypesModel
    {
        [Required]
        public string TypeName { get; set; }
    }

    /// <summary>
    /// Project models
    /// </summary>
    public class ProjectView
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public DateTime DateUpdated { get; set; }
    }

    public class ProjectModelView
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public ICollection<ProjectFunderView> Funders { get; set; }
        public ICollection<ProjectImplementerView> Implementers { get; set; }
        public ICollection<SectorView> Sectors { get; set; }
        public ICollection<LocationView> Locations { get; set; }
        public ICollection<ProjectDisbursementView> Disbursements { get; set; }
        public ICollection<ProjectDocumentView> Documents { get; set; }
    }

    public class ProjectProfileView
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public decimal ProjectCost { get; set; }
        public decimal ActualDisbursements { get; set; }
        public decimal PlannedDisbursements { get; set; }
        public ICollection<ProjectFunderView> Funders { get; set; }
        public ICollection<ProjectImplementerView> Implementers { get; set; }
        public ICollection<ProjectSectorView> Sectors { get; set; }
        public ICollection<ProjectLocationDetailView> Locations { get; set; }
        public ICollection<ProjectDisbursementView> Disbursements { get; set; }
        public ICollection<ProjectDocumentView> Documents { get; set; }
    }

    public class ProjectModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ProjectTypeId { get; set; }
    }

    public class MergeProjectsModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public List<int> ProjectsIds { get; set; }
    }

    /// <summary>
    /// Project funders models
    /// </summary>
    public class ProjectFunderView
    {
        public int FunderId { get; set; }
        public string Funder { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public decimal ExchangeRate { get; set; }
    }

    public class ProjectFunderModel
    {
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public int FunderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public decimal ExchangeRate { get; set; }
    }

    /// <summary>
    /// Project locations models
    /// </summary>
    public class ProjectLocationView
    {
        public int Id { get; set; }
        public string Project { get; set; }
        public string Location { get; set; }
        public decimal FundsPercentage { get; set; }
    }

    public class ProjectLocationDetailView
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal FundsPercentage { get; set; }
    }

    public class ProjectLocationModel
    {
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public int LocationId { get; set; }
        [Required]
        public decimal FundsPercentage { get; set; }
    }

    /// <summary>
    /// project sectors models
    /// </summary>
    public class ProjectSectorView
    {
        public int SectorId { get; set; }
        public string Sector { get; set; }
        public decimal FundsPercentage { get; set; }
    }

    public class ProjectSectorModel
    {
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public int SectorId { get; set; }
        [Required]
        public decimal FundsPercentage { get; set; }
    }


    /// <summary>
    /// Project implementers models
    /// </summary>
    public class ProjectImplementerView
    {
        public int ImplementerId { get; set; }
        public string Implementer { get; set; }
    }

    public class ProjectImplementerModel
    {
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public int ImplementerId { get; set; }
    }


    /// <summary>
    /// Project funding models
    /// </summary>
    public class ProjectFundsView
    {
        public int FunderId { get; set; }
        public string Funder { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public decimal ExchangeRate { get; set; }
    }

    public class ProjectFundsModel
    {
        public int FunderId { get; set; }
        public int ProjectId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public decimal ExchangeRate { get; set; }
    }

    /// <summary>
    /// Project document model
    /// </summary>
    public class ProjectDocumentView
    {
        public int Id { get; set; }
        public string Project { get; set; }
        public string DocumentTitle { get; set; }
        public string DocumentUrl { get; set; }
    }

    public class ProjectDocumentModel
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Project { get; set; }
        public string DocumentTitle { get; set; }
        public string DocumentUrl { get; set; }
    }

    /// <summary>
    /// Project disbursements models
    /// </summary>
    public class ProjectDisbursementView
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Dated { get; set; }
        public decimal Amount { get; set; }
    }

    public class ProjectDisbursementModel
    {
        public int ProjectId { get; set; }
        public DateTime Dated { get; set; }
        public decimal Amount { get; set; }
    }


    /// <summary>
    /// Project markers models
    /// </summary>
    public class ProjectMarkersView
    {
        public int Id { get; set; }
        public string Project { get; set; }
        public string Marker { get; set; }
        public decimal Percentage { get; set; }
    }

    public class ProjectMarkersModel
    {
        public int ProjectId { get; set; }
        public string Marker { get; set; }
        public decimal Percentage { get; set; }
    }


    /// <summary>
    /// Notifications models
    /// </summary>
    public class NotificationView
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public int TreatmentId { get; set; }
        public string Dated { get; set; }
        public NotificationTypes NotificationType { get; set; }
    }

    /// <summary>
    /// Error models
    /// </summary>
    public class ErrorModel
    {
        public string Error { get; set; }
    }

    /// <summary>
    /// Success models
    /// </summary>
    public class SuccessModel
    {
        public string Message { get; set; }
    }

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

    public class ProjectInfo
    {
        public string ProjectTitle { get; set; }
    }

    public class TransactionTypes
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class FinancialYearView
    {
        public int Id { get; set; }
        public int FinancialYear { get; set; }
    }

    public class FinancialYearModel
    {
        [Required]
        public int FinancialYear { get; set; }
    }

    public class NotificationModel
    {
        public UserTypes UserType { get; set; }
        public int OrganizationId { get; set; }
        public string Message { get; set; }
        public int TreatmentId { get; set; }
        public NotificationTypes NotificationType { get; set; }
    }

    /*public class IATIDBCountry
    {
        public string Code { get; set; }
        public string ContributionPercentage { get; set; }
    }

    public class IATIDBRegion
    {
        public string Code { get; set; }
        public string ContributionPercentage { get; set; }
    }

    public class IATIDBSector
    {
        public string Code { get; set; }
        public string SectorName { get; set; }
        public string FundsPercentage { get; set; }
    }

    public class IATIDBActivity
    {
        public int Id { get; set; }
        public string Identifier { get; set; }
        public string DefaultCurrency { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ICollection<IATIDBSector> Sectors { get; set; }
        public ICollection<IATIDBCountry> Countries { get; set; }
        public ICollection<IATIDBRegion> Regions { get; set; }
        public ICollection<IATIDBOrganization> ParticipatingOrganizations { get; set; }
        public ICollection<IATIDBTransaction> Transactions { get; set; }
    }

    public class IATIDBBudget
    {
        public string PeriodStart { get; set; }
        public string PeriodEnd { get; set; }
        public string PlannedAmount { get; set; }
    }

    public class IATIDBOrganization
    {
        public string Project { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
    }

    public class IATIDBTransaction
    {
        public string AidType { get; set; }
        public string TransactionType { get; set; }
        public string Currency { get; set; }
        public string Amount { get; set; }
        public string Dated { get; set; }
        public string Description { get; set; }
    }

    public class IATIDBModel
    {
        [Required]
        public string Data { get; set; }
        public string Organizations { get; set; }
    }*/

    /// <summary>
    /// SMTP settings model
    /// </summary>
    public class SMTPSettingsView
    {
        public int Id { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string AdminEmail { get; set; }
    }

    public class SMTPSettingsModelView
    {
        public int Id { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string AdminEmail { get; set; }
    }

    public class SMTPSettingsModel
    {
        public string Host { get; set; }
        public int Port { get; set; } = 0;
        public string Username { get; set; }
        public string Password { get; set; }
        public string AdminEmail { get; set; }
    }

    public class IATISettings
    {
        public string BaseUrl { get; set; }
    }

    public class DeleteAccountModel
    {
        [Required]
        public string Password { get; set; }
    }

    /// <summary>
    /// Models for Reports
    /// </summary>
    public class Report
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Dated { get; set; }
        public string Footer { get; set; }
    }

    public class ProjectReport
    {
        public Report ReportSettings { get; set; }
        public List<ProjectView> Projects { get; set; }
    }

    public class ProjectProfileReport
    {
        public Report ReportSettings { get; set; }
        public ProjectProfileView ProjectProfile { get; set; }
    }

    public class FilteredProjectProfileReport
    {
        public Report ReportSettings { get; set; }
        public ICollection<ProjectProfileView> ProjectsList { get; set; }
    }

    public class ProjectProfileReportBySector
    {
        public Report ReportSettings { get; set; } 
        public IEnumerable<ProjectsBySector> SectorProjectsList { get; set; }
    }

    public class ProjectsBySector
    {
        public string SectorName { get; set; }
        public decimal TotalCost { get; set; }
        public IEnumerable<ProjectProfileView> Projects { get; set; }
    }

    public class SectorProjects
    {
        public int SectorId { get; set; }
        public string SectorName { get; set; }
        public List<int> ProjectIds { get; set; }
    }

    public class SearchProjectModel
    {
        public string Title { get; set; } = null;
        public List<int> OrganizationIds { get; set; } = new List<int>();
        public int StartingYear { get; set; } = 0;
        public int EndingYear { get; set; } = 0;
        public List<int> SectorIds { get; set; } = new List<int>();
        public List<int> LocationIds { get; set; } = new List<int>();
    }

    public class IATIByIdModel
    {
        public string Identifier { get; set; }
    }

    public class ReportModelForProjectSectors
    {
        public int Year { get; set; } = 0;
        public List<int> SectorIds { get; set; } = null;
    }

    public class NotificationUpdateModel
    {
        public List<int> Ids { get; set; }
    }

    public class ReportNameView
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }

    public class ReportSubscriptionModel
    {
        public List<int> ReportIds { get; set; }
    }

    public class ReportSubscriptionView
    {
        public int ReportId { get; set; }
    }
}
