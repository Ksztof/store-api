using AutoMapper;
using PerfumeStore.Application.DTOs.Request;
using PerfumeStore.Domain.StoreUsers;

namespace PerfumeStore.Application.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserForRegistrationDto, StoreUser>()
                .ForMember(u => u.UserName, opt => opt.MapFrom(x => x.Email));
        }
    }
}
