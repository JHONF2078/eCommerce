
namespace UsersService.Application.DTO
{    
    public record AuthenticationResponse(
        Guid UserID,
        string? Email,
        string? PersonName,
        string? Gender,
        string? Token,
        bool Success
    )
    {
        //Parameterless constructor
        //Used by automapper
        public AuthenticationResponse() : this(
            default,
            default,
            default,
            default,
            default,
            default
        ) {  }
    };
}
