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
        }
    }
}
