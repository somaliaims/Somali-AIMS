using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AIMS.Models
{
    public enum ContactEmailType
    {
        Help = 1,
        Information = 2
    }

    public enum ProjectSuggestionType
    {
        AddData = 1,
        EditData = 2,
        AmendData = 3
    };

    public enum NoLocationOptions
    {
        ProjectsWithLocations = 1,
        ProjectsWithoutLocations = 2
    }

    public enum NoSectorOptions
    {
        ProjectsWithSectors = 1,
        ProjectsWithoutSectors = 2
    }

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

    public class DefaultCurrencyView
    {
        public int Id { get; set; }
        public string Currency { get; set; }
        public string CurrencyName { get; set; }
    }

    public class EmailModel
    {
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Message { get; set; }
        public string FooterMessage { get; set; } = null;
        [Required]
        public List<EmailAddress> EmailsList { get; set; }
    }

    public class EmailSimpleModel
    {
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Message { get; set; }
    }

    public class FundingTypeModel
    {
        [Required]
        public string FundingType { get; set; }
    }

    public class FundingTypeView
    {
        public int Id { get; set; }
        public string FundingType { get; set; }
    }

    /// <summary>
    /// Token models
    /// </summary>
    public class TokenModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string OrganizationId { get; set; }
        public string OrganizationName { get; set; }
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
        public string CurrencyName { get; set; }
        public bool IsDefault { get; set; }
        public bool IsNational { get; set; }
        public CurrencySource Source { get; set; }
    }

    public class ManualRateModel
    {
        [Required]
        public decimal ExchangeRate { get; set; }
        [Required]
        public string Currency { get; set; }
        [Required]
        public string DefaultCurrency { get; set; }
        [Required]
        public int Year { get; set; }
    }

    public class ManualRatesView
    {
        public int Id { get; set; }
        public string Currency { get; set; }
        public string DefaultCurrency { get; set; }
        public decimal ExchangeRate { get; set; }
        public int Year { get; set; }
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

    public class ExchangeRatesUsageModel
    {
        public ExchangeRateSources Source { get; set; }
        public ExchangeRateUsageSection UsageSection { get; set; }
        public int Order { get; set; }
    }

    public class ExchangeRatesUsageView
    {
        public int Id { get; set; }
        public string Source { get; set; }
        public string UsageSection { get; set; }
        public int Order { get; set; }
    }

    public class ExchangeRatesView
    {
        public string Dated { get; set; }
        public List<CurrencyWithRates> Rates { get; set; }
    }

    public class ExchangeRateForCurrency
    {
        public string Currency { get; set; }
        public decimal ExchangeRate { get; set; }
        public string Dated { get; set; }
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ExRateFinderModel
    {
        [Required]
        public DateTime Dated { get; set; }
    }

    public class CurrencyNamesView
    {
        public string Code { get; set; }
        public string Currency { get; set; }
    }

    public class ExchangeRatesSettingsView
    {
        public bool IsAutomatic { get; set; }
        public bool IsOpenExchangeKeySet { get; set; }
        public string ManualExchangeRateSource { get; set; }
        public IEnumerable<CurrencyWithRates> ManualCurrencyRates { get; set; }
    }

    public class ExRateAutoSetting
    {
        [Required]
        public bool IsAutomatic { get; set; }
    }

    public class CurrencyWithRates
    {
        public string Currency { get; set; }
        public string CurrencyName { get; set; }
        public decimal Rate { get; set; }
    }

    public class CurrencyWithNames
    {
        public string Code { get; set; }
        public string Currency { get; set; }
    }

    public class ExchangeRateAPIKeyModel
    {
        [Required]
        public string Key { get; set; }
    }

    public class ManualExRateSourceModel
    {
        [Required]
        public string Label { get; set; }
    }

    public class CurrencyModel
    {
        [Required]
        public string CurrencyName { get; set; }
        [Required]
        [MaxLength(3)]
        public string Currency { get; set; }
        public bool IsDefault { get; set; } = false;
        public bool IsNational { get; set; } = false;
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
        public int OrganizationTypeId { get; set; }
        public string OrganizationType { get; set; }
        public string OrganizationName { get; set; }
    }

    public class IATIOrganizationView
    {
        public int Id { get; set; }
        public string OrganizationName { get; set; }
    }

    public class OrganizationAbstractView
    {
        public string Name { get; set; }
    }

    public class DisbursementAbstractView
    {
        public int Year { get; set; }
        public decimal Amount { get; set; }
        public string DisbursementType { get; set; }
        public decimal Disbursement { get; set; }
    }

    public class MarkerAbstractView
    {
        public int MarkerId { get; set; }
        public FieldTypes MarkerType { get; set; }
        public string Marker { get; set; }
        public string Values { get; set; }
    }

    public class DocumentAbstractView
    {
        public string DocumentTitle { get; set; }
        public string DocumentUrl { get; set; }
    }


    public class LocationAbstractView
    {
        public string Name { get; set; }
        public decimal FundsPercentage { get; set; }
    }

    public class SectorAbstractView
    {
        public string Name { get; set; }
        public string ParentSector { get; set; }
        public decimal FundsPercentage { get; set; }
    }

    public class OrganizationModel
    {
        [Required]
        public int OrganizationTypeId { get; set; }
        [Required]
        public string Name { get; set; }
    }

    public class SourceOrganizationModel
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
        public int OrganizationTypeId { get; set; }
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
        public bool? IsPrimary { get; set; }
        public bool? IsSourceType { get; set; }
    }

    public class SectorTypesModel
    {
        [Required]
        public string TypeName { get; set; }
        public bool IsPrimary { get; set; } = false;
        public bool IsSourceType { get; set; } = false;
    }

    public class SectorDetailedView
    {
        public int Id { get; set; }
        public int SectorTypeId { get; set; }
        public string SectorType { get; set; }
        public string ParentSector { get; set; }
        public string SectorName { get; set; }
        public bool IsDefault { get; set; } = false;
        public bool IsSourceType { get; set; } = false;
    }

    public class SectorView
    {
        public int Id { get; set; }
        public string ParentSector { get; set; }
        public string SectorName { get; set; }
    }

    public class SectorViewWithParent
    {
        public int Id { get; set; }
        public int ParentSectorId { get; set; }
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

    public class SearchSectorMappingModel
    {
        [Required]
        [MinLength(2)]
        public string Sector { get; set; }
    }

    public class ProjectHelpEmail
    {
        [Required]
        public string SenderName { get; set; }
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public string ProjectTitle { get; set; }
        [EmailAddress]
        public string SenderEmail { get; set; }
        [MaxLength(800)]
        public string Message { get; set; }
    }

    public class ContactEmailRequestModel
    {
        [Required]
        public ContactEmailType EmailType { get; set; }
        [Required]
        public string SenderName { get; set; }
        public int? ProjectId { get; set; } = 0;
        public string ProjectTitle { get; set; } = null;
        [EmailAddress]
        public string SenderEmail { get; set; }
        [Required]
        [MaxLength(100)]
        public string Subject { get; set; }
        [MaxLength(800)]
        public string Message { get; set; }

    }

    public class MappingSectorModel
    {
        public int? ParentId { get; set; }
        [Required]
        public int SectorTypeId { get; set; }
        public int? SectorId { get; set; } = 0;
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
        public string OrganizationName { get; set; }
        public UserTypes UserType { get; set; }
        public bool IsApproved { get; set; }
    }

    public class UserReturnView
    {
        public string Token { get; set; }
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public UserTypes UserType { get; set; }
        public bool IsApproved { get; set; }
    }

    public class EmailsModel
    {
        public string Email { get; set; }
        public UserTypes UserType { get; set; }
    }

    public class EmailAddress
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
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
        public string LoginUrl { get; set; }
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
        public int LocationId { get; set; }
        public string Location { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal FundsPercentage { get; set; }
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
        public string StartingFinancialYear { get; set; }
        public string EndingFinancialYear { get; set; }
        public string DateUpdated { get; set; }
    }

    public class ProjectTitle
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }

    public class LatestProjectView
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal ProjectCost { get; set; }
        public string StartingFinancialYear { get; set; }
        public string EndingFinancialYear { get; set; }
    }

    public class ProjectAbstractView
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal ProjectValue { get; set; }
        public string ProjectCurrency { get; set; }
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string StartingFinancialYear { get; set; }
        public string EndingFinancialYear { get; set; }
        public IEnumerable<SectorAbstractView> Sectors { get; set; }
        public IEnumerable<LocationAbstractView> Locations { get; set; }
        public IEnumerable<OrganizationAbstractView> Organizations { get; set; }
    }

    public class ProjectReportView
    {
        public int StartingFinancialYear { get; set; }
        public int EndingFinancialYear { get; set; }
        public List<ProjectDetailSectorView> Sectors { get; set; }
        public List<ProjectDetailLocationView> Locations { get; set; }
        public List<ProjectDetailMarkerView> Markers { get; set; }
        public List<ProjectDetailView> Projects { get; set; }
    }

    public class ProjectDetailSectorView
    {
        public int Id { get; set; }
        public string ParentSector { get; set; }
        public string Sector { get; set; }
    }

    public class ProjectDetailLocationView
    {
        public int Id { get; set; }
        public string Location { get; set; }
    }

    public class ProjectDetailMarkerView
    {
        public int Id { get; set; }
        public string Marker { get; set; }
    }

    public class ProjectDetailView
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int StartingFinancialYear { get; set; }
        public int EndingFinancialYear { get; set; }
        public string ProjectCurrency { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal ProjectValue { get; set; }
        public ICollection<SectorAbstractView> Sectors { get; set; }
        public ICollection<LocationAbstractView> Locations { get; set; }
        public ICollection<OrganizationAbstractView> Funders { get; set; }
        public ICollection<OrganizationAbstractView> Implementers { get; set; }
        public ICollection<DocumentAbstractView> Documents { get; set; }
        public ICollection<DisbursementAbstractView> Disbursements { get; set; }
        public ICollection<MarkerAbstractView> Markers { get; set; }
    }

    public class EnvelopeTypeModel
    {
        [Required]
        [MaxLength(50)]
        public string TypeName { get; set; }
    }

    public class EnvelopeTypeView
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
    }

    public class EnvelopeModel
    {
        [Required]
        [MaxLength(3)]
        public string Currency { get; set; }
        [Required]
        public decimal ExchangeRate { get; set; }
        public IEnumerable<EnvelopeYearlyBreakupModel> EnvelopeBreakups { get; set; }
    }

    public class EnvelopeYearlyBreakupModel
    {
        public int EnvelopeTypeId { get; set; }
        public int Year { get; set; }
        public decimal Amount { get; set; }
    }

    public class EnvelopeYearlyView
    {
        public int FunderId { get; set; }
        public string Funder { get; set; }
        public string Currency { get; set; }
        public decimal ExchangeRate { get; set; }
        public List<EnvelopeBreakupView> EnvelopeBreakupsByType { get; set; }
    }

    public class EnvelopeBreakupView
    {
        public int EnvelopeTypeId { get; set; }
        public string EnvelopeType { get; set; }
        public List<EnvelopeYearlyBreakUp> YearlyBreakup { get; set; }
    }

    public class EnvelopeYearlyBreakUp
    {
        public int Year { get; set; }
        public decimal Amount { get; set; }
    }

    public class FundBreakup
    {
        [Required]
        public int Year { get; set; }
        [Required]
        public decimal ActualAmount { get; set; }
        public decimal ExpectedAmount { get; set; }
        public decimal ManualAmount { get; set; }
        public string SectorsAmountBreakup { get; set; }
    }

    public class EnvelopeView
    {
        public int FunderId { get; set; }
        public string FunderName { get; set; }
        public string Currency { get; set; }
        public decimal ExchangeRate { get; set; }
        public IEnumerable<EnvelopeBreakupView> YearlyBreakups { get; set; }
    }

    public class ProjectMiniView
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class EnvelopeBreakup
    {
        public int Year { get; set; }
        public decimal ActualAmount { get; set; }
        public decimal ExpectedAmount { get; set; }
        public decimal ManualAmount { get; set; }
    }

    public class EnvelopeSectorBreakup
    {
        public int SectorId { get; set; }
        public string Sector { get; set; }
        public decimal Percentage { get; set; }
        public IEnumerable<SectorYearlyAllocation> YearlyAllocation { get; set; }
    }

    public class SectorYearlyAllocation
    {
        public int Year { get; set; }
        public decimal Amount { get; set; }
        public decimal ExpectedAmount { get; set; }
        public decimal ManualAmount { get; set; }
    }

    public class ProjectModelView
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string StartingFinancialYear { get; set; }
        public string EndingFinancialYear { get; set; }
        public decimal ProjectValue { get; set; }
        public decimal ExchangeRate { get; set; }
        public string ProjectCurrency { get; set; }
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
        public int FundingTypeId { get; set; }
        public string FundingType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string StartingFinancialYear { get; set; }
        public string EndingFinancialYear { get; set; }
        public decimal ProjectValue { get; set; }
        public decimal ProjectPercentValue { get; set; }
        public string ProjectCurrency { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal ActualDisbursements { get; set; }
        public decimal PlannedDisbursements { get; set; }
        public ICollection<ProjectFunderView> Funders { get; set; }
        public ICollection<ProjectImplementerView> Implementers { get; set; }
        public ICollection<ProjectSectorView> Sectors { get; set; }
        public ICollection<ProjectLocationDetailView> Locations { get; set; }
        public ICollection<ProjectDisbursementView> Disbursements { get; set; }
        public ICollection<ProjectDocumentView> Documents { get; set; }
        public ICollection<ProjectMarkersView> Markers { get; set; }
    }

    public class ProjectModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [Required]
        public int StartingFinancialYear { get; set; }
        [Required]
        public int EndingFinancialYear { get; set; }
        [Required]
        [Column(TypeName = "decimal(11,2)")]
        public decimal ProjectValue { get; set; }
        [Column(TypeName = "decimal(9,2)")]
        public decimal ExchangeRate { get; set; }
        [Required]
        public string ProjectCurrency { get; set; }
        public int FundingTypeId { get; set; }
    }

    public class MergeProjectsModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        [Column(TypeName = "decimal(11,2)")]
        public decimal ProjectValue { get; set; }
        [Required]
        public string ProjectCurrency { get; set; }
        public decimal ExchangeRate { get; set; }
        public int FundingTypeId { get; set; }
        public string Description { get; set; }
        [Required]
        public int StartingFinancialYear { get; set; }
        [Required]
        public int EndingFinancialYear { get; set; }
        [Required]
        public List<int> ProjectIds { get; set; }
        public List<int> FunderIds { get; set; }
        public List<int> ImplementerIds { get; set; }
        public List<DocumentModel> Documents { get; set; }
        public List<MergeProjectSectorModel> Sectors { get; set; }
        public List<MergeProjectLocationModel> Locations { get; set; }
        public List<MergeProjectDisbursementsModel> Disbursements { get; set; }
        public List<MergeProjectMarkerModel> Markers { get; set; }
    }

    public class MergeProjectSectorModel
    {
        public int SectorId { get; set; }
        public decimal FundsPercentage { get; set; }
    }

    public class MergeProjectLocationModel
    {
        public int LocationId { get; set; }
        public decimal FundsPercentage { get; set; }
    }

    public class MergeProjectDisbursementsModel
    {
        public int Year { get; set; }
        public decimal Amount { get; set; }
        public DisbursementTypes DisbursementType { get; set; }
    }

    public class MergeProjectMarkerModel
    {
        public int MarkerId { get; set; }
        public FieldTypes FieldType { get; set; }
        public string Values { get; set; }
    }


    /// <summary>
    /// Project funders models
    /// </summary>
    public class ProjectFunderView
    {
        public int FunderId { get; set; }
        public string Funder { get; set; }
    }

    public class ProjectFunderModel
    {
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public List<int> FunderIds { get; set; }
    }

    public class ProjectFunderSourceModel
    {
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public List<string> Funders { get; set; }
    }

    public class ProjectImplementerSourceModel
    {
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public List<string> Implementers { get; set; }
    }

    /// <summary>
    /// Project locations models
    /// </summary>
    public class ProjectLocationView
    {
        public int LocationId { get; set; }
        public string Project { get; set; }
        public string Location { get; set; }
        public decimal FundsPercentage { get; set; }
    }

    public class ProjectLocationDetailView
    {
        public int LocationId { get; set; }
        public string Location { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal FundsPercentage { get; set; }
    }

    public class ProjectLocationModel
    {
        [Required]
        public int ProjectId { get; set; }
        public List<ProjectLocation> ProjectLocations { get; set; }
    }

    public class ProjectLocation
    {
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
        public int ParentSectorId { get; set; }
        public string Sector { get; set; }
        public decimal FundsPercentage { get; set; }
    }

    public class ProjectSectorModel
    {
        [Required]
        public int ProjectId { get; set; }
        public List<ProjectSector> ProjectSectors { get; set; } 
        public List<SectorMappings> NewMappings { get; set; }
    }

    public class SectorMappings
    {
        public int SectorTypeId { get; set; }
        public int SectorId { get; set; }
        public int MappingId { get; set; }
    }

    public class ProjectSector
    {
        public int? SectorTypeId { get; set; }
        [Required]
        public int SectorId { get; set; }
        [Required]
        public string Sector { get; set; }
        [Required]
        public int MappingId { get; set; }
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
        public List<int> ImplementerIds { get; set; }
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

    public class ProjectMarkersView
    {
        public int MarkerId { get; set; }
        public string FieldTitle { get; set; }
        public int ProjectId { get; set; }
        public FieldTypes FieldType { get; set; }
        public string Values { get; set; }
    }

    public class ProjectDocumentModel
    {
        public int ProjectId { get; set; }
        public List<DocumentModel> Documents { get; set; }
    }

    public class DocumentModel
    {
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
        public int Year { get; set; }
        public DisbursementTypes DisbursementType { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public decimal ExchangeRate { get; set; }
    }

    public class ProjectDisbursementModel
    {
        [Required]
        public int ProjectId { get; set; }
        public string Currency { get; set; }
        public decimal ExchangeRate { get; set; }
        public List<YearDisbursements> YearlyDisbursements { get; set; }
    }

    public class YearDisbursements
    {
        [Required]
        public int Year { get; set; }
        [Required]
        public DisbursementTypes  DisbursementType { get; set; }
        [Required]
        public decimal Amount { get; set; }
    }

    public class ExpectedDisbursementModel
    {
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        public decimal ExpectedDisbursement { get; set; }
    }


    public class ProjectMarkersModel
    {
        public int ProjectId { get; set; }
        public string MarkerId { get; set; }
    }


    /// <summary>
    /// Notifications models
    /// </summary>
    public class NotificationView
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string Email { get; set; }
        public int TreatmentId { get; set; }
        public string Dated { get; set; }
        public string Organization { get; set; }
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

    public class FinanceTypes
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class FinancialYearView
    {
        public int Id { get; set; }
        public int FinancialYear { get; set; }
        public string Label { get; set; }
    }

    public class FinancialYearModel
    {
        public int Month { get; set; } = 0;
        [Required]
        public int FinancialYear { get; set; }
    }

    public class FinancialYearRangeModel
    {
        [Required]
        public int StartingMonth { get; set; }
        [Required]
        public int StartingYear { get; set; }
        [Required]
        public int EndingMonth { get; set; }
        [Required]
        public int EndingYear { get; set; }
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
        public string SenderName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string AdminEmail { get; set; }
    }

    public class SMTPSettingsModelView
    {
        public int Id { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string SenderName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string AdminEmail { get; set; }
    }

    public class SMTPSettingsModel
    {
        [Required]
        public string Host { get; set; }
        [Required]
        public int Port { get; set; } = 0;
        [Required]
        public string SenderName { get; set; }
        [Required]
        public string Username { get; set; }
        public string Password { get; set; }
        [Required]
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
        public string ReportUrl { get; set; }
        public string ExcelReportName { get; set; }
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

    public class ProjectDetailReport
    {
        public Report ReportSettings { get; set; }
        public ProjectReportView ProjectProfile { get; set; }
    }

    public class ProjectsBudgetReport
    {
        public Report ReportSettings { get; set; }
        public IEnumerable<ProjectBudgetView> Projects { get; set; }
    }

    public class SearchAllProjectsModel
    {
        public int StartingYear { get; set; }
        public int EndingYear { get; set; }
    }

    public class SearchEnvelopeModel
    {
        public int StartingYear { get; set; }
        public int EndingYear { get; set; }
        public List<int> FunderTypeIds { get; set; }
        public List<int> FunderIds { get; set; }
        public List<int> EnvelopeTypeIds { get; set; }
    }

    public class EnvelopeReport
    {
        public Report ReportSettings { get; set; }
        public ICollection<int> EnvelopeYears { get; set; }
        public ICollection<EnvelopeTypeView> EnvelopeTypes { get; set; }
        public IEnumerable<EnvelopeYearlyView> Envelope { get; set; }
    }

    public class ProjectsBudgetReportSummary
    {
        public Report ReportSettings { get; set; }
        public IEnumerable<ProjectBudgetSummaryView> Projects { get; set; }
        public ICollection<YearlyTotalDisbursementsSummary> TotalYearlyDisbursements { get; set; }
    }

    public class ProjectBudgetSummaryView
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<ProjectYearlyDisbursementsSummary> YearlyDisbursements { get; set; }
        public ICollection<ProjectYearlyDisbursementsBreakup> DisbursementsBreakup { get; set; }
    }

    public class ProjectBudgetView
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal ProjectValue { get; set; } = 0;
        public int StartingFinancialYear { get; set; }
        public int EndingFinancialYear { get; set; }
        public string StartingFinancialYearString { get; set; }
        public string EndingFinancialYearString { get; set; }
        public ICollection<ProjectFunding> Funding { get; set; }
        public ICollection<ProjectDisbursements> Disbursements { get; set; }
        public ICollection<ProjectYearlyDisbursements> YearlyDisbursements { get; set; }
        public ICollection<BudgetFundShare> FundShares { get; set; }
        public int TotalMonths { get; set; }
        public int MonthsLeft { get; set; }
        public int MonthsCurrentYear { get; set; }
        public decimal MoneyLeftForYears { get; set; } = 0;
        public decimal MoneyPreviousTwoYears { get; set; } = 0;
        public decimal ExpectedDisbursementsCurrentYear { get; set; } = 0;
        public decimal ExpectedMinusActual { get; set; } = 0;
    }

    public class BudgetFundShare
    {
        public string FundingType { get; set; }
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
    }

    public class SectorDisbursements
    {
        public string Sector { get; set; }
        public decimal Disbursements { get; set; }
    }

    public class LocationDisbursements
    {
        public string Location { get; set; }
        public decimal Disbursements { get; set; }
    }

    public class ProjectFunding
    {
        public string Funder { get; set; }
        public string FundType { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountInDefault { get; set; }
        public string Currency { get; set; }
        public decimal ExchangeRateToDefault { get; set; }
    }

    public class ProjectDisbursements
    {
        public string Dated { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountInDefault { get; set; }
        public string Currency { get; set; }
        public decimal ExchangeRateToDefault { get; set; }
    }

    public class YearlyTotalDisbursementsSummary
    {
        public int Year { get; set; }
        public decimal TotalDisbursements { get; set; }
        public decimal TotalExpectedDisbursements { get; set; }
    }

    public class ProjectYearlyDisbursements
    {
        public int Year { get; set; }
        public int ActiveMonths { get; set; }
        public decimal Disbursements { get; set; }
        public decimal ExpectedDisbursements { get; set; }
    }

    public class ProjectYearlyDisbursementsSummary
    {
        public int Year { get; set; }
        public decimal Disbursements { get; set; }
    }

    public class ProjectYearlyDisbursementsBreakup
    {
        public int Year { get; set; }
        public decimal ActualDisbursements { get; set; }
        public decimal ExpectedDisbursements { get; set; }
    }

    public class ProjectExpectedDisbursements
    {
        public int Year { get; set; }
        public decimal Disbursements { get; set; } = 0;
        public ICollection<SectorDisbursements> SectorPercentages { get; set; }
        public ICollection<LocationDisbursements> LocationPercentages { get; set; }
    }

    public class FilteredProjectProfileReport
    {
        public Report ReportSettings { get; set; }
        public ICollection<ProjectProfileView> ProjectsList { get; set; }
    }

    public class ProjectProfileReportByLocation
    {
        public Report ReportSettings { get; set; }
        public IEnumerable<ProjectsByLocation> LocationProjectsList { get; set; }
    }


    public class ProjectsBySector
    {
        public string SectorName { get; set; }
        public int ParentSectorId { get; set; }
        public string ParentSector { get; set; }
        public decimal TotalFunding { get; set; }
        public decimal ActualDisbursements { get; set; }
        public decimal PlannedDisbursements { get; set; }
        public decimal TotalDisbursements { get; set; }
        public IEnumerable<ProjectViewForSector> Projects { get; set; }
    }

    public class ProjectProfileReportBySector
    {
        public Report ReportSettings { get; set; }
        public IEnumerable<ProjectsBySector> SectorProjectsList { get; set; }
    }

    public class TimeSeriesReportByYear
    {
        public Report ReportSettings { get; set; }
        public IEnumerable<ProjectsByYear> YearlyProjectsList { get; set; }
    }

    public class ProjectsByYear
    {
        public int Year { get; set; }
        public decimal TotalProjectValue { get; set; }
        public decimal TotalFunding { get; set; }
        public decimal TotalDisbursements { get; set; }
        public decimal TotalActualDisbursements { get; set; }
        public decimal TotalPlannedDisbursements { get; set; }
        public IEnumerable<ProjectViewForYear> Projects { get; set; }
    }

    public class ProjectViewForYear
    {
        public string Title { get; set; }
        public string Funders { get; set; }
        public string Implementers { get; set; }
        public string StartingFinancialYear { get; set; }
        public string EndingFinancialYear { get; set; }
        public decimal ProjectValue { get; set; }
        public decimal ActualDisbursements { get; set; }
        public decimal PlannedDisbursements { get; set; }
    }

    public class ProjectsByLocation
    {
        public string LocationName { get; set; }
        public decimal TotalFunding { get; set; }
        public decimal ActualDisbursements { get; set; }
        public decimal PlannedDisbursements { get; set; }
        public decimal TotalDisbursements { get; set; }
        public IEnumerable<ProjectViewForLocation> Projects { get; set; }
    }

    public class SectorProjects
    {
        public int SectorId { get; set; }
        public string SectorName { get; set; }
        public List<int> ProjectIds { get; set; }
    }

    public class SearchProjectsBySectorModel
    {
        public List<int> ProjectIds { get; set; } = new List<int>();
        public List<int> OrganizationIds { get; set; } = new List<int>();
        public int StartingYear { get; set; } = 0;
        public int EndingYear { get; set; } = 0;
        public int LocationId { get; set; }
        public List<int> SectorIds { get; set; } = new List<int>();
        public NoSectorOptions SectorOption { get; set; } = NoSectorOptions.ProjectsWithSectors;
    }

    public class SearchProjectsWithoutSectorModel
    {
        public List<int> ProjectIds { get; set; } = new List<int>();
        public List<int> OrganizationIds { get; set; } = new List<int>();
        public int StartingYear { get; set; } = 0;
        public int EndingYear { get; set; } = 0;
        public int LocationId { get; set; }
    }

    public class SearchProjectsByLocationModel
    {
        public List<int> ProjectIds { get; set; } = new List<int>();
        public List<int> OrganizationIds { get; set; } = new List<int>();
        public int StartingYear { get; set; } = 0;
        public int EndingYear { get; set; } = 0;
        public List<int> LocationIds { get; set; } = new List<int>();
        public int SectorId { get; set; } = 0;
        public NoLocationOptions LocationOption { get; set; } = NoLocationOptions.ProjectsWithLocations;
    }

    public class SearchProjectsByYearModel
    {
        public List<int> ProjectIds { get; set; } = new List<int>();
        public List<int> OrganizationIds { get; set; } = new List<int>();
        public int StartingYear { get; set; } = 0;
        public int EndingYear { get; set; } = 0;
        public List<int> LocationIds { get; set; } = new List<int>();
        public List<int> SectorIds { get; set; } = new List<int>();
    }

    public class SearchProjectsByBudgetModel
    {
        public string Title { get; set; } = null;
        public List<int> SectorIds { get; set; } = new List<int>();
        public List<int> LocationIds { get; set; } = new List<int>();
        public int StartingYear { get; set; } = 0;
        public int EndingYear { get; set; } = 0;
    }

    public class ProjectViewForSector
    {
        public int ProjectId { get; set; }
        public string Title { get; set; }
        public string Funders { get; set; }
        public string Implementers { get; set; }
        public string StartingFinancialYear { get; set; }
        public string EndingFinancialYear { get; set; }
        public decimal ProjectValue { get; set; }
        public decimal ProjectPercentValue { get; set; }
        public decimal ActualDisbursements { get; set; }
        public decimal PlannedDisbursements { get; set; }
    }

    public class ProjectViewForLocation
    {
        public string Title { get; set; }
        public string Funders { get; set; }
        public string Implementers { get; set; }
        public string StartingFinancialYear { get; set; }
        public string EndingFinancialYear { get; set; }
        public decimal ProjectValue { get; set; }
        public decimal ProjectPercentValue { get; set; }
        public decimal ActualDisbursements { get; set; }
        public decimal PlannedDisbursements { get; set; }
    }

    public class SearchProjectModel
    {
        public string Title { get; set; }
        public List<int> ProjectIds { get; set; } = new List<int>();
        public List<int> OrganizationIds { get; set; } = new List<int>();
        public int StartingYear { get; set; } = 0;
        public int EndingYear { get; set; } = 0;
        public List<int> SectorIds { get; set; } = new List<int>();
        public List<int> LocationIds { get; set; } = new List<int>();
        public string Description { get; set; }
    }

    public class MarkerModel
    {
        public int Id { get; set; }
        [Required]
        public string FieldTitle { get; set; }
        [Required]
        public FieldTypes FieldType { get; set; }
        public string Values { get; set; }
        public string Help { get; set; }
    }

    public class MarkerValues
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }

    public class ProjectMarkerModel
    {
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public int MarkerId { get; set; }
        [Required]
        public FieldTypes FieldType { get; set; }
        [Required]
        public string Values { get; set; }
    }

    public class MarkerView
    {
        public int Id { get; set; }
        public string FieldTitle { get; set; }
        public FieldTypes FieldType { get; set; }
        public string Values { get; set; }
        public string Help { get; set; }
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

    public class EmailMessageModel
    {
        public EmailMessageType MessageType { get; set; }
        [MaxLength(200)]
        public string Subject { get; set; }
        [MaxLength(1000)]
        public string Message { get; set; }
        public string FooterMessage { get; set; }
    }

    public class EmailMessageView
    {
        public int Id { get; set; }
        public EmailMessageType MessageType { get; set; }
        public string TypeDefinition { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string FooterMessage { get; set; }
    }

    public class ProjectMembershipRequestModel
    {
        [Required]
        public int ProjectId { get; set; }
        [Required]
        [EmailAddress]
        public string UserEmail { get; set; }
    }

    public class ProjectDeletionRequestModel
    {
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public int UserId { get; set; }
    }

    public class ProjectMembershipRequestView
    {
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public string UserEmail { get; set; }
        public string Project { get; set; }
        public string UserOrganization { get; set; }
        public string Dated { get; set; }
        public bool IsApproved { get; set; }
    }

    public class ProjectDeletionRequestView
    {
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public string UserEmail { get; set; }
        public string Project { get; set; }
        public string UserOrganization { get; set; }
        public string RequestedOn { get; set; }
        public string StatusUpdatedOn { get; set; }
        public string Status { get; set; }
    }

    public class ProjectRequestStatusModel
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int ProjectId { get; set; }
    }

    public class UserProjectsView
    {
        public int Id { get; set; }
    }

    public class LocationProjects
    {
        public int LocationId { get; set; }
        public string Location { get; set; }
        public ICollection<LocationProject> Projects { get; set; }
    }

    public class SectorWithProjects
    {
        public int SectorId { get; set; }
        public int ParentSectorId { get; set; }
        public string Sector { get; set; }
        public ICollection<SectorProject> Projects { get; set; }
    }

    public class YearWithProjects
    {
        public int Year { get; set; }
        public ICollection<int> Projects { get; set; }
    }

    public class LocationProject
    {
        public int ProjectId { get; set; }
        public decimal FundsPercentage { get; set; }
    }

    public class SectorProject
    {
        public int ProjectId { get; set; }
        public decimal FundsPercentage { get; set; }
    }

    public class HelpViewForProject
    {
        public ProjectHelp HelpForProject { get; set; }
        public ProjectFunderHelp HelpForFunder { get; set; }
        public ProjectImplementerHelp HelpForImplementer { get; set; }
        public ProjectDisbursementHelp HelpForDisbursement { get; set; }
        public ProjectDocumentHelp HelpForDocuments { get; set; }
    }

    /*Help for data entry model*/
    public class ProjectHelp
    {
        [Required]
        public string Title { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        [Required]
        public string StartingFinancialYear { get; set; }
        [Required]
        public string EndingFinancialYear { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string ProjectValue { get; set; }
        [Required]
        public string ProjectCurrency { get; set; }
        [Required]
        public string FundingType { get; set; }
    }

    public class ProjectFunderHelp
    {
        [Required]
        public string Funder { get; set; }
    }

    public class ProjectImplementerHelp
    {
        [Required]
        public string Implementer { get; set; }
    }

    public class ProjectSectorHelp
    {
        [Required]
        public string Sector { get; set; }
        [Required]
        public string Percentage { get; set; }
    }

    public class ProjectLocationHelp
    {
        [Required]
        public string Location { get; set; }
        [Required]
        public string Percentage { get; set; }
    }
    public class ProjectDocumentHelp
    {
        [Required]
        public string Document { get; set; }
        [Required]
        public string DocumentUrl { get; set; }
    }

    public class ProjectDisbursementHelp
    {
        [Required]
        public string Year { get; set; }
        [Required]
        public string DisbursementActual { get; set; }
        [Required]
        public string DisbursementPlanned { get; set; }
    }

    public class HomePageModel
    {
        public string AIMSTitle { get; set; }
        public string IntroductionHeading { get; set; }
        public string IntroductionText { get; set; }
    }

    public class ProjectSummary
    {
        public string Project { get; set; }
        public decimal Funding { get; set; }
        public decimal Disbursement { get; set; }
    }

    public class RestoreDatabaseModel
    {
        [Required]
        [MaxLength(255)]
        public string FileName { get; set; }
    }

    public class DropboxSettingsModel
    {
        [Required]
        [MaxLength(255)]
        public string Token { get; set; }
    }

    public class BackupFiles
    {
        public int Id { get; set; }
        public string BackupFileName { get; set; }
        public DateTime TakenOn { get; set; }
    }
}
