using AutoMapper;
using UsersService.Application.DTO;
using UsersService.Domain.Entities;

namespace UsersService.Application.Mappers
{
    public class UserMappingProfile: Profile
    {
        //Profile es una clase de AutoMapper que se utiliza para definir las
        //configuraciones de mapeo entre diferentes tipos de objetos.

        public UserMappingProfile() {
            CreateMap<User, AuthenticationResponse>()
                .ForMember(
                    dest => dest.UserID,
                    opt => opt.MapFrom(src => src.UserID)
                )
                .ForMember(
                    dest => dest.Email,
                    opt => opt.MapFrom(src => src.Email)
                )
                .ForMember(
                        dest => dest.PersonName,
                        opt => opt.MapFrom(src => src.PersonName)
                )
                .ForMember(
                        dest => dest.Gender,
                        opt => opt.MapFrom(src => src.Gender)
                )
                .ForMember(
                    dest => dest.Success,
                    opt => opt.Ignore()
                )
                .ForMember(
                        dest => dest.Token,
                        opt => opt.Ignore()
                )
                .ForMember(
                        dest => dest.Gender,
                        opt => opt.MapFrom(src => src.Gender)
                );
        }
    }
}
