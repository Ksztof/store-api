using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using PerfumeStore.API.Shared.DTO.Request.StoreUser;
using PerfumeStore.API.Validators;
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
        private readonly IValidationService _validationService;

        public UserController(IUserService userService, IMapper mapper, IValidationService validationService)
        {
            _userService = userService;
            _mapper = mapper;
            _validationService = validationService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthenticateUserDtoApi userAuthRequest)
        {
            var validationResult = await _validationService.ValidateAsync(userAuthRequest);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            AuthenticateUserDtoApp authenticateUserDto = _mapper.Map<AuthenticateUserDtoApp>(userAuthRequest);

            UserResult result = await _userService.Login(authenticateUserDto);

            if (result.IsFailure)
                return Unauthorized(result.Error);

            return Ok("Logged In!");
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDtoApi userRegRequest)
        {
            var validationResult = await _validationService.ValidateAsync(userRegRequest);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            else if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            StoreUser StoreUser = _mapper.Map<StoreUser>(userRegRequest);

            RegisterUserDtoApp registerUserDtoApp = new RegisterUserDtoApp
            {
                StoreUser = StoreUser,
                Password = userRegRequest.Password
            };

            UserResult result = await _userService.RegisterUser(registerUserDtoApp);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return StatusCode(201);
        }

        [HttpGet("confirm/{userId}/{token}")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return BadRequest("Wrong token.");

            UserResult result = await _userService.ConfirmEmail(userId, token);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok();
        }

        [HttpPatch("request-deletion")]
        public async Task<IActionResult> RequestDeletion()
        {
            UserResult result = await _userService.RequestDeletion();

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok("Requested for account deletion");
        }

        [Authorize]//TODO: Authorize("Admin")
        [HttpDelete("{id}")]
        public async Task<IActionResult> SubmitDeletion(string id)
        {
            UserResult result = await _userService.SubmitDeletion(id);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return NoContent();
        }
    }
}
