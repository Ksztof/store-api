using AutoMapper;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.API.Shared.DTO.Request.StoreUser;
using Store.API.Shared.Extensions.Models;
using Store.API.Shared.Extensions.Results;
using Store.API.Validation;
using Store.Application.Users;
using Store.Application.Users.Dto.Request;
using Store.Domain.Abstractions;
using Store.Domain.StoreUsers;
using Store.Domain.StoreUsers.Roles;

namespace Store.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    private readonly IValidationService _validationService;

    public UserController(
        IUserService userService,
        IMapper mapper,
        IValidationService validationService)
    {
        _userService = userService;
        _mapper = mapper;
        _validationService = validationService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginAsync([FromBody] AuthenticateUserDtoApi userAuthRequest)
    {
        ValidationResult validationResult = await _validationService.ValidateAsync(userAuthRequest);

        if (!validationResult.IsValid)
        {
            return validationResult.ToValidationProblemDetails();
        }

        AuthenticateUserDtoApp authenticateUserDto = _mapper.Map<AuthenticateUserDtoApp>(userAuthRequest);

        UserResult result = await _userService.LoginAsync(authenticateUserDto);

        return result.IsSuccess ? NoContent() : result.ToProblemDetails();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterUserDtoApi userRegRequest)
    {
        var validationResult = await _validationService.ValidateAsync(userRegRequest);

        if (!validationResult.IsValid)
        {
            return validationResult.ToValidationProblemDetails();
        }

        StoreUser StoreUser = _mapper.Map<StoreUser>(userRegRequest);

        RegisterUserDtoApp registerUserDtoApp = new RegisterUserDtoApp
        {
            StoreUser = StoreUser,
            Password = userRegRequest.Password
        };

        UserResult result = await _userService.RegisterUserAsync(registerUserDtoApp);

        return result.IsSuccess ? NoContent() : result.ToProblemDetails();
    }

    [HttpGet("confirm/{userId}/{token}")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmailAsync(string userId, string token)
    {
        UserResult result = await _userService.ConfirmEmailAsync(userId, token);

        return result.IsSuccess ? Ok() : result.ToProblemDetails();
    }

    [HttpPatch("request-deletion")]
    [Authorize(Roles = $"{UserRoles.Visitor}, {UserRoles.Administrator}")]
    public async Task<IActionResult> RequestDeletionAsync()
    {
        UserResult result = await _userService.RequestDeletionAsync();

        return result.IsSuccess ? Ok() : result.ToProblemDetails();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = UserRoles.Administrator)]
    public async Task<IActionResult> SubmitDeletionAsync(string id)
    {
        UserResult result = await _userService.SubmitDeletionAsync(id);

        return result.IsSuccess ? NoContent() : result.ToProblemDetails();
    }

    [HttpGet("logout")]
    [Authorize(Roles = $"{UserRoles.Visitor}, {UserRoles.Administrator}")]
    public IActionResult RemoveRefreshToken()
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
    [AllowAnonymous]
    public IActionResult RemoveGuestSessionId()
    {
        UserResult result = _userService.RemoveGuestSessionId();

        return result.IsSuccess ? NoContent() : result.ToProblemDetails();
    }
}
