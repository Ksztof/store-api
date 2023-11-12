using AutoMapper;
using PerfumeStore.Core.DTOs.Request;
using PerfumeStore.Domain.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
