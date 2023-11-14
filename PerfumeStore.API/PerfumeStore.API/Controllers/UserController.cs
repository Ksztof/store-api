using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfumeStore.Core.DTOs.Request;
using PerfumeStore.Core.DTOs.Response;
using PerfumeStore.Core.Services;

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

        [HttpPost("Registration")]
        public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistration)
        {
            if (userForRegistration == null || !ModelState.IsValid)
                return BadRequest();

            RegistrationResponseDto registResponse = await _userService.RegisterUser(userForRegistration);

            if (registResponse.Message != null)
            {
                return Conflict(registResponse.Message);
            }

            if (registResponse.IsSuccessfulRegistration != true)
            {
                return BadRequest(registResponse.Errors);
            }

            return StatusCode(201);
        }

        [HttpGet("ConfirmEmail")] //KM to nazewnictwo jest średnie, lepiej zostaw samo "confirm". Nikt nie musi wiedzieć, że potwierdzneie odbywa się przez email
                                  //KM równie dobrze kiedyś na wejściu możesz przekazać metodę potwierdzenia email/sms/telefon
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
