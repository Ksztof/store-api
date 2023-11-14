using AutoMapper;
using PerfumeStore.Core.DTOs.Request;
using PerfumeStore.Domain.DbModels;

namespace PerfumeStore.Core.Mapper
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
