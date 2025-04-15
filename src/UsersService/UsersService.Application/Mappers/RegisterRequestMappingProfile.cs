using AutoMapper;
using UsersService.Application.DTO;
using UsersService.Domain.Entities;

namespace UsersService.Application.Mappers
{
    public class RegisterRequestMappingProfile: Profile
    {
        public RegisterRequestMappingProfile()
        {
            CreateMap<RegisterRequestCustom,User>()
              .ForMember(
                dest => dest.Email, 
                opt => opt.MapFrom(src => src.Email))
              .ForMember(
                dest => dest.PersonName, 
                opt => opt.MapFrom(src => src.PersonName))
              .ForMember(
                dest => dest.Gender, 
                opt => opt.MapFrom(src => src.Gender.ToString()))
              .ForMember(
                dest => dest.Password,
                opt => opt.MapFrom(src => src.Password));
        }
    }
}
