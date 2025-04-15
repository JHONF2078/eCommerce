
namespace UsersService.Application.DTO
{
    public record RegisterRequestCustom(
       string? Email,
       string? Password,
       string? PersonName,
       GenderOptions Gender
     );
}
