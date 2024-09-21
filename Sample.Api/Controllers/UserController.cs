using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Sample.Api.Config;
using Sample.Api.Helpers;
using Sample.Common.Config.Options.Constants;
using Sample.Common.DTOs.Requests;
using Sample.Common.DTOs.Responses;
using Sample.Core.Services.Interfaces;

namespace Sample.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthManager _authManager;

        public UserController(IUserService userService, IAuthManager authManager)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _authManager = authManager ?? throw new ArgumentNullException(nameof(authManager));
        }

        [HttpPost]
        public async Task Register([FromBody] UserRequest request)
            => await _userService.RegisterAsync(request);

        [HttpPost("login")]
        public async Task<UserResponse> Login([FromBody] LoginRequest request)
            => await _authManager.AuthenticateAsync(HttpContext, request.Username, request.Password);

        [HttpGet]
        [Authorize(Permission.User.List)]
        public async Task<PagedResult<UserResponse>> GetAll([FromQuery] UserFilter request)
            => await _userService.GetAllByFilters(request);
    }
}