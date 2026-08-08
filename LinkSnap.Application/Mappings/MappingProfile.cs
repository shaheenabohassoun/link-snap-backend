using AutoMapper;
using LinkSnap.Domain.Entities;
using LinkSnap.Application.DTOs;

namespace LinkSnap.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Link, LinkDto>().ReverseMap();
        }
    }
}