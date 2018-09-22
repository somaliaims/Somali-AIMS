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
            CreateMap<EFSector, SectorView>().ReverseMap();
            CreateMap<EFOrganizationTypes, OrganizationTypeView>();

            CreateMap<EFUser, UserView>()
                .ForMember(u => u.Organization, opts => opts.MapFrom(source => source.Organization.OrganizationName))
                .ForMember(u => u.OrganizationId, opts => opts.MapFrom(source => source.Organization.Id));

            CreateMap<EFProjectFunders, ProjectFunderView>()
                .ForMember(p => p.Project, opts => opts.MapFrom(source => source.Project.Title))
                .ForMember(u => u.Funder, opts => opts.MapFrom(source => source.Funder.OrganizationName));

            CreateMap<EFOrganization, OrganizationView>()
                .ForMember(o => o.TypeName, opts => opts.MapFrom(source => source.OrganizationType.TypeName));
        }
    }
}
