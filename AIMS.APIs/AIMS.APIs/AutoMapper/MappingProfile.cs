﻿using AIMS.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIMS.APIs.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<EFCurrency, CurrencyView>();

            CreateMap<EFHomePageSettings, HomePageModel>();

            CreateMap<EFFinancialYears, FinancialYearView>();

            CreateMap<EFSectorTypes, SectorTypesView>();

            CreateMap<EFIATIOrganization, IATIOrganizationView>();

            CreateMap<EFOrganization, IATIOrganizationView>();

            CreateMap<EFOrganization, OrganizationViewModel>();

            CreateMap<EFFundingTypes, FundingTypeView>();

            CreateMap<EFIATICountryCodes, IATICountryModel>();

            CreateMap<EFEnvelope, EnvelopeView>();

            CreateMap<EFManualExchangeRates, ManualRatesView>();

            CreateMap<EFMarkers, MarkerView>();

            CreateMap<EFSector, SectorDetailedView>()
                .ForMember(s => s.ParentSector, opts => opts.MapFrom(source => source.ParentSector.SectorName))
                .ForMember(s => s.SectorTypeId, opts => opts.MapFrom(source => source.SectorType.Id))
                .ForMember(s => s.SectorType, opts => opts.MapFrom(source => source.SectorType.TypeName))
                .ForMember(s => s.SectorWithCode, opts => opts.MapFrom(source => (source.IATICode != null) ? (source.IATICode + ": " + source.SectorName) : source.SectorName));

            CreateMap<EFSector, SectorView>()
                .ForMember(s => s.ParentSector, opts => opts.MapFrom(source => source.ParentSector.SectorName))
                .ForMember(s => s.SectorWithCode, opts => opts.MapFrom(source => (source.IATICode != null) ? (source.IATICode + ": " + source.SectorName) : source.SectorName));

            CreateMap<EFSector, SectorViewWithParent>()
                .ForMember(s => s.ParentSector, opts => opts.MapFrom(source => source.ParentSector.SectorName))
                .ForMember(s => s.ParentSectorId, opts => opts.MapFrom(source => source.ParentSector.Id));

            CreateMap<EFSector, SectorViewModel>()
                .ForMember(s => s.ParentId, opts => opts.MapFrom(source => source.ParentSector.Id));

            CreateMap<EFOrganization, OrganizationView>()
                .ForMember(o => o.OrganizationType, opts => opts.MapFrom(source => source.OrganizationType.TypeName))
                .ForMember(o => o.DateUpdated, opts => opts.MapFrom(source => source.DateUpdated.ToShortDateString()));

            CreateMap<EFIATIOrganization, OrganizationView>();

            CreateMap<EFOrganization, OrganizationMiniView>()
                .ForMember(o => o.Id, opts => opts.MapFrom(source => source.Id));

            CreateMap<EFOrganizationTypes, OrganizationTypeView>();

            CreateMap<EFSectorMappings, SectorMappingView>();

            CreateMap<EFProjectDocuments, ProjectDocumentView>();

            CreateMap<EFProjectDocuments, DocumentAbstractView>();

            CreateMap<EFUser, UserView>()
                .ForMember(u => u.Organization, opts => opts.MapFrom(source => source.Organization.OrganizationName))
                .ForMember(u => u.OrganizationId, opts => opts.MapFrom(source => source.Organization.Id));

            CreateMap<EFUserNotifications, NotificationView>()
                .ForMember(n => n.Dated, opts => opts.MapFrom(source => source.Dated.ToShortDateString()))
                .ForMember(n => n.Organization, opts => opts.MapFrom(source => source.Organization.OrganizationName));

            CreateMap<EFLocation, LocationView>().ReverseMap();

            CreateMap<EFProject, ProjectView>();

            CreateMap<EFProject, ProjectModelView>()
                .ForMember(p => p.StartDate, opts => opts.MapFrom(source => source.StartDate.ToShortDateString()))
                .ForMember(p => p.EndDate, opts => opts.MapFrom(source => source.EndDate.ToShortDateString()))
                .ForMember(p => p.DateUpdated, opts => opts.MapFrom(source => source.DateUpdated.ToShortDateString()))
                .ForMember(p => p.LastUpdatedByOrganization, opts => opts.MapFrom(source => source.UpdatedByOrganization.OrganizationName))
                .ForMember(p => p.StartingFinancialYear, opts => opts.MapFrom(source => source.StartingFinancialYear.Label.ToString()))
                .ForMember(p => p.EndingFinancialYear, opts => opts.MapFrom(source => source.EndingFinancialYear.Label.ToString()));

            CreateMap<EFProjectLocations, LocationView>()
                .ForMember(l => l.Id, opts => opts.MapFrom(source => source.Location.Id))
                .ForMember(l => l.Location, opts => opts.MapFrom(source => source.Location.Location))
                .ForMember(l => l.Latitude, opts => opts.MapFrom(source => source.Location.Latitude))
                .ForMember(l => l.Longitude, opts => opts.MapFrom(source => source.Location.Longitude));

            CreateMap<EFProjectLocations, ProjectLocationDetailView>()
                .ForMember(l => l.LocationId, opts => opts.MapFrom(source => source.Location.Id))
                .ForMember(l => l.Location, opts => opts.MapFrom(source => source.Location.Location))
                .ForMember(l => l.Latitude, opts => opts.MapFrom(source => source.Location.Latitude))
                .ForMember(l => l.Longitude, opts => opts.MapFrom(source => source.Location.Longitude))
                .ForMember(l => l.FundsPercentage, opts => opts.MapFrom(source => source.FundsPercentage));

            CreateMap<EFProjectSectors, ProjectSectorView>()
                .ForMember(s => s.SectorId, opts => opts.MapFrom(source => source.Sector.Id))
                .ForMember(s => s.Sector, opts => opts.MapFrom(source => source.Sector.SectorName))
                .ForMember(s => s.ParentSectorId, opts => opts.MapFrom(source => source.Sector.ParentSectorId))
                .ForMember(s => s.FundsPercentage, opts => opts.MapFrom(source => source.FundsPercentage));

            CreateMap<EFProjectFunders, ProjectFunderView>()
                .ForMember(f => f.FunderId, opts => opts.MapFrom(source => source.Funder.Id))
                .ForMember(f => f.Funder, opts => opts.MapFrom(source => source.Funder.OrganizationName));

            CreateMap<EFProjectDisbursements, ProjectDisbursementView>()
                .ForMember(d => d.Year, opts => opts.MapFrom(source => source.Year.FinancialYear))
                .ForMember(d => d.FinancialYear, opts => opts.MapFrom(source => source.Year.Label))
                .ForMember(d => d.DisbursementType, opts => opts.MapFrom(source => (DisbursementTypes)source.DisbursementType));

            CreateMap<EFProjectDisbursements, DisbursementAbstractView>()
                .ForMember(d => d.Disbursement, opts => opts.MapFrom(source => source.Amount))
                .ForMember(d => d.Year, opts => opts.MapFrom(source => source.Year.FinancialYear))
                .ForMember(d => d.FinancialYear, opts => opts.MapFrom(source => source.Year.Label))
                .ForMember(d => d.DisbursementType, opts => opts.MapFrom(source => (DisbursementTypes)source.DisbursementType));

            CreateMap<EFProjectMarkers, MarkerAbstractView>()
                .ForMember(m => m.Marker, opts => opts.MapFrom(source => source.Marker.FieldTitle))
                .ForMember(m => m.MarkerType, opts => opts.MapFrom(source => source.Marker.FieldType))
                .ForMember(m => m.Values, opts => opts.MapFrom(source => source.Values));

            CreateMap<EFProjectImplementers, ProjectImplementerView>()
                .ForMember(i => i.ImplementerId, opts => opts.MapFrom(source => source.Implementer.Id))
                .ForMember(i => i.Implementer, opts => opts.MapFrom(source => source.Implementer.OrganizationName));

            CreateMap<EFIATISettings, IATISettings>().ReverseMap();

            CreateMap<EFMarkers, ProjectMarkersView>()
                .ForMember(c => c.FieldTitle, opts => opts.MapFrom(source => source.FieldTitle));

            CreateMap<EFProjectMarkers, ProjectMarkersView>()
                .ForMember(m => m.FieldTitle, opts => opts.MapFrom(source => source.Marker.FieldTitle));

            CreateMap<EFProjectMembershipRequests, ProjectMembershipRequestView>()
                .ForMember(m => m.Project, opts => opts.MapFrom(source => source.Project.Title))
                .ForMember(m => m.UserEmail, opts => opts.MapFrom(source => source.User.Email))
                .ForMember(m => m.MembershipType, opts => opts.MapFrom(source => source.MembershipType))
                .ForMember(m => m.MembershipTypeId, opts => opts.MapFrom(source => source.MembershipType))
                .ForMember(m => m.UserOrganization, opts => opts.MapFrom(source => source.User.Organization.OrganizationName));

            CreateMap<EFProjectLocations, LocationAbstractView>()
                .ForMember(l => l.Name, opts => opts.MapFrom(source => source.Location.Location))
                .ForMember(l => l.FundsPercentage, opts => opts.MapFrom(source => source.FundsPercentage));

            CreateMap<EFProjectSectors, SectorAbstractView>()
                .ForMember(s => s.Name, opts => opts.MapFrom(source => source.Sector.SectorName))
                .ForMember(s => s.ParentSector, opts => opts.MapFrom(source => source.Sector.ParentSector))
                .ForMember(s => s.FundsPercentage, opts => opts.MapFrom(source => source.FundsPercentage));

            CreateMap<EFProjectDeletionRequests, ProjectDeletionRequestView>()
                .ForMember(d => d.UserOrganization, opts => opts.MapFrom(source => source.RequestedBy.Organization.OrganizationName))
                .ForMember(d => d.UserEmail, opts => opts.MapFrom(source => source.RequestedBy.Email))
                .ForMember(d => d.Project, opts => opts.MapFrom(source => source.Project.Title))
                .ForMember(d => d.RequestedOn, opts => opts.MapFrom(source => source.RequestedOn.ToShortDateString()))
                .ForMember(d => d.StatusUpdatedOn, opts => opts.MapFrom(source => source.StatusUpdatedOn.ToShortDateString()));

            CreateMap<EFExchangeRatesUsageSettings, ExchangeRatesUsageView>()
                .ForMember(e => e.Source, opts => opts.MapFrom(source => source.Source.ToString()))
                .ForMember(e => e.UsageSection, opts => opts.MapFrom(source => source.UsageSection.ToString()));

            CreateMap<EFEnvelopeTypes, EnvelopeTypeView>();

            CreateMap<EFDocumentLinks, DocumentLinkView>();

            CreateMap<EFContactMessages, ContactMessageView>()
                .ForMember(c => c.Dated, opts => opts.MapFrom(source => source.Dated.ToShortDateString()))
                .ForMember(c => c.ProjectTitle, opts => opts.MapFrom(source => source.Project.Title))
                .ForMember(c => c.ProjectId, opts => opts.MapFrom(source => source.Project.Id));

            CreateMap<EFSubLocation, SubLocationView>();

            CreateMap<EFSponsorLogos, SponsorsLogosView>();

        }
    }
}
