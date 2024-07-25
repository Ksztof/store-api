using AutoMapper;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using PerfumeStore.API.Shared.DTO.Request.StoreUser;
using PerfumeStore.API.Shared.Extensions;
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
    }
}
