using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfumeStore.API.Shared.DTO.Request.StoreUser;
using PerfumeStore.Application.Abstractions.Result.Authentication;
using PerfumeStore.Application.Shared.DTO.Request;
using PerfumeStore.Application.Users;
using PerfumeStore.Domain.Entities.StoreUsers;

namespace PerfumeStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthenticateUserDtoApi userAuthRequest)
        {
            AuthenticateUserDtoApp authenticateUserDto = _mapper.Map<AuthenticateUserDtoApp>(userAuthRequest);

            AuthenticationResult result = await _userService.Login(authenticateUserDto);

            if (result.IsFailure)
                return Unauthorized(result.Error);

            return Ok(result.Token);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDtoApi userRegRequest)
        {
            if (userRegRequest == null || !ModelState.IsValid)
                return BadRequest();

            StoreUser StoreUser = _mapper.Map<StoreUser>(userRegRequest);

            RegisterUserDtoApp registerUserDtoApp = new RegisterUserDtoApp
            {
                StoreUser = StoreUser,
                Password = userRegRequest.Password
            };

            AuthenticationResult result = await _userService.RegisterUser(registerUserDtoApp);

            if (result.IsFailure)
                return BadRequest(result.Error.description);

            return StatusCode(201);
        }

        [HttpGet("confirm")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return BadRequest("Wrong token.");

            AuthenticationResult result = await _userService.ConfirmEmail(userId, token);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok();
        }

        //[Authorize]
        [HttpPatch("request-deletion")]
        public async Task<IActionResult> RequestDeletion()
        {
            AuthenticationResult result = await _userService.RequestDeletion();

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok("Requested for account deletion");
        }

        [Authorize]//TODO: Authorize("Admin")
        [HttpDelete("{id}")]
        public async Task<IActionResult> SubmitDeletion(string id)
        {
            AuthenticationResult result = await _userService.SubmitDeletion(id);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return NoContent();
        }
    }
}
