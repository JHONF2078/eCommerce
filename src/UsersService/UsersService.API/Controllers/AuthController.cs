using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using UsersService.Application.DTO;
using UsersService.Application.Interfaces.Services;
using UsersService.Infrastructure;

namespace UsersService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase //api/auth
    {
        private readonly IUserService _userService;

        public AuthController(IUserService usersService)
        {
            _userService = usersService;
        }

        // endpoint for user registrarion use case
        [HttpPost("register")] //api/auth/register
        public async Task<IActionResult> Register(RegisterRequestCustom registerRequest)
        {
            // Check for invalid register Request
            if (registerRequest == null)
            {
                return BadRequest("Invalid registration data");
            }

            //call the userService to handle registration
            AuthenticationResponse? authenticationResponse = await
                _userService.Register(registerRequest);

            if (authenticationResponse == null || authenticationResponse.Success == false)
            {
                return BadRequest(authenticationResponse);
            }

            return Ok(authenticationResponse);
        }
        //Endpoint for user login use case
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestCustom loginRequest)
        {
            //Check for invalid LoginRequest
            if (loginRequest == null)
            {
                return BadRequest("Invalid login data");
            }

            AuthenticationResponse? authenticationResponse = await _userService.Login(loginRequest);

            if (authenticationResponse == null || authenticationResponse.Success == false)
            {
                return Unauthorized(authenticationResponse);
            }

            return Ok(authenticationResponse);
        }
    }
}
