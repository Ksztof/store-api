using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Application.Users;
using Store.Domain.Abstractions;
using FluentValidation.Results;
using Store.API.Shared.DTO.Request.StoreUser;
using Store.Domain.StoreUsers;
using Store.API.Validation;
using Store.Application.Users.Dto.Request;
using Store.API.Shared.Extensions.Results;
using Store.API.Shared.Extensions.Models;

namespace Store.API.Controllers
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
            ValidationResult validationResult = await _validationService.ValidateAsync(userAuthRequest);

            if (!validationResult.IsValid)
                return validationResult.ToValidationProblemDetails();


            AuthenticateUserDtoApp authenticateUserDto = _mapper.Map<AuthenticateUserDtoApp>(userAuthRequest);

            UserResult result = await _userService.Login(authenticateUserDto);

            return result.IsSuccess ? NoContent() : result.ToProblemDetails();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDtoApi userRegRequest)
        {
            var validationResult = await _validationService.ValidateAsync(userRegRequest);

            if (!validationResult.IsValid)
                return validationResult.ToValidationProblemDetails();


            StoreUser StoreUser = _mapper.Map<StoreUser>(userRegRequest);

            RegisterUserDtoApp registerUserDtoApp = new RegisterUserDtoApp
            {
                StoreUser = StoreUser,
                Password = userRegRequest.Password
            };

            UserResult result = await _userService.RegisterUser(registerUserDtoApp);

            return result.IsSuccess ? NoContent() : result.ToProblemDetails();
        }

        [HttpGet("confirm/{userId}/{token}")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            UserResult result = await _userService.ConfirmEmail(userId, token);

            return result.IsSuccess ? Ok() : result.ToProblemDetails();
        }

        [HttpPatch("request-deletion")]
        public async Task<IActionResult> RequestDeletion()
        {
            UserResult result = await _userService.RequestDeletion();

            return result.IsSuccess ? Ok() : result.ToProblemDetails();
        }

        [Authorize]//TODO: Authorize("Admin")
        [HttpDelete("{id}")]
        public async Task<IActionResult> SubmitDeletion(string id)
        {
            UserResult result = await _userService.SubmitDeletion(id);

            return result.IsSuccess ? NoContent() : result.ToProblemDetails();
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            Result authTokRemoveResult = _userService.RemoveAuthToken();

            if (authTokRemoveResult.IsFailure)
            {
                return authTokRemoveResult.ToProblemDetails();
            }

            Result refrTokRemoveResult = _userService.RemoveRefreshToken();

            if (refrTokRemoveResult.IsFailure)
            {
                return refrTokRemoveResult.ToProblemDetails();
            }

            return NoContent();
        }

        [HttpGet]
        public IActionResult RemoveGuestSessionId()
        {
            UserResult result = _userService.RemoveGuestSessionId();

            return result.IsSuccess ? NoContent() : result.ToProblemDetails();
        }
    }
}
