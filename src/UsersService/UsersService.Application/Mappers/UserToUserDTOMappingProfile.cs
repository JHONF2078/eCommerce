using AutoMapper;
using UsersService.Application.DTO;
using UsersService.Domain.Entities;

namespace eCommerce.Core.Mappers;

public class UserToUserDTOMappingProfile : Profile
{
  public UserToUserDTOMappingProfile()
  {
    CreateMap<User, UserDTO>()
      .ForMember(des => des.UserID, opt => opt.MapFrom(src => src.UserID))
      .ForMember(des => des.Email, opt => opt.MapFrom(src => src.Email))
      .ForMember(des => des.PersonName, opt => opt.MapFrom(src => src.PersonName))
      .ForMember(des => des.Gender, opt => opt.MapFrom(src => src.Gender));
  }
}