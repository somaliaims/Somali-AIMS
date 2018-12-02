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
                .ForMember(s => s.Category, opts => opts.MapFrom(source => source.Category.Category))
                .ForMember(s => s.SubCategory, opts => opts.MapFrom(source => source.SubCategory.SubCategory));

            CreateMap<EFSector, SectorViewModel>()
                .ForMember(s => s.SectorTypeId, opts => opts.MapFrom(source => source.SectorType.Id))
                .ForMember(s => s.CategoryId, opts => opts.MapFrom(source => source.Category.Id))
                .ForMember(s => s.SubCategoryId, opts => opts.MapFrom(source => source.SubCategory.Id));

            CreateMap<EFOrganizationTypes, OrganizationTypeView>();

            CreateMap<EFUser, UserView>()
                .ForMember(u => u.Organization, opts => opts.MapFrom(source => source.Organization.OrganizationName))
                .ForMember(u => u.OrganizationId, opts => opts.MapFrom(source => source.Organization.Id));

            CreateMap<EFOrganization, OrganizationView>()
                .ForMember(o => o.TypeName, opts => opts.MapFrom(source => source.OrganizationType.TypeName));

            CreateMap<EFOrganization, OrganizationViewModel>()
                .ForMember(o => o.OrganizationTypeId, opts => opts.MapFrom(source => source.OrganizationType.Id));

            CreateMap<EFUserNotifications, NotificationView>()
                .ForMember(n => n.Dated, opts => opts.MapFrom(source => source.Dated.ToShortDateString()));

            CreateMap<EFLocation, LocationView>().ReverseMap();

            CreateMap<EFSectorCategory, SectorCategoryView>()
                .ForMember(c => c.SectorType, opts => opts.MapFrom(source => source.SectorType.TypeName))
                .ForMember(c => c.SectorTypeId, opts => opts.MapFrom(source => source.SectorType.Id));

            CreateMap<EFSectorCategory, SectorCategoryViewModel>()
                .ForMember(s => s.SectorTypeId, opts => opts.MapFrom(source => source.SectorType.Id));

            CreateMap<EFSectorSubCategory, SectorSubCategoryView>()
                .ForMember(s => s.SectorCategory, opts => opts.MapFrom(source => source.SectorCategory.Category))
                .ForMember(s => s.CategoryId, opts => opts.MapFrom(source => source.SectorCategory.Id));

            CreateMap<EFSectorSubCategory, SectorSubCategoryViewModel>()
                .ForMember(s => s.CategoryId, opts => opts.MapFrom(source => source.SectorCategory.Id));
        }
    }
}
