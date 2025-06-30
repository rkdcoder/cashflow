using Asp.Versioning;
using CashFlow.IdentityAndAccess.Application.Users.Command;
using CashFlow.IdentityAndAccess.Application.Users.Dto;
using CashFlow.IdentityAndAccess.Application.Users.Query;
using CashFlow.Shared.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CashFlow.IdentityAndAccess.Api.Controllers.V1
{
    /// <summary>
    /// Controller responsible for user authentication and token management endpoints.
    /// </summary>
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Authenticates user credentials and issues a JWT access token and refresh token.
        /// </summary>
        /// <param name="loginRequest">Login credentials (login/email + password)</param>
        /// <returns>Access token, refresh token and expiration info</returns>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), 200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto loginRequest)
        {
            var result = await _mediator.Send(new LoginCommand(loginRequest));
            return Ok(result);
        }

        /// <summary>
        /// Renews the access token using a valid refresh token.
        /// </summary>
        /// <param name="refreshRequest">Refresh token</param>
        /// <returns>New access token, new refresh token and expiration info</returns>
        [AllowAnonymous]
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(LoginResponseDto), 200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<LoginResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto refreshRequest)
        {
            var result = await _mediator.Send(new RefreshTokenCommand(refreshRequest));
            return Ok(result);
        }

        /// <summary>
        /// Allows a user to change their own password. ADMIN and MANAGER roles can also change the passwords of their subordinates.
        /// </summary>
        [HttpPost("change-password")]
        [ProducesResponseType(typeof(CommandResultDto<object>), 200)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangeUserPasswordRequestDto request)
        {
            var result = await _mediator.Send(new ChangeUserPasswordCommand(request));
            return Ok(result);
        }

        /// <summary>
        /// Creates a new user (ADMIN only).
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(CommandResultDto<SimpleUserDto>), 200)]
        public async Task<IActionResult> Create([FromBody] CreateUserRequestDto request)
        {
            var result = await _mediator.Send(new CreateUserCommand(request));
            return Ok(result);
        }

        /// <summary>
        /// Updates a user's info (ADMIN only).
        /// </summary>
        [HttpPut("update-info")]
        [ProducesResponseType(typeof(CommandResultDto<SimpleUserDto>), 200)]
        public async Task<IActionResult> UpdateInfo([FromBody] UpdateUserRequestDto request)
        {
            var result = await _mediator.Send(new UpdateUserCommand(request));
            return Ok(result);
        }

        /// <summary>
        /// Returns a filtered list of users.
        /// If userId is provided, other filters are ignored.
        /// </summary>
        /// <param name="userId">Optional: user GUID</param>
        /// <param name="loginOrEmail">Optional: filter by login or email</param>
        /// <param name="enabled">Optional: filter by enabled (active/inactive)</param>
        [HttpGet]
        [ProducesResponseType(typeof(List<UserListItemDto>), 200)]
        public async Task<ActionResult<List<UserListItemDto>>> GetAll([FromQuery] Guid? userId, [FromQuery] string? loginOrEmail, [FromQuery] bool? enabled)
        {
            var result = await _mediator.Send(new GetUsersQuery(userId, loginOrEmail, enabled));
            return Ok(result);
        }
    }
}