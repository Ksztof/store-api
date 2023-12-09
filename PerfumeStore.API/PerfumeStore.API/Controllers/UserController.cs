using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfumeStore.API.DTOs.Request;
using PerfumeStore.Application.DTOs.Request;
using PerfumeStore.Application.Users;
using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.StoreUsers;

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

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] AuthenticateUserDtoApi userAuthRequest)
        {
            AuthenticateUserDtoApp authenticateUserDto = _mapper.Map<AuthenticateUserDtoApp>(userAuthRequest);

            AuthenticationResult result = await _userService.Login(authenticateUserDto);
            if (result.IsFailure)
            {
                return Unauthorized(result.Error);
            }

            return Ok(result.Token);
        }

        [HttpPost("Register")]
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
                return BadRequest(result.Error);

            return StatusCode(201);
        }

        [HttpGet("Confirm")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return BadRequest("Wrong token.");

            bool result = await _userService.ConfirmEmail(userId, token);
            if (!result)
                return BadRequest("Can't confirm email");

            return Ok();
        }

        //[Authorize]
        [HttpGet("RequestDeletion")]
        public async Task<IActionResult> RequestDeletion()
        {
            AuthenticationResult result = await _userService.RequestDeletion();
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok("Requested for account deletion");
        }

        [Authorize]//TODO: Authorize("Admin")
        [HttpGet("SubmitDeletion/{id}")]
        public async Task<IActionResult> SubmitDeletion(string id)
        {
            AuthenticationResult result = await _userService.SubmitDeletion(id);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return NoContent();
        }
    }
}
