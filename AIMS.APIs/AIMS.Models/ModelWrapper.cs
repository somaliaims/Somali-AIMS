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
    /// Organization models
    /// </summary>
    public class OrganizationView
    {
        public int Id { get; set; }
        public string OrganizationName { get; set; }
        public string TypeName { get; set; }
    }

    public class OrganizationModel
    {
        [Required]
        public string Name { get; set; }
        public int TypeId { get; set; }
    }

    public class OrganizationViewModel
    {
        public int Id { get; set; }
        public string OrganizationName { get; set; }
        public int OrganizationTypeId { get; set; }
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
    }

    public class SectorTypesModel
    {
        [Required]
        public string TypeName { get; set; }
    }

    public class SectorView
    {
        public int Id { get; set; }
        public string SectorType { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string SectorName { get; set; }
    }

    public class SectorViewModel
    {
        public int Id { get; set; }
        public int SectorTypeId { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public string SectorName { get; set; }
    }

    public class SectorModel
    {
        [Required]
        public int SectorTypeId { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int SubCategoryId { get; set; }
        [Required]
        public string SectorName { get; set; }
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
        public string Name { get; set; }
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
        public string Name { get; set; }
        public string Email { get; set; }
        public int OrganizationId { get; set; }
        public UserTypes UserType { get; set; }
    }

    public class UserReturnView
    {
        public string Name { get; set; }
        public string Token { get; set; }
        public int OrganizationId { get; set; }
        public UserTypes UserType { get; set; }
    }

    public class EmailsModel
    {
        public string Email { get; set; }
        public string UserName { get; set; }
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
        public string Name { get; set; }
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
        public int UserId { get; set; }
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
        public string ProjectType { get; set; }
    }

    public class ProjectModelView
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int ProjectTypeId { get; set; }
        public ICollection<ProjectFunderView> Funders { get; set; }
        public ICollection<ProjectImplementorView> Implementers { get; set; }
        public ICollection<SectorView> Sectors { get; set; }
        public ICollection<LocationView> Locations { get; set; }
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
        public decimal FundPercentage { get; set; }
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
        public string Currency { get; set; }
        public decimal ExchangeRate { get; set; }
    }

    public class ProjectSectorModel
    {
        public int ProjectId { get; set; }
        public int SectorId { get; set; }
        public decimal FundsPercentage { get; set; }
        public string Currency { get; set; }
        public decimal ExchangeRate { get; set; }
    }


    /// <summary>
    /// Project implementors models
    /// </summary>
    public class ProjectImplementorView
    {
        public int ImplementorId { get; set; }
        public string Implementor { get; set; }
    }

    public class ProjectImplementorModel
    {
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public int ImplementorId { get; set; }
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
        public int StartingYear { get; set; }
        public int StartingMonth { get; set; }
        public int EndingYear { get; set; }
        public int EndingMonth { get; set; }
        public decimal Percentage { get; set; }
    }

    public class ProjectDisbursementModel
    {
        public int ProjectId { get; set; }
        public int StartingYear { get; set; }
        public int StartingMonth { get; set; }
        public int EndingYear { get; set; }
        public int EndingMonth { get; set; }
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
        public string Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string AdminEmail { get; set; }
    }

    public class SMTPSettingsModel
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string AdminEmail { get; set; }
    }

}
