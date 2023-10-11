using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PerfumeStore.Core.DTOs.Request;
using PerfumeStore.Core.DTOs.Response;
using PerfumeStore.Core.Services;
using System.IdentityModel.Tokens.Jwt;

namespace PerfumeStore.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class UserController : ControllerBase
  {
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
      _userService = userService;
    } 

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] UserForAuthenticationDto userForAuthentication)
    {
      AuthResponseDto authResponse = await _userService.Login(userForAuthentication);
      if (authResponse.ErrorMessage != null)
      {
        return Unauthorized(authResponse.ErrorMessage);
      }

      return Ok(authResponse);
    }
  }
}
