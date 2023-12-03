using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using PerfumeStore.API.DTOs.Request;
using PerfumeStore.Application.DTOs.Request;
using PerfumeStore.Application.DTOs.Response;
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

            AuthenticationResult response = await _userService.Login(authenticateUserDto);
            if (response.IsFailure)
            {
                return Unauthorized(response.Error);
            }

            return Ok(response.Token);
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

            RegistrationResponseDto registResponse = await _userService.RegisterUser(registerUserDtoApp);

            if (registResponse.i)
            {

            }

            return StatusCode(201);
        }

        [HttpGet("Confirm")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return BadRequest("Validation token and user id are required");

            bool result = await _userService.ConfirmEmail(userId, token);
            if (!result)
            {
                return BadRequest();
            }

            return Ok();
        }

        [Authorize]
        [HttpGet("RequestDeletion")]
        public async Task<IActionResult> RequestDeletion()
        {
            bool requested = await _userService.RequestDeletion();
            if (!requested)
                return BadRequest("Can't request deletion");

            return Ok();
        }

        [Authorize]//TODO: Authorize("Admin")
        [HttpGet("SubmitDeletion/{id}")]
        public async Task<IActionResult> SubmitDeletion(string id)
        {
            bool deleted = await _userService.SubmitDeletion(id);
            if (!deleted)
                return BadRequest("Can't delete account");

            return NoContent();
        }
    }
}
