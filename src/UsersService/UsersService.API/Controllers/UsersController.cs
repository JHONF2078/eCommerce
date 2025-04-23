using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UsersService.Application.DTO;
using UsersService.Application.Interfaces.Services;

namespace UsersService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
  private readonly IUserService _usersService;

  public UsersController(IUserService usersService)
  {
    _usersService = usersService;
  }


  //GET /api/Users/{userID}
  [HttpGet("{userID}")]
  public async Task<IActionResult> GetUserByUserID(Guid userID)
  {
    //await Task.Delay(10000); // Simulate some delay
    //throw new NotImplementedException("This method is not implemented yet.");


    if (userID == Guid.Empty)
    {
      return BadRequest("Invalid User ID");
    }

    UserDTO? response = await _usersService.GetUserByUserID(userID);

    if (response == null)
    {
      return NotFound(response);
    }

    return Ok(response);
  }
}
