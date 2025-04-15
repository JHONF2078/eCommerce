namespace UsersService.Application.DTO
{
    public record LoginRequestCustom(
        string? Email,
        string? Password
    );
}
