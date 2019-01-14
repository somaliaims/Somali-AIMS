using AIMS.Models;
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
            CreateMap<EFSector, SectorView>()
                .ForMember(s => s.ParentSector, opts => opts.MapFrom(source => source.ParentSector.SectorName));

            CreateMap<EFSector, SectorViewModel>()
                .ForMember(s => s.ParentId, opts => opts.MapFrom(source => source.ParentSector.Id));

            CreateMap<EFOrganizationTypes, OrganizationTypeView>();

            CreateMap<EFUser, UserView>()
                .ForMember(u => u.Organization, opts => opts.MapFrom(source => source.Organization.OrganizationName))
                .ForMember(u => u.OrganizationId, opts => opts.MapFrom(source => source.Organization.Id));

            CreateMap<EFUserNotifications, NotificationView>()
                .ForMember(n => n.Dated, opts => opts.MapFrom(source => source.Dated.ToShortDateString()));

            CreateMap<EFLocation, LocationView>().ReverseMap();

            /*CreateMap<EFSectorCategory, SectorCategoryView>()
                .ForMember(c => c.SectorType, opts => opts.MapFrom(source => source.SectorType.TypeName))
                .ForMember(c => c.SectorTypeId, opts => opts.MapFrom(source => source.SectorType.Id));

            CreateMap<EFSectorCategory, SectorCategoryViewModel>()
                .ForMember(s => s.SectorTypeId, opts => opts.MapFrom(source => source.SectorType.Id));

            CreateMap<EFSectorSubCategory, SectorSubCategoryView>()
                .ForMember(s => s.SectorCategory, opts => opts.MapFrom(source => source.SectorCategory.Category))
                .ForMember(s => s.CategoryId, opts => opts.MapFrom(source => source.SectorCategory.Id));

            CreateMap<EFSectorSubCategory, SectorSubCategoryViewModel>()
                .ForMember(s => s.CategoryId, opts => opts.MapFrom(source => source.SectorCategory.Id));*/

            CreateMap<EFProject, ProjectView>()
                .ForMember(p => p.StartDate, opts => opts.MapFrom(source => source.StartDate.ToShortDateString()))
                .ForMember(p => p.EndDate, opts => opts.MapFrom(source => source.EndDate.ToShortDateString()));

            CreateMap<EFProject, ProjectModelView>()
                .ForMember(p => p.StartDate, opts => opts.MapFrom(source => source.StartDate.ToShortDateString()))
                .ForMember(p => p.EndDate, opts => opts.MapFrom(source => source.EndDate.ToShortDateString()));

            CreateMap<EFProjectLocations, LocationView>()
                .ForMember(l => l.Id, opts => opts.MapFrom(source => source.Location.Id))
                .ForMember(l => l.Location, opts => opts.MapFrom(source => source.Location.Location))
                .ForMember(l => l.Latitude, opts => opts.MapFrom(source => source.Location.Latitude))
                .ForMember(l => l.Longitude, opts => opts.MapFrom(source => source.Location.Longitude));

            CreateMap<EFProjectSectors, ProjectSectorView>()
                .ForMember(s => s.SectorId, opts => opts.MapFrom(source => source.Sector.Id))
                .ForMember(s => s.Sector, opts => opts.MapFrom(source => source.Sector.SectorName))
                .ForMember(s => s.FundsPercentage, opts => opts.MapFrom(source => source.FundsPercentage))
                .ForMember(s => s.Currency, opts => opts.MapFrom(source => source.Currency))
                .ForMember(s => s.ExchangeRate, opts => opts.MapFrom(source => source.ExchangeRate));

            CreateMap<EFProjectFunders, ProjectFunderView>()
                .ForMember(f => f.FunderId, opts => opts.MapFrom(source => source.Funder.Id))
                .ForMember(f => f.Funder, opts => opts.MapFrom(source => source.Funder.OrganizationName));

            CreateMap<EFProjectImplementors, ProjectImplementorView>()
                .ForMember(i => i.ImplementorId, opts => opts.MapFrom(source => source.Implementor.Id))
                .ForMember(i => i.Implementor, opts => opts.MapFrom(source => source.Implementor.OrganizationName));

            CreateMap<EFIATISettings, IATISettings>().ReverseMap();

        }
    }
}
